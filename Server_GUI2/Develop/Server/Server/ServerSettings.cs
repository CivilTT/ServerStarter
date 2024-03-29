﻿using Server_GUI2.Develop.Server;
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
        public List<BannedPlayerRecord> BannedPlayers;

        [JsonProperty("banned_ips")]
        public List<BannedIpRecord> BannedIps;

        public ServerSettings()
        {
            ServerProperties = ServerProperty.GetUserDefault();
            Ops = new List<OpsRecord>();
            WhiteList = new List<Player>();
            BannedPlayers = new List<BannedPlayerRecord>();
            BannedIps = new List<BannedIpRecord>();
        }

        public ServerSettings(WorldPath path) {
            ServerProperties = path.ServerProperties.ReadAllText().SuccessFunc( x => new ServerProperty(x)).SuccessOrDefault(ServerProperty.GetUserDefault());
            Ops = path.Ops.ReadJson().SuccessOrDefault(new List<OpsRecord>());
            WhiteList = path.WhiteList.ReadJson().SuccessOrDefault(new List<Player>());
            BannedPlayers = path.BannedPlayers.ReadJson().SuccessOrDefault(new List<BannedPlayerRecord>());
            BannedIps = path.BannedIps.ReadJson().SuccessOrDefault(new List<BannedIpRecord>());
        }

        public void Save(VersionPath path)
        {
            path.ServerProperties.WriteAllText(ServerProperties.ExportProperty());
            path.Ops.WriteJson(Ops,indented:true);
            path.WhiteList.WriteJson(WhiteList,indented:true);
            path.BannedPlayers.WriteJson(BannedPlayers);
            path.BannedIps.WriteJson(BannedIps);
        }

        public void Save(WorldPath path)
        {
            path.ServerProperties.WriteAllText(ServerProperties.ExportProperty());
            path.Ops.WriteJson(Ops, indented: true);
            path.WhiteList.WriteJson(WhiteList, indented: true);
            path.BannedPlayers.WriteJson(BannedPlayers, indented: true);
            path.BannedIps.WriteJson(BannedIps, indented: true);
        }

        public void Load(VersionPath path)
        {
            path.ServerProperties.ReadAllText().SuccessAction( prop => ServerProperties = new ServerProperty(prop) );
            path.Ops.ReadJson().SuccessAction( ops => Ops = ops);
            path.WhiteList.ReadJson().SuccessAction( whiteList => WhiteList = whiteList);
            path.BannedPlayers.ReadJson().SuccessAction(bannedPlayers => BannedPlayers = bannedPlayers);
            path.BannedIps.ReadJson().SuccessAction(bannedIps => BannedIps = bannedIps);
        }
    }
}
