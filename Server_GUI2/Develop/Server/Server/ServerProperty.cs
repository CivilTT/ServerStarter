﻿using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;


namespace Server_GUI2
{
    /// <summary>
    /// server.propertiesの内容
    /// 
    /// Json.JsonConvert.DeserializeObject<ServerProperty>でJsonデシリアライズ
    /// Json.JsonConvert.SerializeObjectでJsonシリアライズ
    /// </summary>
    public class ServerProperty
    {
        /// <summary>
        /// デフォルト値のserver.propertiesを生成
        /// </summary>
        public ServerProperty() { }

        /// <summary>
        /// server.propertiesの内容を引数にとる
        /// </summary>
        public ServerProperty(string properties)
        {
            ReadProperty(properties);
        }

        /// <summary>
        /// 参照渡しを切って新しいインスタンスの生成
        /// </summary>
        public ServerProperty(ServerProperty property)
        {
            BoolOption = new SortedDictionary<string, bool>(property.BoolOption);
            StringOption = new SortedDictionary<string, string>(property.StringOption);
        }

        /// <summary>
        /// ユーザー設定のデフォルト設定のコピーインスタンスを返す
        /// </summary>
        public static ServerProperty GetUserDefault()
        {
            return UserSettings.Instance.userSettings.DefaultProperties.Copy();
        }

        public ServerProperty Copy()
        {
            return new ServerProperty(this);
        }

        /// <summary>
        /// server.propertiesの内容を引数にとる
        /// </summary>
        private void ReadProperty(string content)
        {
            // MoreSettingsが複数回呼び出されても表示内容を更新
            foreach (var l in content.Split('\n'))
            {
                var line = l.Trim();

                var splitIndex = line.IndexOf("=");

                //空行 or "="がない or #で始まるものはスキップ
                if (line == "" || splitIndex == -1 || line.Substring(0, 1) == "#")
                {
                    continue;
                }

                string indexName = line.Substring(0, splitIndex).Trim();
                string strValue = (line == (indexName + "=")) ? "" : line.Substring(splitIndex + 1).Trim();

                if (strValue == "true" || strValue == "false" || strValue == "True" || strValue == "False")
                {
                    BoolOption[indexName] = Convert.ToBoolean(strValue);
                }
                else
                {
                    StringOption[indexName] = strValue;
                }
            }
        }
        public string ExportProperty(bool minify = false)
        {
            var result = new List<string>();
            var defaultProperty = new ServerProperty();
            foreach (var item in StringOption)
            {
                if (!minify || defaultProperty.StringOption[item.Key] != item.Value)
                    result.Add($"{item.Key}={item.Value}");
            }
            foreach (KeyValuePair<string, bool> item in BoolOption)
            {
                if (!minify || defaultProperty.BoolOption[item.Key] != item.Value)
                    result.Add($"{item.Key}={item.Value}");
            }
            return string.Join("\n", result);
        }


        [JsonIgnore]
        public SortedDictionary<string, string> StringOption { get; } = new SortedDictionary<string, string>()
        {
            {"view-distance","10"},
            {"resource-pack-prompt",""},
            {"server-ip",""},
            {"rcon.port","25575"},
            {"gamemode","survival"},
            {"server-port","25565"},
            {"op-permission-level","4"},
            {"resource-pack",""},
            {"entity-broadcast-range-percentage","100"},
            {"level-name",""},
            {"level-type", "default"},
            {"player-idle-timeout","0"},
            {"rcon.password",""},
            {"motd","A Minecraft Server"},
            {"query.port","25565"},
            {"rate-limit","0"},
            {"function-permission-level","2"},
            {"difficulty","easy"},
            {"network-compression-threshold","256"},
            {"text-filtering-config",""},
            {"max-tick-time","60000"},
            {"max-players","20"},
            {"resource-pack-sha1",""},
            {"spawn-protection","16"},
            {"max-world-size","29999984"},
            {"level-seed", ""}
        };

        [JsonIgnore]
        public SortedDictionary<string, bool> BoolOption { get; } = new SortedDictionary<string, bool>()
        {
            {"broadcast-rcon-to-ops",true},
            {"enable-jmx-monitoring",false},
            {"allow-nether",true},
            {"enable-command-block",false},
            {"enable-rcon",false},
            {"sync-chunk-writes",true},
            {"enable-query",false},
            {"prevent-proxy-connections",false},
            {"force-gamemode",false},
            {"hardcore",false},
            {"white-list",false},
            {"broadcast-console-to-ops",true},
            {"pvp",true},
            {"spawn-npcs",true},
            {"spawn-animals",true},
            {"snooper-enabled",true},
            {"require-resource-pack",false},
            {"spawn-monsters",true},
            {"enforce-whitelist",false},
            {"use-native-transport",true},
            {"enable-status",true},
            {"online-mode",true},
            {"allow-flight",false}
        };

        [JsonIgnore]
        public static Dictionary<string, string> Descriptions { get; } = new Dictionary<string, string>()
        {
            {"view-distance",Properties.Resources.P_ViewDistance},
            {"resource-pack-prompt",""},
            {"server-ip",Properties.Resources.P_ServerIP},
            {"rcon.port",Properties.Resources.P_RconPort},
            {"gamemode",Properties.Resources.P_Gamemode},
            {"server-port",Properties.Resources.P_ServerPort},
            {"op-permission-level",Properties.Resources.P_OpPermissionLevel},
            {"resource-pack",Properties.Resources.P_ResourcePack},
            {"entity-broadcast-range-percentage",Properties.Resources.P_EntityBroadcastRangePercentage},
            {"level-name",Properties.Resources.P_LevelName},
            {"level-type", Properties.Resources.P_LevelType},
            {"player-idle-timeout",Properties.Resources.P_PlayerIdleTimeout},
            {"rcon.password",Properties.Resources.P_RconPassword},
            {"motd",Properties.Resources.P_Motd},
            {"query.port",Properties.Resources.P_QueryPort},
            {"rate-limit",Properties.Resources.P_RateLimit},
            {"function-permission-level",Properties.Resources.P_FunctionPermissionLevel},
            {"difficulty", Properties.Resources.P_Difficult},
            {"network-compression-threshold",Properties.Resources.P_NetworkCompressionThreshold},
            {"text-filtering-config",Properties.Resources.P_TextFilteringConfig},
            {"max-tick-time",Properties.Resources.P_MaxtickTime},
            {"max-players",Properties.Resources.P_MaxPlayers},
            {"resource-pack-sha1",Properties.Resources.P_ResourcePacksha1},
            {"spawn-protection",Properties.Resources.P_SpawnProtection},
            {"max-world-size",Properties.Resources.P_MaxWorldSize},
            {"broadcast-rcon-to-ops",Properties.Resources.P_BroadcastRconToOps},
            {"enable-jmx-monitoring",Properties.Resources.P_EnableJMXmonitoring},
            {"allow-nether",Properties.Resources.P_AllowNether},
            {"enable-command-block",Properties.Resources.P_EnableCommandBlock},
            {"enable-rcon",Properties.Resources.P_EnableRcon},
            {"sync-chunk-writes",Properties.Resources.P_SyncChunkWrites},
            {"enable-query",Properties.Resources.P_EnableQuery},
            {"prevent-proxy-connections",Properties.Resources.P_PreventProxyConnections},
            {"force-gamemode",Properties.Resources.P_ForceGamemode},
            {"hardcore",Properties.Resources.P_Hardcore},
            {"white-list",Properties.Resources.P_WhiteList},
            {"broadcast-console-to-ops",Properties.Resources.P_BroadcastConsoleToOps},
            {"pvp",Properties.Resources.P_Pvp},
            {"spawn-npcs",Properties.Resources.P_SpawnNpcs},
            {"spawn-animals",Properties.Resources.P_SpawnAnimals},
            {"snooper-enabled",Properties.Resources.P_SnooperEnabled},
            {"require-resource-pack",Properties.Resources.P_RequireResourcePack},
            {"spawn-monsters",Properties.Resources.P_SpawnMonsters},
            {"enforce-whitelist",Properties.Resources.P_EnforceWhiteList},
            {"use-native-transport",Properties.Resources.P_UseNativeTransport},
            {"enable-status",Properties.Resources.P_EnableStatus},
            {"online-mode",Properties.Resources.P_OnlineMode},
            {"allow-flight", Properties.Resources.P_AllowFlight},
            {"level-seed", Properties.Resources.P_LevelSeed}
        };

        [JsonProperty("broadcast-rcon-to-ops")]
        [DefaultValue(true)]
        public bool BroadcastRconToOps
        {
            get { return BoolOption["broadcast-rcon-to-ops"]; }
            set { BoolOption["broadcast-rcon-to-ops"] = value; }
        }

        [JsonProperty("enable-jmx-monitoring")]
        [DefaultValue(false)]
        public bool EnableJmxMonitoring
        {
            get { return BoolOption["enable-jmx-monitoring"]; }
            set { BoolOption["enable-jmx-monitoring"] = value; }
        }

        [JsonProperty("view-distance")]
        [DefaultValue("10")]
        public string ViewDistance
        {
            get { return StringOption["view-distance"]; }
            set { StringOption["view-distance"] = value; }
        }

        [JsonProperty("resource-pack-prompt")]
        [DefaultValue("")]
        public string ResourcePackPrompt
        {
            get { return StringOption["resource-pack-prompt"]; }
            set { StringOption["resource-pack-prompt"] = value; }
        }

        [JsonProperty("server-ip")]
        [DefaultValue("")]
        public string ServerIp
        {
            get { return StringOption["server-ip"]; }
            set { StringOption["server-ip"] = value; }
        }

        [JsonProperty("rcon.port")]
        [DefaultValue("25575")]
        public string RconPort
        {
            get { return StringOption["rcon.port"]; }
            set { StringOption["rcon.port"] = value; }
        }

        [JsonProperty("allow-nether")]
        [DefaultValue(true)]
        public bool AllowNether
        {
            get { return BoolOption["allow-nether"]; }
            set { BoolOption["allow-nether"] = value; }
        }

        [JsonProperty("enable-command-block")]
        [DefaultValue(false)]
        public bool EnableCommandBlock
        {
            get { return BoolOption["enable-command-block"]; }
            set { BoolOption["enable-command-block"] = value; }
        }

        [JsonProperty("gamemode")]
        [DefaultValue("survival")]
        public string Gamemode
        {
            get { return StringOption["gamemode"]; }
            set { StringOption["gamemode"] = value; }
        }

        [JsonProperty("server-port")]
        [DefaultValue("25565")]
        public string ServerPort
        {
            get { return StringOption["server-port"]; }
            set { StringOption["server-port"] = value; }
        }

        [JsonProperty("enable-rcon")]
        [DefaultValue(false)]
        public bool EnableRcon
        {
            get { return BoolOption["enable-rcon"]; }
            set { BoolOption["enable-rcon"] = value; }
        }

        [JsonProperty("sync-chunk-writes")]
        [DefaultValue(true)]
        public bool SyncChunkWrites
        {
            get { return BoolOption["sync-chunk-writes"]; }
            set { BoolOption["sync-chunk-writes"] = value; }
        }

        [JsonProperty("enable-query")]
        [DefaultValue(false)]
        public bool EnableQuery
        {
            get { return BoolOption["enable-query"]; }
            set { BoolOption["enable-query"] = value; }
        }

        [JsonProperty("op-permission-level")]
        [DefaultValue("4")]
        public string OpPermissionLevel
        {
            get { return StringOption["op-permission-level"]; }
            set { StringOption["op-permission-level"] = value; }
        }

        [JsonProperty("prevent-proxy-connections")]
        [DefaultValue(false)]
        public bool PreventProxyConnections
        {
            get { return BoolOption["prevent-proxy-connections"]; }
            set { BoolOption["prevent-proxy-connections"] = value; }
        }

        [JsonProperty("resource-pack")]
        [DefaultValue("")]
        public string ResourcePack
        {
            get { return StringOption["resource-pack"]; }
            set { StringOption["resource-pack"] = value; }
        }

        [JsonProperty("entity-broadcast-range-percentage")]
        [DefaultValue("100")]
        public string EntityBroadcastRangePercentage
        {
            get { return StringOption["entity-broadcast-range-percentage"]; }
            set { StringOption["entity-broadcast-range-percentage"] = value; }
        }

        [JsonProperty("level-name")]
        [DefaultValue("")]
        public string LevelName
        {
            get { return StringOption["level-name"]; }
            set { StringOption["level-name"] = value; }
        }

        [JsonProperty("level-type")]
        [DefaultValue("default")]
        public string LevelType
        {
            get { return StringOption["level-type"]; }
            set { StringOption["level-type"] = value; }
        }

        [JsonProperty("player-idle-timeout")]
        [DefaultValue("0")]
        public string PlayerIdleTimeout
        {
            get { return StringOption["player-idle-timeout"]; }
            set { StringOption["player-idle-timeout"] = value; }
        }

        [JsonProperty("rcon.password")]
        [DefaultValue("")]
        public string RconPassword
        {
            get { return StringOption["rcon.password"]; }
            set { StringOption["rcon.password"] = value; }
        }

        [JsonProperty("motd")]
        [DefaultValue("A Minecraft Server")]
        public string Motd
        {
            get { return StringOption["motd"]; }
            set { StringOption["motd"] = value; }
        }

        [JsonProperty("query.port")]
        [DefaultValue("25565")]
        public string QueryPort
        {
            get { return StringOption["query.port"]; }
            set { StringOption["query.port"] = value; }
        }

        [JsonProperty("force-gamemode")]
        [DefaultValue(false)]
        public bool ForceGamemode
        {
            get { return BoolOption["force-gamemode"]; }
            set { BoolOption["force-gamemode"] = value; }
        }

        [JsonProperty("rate-limit")]
        [DefaultValue("0")]
        public string RateLimit
        {
            get { return StringOption["rate-limit"]; }
            set { StringOption["rate-limit"] = value; }
        }

        [JsonProperty("hardcore")]
        [DefaultValue(false)]
        public bool Hardcore
        {
            get { return BoolOption["hardcore"]; }
            set { BoolOption["hardcore"] = value; }
        }

        [JsonProperty("white-list")]
        [DefaultValue(false)]
        public bool WhiteList
        {
            get { return BoolOption["white-list"]; }
            set { BoolOption["white-list"] = value; }
        }

        [JsonProperty("broadcast-console-to-ops")]
        [DefaultValue(true)]
        public bool BroadcastConsoleToOps
        {
            get { return BoolOption["broadcast-console-to-ops"]; }
            set { BoolOption["broadcast-console-to-ops"] = value; }
        }

        [JsonProperty("pvp")]
        [DefaultValue(true)]
        public bool Pvp
        {
            get { return BoolOption["pvp"]; }
            set { BoolOption["pvp"] = value; }
        }

        [JsonProperty("spawn-npcs")]
        [DefaultValue(true)]
        public bool SpawnNpcs
        {
            get { return BoolOption["spawn-npcs"]; }
            set { BoolOption["spawn-npcs"] = value; }
        }

        [JsonProperty("spawn-animals")]
        [DefaultValue(true)]
        public bool SpawnAnimals
        {
            get { return BoolOption["spawn-animals"]; }
            set { BoolOption["spawn-animals"] = value; }
        }

        [JsonProperty("snooper-enabled")]
        [DefaultValue(true)]
        public bool SnooperEnabled
        {
            get { return BoolOption["snooper-enabled"]; }
            set { BoolOption["snooper-enabled"] = value; }
        }

        [JsonProperty("function-permission-level")]
        [DefaultValue("2")]
        public string FunctionPermissionLevel
        {
            get { return StringOption["function-permission-level"]; }
            set { StringOption["function-permission-level"] = value; }
        }

        [JsonProperty("difficulty")]
        [DefaultValue("easy")]
        public string Difficulty
        {
            get { return StringOption["difficulty"]; }
            set { StringOption["difficulty"] = value; }
        }

        [JsonProperty("network-compression-threshold")]
        [DefaultValue("256")]
        public string NetworkCompressionThreshold
        {
            get { return StringOption["network-compression-threshold"]; }
            set { StringOption["network-compression-threshold"] = value; }
        }

        [JsonProperty("text-filtering-config")]
        [DefaultValue("")]
        public string TextFilteringConfig
        {
            get { return StringOption["text-filtering-config"]; }
            set { StringOption["text-filtering-config"] = value; }
        }

        [JsonProperty("max-tick-time")]
        [DefaultValue("60000")]
        public string MaxTickTime
        {
            get { return StringOption["max-tick-time"]; }
            set { StringOption["max-tick-time"] = value; }
        }

        [JsonProperty("require-resource-pack")]
        [DefaultValue(false)]
        public bool RequireResourcePack
        {
            get { return BoolOption["require-resource-pack"]; }
            set { BoolOption["require-resource-pack"] = value; }
        }

        [JsonProperty("spawn-monsters")]
        [DefaultValue(true)]
        public bool SpawnMonsters
        {
            get { return BoolOption["spawn-monsters"]; }
            set { BoolOption["spawn-monsters"] = value; }
        }

        [JsonProperty("enforce-whitelist")]
        [DefaultValue(false)]
        public bool EnforceWhitelist
        {
            get { return BoolOption["enforce-whitelist"]; }
            set { BoolOption["enforce-whitelist"] = value; }
        }

        [JsonProperty("max-players")]
        [DefaultValue("20")]
        public string MaxPlayers
        {
            get { return StringOption["max-players"]; }
            set { StringOption["max-players"] = value; }
        }

        [JsonProperty("use-native-transport")]
        [DefaultValue(true)]
        public bool UseNativeTransport
        {
            get { return BoolOption["use-native-transport"]; }
            set { BoolOption["use-native-transport"] = value; }
        }

        [JsonProperty("resource-pack-sha1")]
        [DefaultValue("")]
        public string ResourcePackSha1
        {
            get { return StringOption["resource-pack-sha1"]; }
            set { StringOption["resource-pack-sha1"] = value; }
        }

        [JsonProperty("spawn-protection")]
        [DefaultValue("16")]
        public string SpawnProtection
        {
            get { return StringOption["spawn-protection"]; }
            set { StringOption["spawn-protection"] = value; }
        }

        [JsonProperty("enable-status")]
        [DefaultValue(true)]
        public bool EnableStatus
        {
            get { return BoolOption["enable-status"]; }
            set { BoolOption["enable-status"] = value; }
        }

        [JsonProperty("online-mode")]
        [DefaultValue(true)]
        public bool OnlineMode
        {
            get { return BoolOption["online-mode"]; }
            set { BoolOption["online-mode"] = value; }
        }

        [JsonProperty("allow-flight")]
        [DefaultValue(false)]
        public bool AllowFlight
        {
            get { return BoolOption["allow-flight"]; }
            set { BoolOption["allow-flight"] = value; }
        }

        [JsonProperty("max-world-size")]
        [DefaultValue("29999984")]
        public string MaxWorldSize
        {
            get { return StringOption["max-world-size"]; }
            set { StringOption["max-world-size"] = value; }
        }
    }
}
