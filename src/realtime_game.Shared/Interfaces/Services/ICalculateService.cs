using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MagicOnion;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Interfaces.Services
{
    public interface ICalculateService : IService<ICalculateService>
    {
        //インターフェースに追加した3種のメソッドを実装
        //[ここにどのようなAPIを作るのか、関数形式で定義を作成する]

        // 『乗算API』二つの整数を引数で受け取り乗算値を返す
        UnaryResult<int> MulAsync(int x, int y);

        // 受け取った配列の値の合計を返す
        UnaryResult<int> SumAllAsync(int[] numList);
        // x + yを[0]に、x - yを[1]に、x * yを[2]に、x / yを[3]に入れて配列で返す
        UnaryResult<int[]> CalcForOperationAsync(int x, int y);
        // 小数の値3つをフィールドに持つNumberクラスを渡して、3つの値の合計値を返す
        UnaryResult<float> SumAllNumberAsync(Number numData);
    }
}

