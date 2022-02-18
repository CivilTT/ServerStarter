using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using Server_GUI2.Util;

namespace UnitTestProject1
{
    [TestClass]
    public class Logtest
    {
        [TestMethod]
        public void LogTest1()
        {
            // Console.WriteLine(a) 的な奴
            // 任意のobjectに適用可能
            var a = 100;
            a.WriteLine();


            // null の時は"null"がでる
            string n = null;
            n.WriteLine();


            // Listの中身を出力
            var l = new List<string> {
                "a",
                "b",
                "c",
                "d"
            };
            l.WriteLine();


            // Dictの中身を出力
            var d = new Dictionary<string, string> {
                {"a","A"},
                {"b","B"},
                {"c","C"},
                {"d","D"}
            };
            d.WriteLine();


            // ネストしていてもOK
            var ld = new List<Dictionary<string, string>> { d,d,d,d };
            ld.WriteLine();


            // もっとネストしていてもOK
            var dld = new Dictionary<string, List<Dictionary<string, string>>> {
                {"a",ld },
                {"b",ld },
                {"c",ld },
                {"d",ld }
            };
            dld.WriteLine();
        }
    }
}
