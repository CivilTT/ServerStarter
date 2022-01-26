using System.IO;
using System.Collections.ObjectModel;

namespace Server_GUI2.Develop.Server.World
{
    public class World
    {
        public bool Recreate { get; set; }
        public CustomMap CustomMap { get; set; }
        public ServerProperty serverProperty { get; set; }
        public WorldReader WorldReader { get; }
        public ObservableCollection<Datapack> Datapacks = new ObservableCollection<Datapack>();
        public ObservableCollection<Datapack> Pligins = new ObservableCollection<Datapack>();

        public World(WorldReader worldReader)
        {
            WorldReader = worldReader;
        }

        /// <summary>
        /// ワールドデータを必要に応じてDL,移動し
        /// 与えられたバージョン用に変換する
        /// </summary>
        public WorldWriter Preprocess(Version version, WorldSaveLocation saveLocation)
        {
            // ワールド書き込み/アップロード用インスタンス
            var writer = saveLocation.GetWorldWriter(version);
            // ワールドデータを指定位置に展開
            ConverteWorld(writer.Path, version);
            // ワールド書き込みの前処理(Gitに使用中フラグを立てる等)
            writer.Preprocess();
            return writer;
        }

        /// <summary>
        /// Run後に実行
        /// ディレクトリの内容に応じて(Spigot|Vanilla|New)PreWorldインスタンスを返す
        /// </summary>
        private void ConverteWorld(string worldPath,Version version)
        {
            if (!Recreate && WorldReader.Version != null && WorldReader.Version > version)
            {
               // TODO: バージョンが下がる場合は確認画面を表示
            }
            // ワールドデータを指定パスに展開
            WorldReader.ReadTo(worldPath);
            // データパックの追加と削除
            for (var i = 0; i < Datapacks.Count; i++)
            {
                Datapacks[i].Ready(worldPath);
            }
            // 必要に応じてワールドを再生成
            if (Recreate)
            {
                RecreateWorld(worldPath);
                // 必要に応じてカスタムワールドを導入する
                if (CustomMap != null)
                {
                    CustomMap.Import(worldPath);
                }
            }
            GenWorldConverter(worldPath).ConvertTo(version);
        }

        /// <summary>
        /// TODO: ワールドデータをリセット(データパックはそのまま)
        /// </summary>
        private static void RecreateWorld(string worldPath)
        {
        }

        /// <summary>
        /// TOOD: ワールドデータの形式に応じてPreWorldインスタンスを返却
        /// </summary>
        private static WorldConverter GenWorldConverter(string worldPath)
        {
            // world がない -> NewPreWorld
            if ( !Directory.Exists(Path.Combine(worldPath, "world")))
            {
                return new NewWorldConverter(worldPath);
            }
            // world-nether がない -> VanillaPreWorld
            if ( !Directory.Exists(Path.Combine(worldPath, "world-nether")))
            {
                return new VanillaWorldConverter(worldPath);
            }
            // その他 -> SpligotPreWorld
            return new SpigotWorldConverter(worldPath);
        }
    }
}
