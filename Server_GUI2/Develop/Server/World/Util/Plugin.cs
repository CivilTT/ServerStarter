using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Server_GUI2
{
    public class PluginCollection
    {
        public ObservableCollection<APlugin> Plugins { get; }
        private List<Action<string>> operations = new List<Action<string>>();

        public PluginCollection(List<string> pluginNames)
        {
            Plugins = new ObservableCollection<APlugin>(pluginNames.Select(x => new ExistPlugin(x)));
        }

        public List<string> ExportList()
        {
            return Plugins.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// データパックを追加する
        /// (ディレクトリ操作は行わない)
        /// </summary>
        public void Add(Plugin plugin)
        {
            Plugins.Add(plugin);
            operations.Add(plugin.Import);
        }

        /// <summary>
        /// データパックを削除する
        /// (ディレクトリ操作は行わない)
        /// </summary>
        public void Remove(APlugin plugin)
        {
            Plugins.Remove(plugin);
            operations.Add(plugin.Remove);
        }

        /// <summary>
        /// データパックの削除と追加をディレクトリ上で実際に行う
        /// </summary>
        public void Evaluate(string path)
        {
            foreach (var operation in operations)
                operation(path);
        }

        public List<string> GetNames()
        {
            return Plugins.Select(x => x.Name).ToList();
        }
    }


    public class APlugin
    {
        public ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; private set; }

        protected bool IsZip;

        protected APlugin(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 削除の必要性があるかどうか
        /// TODO: Viewを更新するためにEventを作成する
        /// </summary>
        public bool NeedToRemove;

        public virtual void Remove(string path) { }
    }

    public class ExistPlugin : APlugin
    {
        public ExistPlugin(string name) : base(name)
        {

        }

        public override void Remove(string path)
        {
            File.Delete(path);
        }
    }

    public class Plugin : APlugin
    {
        private readonly string SourcePath;

        public Plugin(string name, string sourcePath) : base(name)
        {
            SourcePath = sourcePath;
        }

        public void Import(string path)
        {
            File.Move(SourcePath, path);
        }
    }
}
