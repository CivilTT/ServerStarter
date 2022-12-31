using log4net;
using Newtonsoft.Json;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Server_GUI2.Develop.Util
{

    public static class ManageSystemVersion
    {
        static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static readonly WebClient wc = new WebClient();

        public static string StarterVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public static void CheckVersion()
        {
            SetUp.InitProgressBar.AddMessage("Check the versionUP about this system, Server Starter");

            GitReleaseJson result = ReadJsonFromGit();

            if (result != null && StarterVersion != result.VersionName)
            {
                SetUp.InitProgressBar.Close();
                //.exeをアップデート
                StarterVersionUP(result);
            }
        }

        /// <summary>
        /// GitHubのreleaseから最新バージョンについて取得する
        /// </summary>
        private static GitReleaseJson ReadJsonFromGit()
        {
            try
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                string jsonStr = wc.DownloadString("https://api.github.com/repos/CivilTT/ServerStarter/releases");
                List<GitReleaseJson> root = JsonConvert.DeserializeObject<List<GitReleaseJson>>(jsonStr);
                return root[0];
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"{Properties.Resources.Manage_GetLatestMsg}\n{ex.Message}", ButtonType.OK, Image.Infomation);
                return null;
            }
        }

        private static void StarterVersionUP(GitReleaseJson releaseJson)
        {
            logger.Info("Delete old .exe and download new .exe");

            Asset setupMsiAsset = releaseJson.Assets.Where(x => x.FileName.Contains(".msi")).FirstOrDefault();
            FileInfo tmpBat = new FileInfo(Path.Combine(SetUp.CurrentDirectory, "tmp.bat"));
            FileInfo setupMsi = new FileInfo(Path.Combine(SetUp.CurrentDirectory, setupMsiAsset.FileName));

            if (File.Exists("tmp.bat"))
            {
                string giturl = "https://github.com/CivilTT/ServerStarter";
                int result = CustomMessageBox.Show(
                    Properties.Resources.Manage_Vup1, 
                    new string[] { Properties.Resources.Manage_Vup3, Properties.Resources.Manage_Vup4 }, 
                    Image.Warning, 
                    new LinkMessage(Properties.Resources.Manage_Vup2, giturl)
                    );
                if (result != 0)
                    return;

                tmpBat.Delete();
                setupMsi.Delete();
            }

            wc.DownloadFile(setupMsiAsset.DownloadUrl, setupMsiAsset.FileName);

            // コマンド実行による引数をここで受け取り、再実行する。
            string args_list = "";
            if (App.Args != null)
            {
                foreach (string key in App.Args)
                {
                    args_list += $" {key}";
                }
            }

            string[] versionUP = new string[]
            {
                "@echo off",
                $"call msiexec /i Setup_ServerStarter.msi TARGETDIR=\"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\" /passive /li .\\log\\install.log",
                // 更新後に再起動する
                $@"if not %errorlevel%==1602 (start Server_GUI2.exe{args_list})",
                "del Setup_ServerStarter.msi",
                //自分自身を削除する
                "del /f \"%~dp0%~nx0\""
            };

            try
            {
                using (var writer = new StreamWriter(tmpBat.FullName, false))
                {
                    foreach (var line in versionUP)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"{Properties.Resources.Manage_FailUpdate}\n{ex.Message}", ButtonType.OK, Image.Error);
                logger.Warn($"Failed to write tmp.bat (Error Message : {ex.Message})");
                return;
            }

            CustomMessageBox.Show(Properties.Resources.Manage_Update, ButtonType.OK, Image.Infomation);
            Process.Start(@".\tmp.bat", @" > .\log\tmp(versionUP)_log.txt 2>&1");
            Environment.Exit(0);
        }
    }

    public class GitReleaseJson
    {
        [JsonProperty("name")]
        public string Name;
        [JsonIgnore]
        public string VersionName => Name.Replace("version ", "");
        [JsonProperty("assets")]
        public List<Asset> Assets;
    }

    public class Asset
    {
        [JsonProperty("name")]
        public string FileName;
        [JsonProperty("browser_download_url")]
        public string DownloadUrl;
    }
}
