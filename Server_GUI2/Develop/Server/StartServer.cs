using log4net;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Server_GUI2
{
    static class StartServer
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Version Version;
        private static IWorld World;

        public static Windows.ProgressBar.ProgressBar RunProgressBar;

        public static Windows.ProgressBar.ProgressBar CloseProgressBar;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// </summary>
        public static void Run(Version version, bool reGenerate, IWorld world)
        {
            logger.Info($"<Run>");
            Version = version;
            World = world;

            // ProgressBarの表示
            RunProgressBar = new Windows.ProgressBar.ProgressBar($"{Properties.Resources.RunBar_Title1}{version.Name} / {world.Name}{Properties.Resources.RunBar_Title2}", 12);
            //RunProgressBar.AddMessage("これは追加の例");
            //RunProgressBar.AddMessage("これは追加の例", addCount:false); -> これはメッセージを追加するが数字を進めない
            //RunProgressBar.AddCount(); -> これはメッセージを追加せずに数字だけ進める
            //RunProgressBar.AddMessage("これは追加の例", moving:true); -> これはビルド時やダウンロード時など、長時間値が進まないが処理を行う必要性のある場合に使用する

            // 最新の起動記録を保存
            RecordLatestVerWor();
            RunProgressBar.AddMessage(Properties.Resources.RunBar_Record);

            // versionの導入
            var ( path, jarName, javaVersion) = Version.ReadyVersion();
            logger.Info($"best java version ({javaVersion})");
            var javaPath = Java.GetBestJavaPath(javaVersion);
            logger.Info($"use java path ({javaPath})");
            RunProgressBar.AddMessage($"{Properties.Resources.RunBar_Java} java {javaVersion}");

            // Port Mapping
            var portMapper = Task.Run(AddPort);

            //サーバー実行
            World.WrapRun(
                Version,
                reGenerate,
                (serverProperty,arg) => Server.Start(
                    path,
                    javaPath,
                    jarName,
                    Version.Log4jArgument,
                    serverProperty,
                    arg
                    )
                );

            // PortMapping
            DeletePort(portMapper.Result);

            // Close Progress Bar
            //CloseProgressBar.ShowCount();
            CloseProgressBar.Close();

            //Shutdown
            ShutDown();

            logger.Info($"</Run>");
        }

        private static void RecordLatestVerWor()
        {
            UserSettings.Instance.userSettings.LatestRun = new LatestRun(Version, World);
            UserSettings.Instance.WriteFile();
        }

        private static async Task<bool> AddPort()
        {
            if (!UserSettings.Instance.userSettings.PortSettings.UsingPortMapping)
            {
                World.Settings.ServerProperties.ServerPort = "25565";
                return false;
            }

            bool isSuccess = await PortMapping.AddPort(UserSettings.Instance.userSettings.PortSettings.PortNumber);

            if (!isSuccess)
            {
                CustomMessageBox.Show(Properties.Resources.StartServer_PortMsg, ButtonType.OK, Image.Warning);
            }
            else
            {
                // Upnpに対応したポート番号を設定する
                World.Settings.ServerProperties.ServerPort = PortMapping.LocalPort.ToString();
            }

            return isSuccess;
        }

        private static async void DeletePort(bool isSuccess)
        {
            if (isSuccess)
                await PortMapping.DeletePort(UserSettings.Instance.userSettings.PortSettings.PortNumber);
        }

        private static void ShutDown()
        {
            if (!UserSettings.Instance.userSettings.ShutdownPC)
                return;

            logger.Info("Show window to check PC shutdown");
            int result = CustomMessageBox.Show(Properties.Resources.StartServer_ShutdownMsg, ButtonType.OKCancel, Image.Infomation, 30000);

            if (result == 1)
                return;

            logger.Info("This PC will be shutdown");
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
