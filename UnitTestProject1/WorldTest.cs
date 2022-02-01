using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Server_GUI2.Develop.Server.World;

namespace UnitTestProject1
{
    [TestClass]
    public class WorldTest
    {
        [TestMethod]
        public void WorldTestMethod()
        {
            foreach (var w in WorldFactory.Instance.Worlds)
            {
                Console.WriteLine(w.Name);
            }
        }
    }
}
