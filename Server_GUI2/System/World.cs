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
    class World
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Version Ver;

        public string Name;

        public string CustomSource;

        public string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\{Ver.Name}\{Name}";
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
            Ver = ver;
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
