using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Server_GUI2.Develop.Server.World;
using Server_GUI2;

namespace UnitTestProject1
{
    [TestClass]
    public class WorldTest
    {
        [TestMethod]
        public void WorldTestMethod()
        {
            // ワールド一覧を取得(ローカルとリンクされた状態)
            foreach (var i in WorldCollection.Instance.Worlds)
            {
                Console.WriteLine(i.DisplayName);
                foreach (var j in i.Datapacks.Datapacks)
                    Console.WriteLine("   " + j.Name);
            }
            //var ver = VersionFactory.Instance.GetVersionFromName("1.18.1");
            //WorldCollection.Instance.WorldWrappers[1].WrapRun(ver, x => { } );

        }
    }
}
