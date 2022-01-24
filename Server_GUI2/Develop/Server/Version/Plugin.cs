﻿using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server
{
    class Plugin
    {
        public ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Version Version;

        public string Name { get; private set; }

        public string Path { get { return $@"{Version.Path}\plugins\{Name}.jar"; } }

        public Plugin(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// 削除の必要性があるかどうか
        /// TODO: Viewを更新するためにEventを作成する
        /// </summary>
        public bool IsRemove;

        protected virtual void Remove() { }

        protected virtual void Import() { }

        public void Ready()
        {
            if (IsRemove)
                Remove();

            Import();
        }

    }
    
    class ExistPlugin : Plugin
    {
        public ExistPlugin(string name, Version version) : base(name, version)
        {

        }

        protected override void Remove()
        {
            File.Delete(Path);
        }
    }

    class ImportPlugin : Plugin
    {
        private readonly string SourcePath;

        public ImportPlugin(string name, Version version, string sourcePath) : base(name, version)
        {
            SourcePath = sourcePath;
        }

        protected override void Import()
        {
            File.Move(SourcePath, Path);
        }
    }
}