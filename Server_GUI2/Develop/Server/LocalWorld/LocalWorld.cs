using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server;

namespace Server_GUI2.Develop.Server.LocalWorld
{
    abstract class World
    {
        public DatapackCollection Datapacks { get; protected set; }
        // TODO: pluginの読み込み
        //public ObservableCollection<Datapack> Pligins { get; }
        public ServerProperty Property { get; protected set; }
        public ServerType? Type { get; protected set; }
    }

    /// <summary>
    /// ワールドデータをあらわすクラス
    /// </summary>
    class LocalWorld: World
    {
        public string Path { get; }

        /// <summary>
        /// ワールドの設定をディレクトリに反映させる
        /// </summary>
        public LocalWorld(
            string path,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
            Path = path;
            // フォルダ存在しない場合は新規作成
            if (!Directory.Exists(Path))
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
                while (Directory.Exists(newPath))
                {
                    suffixNum += 1;
                    newPath = $"{path}({suffixNum})";
                }
            }
            Directory.Move(Path, newPath);
            return new LocalWorld(newPath, Type,Property,Datapacks);
        }

        /// <summary>
        /// ワールドの設定をディレクトリから取得する
        /// </summary>
        public LocalWorld(string path)
        {
            Path = path;
            // フォルダ存在しない場合は新規作成
            if (!Directory.Exists(Path))
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
            Directory.CreateDirectory(Path);
        }

        /// <summary>
        /// ワールドデータを削除<br/>
        /// /world /world_nether /world_end を削除
        /// </summary>
        private void DeleteWorldData()
        {
            var worldPath = System.IO.Path.Combine(Path, "world");
            if (Directory.Exists(worldPath)) Directory.Delete(worldPath);
            var netherPath = System.IO.Path.Combine(Path, "world_nether");
            if (Directory.Exists(netherPath)) Directory.Delete(netherPath);
            var endPath = System.IO.Path.Combine(Path, "world_end");
            if (Directory.Exists(endPath)) Directory.Delete(endPath);
        }

        /// <summary>
        // ServerTypeを判定する
        /// </summary>
        private ServerType? GetServerType()
        {
            var worldPath = System.IO.Path.Combine(Path,"world");
            if ( Directory.Exists(worldPath))
            {
                var netherPath = System.IO.Path.Combine(Path,"world_nether");
                if (Directory.Exists(netherPath))
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
            if (Directory.Exists(datapackPath))
            {
                var collection = new List<string>();
                var datapacks = Directory.GetDirectories(datapackPath);
                foreach (var datapack in datapacks)
                {
                    var hasPackMcmeta = File.Exists(System.IO.Path.Combine(datapack, "pack.mcmeta"));
                    var hasData = Directory.Exists(System.IO.Path.Combine(datapack, "Data"));
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

        //TODO: Vtos
        private void VtoS()
        {

        }
        //TODO: Vtos
        private void StoV()
        {

        }
    }

    /// <summary>
    /// リモートにあるワールドの情報
    /// </summary>
    abstract class RemoteWorld: World
    {
        public RemoteWorld(
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
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
        public abstract void FromLocal(LocalWorld local);
    }

    class GitRemoteWorld: RemoteWorld
    {
        public GitRemoteWorld(
           ServerType? type,
           ServerProperty property,
           DatapackCollection datapacks
           ): base(type,property,datapacks)
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
        public override void FromLocal(LocalWorld local)
        {
        }
    }
}
