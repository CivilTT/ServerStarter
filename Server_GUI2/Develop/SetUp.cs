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
        //TODO: リリース時にはCurrentDirectoryの記述を変更する
        //public static string CurrentDirectory => Environment.GetEnvironmentVariable("SERVER_STERTER_TEST");
        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string DataPath => Path.Combine(CurrentDirectory, "World_Data");

        public static void Initialize()
        {
            var progressBar = new ShowNewWindow<ProgressBarDialog, ProgressBarDialogVM>();
            var progressvm = new ProgressBarDialogVM("TEST", 6);
            Thread thread = new Thread(new ParameterizedThreadStart(Show))
            {
                IsBackground = true,
                
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(progressvm);

            progressvm.AddCount();
            Thread.Sleep(1000);
            progressvm.AddCount();
            Thread.Sleep(1000);
            progressvm.AddCount();
            Thread.Sleep(1000);
            progressvm.AddCount();
            Thread.Sleep(1000);
            progressvm.AddCount();
            Thread.Sleep(1000);
            progressvm.AddCount();

            progressvm.ShowCounter();
            progressvm.Close();

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

        private static void Show(object param)
        {
            var progressBar = new ShowNewWindow<ProgressBarDialog, ProgressBarDialogVM>();
            progressBar.ShowDialog((ProgressBarDialogVM)param);
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
