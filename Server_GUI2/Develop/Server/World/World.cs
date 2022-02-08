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
        public WorldPath Path { get; }

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
        /// ワールドデータを指定パスに移動
        /// </summary>
        public LocalWorld Move(WorldPath path,bool addSuffixWhenNameCollided = false)
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
            return new LocalWorld(newPath, Version, Type, Property, Datapacks);
        }

        /// <summary>
        /// ワールドの設定をディレクトリから取得する
        /// </summary>
        public LocalWorld(WorldPath path,Version version)
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
            // 起動
            runFunc(Property);
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
                var collection = new List<string>();
                foreach (var datapack in Path.World.Datapccks.GetDatapackDirectories())
                {
                    if (datapack.Mcmeta.Exists && datapack.Data.Exists)
                        collection.Add(datapack.Name);
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
        public abstract LocalWorld ToLocal(WorldPath Path);

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
        public override LocalWorld ToLocal(WorldPath Path)
        {
            return new LocalWorld(Path,Version);
        }

        /// <summary>
        /// TODO: ワールドデータを指定パスにPushする
        /// </summary>
        public override void FromLocal(LocalWorld local, bool firstPush)
        {
        }
    }
}
