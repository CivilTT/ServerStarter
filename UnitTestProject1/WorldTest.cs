using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Server_GUI2.Develop.Server.World;

namespace UnitTestProject1
{
    [TestClass]
    public class WorldTest
    {
        [TestMethod]
        public void WorldTestMethod()
        {
            // ワールド一覧を取得(ローカルとリンクされた状態)
            foreach (var i in WorldCollection.Instance.WorldWrappers.OrderBy(x => x.Version))
            {
                Console.WriteLine(i.DisplayName);
                foreach (var j in i.World.Datapacks.Datapacks)
                    Console.WriteLine("   " + j.Name);
            }
        }
    }
}
