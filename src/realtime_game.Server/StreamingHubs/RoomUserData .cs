using realtime_game.Shared.Interfaces.StreamingHubs;

using System.Numerics;//Vector3で使う※Server/SharedでUnityEngineはNG

namespace realtime_game.Server.StreamingHubs
{
    // ルーム内のユーザー単体の情報
    public class RoomUserData
    {
        public JoinedUser JoinedUser;
        public Vector3 pos;//移動(場所)
        public Quaternion euler;//回転(ベクトルの回転)

        public bool IsReady { get; set; } = false;//準備完了フラグ

    }

}
