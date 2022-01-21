using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// TODO: VersionとServerの役割
// Version は1.x.xディレクトリの存在とserver.jar, start.batの存在を保証する
// Server はeura.txt等サーバー起動時に必要なデータの存在を保証する

namespace Server_GUI2
{
    public class World
    {
        public Version version;

        public string Name;

        public string CustomSource;

        public string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\{version.Name}\{Name}";
            }
        }

        public bool Exists
        {
            get
            {
                return Directory.Exists(Path);
            }
        }

        protected World(string name, Version ver)
        {
            Name = name;
            version = ver;
        }

        public void ChangeVersion(Version newVer)
        {

        }

        public void Recreate()
        {

        }

        public void Remove()
        {

        }

        public void SetCustomMap()
        {

        }
    }

    class VanillaWorld: World
    {}

    class SpigotWorld : World
    { }
  
    class ShareWorld<T> : World where T : World
    {
        private T World;
        public ShareWorld(T wrold)
        {
            World
        }
    }
}
