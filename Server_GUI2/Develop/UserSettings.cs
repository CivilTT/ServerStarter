using log4net;
using Newtonsoft.Json;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.SystemSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    // TODO: jsonの中身を変更した場合にはここにバージョン変換の実装を書く
                }
            }
            else if (File.Exists(OldInfoPath))
            {
                logger.Info("Read the local info.txt data");
                
                List<string> info = ReadContents.ReadOldInfo(OldInfoPath);
                AccountInfo accountInfo = new AccountInfo(info[5], info[6], "ShareWorld", "main");
                
                userSettings.PlayerName = info[0];
                userSettings.RemoteContents.Add(accountInfo);
            }
            else
            {
                ShowBuilder();
            }
        }

        /// <summary>
        /// 個人設定入力用のUIを開く
        /// 現状ではInfo_builderに該当
        /// </summary>
        private void ShowBuilder()
        {
            // TODO: 個人設定入力用UIの表示を行う
            // 現状のInfo_builderも変更する
        }

        ///// <summary>
        ///// usersettings.jsonを作成する
        ///// 基本的にReadFile()からしか呼ばれない？
        ///// </summary>
        //private void CreateFile()
        //{

        //}

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

        [JsonProperty("RemoteContents")]
        // ShareWorld はワールド名、Gitのアカウント名、GitのE-mailアドレスの情報を記録する
        // ShareWorldという名前でなくても共有ワールド化できるようにする
        public List<AccountInfo> RemoteContents = new List<AccountInfo>();

        [JsonProperty("DefaultProperty")]
        // あくまでデフォルトはシステムで保持しておき、それから変更したものを通常設定としたい場合の部分のみこれで保持する
        // { "difficulty" : "hard" }
        public ServerProperty DefaultProperties = new ServerProperty();

        [JsonProperty("Players")]
        public List<Player> Players = new List<Player>();

        [JsonProperty("PlayerGroups")]
        public List<PlayerGroup> PlayerGroups = new List<PlayerGroup>();

        [JsonProperty("PortMapping")]
        public PortStatus PortStatus = null;
    }

    public class LatestRun
    {
        [JsonProperty("Version")]
        public string VersionName;

        [JsonProperty("Type")]
        public string VersionType;

        [JsonProperty("World")]
        public string WorldName;

        public LatestRun(Version version, World world)
        {
            VersionName = version.Name;
            VersionType = version.Type.ToStr();
            WorldName = world.Name;
        }

        // JsonSerialize用コンストラクタ
        public LatestRun() { }
    }
}
