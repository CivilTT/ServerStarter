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

        public ServerProperty(string versionPath)
        {
            VersionPath = versionPath;

            StringOption = new SortedDictionary<string, string>();
            BoolOption = new SortedDictionary<string, bool>();

            ReadFile();
        }

        /// <summary>
        /// server.propertiesを読み込む
        /// </summary>
        private void ReadFile()
        {
            logger.Info("Read server properties");

            if (File.Exists(Path))
            {
                logger.Info($"Using server.properties at {Path}");
                using (StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("Shift_JIS")))
                {
                    ReadProperty(sr);
                }
            }
            else
            {
                logger.Info("Using default properties");
                // システムが持つ規定値を使用する
                // infoより、propertyのデフォルト設定をしている場合はそれを使用する
                ReadProperty();
            }
        }
        /// <summary>
        /// server.propertiesを読み取る
        /// </summary>
        private void ReadProperty(StreamReader sr)
        {
            //MoreSettingsが複数回呼び出されても表示内容を更新
            string line;
            while ((line = sr.ReadLine()) != null)
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
        /// <summary>
        /// デフォルトのpropertiesを登録する
        /// </summary>
        private void ReadProperty()
        {
            foreach (KeyValuePair<string, string> item in DefaultProperties)
            {
                string indexName = item.Key;
                // null の時に??の右側の値を使用する
                string strValue = UserSettingProperties[indexName] ?? item.Value;

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


        /// <summary>
        /// 設定されてるpropertiesを書き込む
        /// </summary>
        public void WriteFile()
        {
            //propertiesを該当バージョンのserver.propertiesに書き込む
            logger.Info("Write server.properties");
            try
            {
                SortedDictionary<string, string> _boolOption = new SortedDictionary<string, string>((IDictionary<string, string>)BoolOption);
                Dictionary<string, string> writeProperties = StringOption.Concat(_boolOption).ToDictionary(c => c.Key, c => c.Value);

                using (StreamWriter writer = new StreamWriter(Path, false))
                {
                    foreach (KeyValuePair<string, string> item in writeProperties)
                    {
                        writer.WriteLine($"{item.Key}={item.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                        "server.propertiesの書き込みに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to write server.properties (Error Code : {ex.Message})");
            }
        }
    }
}
