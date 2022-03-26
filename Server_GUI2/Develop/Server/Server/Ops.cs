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
    /// TODO: PalyerとOpsRecordを比較するためにPlayerを継承した。動作確認を念のため行う必要性あり。
    /// </summary>
    public class OpsRecord : Player, IEquatable<OpsRecord>, IComparable<OpsRecord>
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("BypassesPlayerLimit")]
        public bool BypassesPlayerLimit { get; set; }

        public OpsRecord(Player player, int opLevel, bool bypassesPlayerLimit = false) : base(player.Name, player.UUID)
        {
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
