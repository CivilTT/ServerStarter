﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            foreach (var g in WorldCollection.Instance.WorldWrappers)
            {
                Console.WriteLine(g);
            }
        }
    }
}
