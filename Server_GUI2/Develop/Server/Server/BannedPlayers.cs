using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2
{
    public class BannedPlayerRecord
    {
        [JsonProperty("uuid")]
        public string UUID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("created")]
        public string Created;

        [JsonProperty("source")]
        public string Source;

        [JsonProperty("expires")]
        public string Expires;

        [JsonProperty("reason")]
        public string Reason;
    }
}
