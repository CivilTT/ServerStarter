using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

//メモ
//launcherに表示されるバージョン名一覧の取得方法は.minecraft\versions\version_manifest_v2.jsonに一覧として保管されている
//バージョンに対応するserver.jarをダウンロードするURLは、jsonファイル内の対応するバージョンのURLにアクセスすると新しくjsonファイルが検索され、その検索されたもののdownloads>server>urlに格納されている


namespace Server_GUI2
{
    public partial class Read_json
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Functions func = new Functions();
        WebClient wc = new WebClient();
        Spigot_Function spi_func = new Spigot_Function();
        private string jsonStr;
        private dynamic root;


        public Read_json()
        {
            logger.Info("Import version_manifest_v2.json");
            try
            {
                jsonStr = wc.DownloadString("https://launchermeta.mojang.com/mc/game/version_manifest_v2.json");
                root = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr);
            }
            catch (Exception ex)
            {
                string message =
                        "Minecraftのバージョン一覧の取得に失敗しました。\n" +
                        $"新しいバージョンのサーバーの導入はできません\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MainWindow.Set_new_Version = false;
            }
        }

        public List<string> Import_version(string type_name)
        {
            logger.Info($"Import new Version List ({type_name})");
            List<string> list_versions = new List<string>() { $"【latest_release】 {root.latest.release}", $"【latest_snapshot】 {root.latest.snapshot}" };
            string id = "";
            int i = 0;

            //ここでrelease、snapshotのみかすべてまとめて取得するのかを決める
            while (id != "1.0")
            {
                id = root.versions[i].id;
                string type = root.versions[i].type;

                if (type_name == "all"|| type_name == type)
                {
                    list_versions.Add($"{type} {id}");
                }

                i++;
            }

            return list_versions;
        }

        public bool Import_server()
        {
            string ver_folder = (Data_list.Import_spigot) ? $"Spigot_{Data_list.Version}" : Data_list.Version;
            logger.Info("There are already new version, or not");
            if (Directory.Exists($@"{MainWindow.Data_Path}\{ver_folder}\"))
            {
                logger.Info("There are already new version");
                return false;
            }
            
            logger.Info("Download server.jar");


            // 試験的な追加
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (Data_list.Version == "1.18-3" && !Data_list.Import_spigot)
            {
                string url2 = "https://launcher.mojang.com/v1/objects/cbe106c19f5072222ce54039aa665a8aaf97097d/server.jar";
                Directory.CreateDirectory($@"{MainWindow.Data_Path}\{Data_list.Version}");
                wc.DownloadFile(url2, $@"{MainWindow.Data_Path}\{Data_list.Version}\server.jar");
                wc.Dispose();

                //一度実行し、eula.txtなどの必要ファイルを書き出す
                func.Start_server();

                //eulaの書き換え
                func.Change_eula();

                return true;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            if (Data_list.Version == "")
            {
                DialogResult result = MessageBox.Show("導入するサーバーのバージョンが選択されていません。\r\nサーバーのバージョンを選択をした上で再度「Run」を押してください。", "Server Starter", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    Application.Restart();
                }
                throw new ArgumentException("Did not select opening version");
            }

            if (Data_list.Import_spigot)
            {
                Import_spigot(ver_folder);
            }
            else
            {
                Import_vanila();
            }

            //一度実行し、eula.txtなどの必要ファイルを書き出す
            func.Start_server();
            MainWindow.Pd.Value = 15;
            MainWindow.Pd.Message = "Output the server.jar, eula.txt and so on";

            //eulaの書き換え
            func.Change_eula();

            return true;
        }

        private void Import_vanila()
        {
            logger.Info("Import vanila Server");
            //新しく入れようとしているバージョンがversionsのどこにあるのかを探索
            int i = 0;
            while (root.versions[i].id != Data_list.Version)
            {
                i++;
            }

            string url2;
            try
            {
                //サーバーダウンロードのurlが記されたjsonをダウンロード
                string url = root.versions[i].url;
                string new_jsonStr = wc.DownloadString(url);

                dynamic root2 = Newtonsoft.Json.JsonConvert.DeserializeObject(new_jsonStr);
                url2 = root2.downloads.server.url;
            }
            catch(Exception ex)
            {
                string message =
                        "Vanila サーバーのダウンロードに失敗しました。\n" +
                        $"{Data_list.Version}はマルチサーバーが存在しない可能性があります。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new DownloadException($"Failed to get url to download server.jar (Error Message : {ex.Message})");
            }

            Directory.CreateDirectory($@"{MainWindow.Data_Path}\{Data_list.Version}");
            
            try
            {
                wc.DownloadFile(url2, $@"{MainWindow.Data_Path}\{Data_list.Version}\server.jar");
                wc.Dispose();
            }
            catch(Exception ex)
            {
                string message =
                        "Vanila サーバーのダウンロードに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new DownloadException($"Failed to download server.jar (Error Message : {ex.Message})");
            }
        }

        private void Import_spigot(string ver_folder)
        {
            logger.Info("Import Spigot Server");
            Directory.CreateDirectory($@"{MainWindow.Data_Path}\{ver_folder}");

            try
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                wc.DownloadFile("https://hub.spigotmc.org/jenkins/job/BuildTools/lastSuccessfulBuild/artifact/target/BuildTools.jar", $@"{MainWindow.Data_Path}\{ver_folder}\BuildTools.jar");
                wc.Dispose();
            }
            catch(Exception ex)
            {
                string message =
                        "Spigot サーバーのビルドファイルのダウンロードに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new DownloadException($"Failed to download BuildTools.jar (Error Message : {ex.Message})");
            }

            Create_bat(ver_folder);
            Process p = Process.Start($@"{MainWindow.Data_Path}\{ver_folder}\build.bat");
            p.WaitForExit();

            if(p.ExitCode != 0)
            {
                spi_func.Delete_dir(ver_folder);
                Move_log(ver_folder);
                Directory.Delete($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\", true);

                string message;
                switch (p.ExitCode)
                {
                    case 1:
                        message = $"バージョン{Data_list.Version}はインポート可能なSpigotサーバーとして見つけられませんでした。";
                        break;
                    default:
                        message = $"Spigotサーバーのビルドに失敗しました（エラーコード：{p.ExitCode}）";
                        break;
                }

                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new ServerException($"Failed to build the spigot server (Error Code : {p.ExitCode})");
            }

            // 余計なファイルの削除
            spi_func.Delete_dir(ver_folder);
            Move_log(ver_folder);
        }

        private void Create_bat(string ver_folder)
        {
            logger.Info("Generate build.bat");
            List<string> build = new List<string>
            {
                "@echo off",
                "cd %~dp0",
                $"java -jar BuildTools.jar --rev {Data_list.Version}"
            };

            try
            {
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{ver_folder}\build.bat", false))
                {
                    foreach (string line in build)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                        "Spigotサーバーをビルドするための必要ファイルの作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException($"Failed to write build.bat (Error Message : {ex.Message})");
            }
        }

        private void Move_log(string ver_folder)
        {
            try
            {
                File.Move($@"{MainWindow.Data_Path}\{ver_folder}\BuildTools.log.txt", @".\log\BuildTools.log.txt");
            }
            catch
            {
                File.Delete(@".\log\BuildTools.log.txt");
                File.Move($@"{MainWindow.Data_Path}\{ver_folder}\BuildTools.log.txt", @".\log\BuildTools.log.txt");
            }
        }

    }
}
