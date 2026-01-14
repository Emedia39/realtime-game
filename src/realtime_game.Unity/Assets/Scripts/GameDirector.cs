using UnityEngine; // Mathf.Pow関数を使用するのに必要

using realtime_game.Shared.Interfaces.StreamingHubs;
using realtime_game.Shared.Models.Entities;
using System;
using System.Collections.Generic;

using UnityEngine.UI;
using System.Linq;//InputFieldで入力された文字列を取得

public class GameDirector : MonoBehaviour
{
    //[SerializeField] RoomModel roomModel;
    [SerializeField] GameObject characterPrefab;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    RoomModel roomModel;
    UserModel userModel;

    int myUserId = 1;//自分のユーザーID

    [SerializeField] InputField InputRoomName;//InputFieldで入力された文字列を取得
    [SerializeField] InputField InputUserId;//InputFieldで入力された文字列を取得

    async void Start()
    {
        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();

        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;

        // ユーザーが退室した時にOnLeftUserメソッドを実行できるよう、モデルに登録しておく
        roomModel.OnLeftUser += this.OnLeftUser;
        // ユーザーが退室した時にOnLeftUserAllメソッドを実行できるよう、モデルに登録しておく
        roomModel.OnLeftUserAll += this.OnLeftUserAll;

        //接続
        await roomModel.ConnectAsync();

        // 弱者が勝利した場合の増減レート
        //Debug.Log($"弱者の勝利{CalcRating(1200, 1600)}");
        // 強者が勝利した場合の増減レート
        //Debug.Log($"強者の勝利{CalcRating(1600, 1200)}");

    }

    private void Update()
    {
        
    }

    public async void JoinRoom()
    {
        string roomName = InputRoomName.text;

        if (!int.TryParse(InputUserId.text, out int userId))//intに変換
        {
            return;
        }

        myUserId = userId;

        if (roomName == "sampleRoom")//InputRoomName内のテキストが未入力またはsampleRoomのとき
        {
            if (userId >= 1 && userId <= 4)//InputUserId内のテキストが1～3の時
            {
                //入室
                await roomModel.JoinAsync(roomName, userId);
                Debug.Log("C：成功");
            }
            else
            {
                Debug.Log("B：惜しい");
            }
        }
        else
        {
            Debug.Log("A：失敗");
        }

    }

    // ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        // すでに表示済みのユーザーは追加しない
        if (characterList.ContainsKey(user.ConnectionId))
        {
            return;
        }

        // 自分は追加しない
        if (user.UserData.Id == myUserId)
        {
            return;
        }

        GameObject characterObject = Instantiate(characterPrefab);  //インスタンス生成
        characterObject.transform.position = new Vector3(0, 0, 0); // 配置位置設定
        characterList[user.ConnectionId] = characterObject;  //フィールドで保持
    }

    public async void LeaveRoom()
    {
        // ルーム名チェック
        //Text text = GameObject.Find("InputRoomName").gameObject.GetComponent<Text>();
        //string roomName = text.text;
        //if (roomName == "")
        //{
        //    // ルーム名が入力されていない場合は何もしない
        //    return;
        //}

        // 退室
        await roomModel.LeaveAsync();
    }

    // ユーザーが退室した時の処理
    private void OnLeftUser(Guid connectionId)
    {
        // いない人は退室できない
        if (!characterList.ContainsKey(connectionId))
        {
            return;
        }

        Destroy(characterList[connectionId]); // 対象のオブジェクトを削除
        characterList.Remove(connectionId); // リストから対象のデータを削除
    }
    // 自分が退室した時の処理
    private void OnLeftUserAll()
    {
        // 自分以外のオブジェクトを削除する
        List<Guid> connectionIdList = characterList.Keys.ToList();
        foreach (Guid connectionId in connectionIdList)
        {
            // 一人分の退室処理
            OnLeftUser(connectionId);
        }
    }

    // 自分以外のユーザーの移動を反映
    private void OnMoveUser(Guid connectionId, Vector3 pos, Quaternion quaternion)
    {
        // いない人は移動できない
        if (!characterList.ContainsKey(connectionId))
        {
            return;
        }

        // DOTweenを使うことでなめらかに動く！
        //characterList[connectionId].transform.DOMove(pos, 0.1f);
        characterList[connectionId].transform.position = pos;
    }



    // 勝者と敗者のレートから、増減レートを計算
    /*private float CalcRating(int winnerRate, int loserRate)
    {
        const int K = 32; // レート計算用の定数。これが大きくなれば増減レートも大きくなる
        return K / Mathf.Pow(10, ((winnerRate - loserRate) / 400f) + 1);
    }*/

}

