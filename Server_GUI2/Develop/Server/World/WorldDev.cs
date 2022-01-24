//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Server_GUI2.Develop.Server.World
//{
//    abstract class PreWorld{
//        public WorldReader WorldReader { get; private set; }

//        public String Path { get; }

//        /// <summary>
//        /// 
//        /// 指定されたパスに指定されたバージョンでワールドデータを用意する
//        /// 配布ワールド、データパックは導入済みの状態にする
//        /// 
//        /// WorldReader内部からのReadToを使う
//        /// 
//        /// </summary>
//        public abstract void ConvertTo(Version version, string name);

//        protected PreWorld(WorldReader worldReader)
//        {
//            WorldReader = worldReader;
//        }
//    }

//    class RunWorld
//    {
//        protected RunWorld(PreWorld preWorld, WorldWriter worldWriter, Version version)
//        {
//            preWorld.ConvertTo(version, worldWriter.Path);
//        }
//    }

//    class WorldReader
//    {
//        public PreWorld PreWorld { get; set; }

//        /// <summary>
//        /// 
//        /// 指定されたパスにワールドデータを用意するだけ
//        /// バージョン変更やSpigot-Vanilla変換はしない
//        /// 
//        /// </summary>
//        public virtual void ReadTo(string path) { }
//    }


//    class WorldWriter
//    {
//        public string Path { get; }
//        protected RunWorld RunWorld { get; }

//        protected WorldWriter(RunWorld runWorld)
//        {
//            RunWorld = runWorld;
//        }

//        public virtual void Write() { }
//    }


//    class Use
//    {
//        public static void Test()
//        {
//            var ver = new Version();
//            new VanillaRunWorld(new VanillaPreWorld(new GitWorldReader(git)),new GitWorldWriter(git,ver));

//            new GitWorldReader(git);
//            new LocalWorldReader(path);
//            new NewWorldReader();
//        }
//    }


//    class NewPreWorld : PreWorld
//    {
//        public NewPreWorld(WorldReader worldReader) : base(worldReader) { }

//        public override void ConvertTo(Version version, string path)
//        {
//            WorldReader.ReadTo(path);
//        }
//    }


//    class CustomPreWorld : PreWorld
//    { }


//    class VanillaPreWorld : PreWorld
//    {
//        public VanillaPreWorld(WorldReader worldReader):base(worldReader){ }

//        public override void ConvertTo(Version version, string path)
//        {
//            // check Version

//            WorldReader.ReadTo(path);

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


//    class SpigotPreWorld : PreWorld
//    {}


//    class VanillaRunWorld : RunWorld
//    {
//        public VanillaRunWorld(PreWorld preWorld, WorldWriter worldWriter, Version version):base(preWorld, worldWriter, version)
//        {
//        }
//    }

//    class SpigotRunWorld : RunWorld
//    {
//        public SpigotRunWorld(PreWorld preWorld, WorldWriter worldWriter, Version version) : base(preWorld, worldWriter, version)
//        {
//        }
//    }




//    class NewWorldReader : WorldReader
//    {
//        // 新規ワールドを返す
//        public override PreWorld Read()
//        {
//            return new NewPreWorld();
//        }
//    }

//    class LocalWorldReader : WorldReader
//    {
//        public override PreWorld Read()
//        {
//            return new VanillaPreWorld();
//        }
//    }

//    class GitWorldReader : WorldReader
//    {
//        public GitWorldReader(PreWorld beforeWorld)
//        {
//            PreWorld = beforeWorld;
//        }
//    }




//    class LocalWorldWriter : WorldWriter { }



//    class GitWorldWriter : WorldWriter
//    {

//    class GitWorldWriter : WorldWriter { }


//}
