using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    class Server
    {
        private World World;
        private Server Version;

        public Server(World world,Server version)
        {
            World = world;
            Version = version;
        }

        public static void Run()
        {
            throw new NotImplementedException();
        }

    }
}
