using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util ;

namespace Server_GUI2.Develop.Server.Storage
{
    public class RemoteWorld
    {
        public string Name;
        public string Id => $"{Storage.Id}/{Name}";

        public ReadOnlyProperty<LocalWorld> Link { get; }
        public bool IsLinked()
        {
            return Link.Value != null;
        }
        public Version Version { get; }
        public Storage Storage { get; }

        public RemoteWorld(Storage storage, string name, WorldState worldState)
        {
            Version = VersionFactory.Instance.GetVersionFromName(worldState.Version);
            Storage = storage;
            Name = name;
            Link = WorldLink.Instance.GetLinkedLocal(this);
        }

        /// <summary>
        /// 即時反映
        /// リンクされたローカルワールドにプルし、リモートブランチを削除
        /// </summary>
        public void Delete()
        {

        }

        /// <summary>
        /// データをプル、関数を実行、データをプッシュ
        /// </summary>
        public void WrapRun(Action func, string path)
        {

        }
    }

}
