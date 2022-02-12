using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Util;

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
                    foreach (var worldDir in verDir.GetWorldDirectories())
                    {
                        // ログフォルダは無視
                        if (worldDir.Name == "logs")
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
        public WorldPath Path { get; private set; }

        public DatapackCollection Datapacks { get; private set; }
        
        public ServerProperty Property { get; private set; }

        public ServerType? Type { get; private set; }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public event EventHandler DeleteEvent;

        /// <summary>
        /// ワールドの設定をディレクトリに反映させる
        /// </summary>
        public LocalWorld(
            WorldPath path,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
            ReConstruct(path, version, type, property, datapacks);
        }

        /// <summary>
        /// ワールドの設定をディレクトリから取得する
        /// </summary>
        public LocalWorld(WorldPath path, Version version)
        {
            ReConstruct(path, version);
        }

        /// <summary>
        /// 同一インスタンスのまま初期化
        /// </summary>
        public void ReConstruct(WorldPath path, Version version)
        {
            Path = path;
            Name = path.Name;
            // フォルダ存在しない場合は新規作成
            if (!Path.Exists)
                CreateWorldData();
            Property = LoadProperties();
            Type = GetServerType();
            Datapacks = LoadDatapacks();
            Version = version;
        }

        /// <summary>
        /// 同一インスタンスのまま初期化
        /// </summary>
        public void ReConstruct(
            WorldPath path,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
            Path = path;
            Version = version;
            Name = path.Name;
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
            Property = property;
            SaveProperties();
            Datapacks = datapacks;
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
            return new LocalWorld(Path, version, type, Property, Datapacks);
        }

        public LocalWorld ToSpigot()
        {
            return new LocalWorld(Path, Version, ServerType.Vanilla, Property, Datapacks);
        }

        /// <summary>
        /// ワールドデータを指定パスに移動し設定を反映
        /// </summary>
        public void Move(
            WorldPath path,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks,
            bool addSuffixWhenNameCollided = false
            )
        {
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
            ReConstruct(newPath, version, type, property, datapacks);
        }

        /// <summary>
        /// ワールドデータを指定パスに移動
        /// </summary>
        public void Move(WorldPath path,  bool addSuffixWhenNameCollided = false)
        {
            Move(path, Version, Type, Property, Datapacks, addSuffixWhenNameCollided);
        }


        /// <summary>
        /// ワールドデータを削除する
        /// </summary>
        public void Delete()
        {
            // 削除イベントの呼び出し
            if (DeleteEvent != null) DeleteEvent(this,null);
            // ワールドデータを全削除
            Path.Delete();
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
        public void WrapRun(Action<ServerProperty> runFunc)
        {
            // levelname を変更
            Property.LevelName = Path.World.FullName;
            // 起動
            runFunc(Property);
            // levelname を空白に戻す
            Property.LevelName = "";
            // ServerPropertyを保存
            Path.ServerProperties.WriteAllText(Property.ExportProperty());
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
            if (Path.ServerProperties.Exists)
                return new ServerProperty(Path.ServerProperties.ReadAllText());
            else
                return new ServerProperty();
        }

        /// <summary>
        // server.propertiesを保存する
        /// </summary>
        private void SaveProperties()
        {
            Path.ServerProperties.WriteAllText(Property.ExportProperty());
        }

        private DatapackCollection LoadDatapacks()
        {
            if (Path.World.Datapccks.Exists)
            {
                Console.WriteLine(Path.World.Datapccks.FullName);
                var collection = new List<string>();
                foreach (var datapack in Path.World.Datapccks.GetDatapackDirectories())
                {
                    Console.WriteLine(datapack.FullName);
                    if (datapack.Mcmeta.Exists && datapack.Data.Exists)
                        collection.Add(datapack.Name);
                }
                return new DatapackCollection(collection);
            }
            else
            {
                return new DatapackCollection(new List<string>());
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
            return new WorldState(Name, Type.ToString(), Version.Name, false, Datapacks.ExportList(), Property);
        }
    }
}
