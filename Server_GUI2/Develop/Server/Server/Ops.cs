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
    public class OpsRecord : IEquatable<OpsRecord>, IComparable<OpsRecord>
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("uuid")]
        public string UUID { get; protected set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("BypassesPlayerLimit")]
        public bool BypassesPlayerLimit { get; set; }

        [JsonIgnore]
        private Player _player { get; set; }
        [JsonIgnore]
        public Player Player
        {
            get
            {
                _player = _player ?? new Player(Name,UUID);
                return _player;
            }
        }
        public OpsRecord() { }

        public OpsRecord(Player player, int opLevel, bool bypassesPlayerLimit = false)
        {
            UUID = player.UUID;
            Name = player.Name;
            Level = opLevel;
            BypassesPlayerLimit = bypassesPlayerLimit;
        }

        public bool Equals(OpsRecord other)
        {
            return other.UUID == UUID;
        }

        public int CompareTo(OpsRecord other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
