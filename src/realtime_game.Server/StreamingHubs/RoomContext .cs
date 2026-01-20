using Cysharp.Runtime.Multicast;
using realtime_game.Shared.Interfaces.StreamingHubs;

namespace realtime_game.Server.StreamingHubs
{
    public enum GameState//ゲームの状況
    {
        Waiting,//ロビー
        Countdown,//開始カウントダウン
        Playing,//試合中
        Result//終了
    }

    public class RoomContext : IDisposable
    {
        public Guid Id { get; } // ルームID
        public string Name { get; } // ルーム名
        public IMulticastSyncGroup<Guid, IRoomHubReceiver> Group { get; } // グループ
        public Dictionary<Guid, RoomUserData> RoomUserDataList { get; } =
            new Dictionary<Guid, RoomUserData>(); // ユーザデータ一覧

        public GameState GameState = GameState.Waiting;//ゲームの状況
        public GameState CurrentGameState = GameState.Waiting;//現在のゲームの状態



        //その他、ルームのデータとして保存したいものをフィールドに追加していく

        // コンストラクタ
        public RoomContext(IMulticastGroupProvider groupProvider, string roomName)
        {
            Id = Guid.NewGuid();  	//ルーム毎のデータにIDを付けておく
            Name = roomName;  	//ルーム名をフィールドに保存
            Group = 			//グループを作成
                  groupProvider.GetOrAddSynchronousGroup<Guid, IRoomHubReceiver>(roomName);
        }

        public void Dispose()
        {
            Group.Dispose();
        }

        // ★ ゲーム制限時間（秒）
        public int GameTimeSeconds = 180;

        // ★ ゲーム開始時刻（UTC）
        public DateTime GameStartTime;

    }
}