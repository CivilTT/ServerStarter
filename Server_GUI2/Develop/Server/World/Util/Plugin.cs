using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Server_GUI2.Develop.Server.World
{
    public class PluginCollection
    {
        public ObservableCollection<APlugin> Plugins { get; }
        private List<Action<string>> operations = new List<Action<string>>();

        public PluginCollection()
        {
            Plugins = new ObservableCollection<APlugin>();
        }

        public PluginCollection(List<string> pluginNames)
        {
            Plugins = new ObservableCollection<APlugin>(pluginNames.Select(x => new ExistPlugin(x)));
        }

        public PluginCollection(PluginCollection plugins)
        {
            Plugins = new ObservableCollection<APlugin>(plugins.Plugins);
            operations = new List<Action<string>>(plugins.operations);
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
        public void Evaluate(PluginsPath path)
        {
            path.Create();
            foreach (var operation in operations)
                operation(path.FullName);
            StartServer.RunProgressBar.AddMessage("Imported Plugins.");
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

        public virtual void Remove(string path) { }
    }

    public class ExistPlugin : APlugin
    {
        public ExistPlugin(string name) : base(name)
        {

        }

        public override void Remove(string path)
        {
            File.Delete(Path.Combine(path,$"world\\plugins\\{Name}"));
        }
    }

    public class Plugin : APlugin
    {
        private readonly string SourcePath;

        public Plugin(string name, string sourcePath) : base(name)
        {
            SourcePath = sourcePath;
        }

        public static Plugin TryGenInstance(string sourcePath, bool isZip)
        {
            if (Path.GetExtension(sourcePath) == ".jar")
            {
                string name = Path.GetFileNameWithoutExtension(sourcePath);
                return new Plugin(name, sourcePath);
            }
            else
            {
                return null;
            }
        }

        public void Import(string path)
        {

            var filepath = Path.Combine(path, Path.GetFileName(SourcePath));

            Console.WriteLine(path);
            File.Move(SourcePath, filepath);
        }
    }
}
