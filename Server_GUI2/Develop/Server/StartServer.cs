using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    static class StartServer
    {
        private static Version Version;
        private static World World;

        /// <summary>
        /// Runボタンが押された時に呼ばれる処理
        /// </summary>
        public static void Run(Version version, World world, Develop.Server.World.WorldSaveLocation location)
        {
            Version = version;
            World = world;

            Version.SetNewVersion();

            Develop.Server.World.WorldWriter worldWriter = World.Preprocess(Version, location);

            Server.Start(Version.Path, Version.JarName, Version.Log4jArgument);

            worldWriter.Postprocess();
        }
    }
}
