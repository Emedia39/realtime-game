using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MagicOnion;
using System.Numerics;//Vector3で使う※Server/SharedでUnityEngineはNG

namespace realtime_game.Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        // [クライアントに実装]
        // [サーバーから呼び出す]

        // ユーザーの入室通知
        void OnJoin(JoinedUser user);

        // ！ユーザーの退出通知
        void OnLeave(Guid connectionId);
        //これ取ればIRoomHubReceiverが正常になるけど…

        //ユーザーの位置同期(接続ID,場所+ベクトルの回転)
        void OnMove(Guid connectionId, Vector3 pos, Quaternion euler);//(接続ID,位置,回転)


        //ゲーム準備完了/開始通知
        void OnGameStart();

        //ゲーム終了通知
        void OnGameEnd();

    }
}
