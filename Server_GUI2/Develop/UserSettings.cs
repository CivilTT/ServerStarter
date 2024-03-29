﻿using log4net;
using Newtonsoft.Json;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Server_GUI2
{
    public class UserSettings
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static UserSettings Instance = new UserSettings();

        public UserSettingsJson userSettings = new UserSettingsJson();

        public static string JsonPath => "info.json";

        public string OldInfoPath => $@"{SetUp.DataPath}\info.txt";

        public static Dictionary<string, string> Languages = new Dictionary<string, string>()
        {
            { "English", "en" },
            { "日本語", "ja" }
        };

        private UserSettings()
        {
            ReadFile();
        }

        private void ReadFile()
        {
            if (ServerGuiPath.Instance.InfoJson.Exists)
            {
                logger.Info("Read the local info.json data");
                userSettings = ReadContents.ReadlocalJson<UserSettingsJson>(ServerGuiPath.Instance.InfoJson.FullName, Properties.Resources.User_Read);
                if (userSettings.StarterVersion != UserSettingsJson.LatestVersion)
                {
                    // TODO: jsonの中身を変更した場合にはここにバージョン変換の実装を書く（必要であれば）
                }
            }
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
        public static string LatestVersion = "2.0.0.0";

        [JsonProperty("StarterVersion")]
        public string StarterVersion = LatestVersion;

        [JsonProperty("OwnerName")]
        public string OwnerName = "";

        [JsonProperty("Language")]
        public string Language = "";

        [JsonProperty("LatestRun")]
        // LatestRun : {
        //  "Version" : "1.17.1", 
        //  "World" : "ShareWorld"
        // }
        public LatestRun LatestRun;

        [JsonProperty("ServerMemorySize")]
        public ServerMemorySize ServerMemorySize = new ServerMemorySize(5, "G");

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

    public class ServerMemorySize
    {
        public static string[] UnitList = new string[3] { "K", "M", "G" };

        [JsonProperty("Size")]
        public int Size;

        [JsonProperty("Unit")]
        public string Unit;

        public ServerMemorySize(int size, string unit)
        {
            Size = size;
            Unit = unit;
        }

        public override string ToString()
        {
            return $"{Size}{Unit}";
        }
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
