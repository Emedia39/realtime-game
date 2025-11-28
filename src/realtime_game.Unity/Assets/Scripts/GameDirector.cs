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
    User myself; //自分のユーザー情報を保持

    public InputField InputRoomName;//InputFieldで入力された文字列を取得
    public InputField InputUserId;//InputFieldで入力された文字列を取得

    async void Start()
    {
        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();

        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;
        //接続
        await roomModel.ConnectAsync();

        try
        {
            //ユーザー情報を取得
            myself = await userModel.GetUser(myUserId);//===！※(GameDirectorより)===
        }
        catch (Exception e)
        {
            Debug.Log("RegistUser failed");
            Debug.LogException(e);
        }
    }
    public async void JoinRoom()
    {
        string roomName = InputRoomName.text;

        if (!int.TryParse(InputUserId.text, out int userId))//intに変換
        {
            return;
        }

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

}

