using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

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
                string message =
                        "Server Starterの更新データの取得に失敗しました。\n" +
                        "最新バージョンの確認を行わずに起動します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
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
                string message =
                    "前回起動時にServer Starterのバージョンアップに失敗しました。\n" +
                    "再度バージョンアップを行う場合は「はい（Yes）」を選択してください。\n\n" +
                    "「はい（Yes）」を選択しているにも関わらず、このメッセージが繰り返し表示される場合、自動更新がこのPC環境に対応していない可能性があります。\n" +
                    "最新バージョンはGitのReleaseに公開されていますが、バグ修正のため、作者のTwitterへDMを頂けますと幸いです。\n" +
                    "お手数をおかけしますが、よろしくお願いします。";
                var result = MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
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
                    writer.WriteLine(versionUP);
                }
            }
            catch (Exception ex)
            {
                string message =
                        "Server Starterの更新ファイルの作成に失敗しました。\n" +
                        "システムの更新をせずに実行します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Warn($"Failed to write tmp.bat (Error Message : {ex.Message})");
                return;
            }

            string mes =
                "Server Starterの更新を開始します。\n" +
                "Setup_ServerStarter.msiによってシステムを更新するため、次に表示される確認画面で許可してください。";
            MW.MessageBox.Show(mes, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
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
