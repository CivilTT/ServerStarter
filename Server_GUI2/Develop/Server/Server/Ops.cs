using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2
{
    /// <summary>
    /// ops.jsonのレコード
    /// </summary>
    public class OpsRecord
    {
        [JsonProperty("uuid")]
        string UUID;
        
        [JsonProperty("name")]
        string Name;

        [JsonProperty("level")]
        int Level;

        [JsonProperty("BypassesPlayerLimit")]
        bool bypassesPlayerLimit;
    }
}
