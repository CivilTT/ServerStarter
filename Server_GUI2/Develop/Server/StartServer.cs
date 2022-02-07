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
        private static WorldWrapper World;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// </summary>
        public static void Run(Version version, WorldWrapper world)
        {
            Version = version;
            World = world;

            // versionの導入
            var ( path, jarName ) = Version.ReadyVersion();

            //サーバー実行
            World.WrapRun(
                Version,
                serverProperty => Server.Start(
                    path,
                    jarName,
                    Version.Log4jArgument,
                    serverProperty
                    )
                );
        }
    }
}
