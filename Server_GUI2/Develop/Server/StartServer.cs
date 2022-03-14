using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Windows.ProgressBar;

namespace Server_GUI2
{
    static class StartServer
    {
        private static Version Version;
        private static IWorld World;

        public static ProgressBar RunProgressBar;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// TODO: Op、WhiteListの登録、実行VersionとWorldをinfo.jsonに記入、自動シャットダウン、AutoPortMappingに基づいたポート開放＆閉鎖の実行
        /// </summary>
        public static void Run(Version version, IWorld world)
        {
            Version = version;
            World = world;

            // ProgressBarの表示
            RunProgressBar = new ProgressBar($"Ready to start the server ({version.Name}/{world.Name})", 10);
            //RunProgressBar.AddMessage("これは追加の例");
            //RunProgressBar.AddMessage("これは追加の例", false); -> これはメッセージを追加するが数字を進めない
            //RunProgressBar.AddCount(); -> これはメッセージを追加せずに数字だけ進める

            // versionの導入
            var ( path, jarName ) = Version.ReadyVersion();

            // Port Mapping
            

            //サーバー実行
            World.WrapRun(
                Version,
                serverProperty => Server.Start(
                    path.FullName,
                    jarName,
                    Version.Log4jArgument,
                    serverProperty
                    )
                );
        }
    }
}
