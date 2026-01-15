using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using realtime_game.Shared.Interfaces.StreamingHubs;
using System;
using UnityEngine;

//using System.Numerics;
using System.Threading.Tasks;//Taskのため

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannelx channel;
    private IRoomHub roomHub;

    //　接続ID
    public Guid ConnectionId { get; set; }

    //　ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }
    // ユーザー切断通知
    public Action<Guid> OnLeftUser { get; set; }
    // ユーザー切断通知
    public Action OnLeftUserAll { get; set; }

    // 他ユーザー移動+回転通知
    public Action<Guid, Vector3, Quaternion> OnMoveCharacter { get; set; }//★ 他ユーザー移動通知※Guid,Vector3, Quaternion

    public Action GameStarted;//ゲーム準備完了/開始通知
    public Action GameEnded;//ゲーム終了通知

    //　MagicOnion接続処理
    public async UniTask ConnectAsync()
    {
        channel = GrpcChannelx.ForAddress(ServerURL);
        roomHub = await StreamingHubClient.
             ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
        this.ConnectionId = await roomHub.GetConnectionId();
    }

    //　MagicOnion切断処理
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    //　破棄処理 
    async void OnDestroy()
    {
        await DisconnectAsync();
    }

    //　入室
    public async UniTask JoinAsync(string roomName, int userId)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, userId);
        foreach (var user in users)
        {
            if (OnJoinedUser != null)
            {
                OnJoinedUser(user);
            }
        }
    }

    //　入室通知 (IRoomHubReceiverインタフェースの実装)
    public void OnJoin(JoinedUser user)
    {
        if (OnJoinedUser != null)
        {
            OnJoinedUser(user);
        }
    }

    // 退室通知 (IRoomHubReceiverインタフェースの実装)
    public void OnLeave(Guid connectionId)
    {
        if (OnLeftUser != null)
        {
            OnLeftUser(connectionId);
        }
    }

    // 退室
    public async UniTask LeaveAsync()
    {
        await roomHub.LeaveAsync();
        Debug.Log("退室完了");

        // 自分以外のオブジェクトを削除する
        if (OnLeftUserAll != null)
        {
            OnLeftUserAll();
        }

    }

    //位置・回転を送信
    public Task MoveAsync(Vector3 pos, Quaternion euler)
    {
        // Unity → Numerics へ変換
        var nPos = new System.Numerics.Vector3(pos.x, pos.y, pos.z);
        var nRot = new System.Numerics.Quaternion(euler.x, euler.y, euler.z, euler.w);

        //「サーバーの関数呼び出し」
        return roomHub.MoveAsync(nPos, nRot);
    }

    // 位置・回転を受信
    public void OnMove(Guid connectionId,
                       System.Numerics.Vector3 pos,
                       System.Numerics.Quaternion euler)
    {
        // Numerics → Unity に変換
        var uPos = new UnityEngine.Vector3(pos.X, pos.Y, pos.Z);
        var uRot = new UnityEngine.Quaternion(euler.X, euler.Y, euler.Z, euler.W);

        OnMoveCharacter?.Invoke(connectionId, uPos, uRot);//?を使い、安全チェック
    }

    public void OnGameStart()
    {
        Debug.Log("ゲーム開始！");
        GameStarted?.Invoke();
    }
    public void OnGameEnd()
    {
        Debug.Log("ゲーム終了！");
        GameEnded?.Invoke();
    }
    // Ready送信
    public Task ReadyAsync()
    {
        return roomHub.ReadyAsync();
    }

}
