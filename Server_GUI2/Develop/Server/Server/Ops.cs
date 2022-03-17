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
        public string UUID { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("BypassesPlayerLimit")]
        public bool BypassesPlayerLimit { get; set; }

        public OpsRecord(Player player, int opLevel, bool bypassesPlayerLimit = false)
        {
            Name = player.Name;
            UUID = player.UUID;
            Level = opLevel;
            BypassesPlayerLimit = bypassesPlayerLimit;
        }

        public bool Equals(OpsRecord other)
        {
            return other.UUID == UUID;
        }
    }
}
