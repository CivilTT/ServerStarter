﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using log4net;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using Server_GUI2.Windows.ProgressBar;
using MW = ModernWpf;

namespace Server_GUI2
{
    static class StartServer
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Version Version;
        private static IWorld World;

        public static Windows.ProgressBar.ProgressBar RunProgressBar;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// </summary>
        public static async void Run(Version version, IWorld world)
        {
            logger.Info($"<Run>");
            Version = version;
            World = world;

            // ProgressBarの表示
            RunProgressBar = new Windows.ProgressBar.ProgressBar($"Starting {version.Name} / {world.Name}", 12);
            //RunProgressBar.AddMessage("これは追加の例");
            //RunProgressBar.AddMessage("これは追加の例", addCount:false); -> これはメッセージを追加するが数字を進めない
            //RunProgressBar.AddCount(); -> これはメッセージを追加せずに数字だけ進める
            //RunProgressBar.AddMessage("これは追加の例", moving:true); -> これはビルド時やダウンロード時など、長時間値が進まないが処理を行う必要性のある場合に使用する

            // 最新の起動記録を保存
            RecordLatestVerWor();
            RunProgressBar.AddMessage("Recorded Latest Version and World");

            // versionの導入
            var ( path, jarName, javaVersion) = Version.ReadyVersion();
            logger.Info($"best java version ({javaVersion})");
            var javaPath = Java.GetBestJavaPath(javaVersion);
            logger.Info($"use java path ({javaPath})");
            RunProgressBar.AddMessage($"Decide to using java {javaVersion}");

            // Port Mapping
            bool successPortMapping = await AddPort();
            RunProgressBar.AddMessage("Finished Port Mapping");

            //サーバー実行
            // TODO: Run内にプログレスバーのチェックポイントを立て、起動直前にBarを閉じる
            World.WrapRun(
                Version,
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
            DeletePort(successPortMapping);

            //Shutdown
            ShutDown();

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
            logger.Info($"</Run>");
        }

        static void DummyRun(VersionPath path, string javaPath,string jarName, string log4jArgument, ServerSettings settings,string arg)
        {
            settings.Save(path);
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
                string message =
                    "自動ポート開放に失敗しました。\n" +
                    "ポートを開放せずにサーバーを起動します。";
                CustomMessageBox.Show(message, ButtonType.OK, Image.Warning);
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

            string result = CustomMessageBox.Show(
                "PCを30秒後にシャットダウンしようとしています。\n" +
                "シャットダウンしない場合は「キャンセル」を押してください。", ButtonType.OKCancel, Image.Infomation, 30000);

            if (result == "Cancel")
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
