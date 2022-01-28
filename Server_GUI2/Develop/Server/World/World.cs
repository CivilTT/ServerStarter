using System;
using System.IO;
using System.Collections.ObjectModel;
using Server_GUI2.Develop.Server.Storage;

namespace Server_GUI2.Develop.Server.World
{
    public class World
    {
        public bool Recreate { get; set; }
        public CustomMap CustomMap { get; set; }
        public ServerProperty serverProperty { get; }
        public Version Version { get; }

        public WorldReader WorldReader { get; }
        public ObservableCollection<Datapack> Datapacks = new ObservableCollection<Datapack>();
        public ObservableCollection<Datapack> Pligins = new ObservableCollection<Datapack>();

        public World(WorldReader worldReader, Version version)
        {
            Version = version;
            WorldReader = worldReader;
        }

        /// <summary>
        /// ワールドデータを読み込むー＞与えられた関数を実行ー＞ワールドデータを書き出す
        /// RUNするときはサーバー起動関数を引数に与えること
        /// </summary>
        public void WrapRunAction( Action func, Version version, Storage.Storage stoarge)
        {
            var writer = Preprocess(version,stoarge);
            func();
            writer.Postprocess();
        }

        private WorldWriter Preprocess(Version version, Storage.Storage stoarge)
        {
            // ワールド書き込み/アップロード用インスタンス
            var writer = stoarge.GetWorldWriter(version);
            // ワールドデータを指定位置に展開
            ConvertWorld(writer.Path, version);
            // ワールド書き込みの前処理(Gitに使用中フラグを立てる等)
            writer.Preprocess();
            return writer;
        }

        /// <summary>
        /// Run後に実行
        /// ディレクトリの内容に応じて(Spigot|Vanilla|New)PreWorldインスタンスを返す
        /// </summary>
        private void ConvertWorld(string worldPath,Version version)
        {
            if (!Recreate && Version != null && Version > version)
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
