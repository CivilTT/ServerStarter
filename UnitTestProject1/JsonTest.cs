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
            //var json = "{\"{ branch0}\": {\"type\": \"new\"},\"{branch1}\": {\"type\": \"vanilla\",\"version\": \"1.18.1\",\"using\": false,\"datapacks\": [],\"properties\": {\"{key}\": \"{value}\"},\"ops\": {},\"banned-ips\": [],\"banned-players\": []}}";

            //var w = JsonConvert.DeserializeObject<Dictionary<string, WorldState>>(json, new JsonWorldStateConverter());
            //var x = JsonConvert.SerializeObject(w,Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore});
            //Console.WriteLine(x);
        }
    }
}
