using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server;
using Newtonsoft.Json;

namespace UnitTestProject1
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void GitTestMethod()
        {
            WorldName = "test";
            Console.WriteLine(WorldName);
        }

        public string _worldName;
        public string WorldName
        {
            get => _worldName;
            set => _worldName = value;
        }
    }
}
