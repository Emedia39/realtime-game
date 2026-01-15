using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicOnion;
using System.Numerics;//Vector3で使う※Server/SharedでUnityEngineはNG

namespace realtime_game.Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// クライアントから呼び出す処理を実装するクラス用インターフェース
    /// </summary>
    public interface IRoomHub : IStreamingHub<IRoomHub, IRoomHubReceiver>
    {
        // [サーバーに実装]
        // [クライアントから呼び出す]

        // ユーザー入室
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        // ユーザー退室
        Task LeaveAsync();

        //ユーザーの位置同期(場所+ベクトルの回転)
        Task MoveAsync(Vector3 pos, Quaternion euler);//(位置,回転)

        // 接続ID取得
        public Task<Guid> GetConnectionId();
    }

}
