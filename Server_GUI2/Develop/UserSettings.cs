using log4net;
using Newtonsoft.Json;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.WelcomeWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Server_GUI2
{
    public class UserSettings
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static UserSettings Instance = new UserSettings();

        public UserSettingsJson userSettings = new UserSettingsJson();

        public static string JsonPath => "info.json";

        public string OldInfoPath => $@"{SetUp.DataPath}\info.txt";

        private UserSettings()
        {
            ReadFile();
        }

        private void ReadFile()
        {
            string errorMessage =
                "個人設定の読み込みに失敗しました。\n" +
                "個人設定の再設定を行ってください。";
            if (File.Exists(JsonPath))
            {
                logger.Info("Read the local info.json data");
                userSettings = ReadContents.ReadlocalJson<UserSettingsJson>(JsonPath, errorMessage);
                if (userSettings.JsonVersion != UserSettingsJson.LatestJsonVer)
                {
                    // TODO: jsonの中身を変更した場合にはここにバージョン変換の実装を書く（必要であれば）
                }
            }
            //else if (File.Exists(OldInfoPath))
            //{
            //    logger.Info("Read the local info.txt data");
                
            //    List<string> info = ReadContents.ReadOldInfo(OldInfoPath);
            //    // TODO: info.txtから読み込んだ情報を新システムに登録（そのくらいもう一度自分でWelcomeWindowにて登録してもらう？）
            //    // info.txtのユーザー名が正しいか確認の意味でもShowBuilderするべき？
            //}
        }

        public void WriteFile()
        {
            string jsonData = JsonConvert.SerializeObject(userSettings, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            using (var sw = new StreamWriter(JsonPath, false, Encoding.UTF8))
            {
                sw.Write(jsonData);
            }
        }
    }

    public class UserSettingsJson
    {
        [JsonIgnore]
        public static int LatestJsonVer = 1;

        [JsonProperty("JsonVersion")]
        public int JsonVersion = LatestJsonVer;

        [JsonProperty("PlayerName")]
        public string PlayerName;

        [JsonProperty("Language")]
        public string Language;

        [JsonProperty("LatestRun")]
        // LatestRun : {
        //  "Version" : "1.17.1", 
        //  "World" : "ShareWorld"
        // }
        public LatestRun LatestRun;

        [JsonProperty("OwnerHasOp")]
        public bool OwnerHasOp;

        [JsonProperty("ShutdownPC")]
        public bool ShutdownPC;

        [JsonProperty("DefaultProperty")]
        public ServerProperty DefaultProperties = new ServerProperty();

        [JsonProperty("Players")]
        public List<Player> Players = new List<Player>();

        [JsonProperty("PlayerGroups")]
        public List<PlayerGroup> PlayerGroups = new List<PlayerGroup>();

        [JsonProperty("PortSettings")]
        public PortSettings PortSettings = new PortSettings();

        [JsonProperty("Agreement")]
        public Agreement Agreement = new Agreement();
    }

    public class LatestRun
    {
        [JsonProperty("Version")]
        public string VersionName;

        [JsonProperty("Type")]
        public string VersionType;

        [JsonProperty("World")]
        public string WorldName;

        public LatestRun(Version version, IWorld world)
        {
            VersionName = version.Name;
            VersionType = version.Type.ToStr();
            WorldName = world.Name;
        }

        // JsonSerialize用コンストラクタ
        public LatestRun() { }
    }

    public class PlayerGroup : IEquatable<PlayerGroup>, IComparable<PlayerGroup>
    {
        [JsonProperty("GroupName")]
        public string GroupName { get; private set; }

        [JsonProperty("PlayerList")]
        public ObservableCollection<Player> PlayerList { get; private set; }

        public PlayerGroup(string name, ObservableCollection<Player> list)
        {
            GroupName = name;
            PlayerList = list;
        }

        public bool Equals(PlayerGroup other)
        {
            return other.GroupName == GroupName;
        }

        public int CompareTo(PlayerGroup other)
        {
            return GroupName.CompareTo(other.GroupName);
        }
    }

    public class PortSettings
    {
        [JsonProperty("UsingPortMapping")]
        public bool UsingPortMapping = false;

        [JsonProperty("PortNumber")]
        public int PortNumber = 25565;
    }

    public class Agreement
    {
        [JsonProperty("SystemTerms")]
        public bool SystemTerms = false;

        // ServerStarterとして保持しておくべき同意事項を保持する

    }
}
