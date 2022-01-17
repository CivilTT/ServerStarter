using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using log4net;

namespace Server_GUI2
{
    public class Version
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name;
        private string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\{Name}\";
            }
        }

        public bool Exists
        {
            get
            {
                return Directory.Exists(Path);
            }
        }

        public string downloadURL;
        
        // このバージョンがリリース版かスナップショットか
        public bool isRelease;

        // VanilaかSpigotか
        public bool hasSpigot;

        // 最新バージョンか否か
        public bool isLatest;


        public ServerProperty ServerProperty { get; set; }

        public Version(string name, string downloadurl, bool hasspigot, bool isrelease, bool islatest)
        {
            Name = name;
            downloadURL = downloadurl;
            isRelease = isrelease;
            hasSpigot = hasspigot;
            isLatest = islatest;
        }

        public void DownloadVersion() { }

        // 比較演算子系のオーバーライド
        // 最新、旧版の比較をする
        //public bool gt(Version version) { }
    }
}
