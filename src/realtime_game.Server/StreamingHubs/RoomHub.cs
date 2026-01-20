using MagicOnion.Server.Hubs;
using realtime_game.Server.Models.Contexts;
using realtime_game.Shared.Models.Entities;
using realtime_game.Shared.Interfaces.StreamingHubs;
using realtime_game.Server.StreamingHubs;

using System.Numerics;//Vector3で使う※Server/SharedでUnityEngineはNG

namespace Server.StreamingHubs
{
    public class RoomHub(RoomContextRepository roomContextRepository) : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        private RoomContextRepository roomContextRepos;
        private RoomContext roomContext;

        // ルームに接続
        public async Task<JoinedUser[]> JoinAsync(string roomName, int userId)
        {
            //試合中なら参加拒否
            var context = roomContextRepos.GetContext(roomName);

            if (context != null &&
                context.CurrentGameState == GameState.Playing)
            {
                throw new InvalidOperationException("Game already started.");
            }
            /*if (roomContext != null &&
                roomContext.CurrentGameState == GameState.Playing)
            {
                throw new InvalidOperationException("Game already started.");
            }*/

            //※ここからJoin処理

            //！同時に生成しないよう「排他制御」
            lock (roomContextRepos)
            {
                // 指定の名前のルームがあるかどうかを確認
                this.roomContext = roomContextRepos.GetContext(roomName);
                if (this.roomContext == null)
                { // 無かったら生成
                    this.roomContext = roomContextRepos.CreateContext(roomName);
                }
            }

            // ルームに参加 & ルームを保持
            this.roomContext.Group.Add(this.ConnectionId, Client);

            // DBからユーザー情報取得
            GameDbContext Dbcontext = new GameDbContext();
            User user = Dbcontext.Users.Where(user => user.Id == userId).First();

            // 入室済みユーザーのデータを作成
            var joinedUser = new JoinedUser();
            joinedUser.ConnectionId = this.ConnectionId;
            joinedUser.UserData = user;
            // ルームコンテキストにユーザー情報を登録
            var roomUserData = new RoomUserData() { JoinedUser = joinedUser };
            this.roomContext.RoomUserDataList[ConnectionId] = roomUserData;

            // 自分以外のルーム参加者全員に、ユーザーの入室通知を送信
            this.roomContext.Group.Except([this.ConnectionId]).OnJoin(joinedUser);

            // 入室リクエストをしたユーザーに、参加者の情報をリストで返す
            return this.roomContext.RoomUserDataList.Select(
                f => f.Value.JoinedUser).ToArray();
        }

        // 接続時の処理
        protected override ValueTask OnConnected()
        {
            roomContextRepos = roomContextRepository;
            return default;
        }
        // 切断時の処理
        protected override ValueTask OnDisconnected()
        {
            LeaveAsync();
            return CompletedTask;
            //return default;
        }

        // 接続ID取得
        public Task<Guid> GetConnectionId()
        {
            return Task.FromResult<Guid>(this.ConnectionId);
        }

        // 退出処理
        public Task LeaveAsync()
            {
            //　退室したことを全メンバーに通知
            /*if (roomContext == null)
            return Task.CompletedTask;*/
            this.roomContext.Group.All.OnLeave(this.ConnectionId);//！OnLeave？

            //　ルーム内のメンバーから自分を削除
            this.roomContext.Group.Remove(this.ConnectionId);

            //　ルームデータから退室したユーザーを削除
            this.roomContext.RoomUserDataList.Remove(this.ConnectionId);
            //roomContextRepos.RemoveContext(roomContext.Name);

            if (this.roomContext.RoomUserDataList.Count == 0)//ルーム内にユーザーが一人もいないなら
            {
                    roomContextRepos.RemoveContext("sampleRoom");//[ルーム(名)]を削除
                }
                return Task.CompletedTask;
        }

        // 移動+回転
        public Task MoveAsync(Vector3 pos, Quaternion euler)
        {
            // 位置情報を記録
            this.roomContext.RoomUserDataList[this.ConnectionId].pos = pos;

            //回転情報を記録
            this.roomContext.RoomUserDataList[this.ConnectionId].euler = euler;

            // 移動情報を自分以外の全メンバーに通知
            this.roomContext.Group.Except([this.ConnectionId]).OnMove(this.ConnectionId, pos, euler);

            return Task.CompletedTask;
        }

        //準備完了
        public async Task ReadyAsync()
        {
            //！同時に生成しないよう「排他制御」
            lock (this.roomContext)
            {

                //準備出来たことを自分のRoomUserDataに保存
                var roomUserData = this.roomContext.RoomUserDataList[this.ConnectionId];

                //roomUserDataにboolやintで準備完了を保存しておく
                roomUserData.IsReady = true;

                //全員準備できたか判定
                bool isReady = true;
                var roomUserDataList = this.roomContext.RoomUserDataList.Values.ToArray();
                foreach (var targetRoomUserData in roomUserDataList)
                {
                    //targetRoomUserDataに保存した準備完了状態を確認
                    if (!targetRoomUserData.IsReady)
                    {
                        isReady = false;
                        break;
                    }

                }

                //全員準備していたら、全員にゲーム開始を通知
                if (isReady)
                {
                    //Broadcast(x => x.OnGameStart());
                    //this.roomContext.Group.All.OnGameStart();//即開始

                    _ = StartCountdownAsync();//開始カウントダウン

                }
            }

        }

        public async Task StartGameAsync()
        {
            //！同時に生成しないよう「排他制御」
            lock (this.roomContext)
            {

                roomContext.GameState = GameState.Playing;
                roomContext.CurrentGameState = GameState.Playing;//全員Ready→試合開始

                // ★ ゲーム開始時刻を確定
                roomContext.GameStartTime = DateTime.UtcNow;

                // ゲーム開始通知
                roomContext.Group.All.OnGameStart();

                // タイマー開始
                _ = GameTimerLoop();
            }

        }

        private async Task GameTimerLoop()
        {
 
                while (roomContext.GameState == GameState.Playing)
                {
                    // 経過時間
                    var elapsed = DateTime.UtcNow - roomContext.GameStartTime;

                    // 残り時間（秒）
                    int remaining =
                        roomContext.GameTimeSeconds - (int)elapsed.TotalSeconds;

                    if (remaining <= 0)
                    {
                        // 時間切れ
                        roomContext.Group.All.OnTimeUpdate(0);
                        await EndGameAsync();
                        return;
                    }

                    // 全員に残り時間を通知
                    roomContext.Group.All.OnTimeUpdate(remaining);

                    // ★ 1秒待つ
                    await Task.Delay(1000);
                }
        }
        

        public async Task EndGameAsync()
        {
            // 全員の準備状態をリセット
            /*foreach (var user in roomContext.RoomUserDataList.Values)
            {
                user.IsReady = false;
            }

            // クライアントに通知
            //Broadcast(x => x.OnGameEnd());
            this.roomContext.Group.All.OnGameEnd();*/

            //！同時に生成しないよう「排他制御」
            lock (this.roomContext)
            {

                //ゲームが終わっていないのに終了しない
                if (roomContext.GameState != GameState.Playing)
                    return;

                roomContext.GameState = GameState.Result;
                roomContext.CurrentGameState = GameState.Result;
                //またはroomContext.Reset(); // ユーザーも状態も全初期化

                roomContext.Group.All.OnGameEnd();
            }
        }

        //開始カウントダウン
        private async Task StartCountdownAsync()
        {

                // 二重開始防止？
                if (roomContext.GameState != GameState.Waiting)
                    return;

                roomContext.GameState = GameState.Countdown;
                //roomContext.GameState = GameState.Waiting; // まだプレイ開始ではない

                for (int i = 3; i > 0; i--)
                {
                    roomContext.Group.All.OnCountdown(i);
                    await Task.Delay(1000);
                }

                await StartGameAsync();
        }

    }

}
