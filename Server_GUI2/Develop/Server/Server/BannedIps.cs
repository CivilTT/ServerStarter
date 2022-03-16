using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2
{
    public class BannedIpRecord
    {
        [JsonProperty("ip")]
        public string Ip;

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
