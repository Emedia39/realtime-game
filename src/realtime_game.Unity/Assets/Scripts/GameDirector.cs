using UnityEngine; // Mathf.Pow関数を使用するのに必要

using realtime_game.Shared.Interfaces.StreamingHubs;
using realtime_game.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;//InputFieldで入力された文字列を取得

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
        //接続
        await roomModel.ConnectAsync();

        // 弱者が勝利した場合の増減レート
        //Debug.Log($"弱者の勝利{CalcRating(1200, 1600)}");
        // 強者が勝利した場合の増減レート
        //Debug.Log($"強者の勝利{CalcRating(1600, 1200)}");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {//ESCキーを押した場合
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームを強制終了
#else//ビルドの場合
        Application.Quit();
#endif
        }
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
            if (userId >= 1 && userId <= 3)//InputUserId内のテキストが1～3の時
            {
                //入室
                await roomModel.JoinAsync(roomName, userId);
                Debug.Log("C");
            }
            else
            {
                Debug.Log("B");
            }
        }
        else
        {
            Debug.Log("A");
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

    // 勝者と敗者のレートから、増減レートを計算
    /*private float CalcRating(int winnerRate, int loserRate)
    {
        const int K = 32; // レート計算用の定数。これが大きくなれば増減レートも大きくなる
        return K / Mathf.Pow(10, ((winnerRate - loserRate) / 400f) + 1);
    }*/

}

