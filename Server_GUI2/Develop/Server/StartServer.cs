using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.ProgressBar;

namespace Server_GUI2
{
    static class StartServer
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Version Version;
        private static IWorld World;

        public static ProgressBar RunProgressBar;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// TODO: Op、WhiteListの登録、実行VersionとWorldをinfo.jsonに記入、自動シャットダウン、AutoPortMappingに基づいたポート開放＆閉鎖の実行
        /// </summary>
        public static void Run(Version version, IWorld world)
        {
            logger.Info($"<Run>");
            Version = version;
            World = world;

            // ProgressBarの表示
            RunProgressBar = new ProgressBar($"Starting {version.Name} / {world.Name}", 10);
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

            // Port Mapping


            //サーバー実行
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

        static void DummyRun(VersionPath path, string jarName, string log4jArgument, ServerSettings settings)
        {
            settings.Save(path);
        }

        private static void RecordLatestVerWor()
        {
            //UserSettings.Instance.userSettings.LatestRun.VersionName = Version.Name;
            //UserSettings.Instance.userSettings.LatestRun.VersionType = Version is VanillaVersion ? "vanilla" : "spigot";
            //UserSettings.Instance.userSettings.LatestRun.WorldName = World.Name;
            //UserSettings.Instance.WriteFile();
        }
    }
}
