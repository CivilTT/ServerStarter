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

        // 最新版か否か
        public bool isLatest;

        // VanilaかSpigotか
        public bool isVanila;


        public ServerProperty ServerProperty { get; set; }

        public Version(string name, string downloadurl, bool isrelease=true, bool islatest=false, bool isvanila=true)
        {
            Name = name;
            downloadURL = downloadurl;
            isRelease = isrelease;
            isLatest = islatest;
            isVanila = isvanila;
        }

        public void DownloadVersion() { }

        // 比較演算子系のオーバーライド
        // 最新、旧版の比較をする
        public bool gt(Version version) { }
    }
}
