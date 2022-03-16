using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.ProgressBar;
using MW = ModernWpf;

namespace Server_GUI2
{
    static class StartServer
    {
        private static Version Version;
        private static IWorld World;

        public static Windows.ProgressBar.ProgressBar RunProgressBar;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// </summary>
        public static void Run(Version version, IWorld world, bool isShutdown)
        {
            Version = version;
            World = world;

            // ProgressBarの表示
            RunProgressBar = new Windows.ProgressBar.ProgressBar($"Starting {version.Name} / {world.Name}", 10);
            //RunProgressBar.AddMessage("これは追加の例");
            //RunProgressBar.AddMessage("これは追加の例", addCount:false); -> これはメッセージを追加するが数字を進めない
            //RunProgressBar.AddCount(); -> これはメッセージを追加せずに数字だけ進める
            //RunProgressBar.AddMessage("これは追加の例", moving:true); -> これはビルド時やダウンロード時など、長時間値が進まないが処理を行う必要性のある場合に使用する

            // 最新の起動記録を保存
            RecordLatestVerWor();
            RunProgressBar.AddMessage("Recorded Latest Version and World");

            // versionの導入
            var ( path, jarName ) = Version.ReadyVersion();

            // Port Mapping
            bool successPortMapping = AddPort().Result;

            //サーバー実行
            World.WrapRun(
                Version,
                (serverProperty,arg) => Server.Start(
                    path,
                    jarName,
                    Version.Log4jArgument,
                    serverProperty,
                    arg
                    )
                );

            // PortMapping
            DeletePort(successPortMapping);

            //Shutdown
            ShutDown(isShutdown);

            ////サーバー実行
            //World.WrapRun(
            //    Version,
            //    serverProperty => DummyRun(
            //        path,
            //        jarName,
            //        Version.Log4jArgument,
            //        serverProperty
            //        )
            //    );
        }

        static void DummyRun(VersionPath path, string jarName, string log4jArgument, ServerSettings settings)
        {
            settings.Save(path);
        }

        private static void RecordLatestVerWor()
        {
            UserSettings.Instance.userSettings.LatestRun.VersionName = Version.Name;
            UserSettings.Instance.userSettings.LatestRun.VersionType = Version is VanillaVersion ? "vanilla" : "spigot";
            UserSettings.Instance.userSettings.LatestRun.WorldName = World.Name;
            UserSettings.Instance.WriteFile();
        }

        private static async Task<bool> AddPort()
        {
            if (!UserSettings.Instance.userSettings.PortSettings.UsingPortMapping)
                return false;

            bool isSuccess = await PortMapping.AddPort(UserSettings.Instance.userSettings.PortSettings.PortNumber);

            if (!isSuccess)
            {
                string message =
                    "自動ポート開放に失敗しました。\n" +
                    "ポートを開放せずにサーバーを起動します。";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Upnpに対応したポート番号を設定する
                World.Settings.ServerProperties.ServerPort = "2869";
            }

            return isSuccess;
        }

        private static async void DeletePort(bool isSuccess)
        {
            if (isSuccess)
                await PortMapping.DeletePort(UserSettings.Instance.userSettings.PortSettings.PortNumber);
        }

        private static void ShutDown(bool isShutdown)
        {
            if (!isShutdown)
                return;

            DialogResult result = AutoClosingMessageBox.Show(
                "PCを30秒後にシャットダウンしようとしています。\n" +
                "シャットダウンしない場合は「キャンセル」を押してください。", "Server Starter", 30000, MessageBoxButtons.OKCancel);

            if (result == DialogResult.Cancel)
                return;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = "/s /f /t 0",
                // ウィンドウを表示しない
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }
    }
}
