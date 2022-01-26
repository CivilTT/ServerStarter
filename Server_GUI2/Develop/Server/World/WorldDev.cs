using System.IO;
using System.Collections.ObjectModel;

namespace Server_GUI2.Develop.Server.World
{
    public class World
    {
        public bool Recreate { get; set; }
        public CustomMap CustomMap{ get; set; }
        public WorldReader WorldReader { get; }
        public ObservableCollection<Datapack> Datapacks = new ObservableCollection<Datapack>();

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

    public abstract class WorldConverter
    {
        public string Path { get; }

        /// <summary>
        /// 
        /// 指定されたパスに指定されたバージョンでワールドデータを用意する
        /// 配布ワールド、データパックは導入済みの状態にする
        /// 
        /// WorldReader内部からのReadToを使う
        /// 
        /// </summary>
        public abstract void ConvertTo(Version version);

        protected WorldConverter(string path)
        {
            Path = path;
        }
    }

    public class WorldReader
    {
        public Version Version { get; }
        /// <summary>
        /// 
        /// 指定されたパスにワールドデータを用意するだけ
        /// バージョン変更やSpigot-Vanilla変換はしない
        /// 
        /// 
        /// </summary>
        public virtual void ReadTo(string path)
        {

        }
    }

    //    class Use
    //    {
    //        public static void Test()
    //        {
    //            var ver = new Version();
    //            new VanillaRunWorld(new VanillaPreWorld(new GitWorldReader(git)),new GitWorldWriter(git,ver));

    public class WorldWriter
    {
        public string Path { get; }
        protected WorldWriter() { }
        /// <summary>
        /// gitの使用中フラグ
        /// </summary>
        public virtual void Preprocess() { }
        public virtual void Postprocess() { }
    }

    class NewWorldConverter : WorldConverter
    {
        public NewWorldConverter(string path) : base(path) { }

        public override void ConvertTo(Version version)
        {
        }
    }

    class VanillaWorldConverter : WorldConverter
    {
        public VanillaWorldConverter(string path) : base(path) { }

        public override void ConvertTo(Version version)
        {
            // check Version
            switch (version)
            {
                case VanillaVersion v:
                    // v2v
                    // move Vanilla data
                    break;
                case SpigotVersion v:
                    // v2s
                    // convert Spigot to Vanilla
                    break;
                default:
                    throw new ArgumentException("未知のVersionであるため変換できません");
            }
        }
    }

//            switch (version)
//            {
//                case VanillaVersion v:
//                    // v2v
//                    // move Vanilla data
//                    break;
//                case SpigotVersion v:
//                    // v2s
//                    // convert Spigot to Vanilla
//                    break;
//                default:
//                    throw new ArgumentException("未知のVersionであるため変換できません");
//            }
//        }
//    }

    class SpigotWorldConverter : WorldConverter
    {
        public SpigotWorldConverter(string path) : base(path) { }

        public override void ConvertTo(Version version)
        {
            // check Version
            switch (version)
            {
                case VanillaVersion v:
                    // s2v
                    // move Vanilla data
                    break;
                case SpigotVersion v:
                    // s2s
                    // convert Spigot to Vanilla
                    break;
                default:
                    throw new ArgumentException("未知のVersionであるため変換できません");
            }
        }
    }

    class NewWorldReader : WorldReader
    {
        // 新規ワールドを返す
        public override WorldConverter Read()
        {
            return new NewPreWorld();
        }
    }

    class LocalWorldReader : WorldReader
    {
        public override WorldConverter Read()
        {
            return new VanillaPreWorld();
        }
    }

    class GitWorldReader : WorldReader
    {
        public GitWorldReader(WorldConverter beforeWorld)
        {
            PreWorld = beforeWorld;
        }
    }




    class LocalWorldWriter : WorldWriter { }

    class GitWorldWriter : WorldWriter { }
}
