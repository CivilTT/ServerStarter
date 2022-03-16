using Server_GUI2.Develop.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2
{
    /// <summary>
    /// server.properties
    /// ops.json
    /// banned-ips.json
    /// banned-players.json
    /// whitelist.json
    /// をひとまとめにしたクラス
    /// </summary>
    public class ServerSettings
    {
        [JsonProperty("properties")]
        public ServerProperty ServerProperties;

        [JsonProperty("ops")]
        public List<OpsRecord> Ops;

        [JsonProperty("whitelist")]
        public List<Player> WhiteList;

        [JsonProperty("banned_players")]
        public string BannedPlayers;

        [JsonProperty("banned_ips")]
        public string BannedIps;

        public ServerSettings()
        {
            ServerProperties = new ServerProperty();
            Ops = new List<OpsRecord>();
            WhiteList = new List<Player>();
            BannedPlayers = "";
            BannedIps = "";
        }

        public ServerSettings(WorldPath path) {
            ServerProperties = path.ServerProperties.ReadAllText().SuccessFunc( x => new ServerProperty(x)).SuccessOrDefault(new ServerProperty());
            Ops = path.Ops.ReadJson().SuccessOrDefault(new List<OpsRecord>());
            WhiteList = path.WhiteList.ReadJson().SuccessOrDefault(new List<Player>());
            BannedPlayers = path.BannedPlayers.ReadAllText().SuccessOrDefault("");
            BannedIps = path.BannedIps.ReadAllText().SuccessOrDefault("");
        }

        public void Save(VersionPath path)
        {
            path.ServerProperties.WriteAllText(ServerProperties.ExportProperty());
            path.Ops.WriteJson(Ops,indented:true);
            path.WhiteList.WriteJson(WhiteList,indented:true);
            path.BannedPlayers.WriteAllText(BannedPlayers);
            path.BannedIps.WriteAllText(BannedIps);
        }

        public void Save(WorldPath path)
        {
            path.ServerProperties.WriteAllText(ServerProperties.ExportProperty());
            path.Ops.WriteJson(Ops, indented: true);
            path.WhiteList.WriteJson(WhiteList, indented: true);
            path.BannedPlayers.WriteAllText(BannedPlayers);
            path.BannedIps.WriteAllText(BannedIps);
        }
    }
}
