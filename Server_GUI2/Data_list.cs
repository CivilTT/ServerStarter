using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Server_GUI2
{
    class Data_list
    {
        public static List<string> Info_index { get; } = new List<string>()
        {
            "1>Your name in Minecraft------------",
            "2>The version of 'Server_bat-files'-",
            "3>The latest Minecraft version------",
            "4>The latest Minecraft World name---",
            "5>Is any Servers opening now ?------",
            "6>Git Account Name------------------",
            "7>Git Account E-mail address--------"
        };
        public static SortedDictionary<string, string> Server_Properties { get; set; } = new SortedDictionary<string, string>()
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
            {"level-name","test" },
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

        public static string Starter_Version { get { return "5.1"; } }
        public static string Version { get; set; }
        public static bool Import_spigot { get; set; }
        public static string Copy_version { get; set; }
        public static bool CopyVer_IsSpigot { get; set; }
        public static string World { get; set; }
        public static bool Avail_sw { get; set; } = true;
        public static List<string> Info { get; set; }
        public static Dictionary<string, List<string>> VerWor_list { get; set; }
        public static Dictionary<string, string> Env_list { get; set; }

        public static List<string> Argument { get; set; }

        private readonly Functions func = new Functions();
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        WebClient wc = new WebClient();

        //OS情報の読み取りで使用
        public static System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_OperatingSystem");
        System.Management.ManagementObjectCollection moc = mc.GetInstances();

        //CPU情報の読み取りで使用
        public static System.Management.ManagementClass mc2 = new System.Management.ManagementClass("Win32_Processor");
        System.Management.ManagementObjectCollection moc2 = mc2.GetInstances();

        //GPU情報の読み取りで使用
        public static System.Management.ManagementClass mc3 = new System.Management.ManagementClass("Win32_VideoController");
        System.Management.ManagementObjectCollection moc3 = mc3.GetInstances();


        public void Set_Version(System.Windows.Controls.ComboBox combo_version1, string combo_version2)
        {
            Import_spigot = combo_version1.Text.Contains("Spigot");
            Version = Import_spigot ? combo_version1.Text.Substring(combo_version1.Text.IndexOf("_") + 1) : combo_version1.Text;

            if (combo_version1.SelectedIndex == -1)
            {
                Version = combo_version2.Substring(combo_version2.IndexOf(" ") + 1);
            }

            Properties.Settings.Default.Version = Import_spigot ? $"Spigot_{Version}" : Version;
            Properties.Settings.Default.Save();
        }

        public void Set_World(System.Windows.Controls.ComboBox combo_world, string text_world = "input_name")
        {
            World = combo_world.Text;
            if (combo_world.Text.Contains("/"))
            {
                Copy_version = combo_world.Text.Substring(0, combo_world.Text.IndexOf("/"));
                // Spigot_の文言を削除して登録する
                if (Copy_version.Contains("Spigot"))
                {
                    Copy_version = Copy_version.Substring(Copy_version.IndexOf("_") + 1);
                    CopyVer_IsSpigot = true;
                }
                else
                {
                    CopyVer_IsSpigot = false;
                }
                World = combo_world.Text.Substring(combo_world.Text.IndexOf("/") + 1);
            }
            else if(combo_world.SelectedIndex == -1)
            {
                // World名に1.17.1/(World)のようにバージョンが入らなかった場合のVdownでのバグを防止している
                Copy_version = "";
                World = text_world;
            }
        }

        public void Set_info(StreamReader sr)
        {
            logger.Info("Read the local info data");

            string line;
            List<string> tmp_Info = new List<string>();
            try
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //＞が入っていない行ははじく
                    if (line.IndexOf(">") == -1)
                    {
                        continue;
                    }

                    //-＞の前後をリストとして登録している
                    tmp_Info.Add(line.Substring(line.IndexOf("->") + 2));
                    
                }
                Info = tmp_Info;
            }
            catch (Exception ex)
            {
                func.Error(ex.Message);
            }
        }

        public void Set_SW()
        {
            if(Info[5] == "Example" || Info[5] == "")
            {
                Avail_sw = false;
            }
        }

        public void Set_properties(StreamReader sr)
        {
            //MoreSettingsが複数回呼び出されても表示内容を更新
            SortedDictionary<string, string> properties_const = new SortedDictionary<string, string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                //冒頭2行は＝が入っていないため、そのまま登録
                if (line.IndexOf("=") == -1)
                {
                    properties_const.Add(line.Substring(0), "");
                    continue;
                }

                //" = "の前後を辞書として登録している(項目に値が登録されていなかった場合、例外処理を行う)
                try
                {
                    properties_const.Add(line.Substring(0, line.IndexOf("=")), line.Substring(line.IndexOf("=") + 1));
                }
                catch (ArgumentOutOfRangeException)
                {
                    properties_const.Add(line.Substring(0, line.IndexOf("=")), "");
                }
            }

            // More_Settings.Set_value()でキーがないエラーをはかないように事前に確認している
            if(properties_const.ContainsKey("difficulty") && properties_const.ContainsKey("hardcore") && properties_const.ContainsKey("gamemode") && properties_const.ContainsKey("force-gamemode") && properties_const.ContainsKey("white-list") && properties_const.ContainsKey("enforce-whitelist"))
            {
                Server_Properties = properties_const;
            }
        }

        public void Set_VerWor()
        {
            logger.Info("Check the all Directories of Server Version and World");
            Dictionary<string, List<string>> tmp_VerWor_list = new Dictionary<string, List<string>>();

            string[] subFolders = Directory.GetDirectories(
                $@"{MainWindow.Data_Path}\", "*", SearchOption.TopDirectoryOnly);
            foreach (string ver_path in subFolders)
            {
                string subfolder = Path.GetFileName(ver_path);

                //各Version内のWorldをforで回す
                string[] Worlds = Directory.GetDirectories(
                    $@"{MainWindow.Data_Path}\{subfolder}", "*", SearchOption.TopDirectoryOnly);

                List<string> World_list = new List<string>();
                foreach (string wor_path in Worlds)
                {
                    string World_name = Path.GetFileName(wor_path);
                    if (World_name != "logs")
                    {
                        World_list.Add(World_name);
                    }
                }
                tmp_VerWor_list.Add(subfolder, World_list);
            }

            VerWor_list = tmp_VerWor_list;
        }

        public void Set_env()
        {
            logger.Info("Check the environment of this PC");

            string os_name = "Can not read";
            string cpu_name = "Can not read";
            string gpu_name = "Can not read";
            string memory = "Can not read";
            string memory2 = "Can not read";
            string ip_address = wc.DownloadString("https://ipv4.icanhazip.com/").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            string git_version = "Git is not installed in this PC";
            string java_version = "Java is not installed in this PC";

            foreach (System.Management.ManagementObject mo in moc)
            {
                os_name = mo["Caption"].ToString();
                memory = mo["TotalVisibleMemorySize"].ToString();
                memory2 = mo["FreePhysicalMemory"].ToString();
            }
            foreach (System.Management.ManagementObject mo in moc2)
            {
                cpu_name = mo["Name"].ToString();
            }
            foreach (System.Management.ManagementObject mo in moc3)
            {
                gpu_name = mo["Name"].ToString();
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            psi.FileName = "git";
            psi.Arguments = "--version";
            Process p = Process.Start(psi);
            p.WaitForExit();
            if (p.ExitCode == 0)
            {
                git_version = p.StandardOutput.ReadLine();
            }

            psi.FileName = "java";
            psi.Arguments = "--version";
            p = Process.Start(psi);
            p.WaitForExit();
            if (p.ExitCode == 0)
            {
                java_version = p.StandardOutput.ReadLine();
            }

            Avail_sw = true;
            if(git_version == "Git is not installed in this PC")
            {
                Avail_sw = false;
                MessageBox.Show("このPCではGitが導入されていないため、ShareWorldが使えません。", "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if(java_version == "Java is not installed in this PC")
            {
                MessageBox.Show(
                    "このPCにはJavaが導入されていません。以下の手順に従ってJava (JDK)をインストールしてください。\r\n\n" +
                    "・Step１\r\n　Oracleのサイト（https://www.oracle.com/java/technologies/javase-jdk16-downloads.html）より最新のJDKのインストーラーをダウンロードする。\r\n\n" +
                    "・Step２\r\n　以下二つの環境変数を「システム環境変数」に登録する。\r\n" +
                    "　　・【JAVA_HOME】 ex) C:\\Program Files\\Java\\jdk-16.0.1\r\n" +
                    "　　・【Path】 ex) C:\\Program Files\\Java\\jdk-16.0.1\\bin", "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(!java_version.Contains("java 16"))
            {
                MessageBox.Show(
                    $"このPCに入っているJavaのバージョンは最新バージョンではありません。\n" +
                    $"バージョン1.17以降のサーバーは開設できない可能性があります。\n\n" +
                    $"【Javaのバージョン情報】\n    {java_version}", "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            Env_list = new Dictionary<string, string>
            {
                ["OS"] = os_name,
                ["CPU"] = cpu_name,
                ["GPU"] = gpu_name,
                ["Memory_All"] = memory,
                ["Memory_Ava"] = memory2,
                ["IP"] = ip_address,
                ["Git"] = git_version,
                ["Java"] = java_version
            };
        }
    }
}
