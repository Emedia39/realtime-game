using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using realtime_game.Shared.Interfaces.StreamingHubs;
using System;
using UnityEngine;

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

    // 他ユーザー移動通知
    public Action<Guid, Vector3> OnMoveUser { get; set; }// ★

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

    // ★他ユーザー移動情報
    public void OnMove(Guid connectionId, System.Numerics.Vector3 pos)
    {
        // 自分自身は無視
        if (connectionId == this.ConnectionId)
            return;

        // Numerics → Unity へ変換
        Vector3 unityPos = new Vector3(pos.X, pos.Y, pos.Z);

        OnMoveUser?.Invoke(connectionId, unityPos);
    }

}
