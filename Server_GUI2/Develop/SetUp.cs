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

namespace Server_GUI2
{
    public class SetUp
    {
        //TODO: リリース時にはCurrentDirectoryの記述を変更する
        //public static string CurrentDirectory { get { return Environment.GetEnvironmentVariable("SERVER_STERTER_TEST"); } }
        public static string CurrentDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string DataPath { get { return Path.Combine(CurrentDirectory, "World_Data"); } }

        public static void Initialize()
        {
            // 利用規約に同意しているか
            if (!UserSettings.Instance.userSettings.Agreement.SystemTerms)
            {
                WelcomeWindow window = new WelcomeWindow();
                bool? result = window.ShowDialog();
                if (result != true)
                    Environment.Exit(0);
            }

            // 仕様変更が必要な場合に実装
            ChangeSpecification();

            // SystemVersionの確認＆バージョンアップ
            ManageSystemVersion.CheckVersion();
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

        //public void ShowProgressBar()
        //{

        //}
    }
}
