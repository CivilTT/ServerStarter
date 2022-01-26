using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;
using Newtonsoft.Json;


namespace Server_GUI2
{
    public class ServerProperty
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedDictionary<string, string> DefaultProperties { get; set; } = new SortedDictionary<string, string>()
        {
            { "broadcast-rcon-to-ops","true" },
            {"enable-jmx-monitoring","false" },
            {"view-distance","10" },
            {"resource-pack-prompt","" },
            {"server-ip","" },
            {"rcon.port","25575" },
            {"allow-nether","true" },
            {"enable-command-block","false" },
            {"gamemode","survival" },
            {"server-port","25565" },
            {"enable-rcon","false" },
            {"sync-chunk-writes","true" },
            {"enable-query","false" },
            {"op-permission-level","4" },
            {"prevent-proxy-connections","false" },
            {"resource-pack","" },
            {"entity-broadcast-range-percentage","100" },
            {"level-name","" },
            {"player-idle-timeout","0" },
            {"rcon.password","" },
            {"motd","A Minecraft Server" },
            {"query.port","25565" },
            {"force-gamemode","false" },
            {"rate-limit","0" },
            {"hardcore","false" },
            {"white-list","false" },
            {"broadcast-console-to-ops","true" },
            {"pvp","true" },
            {"spawn-npcs","true" },
            {"spawn-animals","true" },
            {"snooper-enabled","true" },
            {"function-permission-level","2" },
            {"difficulty","easy" },
            {"network-compression-threshold","256" },
            {"text-filtering-config","" },
            {"max-tick-time","60000" },
            {"require-resource-pack","false" },
            {"spawn-monsters","true" },
            {"enforce-whitelist","false" },
            {"max-players","20" },
            {"use-native-transport","true" },
            {"resource-pack-sha1","" },
            {"spawn-protection","16" },
            {"enable-status","true" },
            {"online-mode","true" },
            {"allow-flight","false" },
            {"max-world-size","29999984" }
        };
        private static Dictionary<string, string> UserSettingProperties = UserSettings.userSettings.defaultProperties;

        private readonly string VersionPath;
        private string Path { get { return $@"{VersionPath}\server.properties"; } }

        public string LevelName
        {
            get
            {
                return StringOption["level-name"];
            }
            set
            {
                StringOption["level-name"] = value;
            }
        }

        public SortedDictionary<string, string> StringOption { get; }
        public SortedDictionary<string, bool> BoolOption { get; }

        /// <summary>
        /// server.propertiesの中身の文字列を読み込む
        /// </summary>  
        public ServerProperty(string properties)
        {
            VersionPath = versionPath;

            StringOption = new SortedDictionary<string, string>();
            BoolOption = new SortedDictionary<string, bool>();
            ReadProperty(properties);
        }

        /// <summary>
        /// server.propertiesと同内容のjsonを読み込む
        /// </summary>  
        public ServerProperty(ServerPropertiesJson jsonData)
        {
            StringOption = jsonData.StringOption;
            BoolOption = jsonData.BoolOption;
        }

        /// <summary>
        /// デフォルト設定を読み込む
        /// </summary>  
        public ServerProperty()
        {
            var jsonData = new ServerPropertiesJson();
            StringOption = jsonData.StringOption;
            BoolOption = jsonData.BoolOption;
        }

        /// <summary>
        /// server.propertiesを読み取る
        /// </summary>
        private void ReadProperty(string content)
        {
            // MoreSettingsが複数回呼び出されても表示内容を更新
            // TODO: = 行頭/行末/=の左右の空白文字が無視されるかどうかを検証し、そうれあればコードを修正
            foreach ( var line in content.Split('\n'))
            {
                //＝がなく、#で始まるものはスキップ
                if (line.IndexOf("=") == -1 || line.Substring(0, 1) == "#")
                {
                    continue;
                }

                string indexName = line.Substring(0, line.IndexOf("="));
                string strValue = (line == (indexName + "=")) ? "" : line.Substring(line.IndexOf("=") + 1);

                if (strValue == "true" || strValue == "false")
                {
                    BoolOption[indexName] = Convert.ToBoolean(strValue);
                }
                else
                {
                    StringOption[indexName] = strValue;
                }

            }
        }

        public void SetProperty(string indexName, string strContent)
        {
            StringOption[indexName] = strContent;
        }
        public void SetProperty(string indexName, bool boolContent)
        {
            BoolOption[indexName] = boolContent;
        }

        public string ExportProperty(bool minify = false)
        {
            var result = new List<string>();
            foreach (KeyValuePair<string, string> item in StringOption)
            {
                if ( ! minify || DefaultProperties[item.Key] != item.Value)
                    result.Add($"{item.Key}={item.Value}");
            }
            foreach (KeyValuePair<string, bool> item in BoolOption)
            {
                if ( ! minify || DefaultProperties[item.Key] != item.Value.ToString())
                    result.Add($"{item.Key}={item.Value}");
            }
            return string.Join("\n", result);
        }

        // TODO: jsonのminify方法の検討
        public ServerPropertiesJson ExportJson(bool minify = false)
        {
            return new ServerPropertiesJson(StringOption,BoolOption);
        }


        ///// <summary>
        ///// 設定されてるpropertiesを書き込む
        ///// </summary>
        //public void WriteFile()
        //{
        //    //propertiesを該当バージョンのserver.propertiesに書き込む
        //    logger.Info("Write server.properties");
        //    try
        //    {
        //        SortedDictionary<string, string> _boolOption = new SortedDictionary<string, string>((IDictionary<string, string>)BoolOption);
        //        Dictionary<string, string> writeProperties = StringOption.Concat(_boolOption).ToDictionary(c => c.Key, c => c.Value);

        //        using (StreamWriter writer = new StreamWriter(Path, false))
        //        {
        //            foreach (KeyValuePair<string, string> item in writeProperties)
        //            {
        //                writer.WriteLine($"{item.Key}={item.Value}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message =
        //                "server.propertiesの書き込みに失敗しました。\n\n" +
        //                $"【エラー要因】\n{ex.Message}";
        //        MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
        //        throw new IOException($"Failed to write server.properties (Error Code : {ex.Message})");
        //    }
        //}
    }
        public void SetProperty(string indexName, bool boolContent)
        {
            BoolOption[indexName] = boolContent;
        }


    public class ServerPropertiesJson
    {
        [JsonProperty("broadcast-rcon-to-ops")]
        public bool BroadcastRconToOps { get; set; } = true;
        [JsonProperty("enable-jmx-monitoring")]
        public bool EnableJmxMonitoring { get; set; } = false;
        [JsonProperty("view-distance")]
        public string ViewDistance { get; set; } = "10";
        [JsonProperty("resource-pack-prompt")]
        public string ResourcePackPrompt { get; set; } = "";
        [JsonProperty("server-ip")]
        public string ServerIp { get; set; } = "";
        [JsonProperty("rcon.port")]
        public string RconPort { get; set; } = "25575";
        [JsonProperty("allow-nether")]
        public bool AllowNether { get; set; } = true;
        [JsonProperty("enable-command-block")]
        public bool EnableCommandBlock { get; set; } = false;
        [JsonProperty("gamemode")]
        public string Gamemode { get; set; } = "survival";
        [JsonProperty("server-port")]
        public string ServerPort { get; set; } = "25565";
        [JsonProperty("enable-rcon")]
        public bool EnableRcon { get; set; } = false;
        [JsonProperty("sync-chunk-writes")]
        public bool SyncChunkWrites { get; set; } = true;
        [JsonProperty("enable-query")]
        public bool EnableQuery { get; set; } = false;
        [JsonProperty("op-permission-level")]
        public string OpPermissionLevel { get; set; } = "4";
        [JsonProperty("prevent-proxy-connections")]
        public bool PreventProxyConnections { get; set; } = false;
        [JsonProperty("resource-pack")]
        public string ResourcePack { get; set; } = "";
        [JsonProperty("entity-broadcast-range-percentage")]
        public string EntityBroadcastRangePercentage { get; set; } = "100";
        [JsonProperty("level-name")]
        public string LevelName { get; set; } = "";
        [JsonProperty("player-idle-timeout")]
        public string PlayerIdleTimeout { get; set; } = "0";
        [JsonProperty("rcon.password")]
        public string RconPassword { get; set; } = "";
        [JsonProperty("motd")]
        public string Motd { get; set; } = "A Minecraft Server";
        [JsonProperty("query.port")]
        public string QueryPort { get; set; } = "25565";
        [JsonProperty("force-gamemode")]
        public bool ForceGamemode { get; set; } = false;
        [JsonProperty("rate-limit")]
        public string RateLimit { get; set; } = "0";
        [JsonProperty("hardcore")]
        public bool Hardcore { get; set; } = false;
        [JsonProperty("white-list")]
        public bool WhiteList { get; set; } = false;
        [JsonProperty("broadcast-console-to-ops")]
        public bool BroadcastConsoleToOps { get; set; } = true;
        [JsonProperty("pvp")]
        public bool Pvp { get; set; } = true;
        [JsonProperty("spawn-npcs")]
        public bool SpawnNpcs { get; set; } = true;
        [JsonProperty("spawn-animals")]
        public bool SpawnAnimals { get; set; } = true;
        [JsonProperty("snooper-enabled")]
        public bool SnooperEnabled { get; set; } = true;
        [JsonProperty("function-permission-level")]
        public string FunctionPermissionLevel { get; set; } = "2";
        [JsonProperty("difficulty")]
        public string Difficulty { get; set; } = "easy";
        [JsonProperty("network-compression-threshold")]
        public string NetworkCompressionThreshold { get; set; } = "256";
        [JsonProperty("text-filtering-config")]
        public string TextFilteringConfig { get; set; } = "";
        [JsonProperty("max-tick-time")]
        public string MaxTickTime { get; set; } = "60000";
        [JsonProperty("require-resource-pack")]
        public bool RequireResourcePack { get; set; } = false;
        [JsonProperty("spawn-monsters")]
        public bool SpawnMonsters { get; set; } = true;
        [JsonProperty("enforce-whitelist")]
        public bool EnforceWhitelist { get; set; } = false;
        [JsonProperty("max-players")]
        public string MaxPlayers { get; set; } = "20";
        [JsonProperty("use-native-transport")]
        public bool UseNativeTransport { get; set; } = true;
        [JsonProperty("resource-pack-sha1")]
        public string ResourcePackSha1 { get; set; } = "";
        [JsonProperty("spawn-protection")]
        public string SpawnProtection { get; set; } = "16";
        [JsonProperty("enable-status")]
        public bool EnableStatus { get; set; } = true;
        [JsonProperty("online-mode")]
        public bool OnlineMode { get; set; } = true;
        [JsonProperty("allow-flight")]
        public bool AllowFlight { get; set; } = false;
        [JsonProperty("max-world-size")]
        public string MaxWorldSize { get; set; } = "29999984";

        public ServerPropertiesJson() { }
        public ServerPropertiesJson(SortedDictionary<string, string> stringOption, SortedDictionary<string, bool> boolOption)
        {
            ViewDistance = stringOption["view-distance"];
            ResourcePackPrompt = stringOption["resource-pack-prompt"];
            ServerIp = stringOption["server-ip"];
            RconPort = stringOption["rcon.port"];
            Gamemode = stringOption["gamemode"];
            ServerPort = stringOption["server-port"];
            OpPermissionLevel = stringOption["op-permission-level"];
            ResourcePack = stringOption["resource-pack"];
            EntityBroadcastRangePercentage = stringOption["entity-broadcast-range-percentage"];
            LevelName = stringOption["level-name"];
            PlayerIdleTimeout = stringOption["player-idle-timeout"];
            RconPassword = stringOption["rcon.password"];
            Motd = stringOption["motd"];
            QueryPort = stringOption["query.port"];
            RateLimit = stringOption["rate-limit"];
            FunctionPermissionLevel = stringOption["function-permission-level"];
            Difficulty = stringOption["difficulty"];
            NetworkCompressionThreshold = stringOption["network-compression-threshold"];
            TextFilteringConfig = stringOption["text-filtering-config"];
            MaxTickTime = stringOption["max-tick-time"];
            MaxPlayers = stringOption["max-players"];
            ResourcePackSha1 = stringOption["resource-pack-sha1"];
            SpawnProtection = stringOption["spawn-protection"];
            MaxWorldSize = stringOption["max-world-size"];

            BroadcastRconToOps = boolOption["broadcast-rcon-to-ops"];
            EnableJmxMonitoring = boolOption["enable-jmx-monitoring"];
            AllowNether = boolOption["allow-nether"];
            EnableCommandBlock = boolOption["enable-command-block"];
            EnableRcon = boolOption["enable-rcon"];
            SyncChunkWrites = boolOption["sync-chunk-writes"];
            EnableQuery = boolOption["enable-query"];
            PreventProxyConnections = boolOption["prevent-proxy-connections"];
            ForceGamemode = boolOption["force-gamemode"];
            Hardcore = boolOption["hardcore"];
            WhiteList = boolOption["white-list"];
            BroadcastConsoleToOps = boolOption["broadcast-console-to-ops"];
            Pvp = boolOption["pvp"];
            SpawnNpcs = boolOption["spawn-npcs"];
            SpawnAnimals = boolOption["spawn-animals"];
            SnooperEnabled = boolOption["snooper-enabled"];
            RequireResourcePack = boolOption["require-resource-pack"];
            SpawnMonsters = boolOption["spawn-monsters"];
            EnforceWhitelist = boolOption["enforce-whitelist"];
            UseNativeTransport = boolOption["use-native-transport"];
            EnableStatus = boolOption["enable-status"];
            OnlineMode = boolOption["online-mode"];
            AllowFlight = boolOption["allow-flight"];
        }

        public SortedDictionary<string,string> StringOption
        {
            get
            {
                return new SortedDictionary<string, string>() {
                    { "view-distance",ViewDistance},
                    { "resource-pack-prompt",ResourcePackPrompt},
                    { "server-ip",ServerIp},
                    { "rcon.port",RconPort},
                    { "gamemode",Gamemode},
                    { "server-port",ServerPort},
                    { "op-permission-level",OpPermissionLevel},
                    { "resource-pack",ResourcePack},
                    { "entity-broadcast-range-percentage",EntityBroadcastRangePercentage},
                    { "level-name",LevelName},
                    { "player-idle-timeout",PlayerIdleTimeout},
                    { "rcon.password",RconPassword},
                    { "motd",Motd},
                    { "query.port",QueryPort},
                    { "rate-limit",RateLimit},
                    { "function-permission-level",FunctionPermissionLevel},
                    { "difficulty",Difficulty},
                    { "network-compression-threshold",NetworkCompressionThreshold},
                    { "text-filtering-config",TextFilteringConfig},
                    { "max-tick-time",MaxTickTime},
                    { "max-players",MaxPlayers},
                    { "resource-pack-sha1",ResourcePackSha1},
                    { "spawn-protection",SpawnProtection},
                    { "max-world-size",MaxWorldSize}
                };
            }
        }
        public SortedDictionary<string, bool> BoolOption
        {
            get
            {
                return new SortedDictionary<string, bool>() {
                    {"broadcast-rcon-to-ops",BroadcastRconToOps},
                    {"enable-jmx-monitoring",EnableJmxMonitoring},
                    {"allow-nether",AllowNether},
                    {"enable-command-block",EnableCommandBlock},
                    {"enable-rcon",EnableRcon},
                    {"sync-chunk-writes",SyncChunkWrites},
                    {"enable-query",EnableQuery},
                    {"prevent-proxy-connections",PreventProxyConnections},
                    {"force-gamemode",ForceGamemode},
                    {"hardcore",Hardcore},
                    {"white-list",WhiteList},
                    {"broadcast-console-to-ops",BroadcastConsoleToOps},
                    {"pvp",Pvp},
                    {"spawn-npcs",SpawnNpcs},
                    {"spawn-animals",SpawnAnimals},
                    {"snooper-enabled",SnooperEnabled},
                    {"require-resource-pack",RequireResourcePack},
                    {"spawn-monsters",SpawnMonsters},
                    {"enforce-whitelist",EnforceWhitelist},
                    {"use-native-transport",UseNativeTransport},
                    {"enable-status",EnableStatus},
                    {"online-mode",OnlineMode},
                    {"allow-flight",AllowFlight}
                };
            }
        }
    }
}
