using log4net;
using Newtonsoft.Json;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.WelcomeWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Server_GUI2.Windows;
using MW = ModernWpf;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.ProgressBar;
using System.Threading;

namespace Server_GUI2
{
    public static class SetUp
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //TODO: リリース時にはCurrentDirectoryの記述を変更する
        //public static string CurrentDirectory => Environment.GetEnvironmentVariable("SERVER_STERTER_TEST");
        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string DataPath => Path.Combine(CurrentDirectory, "World_Data");

        // Initialize()より前には呼ばない前提
        public static ProgressBar InitProgressBar;


        public static void Initialize()
        {
            // 利用規約に同意しているか
            logger.Info("Check to already agree to the system terms");
            if (!UserSettings.Instance.userSettings.Agreement.SystemTerms)
            {
                logger.Info("Started Welcome Window");
                WelcomeWindow window = new WelcomeWindow();
                bool? result = window.ShowDialog();
                if (result != true)
                    Environment.Exit(0);
            }

            //CheckConnetNet();

            // ProgressBarを表示する
            InitProgressBar = new ProgressBar("Ready to Server Starter", 9);
            InitProgressBar.AddMessage("Checked first User Settings");

            // 仕様変更が必要な場合に実装
            ChangeSpecification();
            InitProgressBar.AddMessage("Checked ChangeSpecification");

            // SystemVersionの確認＆バージョンアップ
            ManageSystemVersion.CheckVersion();
            InitProgressBar.AddMessage("Checked the versionUP about this system, Server Starter");
        }

        /// <summary>
        /// ネットワークに接続されているか調べる
        /// これを実装しなくてもネットがないことを想定して処理されてるところ多め
        /// TODO: 動作確認
        /// </summary>
        private static void CheckConnetNet()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                string message =
                    "本システムはインターネット環境下のみで動作します。\n" +
                    "インターネットに接続したうえで、再度起動してください。";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// TODO: システムが使用するディレクトリが変更された場合、ここに書いていく
        /// （将来的に変更が増えてきたら別の持ち方を検討すべき）
        /// </summary>
        private static void ChangeSpecification()
        {
            // 0.X -> 1.0.0.0
            FileInfo starterJson = new FileInfo(Path.Combine(DataPath, "Starter_Version.json"));
            starterJson.Delete();

            // 1.X -> 2.0.0.0
            FileInfo infoTxt = new FileInfo(Path.Combine(DataPath, "info.txt"));
            infoTxt.Delete();

        }
    }
}
