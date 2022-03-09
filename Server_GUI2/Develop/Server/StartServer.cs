using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;

namespace Server_GUI2
{
    static class StartServer
    {
        private static Version Version;
        private static World World;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// TODO: Op、WhiteListの登録、実行VersionとWorldをinfo.jsonに記入、自動シャットダウン、AutoPortMappingに基づいたポート開放＆閉鎖の実行
        /// </summary>
        public static void Run(Version version, World world)
        {
            Version = version;
            World = world;

            // versionの導入
            var ( path, jarName ) = Version.ReadyVersion();

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
