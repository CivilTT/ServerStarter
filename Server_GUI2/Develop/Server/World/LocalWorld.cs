using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Util;
using log4net;
using System.Reflection;

namespace Server_GUI2.Develop.Server.World
{
    public class LocalWorldCollection
    {
        public readonly ObservableCollection<LocalWorld> LocalWorlds = new ObservableCollection<LocalWorld>();
        public static LocalWorldCollection Instance { get; } = new LocalWorldCollection(ServerGuiPath.Instance.WorldData);
        private LocalWorldCollection(WorldDataPath path)
        {
            //　ディレクトリを走査し既存ワールド一覧を取得
            foreach (var verDir in path.GetVersionDirectories())
            {
                try
                {
                    var version = VersionFactory.Instance.GetVersionFromName(verDir.Name);
                    verDir.Worlds.Create(true);
                    foreach (var worldDir in verDir.Worlds.GetWorldDirectories())
                    {
                        // ログフォルダは無視
                        if (worldDir.Name == "logs" || worldDir.Name == "crash-reports")
                            continue;
                        var world = new LocalWorld(worldDir, version);
                        // コレクションに追加
                        LocalWorlds.Add(world);

                        // ワールド削除時にコレクションから削除
                        world.DeleteEvent += new EventHandler((sender,arg) => LocalWorlds.Remove(world));
                    }
                }
                catch (KeyNotFoundException)
                {
                    //version名でないディレクトリは無視
                    continue;
                }
            }
        }

        /// <summary>
        /// 条件に当てはまるローカルワールドを検索する
        /// 無かったらnull
        /// </summary>
        public LocalWorld FindLocalWorld(string version,string world)
        {
            return LocalWorlds.Where(x => x.Name == world && x.Version.Name == version).FirstOrDefault();
        }
    }

    /// <summary>
    /// ワールドデータをあらわすクラス
    /// </summary>
    public class LocalWorld : IWorldBase
    {
        protected readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WorldPath Path { get; private set; }

        public DatapackCollection Datapacks { get; set; }

        public PluginCollection Plugins { get; set; }
        
        public ServerSettings Settings { get; set; }

        public ServerType? Type { get; private set; }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public event EventHandler DeleteEvent;

        // バージョンデータがなくなったら削除
        private EventHandler WhenVersionDeleted;

        /// <summary>
        /// ワールドの設定をディレクトリに反映させる
        /// </summary>
        public LocalWorld(
            WorldPath path,
            Version version,
            ServerType? type,
            ServerSettings settings,
            DatapackCollection datapacks,
            PluginCollection plugins
            )
        {
            WhenVersionDeleted = new EventHandler((_, __) => Delete());
            ReConstruct(path, version, type, settings, datapacks, plugins);
        }

        public void SetVersion(Version version)
        {
            Version = version;
        }

        /// <summary>
        /// ワールドの設定をディレクトリから取得する
        /// </summary>
        public LocalWorld(WorldPath path, Version version)
        {
            WhenVersionDeleted = new EventHandler((_, __) => Delete());
            ReConstruct(path, version);
        }

        /// <summary>
        /// 同一インスタンスのまま初期化
        /// </summary>
        public void ReConstruct(WorldPath path, Version version)
        {
            // 元のバージョンからイベントを削除
            if (Version != null) Version.DeleteEvent -= WhenVersionDeleted;

            Path = path;
            Name = path.Name;
            // フォルダ存在しない場合は新規作成
            if (!Path.Exists)
                CreateWorldData();
            Settings = new ServerSettings(Path);
            Type = GetServerType();
            Datapacks = LoadDatapacks();
            Plugins = LoadPlugins();
            Version = version;

            // 新しいバージョンにイベントを登録
            version.DeleteEvent += WhenVersionDeleted;
        }

        /// <summary>
        /// 同一インスタンスのまま初期化
        /// </summary>
        public void ReConstruct(
            WorldPath path,
            Version version,
            ServerType? type,
            ServerSettings setting,
            DatapackCollection datapacks,
            PluginCollection plugins
            )
        {
            // 元のバージョンからイベントを削除
            Version.DeleteEvent -= WhenVersionDeleted;

            Path = path;
            Name = path.Name;

            Version = version;

            // フォルダ存在しない場合は新規作成
            if (!Path.Exists)
                CreateWorldData();

            var currentType = GetServerType();
            //if (currentType == null) : 変換不要
            // 目的サーバーがない場合はワールドを初期化
            if (type == null)
                DeleteWorldData();
            if (currentType == ServerType.Vanilla && type == ServerType.Spigot)
                VtoS();
            else if (currentType == ServerType.Spigot && type == ServerType.Vanilla)
                StoV();

            Type = type;
            Settings = setting;
            SaveSettings();
            Datapacks = datapacks;
            Plugins = plugins;

            // 新しいバージョンにイベントを登録
            version.DeleteEvent += WhenVersionDeleted;

        }


        public LocalWorld ConvertVersion(Version version)
        {
            ServerType type;
            if (version is VanillaVersion)
                type = ServerType.Vanilla;
            else if (version is SpigotVersion)
                type = ServerType.Spigot;
            else                
                throw new ArgumentException($"\"{version.GetType()}\" is unknowen version.");
            return new LocalWorld(Path, version, type, Settings, Datapacks, Plugins);
        }

        public LocalWorld ToSpigot()
        {
            return new LocalWorld(Path, Version, ServerType.Vanilla, Settings, Datapacks, Plugins);
        }

        /// <summary>
        /// ワールドデータを指定パスに移動し設定を反映
        /// </summary>
        public void Move(
            WorldPath path,
            Version version,
            ServerType? type,
            ServerSettings settings,
            DatapackCollection datapacks,
            PluginCollection plugins,
            bool addSuffixWhenNameCollided = false
            )
        {
            // パスとバージョンが同じだった場合何もしない
            if (Path.FullName == path.FullName && Version == version)
                return;

            var name = path.Name;
            var newPath = path;
            // 名前が衝突したら filename(x) と名前を変更
            if (addSuffixWhenNameCollided)
            {
                var suffixNum = 1;
                while (newPath.Exists)
                {
                    suffixNum += 1;
                    newPath = path.Parent.GetWorldDirectory($"{name}({suffixNum})");
                }
            }
            Path.MoveTo(newPath);
            ReConstruct(newPath, version, type, settings, datapacks, plugins);
        }

        /// <summary>
        /// ワールドデータを指定パスに移動
        /// </summary>
        public void Move(WorldPath path,  bool addSuffixWhenNameCollided = false)
        {
            Move(path, Version, Type, Settings, Datapacks, Plugins, addSuffixWhenNameCollided);
        }


        /// <summary>
        /// ワールドデータを削除する
        /// </summary>
        public void Delete()
        {
            // 削除イベントの呼び出し
            DeleteEvent?.Invoke(this,null);

            // バージョン削除イベントからこのワールドの削除イベントを削除
            Version.DeleteEvent -= WhenVersionDeleted;

            // ワールドデータを全削除
            Path.Delete(force:true);
        }

        /// <summary>
        /// 指定パスにワールドデータを新規作成する
        /// </summary>
        private void CreateWorldData()
        {
            Path.Create();
        }

        /// <summary>
        /// ワールドデータを削除<br/>
        /// /world /world_nether /world_end を削除
        /// </summary>
        private void DeleteWorldData()
        {
            if (Path.World.Exists) Path.World.Delete();
            if (Path.Nether.Exists) Path.Nether.Delete();
            if (Path.End.Exists) Path.End.Delete();
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public void WrapRun(Version version, Action<ServerSettings, string> runFunc)
        {
            logger.Info("<WrapRun>");
            
            logger.Info($"change levelname to 'worlds/{Path.Name}/{Path.World.Name}'");

            string arg = "";

            // level-name を変更
            switch (version.Type)
            {
                // vanillaの場合はlevel-nameを{worldname}/worldに
                case ServerType.Vanilla:
                    Settings.ServerProperties.LevelName = $"worlds/{Path.Name}/{Path.World.Name}";
                    break;
                // spigotの場合は起動時引数に level-nameをworldに
                case ServerType.Spigot:
                    Settings.ServerProperties.LevelName = Path.World.Name;
                    arg = $" --world-container worlds/{Path.Name}";
                    break;
            }

            StartServer.RunProgressBar.AddMessage("saving setting data");

            // 設定データをサーバー起動用に保存
            Settings.Save(version.Path);

            // 起動
            runFunc(Settings, arg);

            // levelname を空白に戻す
            logger.Info("delete levelname");
            Settings.ServerProperties.LevelName = "";

            // 設定データを保存
            logger.Info("save world settings");
            Settings.Save(Path);

            logger.Info("</WrapRun>");
        }

        /// <summary>
        // ServerTypeを判定する
        /// </summary>
        private ServerType? GetServerType()
        {
            if (Path.World.Exists)
            {
                if (Path.Nether.Exists)
                    return ServerType.Spigot;
                else 
                    return ServerType.Vanilla;
            }
            else
                return null;
        }

        /// <summary>
        // server.propertiesを読み込む
        /// </summary>
        private ServerProperty LoadProperties()
        {
            return Path.ServerProperties.ReadAllText().SuccessFunc(x => new ServerProperty(x)).SuccessOrDefault(ServerProperty.GetUserDefault());
        }

        /// <summary>
        // server.propertiesを保存する
        /// </summary>
        private void SaveSettings()
        {
            Settings.Save(Path);
        }

        private DatapackCollection LoadDatapacks()
        {
            if (Path.World.Datapccks.Exists)
            {
                var collection = new List<string>();
                foreach (var datapack in Path.World.Datapccks.GetDatapackDirectories())
                {
                    if (datapack.Mcmeta.Exists && datapack.Data.Exists)
                        collection.Add(datapack.Name);
                }
                return new DatapackCollection(collection);
            }
            else
            {
                return new DatapackCollection();
            }
        }
        private PluginCollection LoadPlugins()
        {
            if (Path.World.Plugins.Exists)
            {
                var collection = new List<string>();
                foreach (var plugin in Path.World.Plugins.GetPluginDirectories())
                {
                    collection.Add(plugin.Name);
                }
                return new PluginCollection(collection);
            }
            else
            {
                return new PluginCollection();
            }
        }

        /// <summary>
        /// Convert Vanilla to Spigot
        /// </summary>
        private void VtoS()
        {
            Path.Nether.Create(true);
            Path.End.Create(true);

            Path.World.DIM_1.MoveTo(Path.Nether.DIM_1);
            Path.World.DIM1.MoveTo(Path.End.DIM1);

            foreach (var dim in new WorldSubPath[]{ Path.Nether, Path.End })
            {
                Path.World.LevelDat.MoveTo(dim.LevelDat.File);
                Path.World.LevelDatOld.MoveTo(dim.LevelDatOld.File);
                Path.World.SessionLock.MoveTo(dim.SessionLock.File);
            }
        }

        /// <summary>
        /// Convert Spigot to Vanilla
        /// </summary>
        private void StoV()
        {
            Path.World.UidDat.Delete(true);

            Path.Nether.DIM_1.MoveTo(Path.World.DIM_1);
            Path.End.DIM1.MoveTo(Path.World.DIM1);

            Path.Nether.Delete();
            Path.End.Delete();
        }

        public WorldState ExportWorldState()
        {
            throw new NotImplementedException("local world is not intended to export worldstate");
        }
    }
}
