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
    public class Version:IComparable<Version>
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

        //public string downloadURL;

        // このバージョンはVanilaか
        // public bool isVanila = true;


        // 最新バージョンか否か
        // public bool isLatest;


        public ServerProperty ServerProperty { get; set; }

        protected Version(string name)
        {
            Name = name;
        }

        public virtual void DownloadVersion() { }
        public virtual void Remove() { }

        // 比較可能にする
        public virtual int CompareTo(Version obj)
        {
            throw new NotImplementedException();
        }
    }

    public class VanillaVersion: Version
    {
        // このバージョンがリリース版かスナップショットか
        public bool IsRelease;

        // Spigotとしてこのバージョンはありうるのか（ローカルにあるか否かは関係ない）
        public bool HasSpigot;

        private string DownloadURL;
        public VanillaVersion(string name, string downloadURL, bool isRelease, bool hasSpigot): base(name)
        {
            IsRelease = isRelease;
            HasSpigot = hasSpigot;
            DownloadURL = downloadURL;
        }
    }

    public class SpigotVersion: Version
    { 
        private string DownloadURL;
        public SpigotVersion(string name, string downloadURL) : base(name)
        {
            DownloadURL = downloadURL;
        }
    }
}
