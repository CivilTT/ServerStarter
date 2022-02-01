using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;

namespace Server_GUI2.Develop.Server.Storage
{
    public abstract class RemoteWorld
    {
        public LocalWorld Link { get; }
        public abstract bool IsLinked();
        public Version Version { get; }
        public Storage Storage { get; }

        /// <summary>
        /// 即時反映
        /// リンクされたローカルワールドにプルし、リモートブランチを削除
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// データをプル、関数を実行、データをプッシュ
        /// </summary>
        public abstract void WrapRun(Action func, string path);
    }

}
