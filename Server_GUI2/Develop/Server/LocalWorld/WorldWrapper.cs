using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.LocalWorld
{
    class WorldWrapper
    {
        protected virtual World World { get;}
    }

    class UnLinkedLocalWorldWrapper: WorldWrapper
    {
        private LocalWorld LocalWorld { get; }
        protected override World World => LocalWorld;
        public UnLinkedLocalWorldWrapper(LocalWorld world)
        {
            LocalWorld = world;
        }
        public LinkedRemoteWorldWrapper Link(RemoteWorld remoteWorld)
        {
            return new LinkedRemoteWorldWrapper(remoteWorld, LocalWorld.Path);
        }
    }

    class LinkedRemoteWorldWrapper : WorldWrapper
    {
        private string path;
        private RemoteWorld RemoteWorld { get; }
        protected override World World => RemoteWorld;

        public LinkedRemoteWorldWrapper(RemoteWorld remoteWorld,string path)
        {
            this.path = path;
            RemoteWorld = remoteWorld;
        }

        public UnLinkedLocalWorldWrapper UnLink()
        {
            // リモートの情報をpullしてから接続を解除
            return new UnLinkedLocalWorldWrapper(RemoteWorld.ToLocal(path));
        }
    }
}
