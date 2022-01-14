using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server_GUI2
{
    class Vanila
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string StartVersion;
        private string jarName
        {
            get
            {
                if (Data_list.Import_spigot)
                {
                    return $"spigot-{Data_list.Version}.jar";
                }
                else
                {
                    return "server.jar";
                }
            }
        }


        public Vanila(string version)
        {
            StartVersion = version;
        }

        public void StartServer()
        {
            Create_bat_start();

            Open();
        }

        public void ImportServer()
        {
            Download();

            Open();

            Eula();
        }

        private void Create_bat_start()
        {
            //if (File.Exists($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\start.bat"))
            // log4j対応のbatをインストールするために緊急でこのようにしている
            // 本来は特定のバージョンにおいて、batの内容に応じて処理を行うか判断するべき
            if (false)
            {
                return;
            }
            else
            {
                logger.Info("Generate start.bat");
                List<string> start = new List<string>
                {
                    "@echo off",
                    "cd %~dp0",
                    $"java -Xmx5G -Xms5G -jar {jarName} nogui"
                };

                try
                {
                    using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\start.bat", false))
                    {
                        foreach (string line in start)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message =
                        "サーバーの実行ファイル(start.bat)の作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new IOException($"Failed to write start.bat (Error Message : {ex.Message})");
                }
            }
        }

        private void Open()
        {
            // サーバーを起動するだけ
            // Eulaの確認などは行わない
        }

        private void Eula()
        {
            // Eulaの確認 (falseの場合は同意についてもやらせる)
            // Eulaがない場合はpropertiesについても設定を反映させる
        }

        private void Download()
        {
            // ダウンロードするだけ
            
        }
    }
}
