using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
    public class WorldWriter
    {
        public string Path { get; }
        protected WorldWriter() { }
        /// <summary>
        /// gitの使用中フラグ
        /// </summary>
        public virtual void Preprocess() { }
        public virtual void Postprocess() { }
    }


    public class LocalWorldWriter : WorldWriter { }

    public class GitWorldWriter : WorldWriter { }
}
