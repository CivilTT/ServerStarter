using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using log4net;

namespace Server_GUI2
{
    class Version
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name;
        private readonly string Path;
        private readonly bool Exists;

        public ServerProperty ServerProperty { get; set; }

        public Version(string name, string path, bool exists)
        {
            Name = name;
            Path = path;
            Exists = exists;
        }

        public void DownloadVersion() { }

        // 比較演算子系のオーバーライド
        // 最新、旧版の比較をする
        public bool gt(Version version) { }
    }
}
