using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2;
using Newtonsoft.Json;

namespace UnitTestProject1
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void GitTestMethod()
        {
            var property = new ServerProperty();
            property.Difficulty = "hard";
            Console.WriteLine(property.ExportProperty(true));
            //Console.WriteLine(JsonConvert.SerializeObject(
            //    property,
            //    Formatting.None,
            //    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include }
            //    ));
        }
    }
}
