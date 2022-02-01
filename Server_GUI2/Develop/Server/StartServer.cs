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
        private static LocalWorld World;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// </summary>
        public static void Run(Version version, LocalWorld world, Develop.Server.Storage.Storage storage)
        {
            Version = version;
            World = world;

            // versionの導入
            var ( path, jarName ) = Version.ReadyVersion();

            // サーバー実行
            //world.WrapRun(
            //    Version,
            //    storage
            //    () => Server.Start(
            //        path,
            //        jarName,
            //        Version.Log4jArgument,
            //        world.serverProperty
            //        ),
            //    );
        }
    }
}
