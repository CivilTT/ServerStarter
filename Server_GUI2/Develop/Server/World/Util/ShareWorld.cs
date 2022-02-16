using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2.Develop.Server
{
    public class ShareWorld
    {

        [JsonProperty("DefaultProperty")]
        public string gitAccountName;

        [JsonProperty("DefaultProperty")]
        public string gitAccountMail;

        //public ShareWorld(string name, Version ver)
        //{
        //    Name = name;
        //    version = ver;
        //}
    }
}
