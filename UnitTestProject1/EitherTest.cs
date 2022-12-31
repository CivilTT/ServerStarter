using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2.Develop.Util;
using Server_GUI2.Util;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class EitherTest
    {
        [TestMethod]
        public void EitherTestMethod()
        {            
            // 偶数の場合半分にして奇数の場合エラー
            Either<int,string> Half(int num)
            {
                if (num % 2 == 0)
                    return new Success<int, string>(num / 2);
                else
                    return new Failure<int, string>($"Error: {num} is not even number.");
            }

            var result = Half(9);

            // エラーの場合にデフォルト値を適用
            result.SuccessOrDefault(-1).WriteLine();

            // 成功の場合にデフォルト値を適用
            result.FailureOrDefault("Success:").WriteLine();

            // 成功の場合に関数を適用
            result.SuccessFunc(x => x * 2).SuccessOrDefault(-1).WriteLine();

            // 失敗の場合にvoidな関数を適用
            result.FailureAction(x => { x.WriteLine(); });
        }
    }
}
