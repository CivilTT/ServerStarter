using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server;

namespace Server_GUI2.Develop.Server.World
{
    public abstract class World
    {
        public DatapackCollection Datapacks { get; protected set; }
        // TODO: pluginの読み込み
        //public ObservableCollection<Datapack> Pligins { get; }
        public ServerProperty Property { get; protected set; }
        public ServerType? Type { get; protected set; }
        public string Name { get; protected set; }
        public Version Version { get; protected set; }
    }

    /// <summary>
    /// ワールドデータをあらわすクラス
    /// </summary>
    public class LocalWorld : World
    {
        public string Path { get; }

        /// <summary>
        /// ワールドの設定をディレクトリに反映させる
        /// </summary>
        public LocalWorld(
            string path,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
            Path = path;
            Version = version;
            Name = System.IO.Path.GetDirectoryName(path);
            // フォルダ存在しない場合は新規作成
            if (!System.IO.Directory.Exists(Path))
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
                throw new ArgumentException($"\"{version.GetType().ToString()}\" is unknowen version.");
            return new LocalWorld(Path, version, type, Property, Datapacks);
        }

        public LocalWorld ToSpigot()
        {
            return new LocalWorld(Path, Version, ServerType.Vanilla, Property, Datapacks);
        }

        /// <summary>
        /// ワールドデータを指定パスに移動
        /// </summary>
        public LocalWorld Move(string path,bool addSuffixWhenNameCollided = false)
        {
            var newPath = path;
            // 名前が衝突したら filename(x) と名前を変更
            if (addSuffixWhenNameCollided)
            {
                var suffixNum = 1;
                while (System.IO.Directory.Exists(newPath))
                {
                    suffixNum += 1;
                    newPath = $"{path}({suffixNum})";
                }
            }
            System.IO.Directory.Move(Path, newPath);
            return new LocalWorld(newPath, Version, Type, Property, Datapacks);
        }

        /// <summary>
        /// ワールドの設定をディレクトリから取得する
        /// </summary>
        public LocalWorld(string path)
        {
            Path = path;
            Name = System.IO.Path.GetDirectoryName(path);
            // フォルダ存在しない場合は新規作成
            if (!System.IO.Directory.Exists(Path))
                CreateWorldData();

            Property = LoadProperties();
            Type = GetServerType();
            Datapacks = LoadDatapacks();
        }

        /// <summary>
        /// 指定パスにワールドデータを新規作成する
        /// </summary>
        private void CreateWorldData()
        {
            System.IO.Directory.CreateDirectory(Path);
        }

        /// <summary>
        /// ワールドデータを削除<br/>
        /// /world /world_nether /world_end を削除
        /// </summary>
        private void DeleteWorldData()
        {
            var worldPath = System.IO.Path.Combine(Path, "world");
            if (System.IO.Directory.Exists(worldPath)) System.IO.Directory.Delete(worldPath);
            var netherPath = System.IO.Path.Combine(Path, "world_nether");
            if (System.IO.Directory.Exists(netherPath)) System.IO.Directory.Delete(netherPath);
            var endPath = System.IO.Path.Combine(Path, "world_end");
            if (System.IO.Directory.Exists(endPath)) System.IO.Directory.Delete(endPath);
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public void WrapRun(Action<ServerProperty> runFunc)
        {
            // 起動
            runFunc(Property);
        }

        /// <summary>
        // ServerTypeを判定する
        /// </summary>
        private ServerType? GetServerType()
        {
            var worldPath = System.IO.Path.Combine(Path,"world");
            if (System.IO.Directory.Exists(worldPath))
            {
                var netherPath = System.IO.Path.Combine(Path, "world_nether");
                if (System.IO.Directory.Exists(netherPath))
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
            var propertyPath = System.IO.Path.Combine(Path, "server.properties");
            if (File.Exists(propertyPath))
                return new ServerProperty( File.ReadAllText(propertyPath) );
            else
                return new ServerProperty();
        }

        /// <summary>
        // server.propertiesを保存する
        /// </summary>
        private void SaveProperties()
        {
            var propertyPath = System.IO.Path.Combine(Path, "server.properties");
            File.WriteAllText(propertyPath,Property.ExportProperty());
        }

        private DatapackCollection LoadDatapacks()
        {
            var datapackPath = System.IO.Path.Combine(Path, "datapack");
            if (System.IO.Directory.Exists(datapackPath))
            {
                var collection = new List<string>();
                var datapacks = System.IO.Directory.GetDirectories(datapackPath);
                foreach (var datapack in datapacks)
                {
                    var hasPackMcmeta = File.Exists(System.IO.Path.Combine(datapack, "pack.mcmeta"));
                    var hasData = System.IO.Directory.Exists(System.IO.Path.Combine(datapack, "Data"));
                    if (hasPackMcmeta && hasData)
                    {
                        var name = System.IO.Path.GetFileName(datapack);
                        collection.Add(name);
                    }
                }
                return new DatapackCollection(collection);
            }
            else
                return new DatapackCollection(new List<string>());
        }

        //TODO: VtoS
        private void VtoS()
        {

        }
        //TODO: StoV
        private void StoV()
        {

        }
    }

    /// <summary>
    /// リモートにあるワールドの情報
    /// </summary>
    public abstract class RemoteWorld: World
    {
        public RemoteWorld(
            string name,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
            Version = version;
            Name = name;
            Type = type;
            Property = property;
            Datapacks = datapacks;
        }

        /// <summary>
        /// ワールドデータを指定パスにPullする
        /// </summary>
        public abstract LocalWorld ToLocal(string Path);

        /// <summary>
        /// ローカルワールドデータをPushする
        /// </summary>
        public abstract void FromLocal(LocalWorld local,bool firstPush);
    }

    public class GitRemoteWorld : RemoteWorld
    {
        public GitRemoteWorld(
            string name,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            ): base(name, version, type, property, datapacks)
        {}

        /// <summary>
        /// TODO: ワールドデータを指定パスにPull/Cloneする
        /// </summary>
        public override LocalWorld ToLocal(string Path)
        {
            return new LocalWorld(Path);
        }

        /// <summary>
        /// TODO: ワールドデータを指定パスにPushする
        /// </summary>
        public override void FromLocal(LocalWorld local, bool firstPush)
        {
        }
    }
}
