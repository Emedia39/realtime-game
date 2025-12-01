using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnion;
using realtime_game.Shared.Interfaces.Services;
using System.Threading.Tasks;
using realtime_game.Shared.Models.Entities;
using System;
using UnityEngine;

public class UserModel : BaseModel
{
    private int userId;  //ìoò^ÉÜÅ[ÉUÅ[ID

    public async UniTask<bool> RegistUserAsync(string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        try
        {  //ìoò^ê¨å˜
            userId = await client.RegistUserAsync(name);
            return true;
        }
        catch (RpcException e)
        {  //ìoò^é∏îs
            Debug.Log(e);
            return false;
            
        }
    }

    internal async Task<User> GetUser(int myUserId)//===ÅIÅ¶(GameDirectorÇÊÇË)===
    {
        //
        var channel = GrpcChannelx.ForAddress("http://localhost:5244"); //ServerURL
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {
            // MagicOnion ÇÃ RPC ÇÉRÅ[Éã
            var user = await client.GetUserByIdAsync(myUserId);
            return user;
        }
        catch (RpcException e)
        {
            UnityEngine.Debug.LogError($"GetUser failed: {e.Status.Detail}");
            throw;
        }
    }
    
}
