using System;
using System.IO;
using System.Collections.ObjectModel;

namespace Server_GUI2.Develop.Server.World
{
    public class World
    {
        public bool Recreate { get; set; }
        public CustomWorld CustomWorld { get; set; }
        public WorldReader WorldReader { get; }
        public ObservableCollection<Datapack> Datapacks = new ObservableCollection<Datapack>();

//        public String Path { get; }

        public void Run(string worldPath,Version version)
        {
            var preWorld = GetPreWorld(worldPath);
            var runWorld = preWorld.ConvertTo(version);
        }

        /// <summary>
        /// Run後に実行
        /// ディレクトリの内容に応じて(Spigot|Vanilla|New)PreWorldインスタンスを返す
        /// </summary>
        public PreWorld GetPreWorld(string worldPath)
        {
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
                if (CustomWorld != null)
                {
                    CustomWorld.Ready(worldPath);
                }
            }
            return GenPreWorld(worldPath);
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
        private static PreWorld GenPreWorld(string worldPath)
        {
            // world がない -> NewPreWorld
            if ( !Directory.Exists(Path.Combine(worldPath, "world")))
            {
                return new NewPreWorld();
            }
            // world-nether がない -> VanillaPreWorld
            if ( !Directory.Exists(Path.Combine(worldPath, "world-nether")))
            {
                return new VanillaPreWorld();
            }
            // その他 -> SpligotPreWorld
            return new SpigotPreWorld();
        }
    }

    public abstract class PreWorld
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
        public abstract RunWorld ConvertTo(Version version);

        protected PreWorld(string path)
        {
            Path = path;
        }
    }

    public class RunWorld
    {
        Version Version;
        protected RunWorld(Version version)
        {
            Version = version;
        }
    }

    public class WorldReader
    {
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

    class WorldWriter
    {
        public string Path { get; }
        protected RunWorld RunWorld { get; }

        protected WorldWriter(RunWorld runWorld)
        {
            RunWorld = runWorld;
        }

        public virtual void Write() { }
    }

    class NewPreWorld : PreWorld
    {
        public NewPreWorld(string path) : base(path) { }

        public override void ConvertTo(Version version)
        {
        }
    }

    class VanillaPreWorld : PreWorld
    {
        VanillaVersion Version;
        public VanillaPreWorld(VanillaVersion version, string path) : base(path)
        {
            Version = version;
        }

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

    class SpigotPreWorld : PreWorld
    {
        SpigotVersion Version;
        public SpigotPreWorld(SpigotVersion version,string path) : base(path)
        {
            Version = version;
        }

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


    class VanillaRunWorld : RunWorld
    {
        public VanillaRunWorld(PreWorld preWorld, WorldWriter worldWriter, Version version) : base(preWorld, worldWriter, version)
        {
        }
    }

    class SpigotRunWorld : RunWorld
    {
        public SpigotRunWorld(PreWorld preWorld, WorldWriter worldWriter, Version version) : base(preWorld, worldWriter, version)
        {
        }
    }




    class NewWorldReader : WorldReader
    {
        // 新規ワールドを返す
        public override PreWorld Read()
        {
            return new NewPreWorld();
        }
    }

    class LocalWorldReader : WorldReader
    {
        public override PreWorld Read()
        {
            return new VanillaPreWorld();
        }
    }

    class GitWorldReader : WorldReader
    {
        public GitWorldReader(PreWorld beforeWorld)
        {
            PreWorld = beforeWorld;
        }
    }




    class LocalWorldWriter : WorldWriter { }



    //    class GitWorldWriter : WorldWriter
    //    {

    //    class GitWorldWriter : WorldWriter { }


}
