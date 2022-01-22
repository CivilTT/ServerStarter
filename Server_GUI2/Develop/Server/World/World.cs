using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Version Version;

        public string Name;

        public bool Exists;

        private ObservableCollection<Datapack> Datapacks;

        public virtual string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\{Version.Name}\{Name}";
            }
        }

        /// <summary>
        /// generate World from other World instance
        /// </summary>
        /// <returns></returns>
        protected static World ConvertFrom(World world, Version version)
        {
            throw new NotImplementedException();
        }

        protected World(string name, Version version)
        {
            Name = name;
            Version = version;
            Exists = Directory.Exists(Path);
        }

        /// <summary>
        /// ServerPropertyにlevel-name等を記入
        /// </summary>
        /// <param name="serverProperty"></param>
        public void WriteProperty(ServerProperty serverProperty)
        {
            
        }

        public virtual void Recreate()
        {

        }

        public virtual void Remove()
        {

        }

        public virtual void SetCustomMap(string path)
        {

        }

    }

    class VanillaWorld: World
    {}

    class SpigotWorld : World
    {}
  
    class ShareWorld<T> : World
    {
        string GitAccount;
        string GitRepository;

        World World;
        Git Git;

        public ShareWorld(string name, Version version, World world, Git git) : base(name, version)
        {
            World = world;
            Git = git;
        }

        public override string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\{Version.Name}\{Name}\world";
            }
        }


        private void Pull()
        { }

        private void Push()
        { }

        private string GitURL
        {
            get
            {
                return $@"https://{GitAccount}@github.com/{GitAccount}/{GitRepository}";
            }
        }
    }
}
