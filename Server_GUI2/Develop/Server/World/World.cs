using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public World(string name, Version ver)
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
}
