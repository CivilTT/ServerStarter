using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Server_GUI2;
using Server_GUI2.Develop.Server;
using Server_GUI2.Util;

namespace UnitTestProject1
{
    [TestClass]
    public class DirectoryTest
    {
        [TestMethod]
        public void DirectoryTestMethod()
        {
            var p = ServerGuiPath.Instance.WorldData.GetVersionDirectory("Spigot_1.18.2");
            p.Exists.WriteLine();
            p.Create();
            p.Exists.WriteLine();
        }
    }
}
