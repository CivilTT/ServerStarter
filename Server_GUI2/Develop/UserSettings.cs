using log4net;
using Newtonsoft.Json;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Util;
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

        public static UserSettingsJson userSettings = new UserSettingsJson();

        public static string JsonPath
        {
            get
            {
                return "info.json";
            }
        }

        public string OldInfoPath
        {
            get
            {
                return $@"{SetUp.DataPath}\info.txt";
            }
        }

        //public UserSettings()
        //{
        //    ReadFile();
        //}

        public void ReadFile()
        {
            string errorMessage =
                "個人設定の読み込みに失敗しました。\n" +
                "個人設定の再設定を行ってください。";
            if (File.Exists(JsonPath))
            {
                logger.Info("Read the local info data");
                ReadContents.ReadlocalJson(JsonPath, errorMessage);
            }
            else if (File.Exists(OldInfoPath))
            {
                logger.Info("Read the local info data");
                List<string> info = ReadContents.ReadOldInfo(OldInfoPath);
                ShareWorld sw = new ShareWorld
                {
                    gitAccountName = info[5],
                    gitAccountMail = info[6]
                };
                userSettings.playerName = info[0];
                userSettings.shareworlds = new List<ShareWorld>() { sw };
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

        }

        ///// <summary>
        ///// usersettings.jsonを作成する
        ///// 基本的にReadFile()からしか呼ばれない？
        ///// </summary>
        //private void CreateFile()
        //{

        //}

        public static void WriteFile()
        {
            string jsonData = JsonConvert.SerializeObject(userSettings);
            using (var sw = new StreamWriter(JsonPath, false, Encoding.UTF8))
            {
                sw.Write(jsonData);
            }
        }
    }

    public class UserSettingsJson
    {
        [JsonProperty("PlayerName")]
        public string playerName;

        [JsonProperty("Language")]
        public string language;

        [JsonProperty("LatestRun")]
        // LatestRun : {
        //  "Version" : "1.17.1", 
        //  "World" : "ShareWorld"
        // }
        public LatestRun latestRun;

        [JsonProperty("ShareWorld")]
        // ShareWorld はワールド名、Gitのアカウント名、GitのE-mailアドレスの情報を記録する
        // ShareWorldという名前でなくても共有ワールド化できるようにする
        public List<ShareWorld> shareworlds;

        [JsonProperty("DefaultProperty")]
        // あくまでデフォルトはシステムで保持しておき、それから変更したものを通常設定としたい場合の部分のみこれで保持する
        // { "difficulty" : "hard" }
        public Dictionary<string, string> defaultProperties;

    }

    public class LatestRun
    {
        public static string Version;
        public static string World;
    }
}
