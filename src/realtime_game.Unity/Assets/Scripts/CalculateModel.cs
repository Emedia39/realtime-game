using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using UnityEngine;
using Shared.Interfaces.Services;

public class CalculateModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:5244";
    async void Start()
    {
        int result = await Mul(100, 323);
        Debug.Log(result);

        int[] ArrayValueName;
        ArrayValueName = new int[3]{1,2,3};
        //SumAll‚ÌŒ‹‰Ê‚ðŽó‚¯Žæ‚è‚½‚¢
        await SumAll(ArrayValueName);
    }

    public async UniTask<int> Mul(int x, int y)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.MulAsync(x, y);
        return result;
    }
    public async UniTask<int> SumAll(int[] numList)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.SumAllAsync(numList);
        return result; 
    }

    public async UniTask<int[]> CalcForOperation(int x ,int y)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.CalcForOperationAsync(x , y);
        return result;
    }

    public async UniTask<float> SumAllNumber(Number numData)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.SumAllNumberAsync(numData);
        return result;
    }

}

