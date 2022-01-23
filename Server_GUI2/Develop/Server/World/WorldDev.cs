using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{

    class BeforeWorld
    {
        Version Version { get; }
        ServerProperty ServerProperty { get; }
    }

    class AfterWorld
    {
        WorldWriter Writer;
        ServerProperty ServerProperty { get; }
        /// <summary>
        /// ワールド変換(v2v s2s etc...)をコンストラクタに組み込む。
        /// </summary>
        protected AfterWorld(BeforeWorld beforeWorld, WorldWriter writer)
        {
            Writer = writer;
        }
    }

    class WorldReader
    {
        protected BeforeWorld BeforeWorld;

        /// <summary>
        /// git pullなりしてワールドデータを用意する
        /// </summary>
        public virtual BeforeWorld Read(){}
    }


    class WorldWriter
    {
        protected string Path { get; }
        protected Version Version;
        protected AfterWorld AfterWorld { get; }

        protected WorldWriter(Version version)
        {
            Version = version;
        }

        /// <summary>
        /// git pushなりしてワールドデータを書き出す
        /// </summary>
        public virtual void Write() { }

    }


    class Use
    {
        public static void Test()
        {
            var reader = new WorldReader();
            new VanillaAfterWorld(reader.Read(), new LocalWorldWriter());
        }
    }


    class NewBeforeWorld : BeforeWorld
    { }
    class CustomMapBeforeWorld : BeforeWorld
    { }

    class VanillaBeforeWorld : BeforeWorld
    { }

    class SpigotBeforeWorld : BeforeWorld
    { }



    class VanillaAfterWorld : AfterWorld
    {
        protected VanillaAfterWorld(BeforeWorld beforeWorld, WorldWriter writer): base(beforeWorld, writer)
        {
        }
    }

    class SpigotAfterWorld : AfterWorld
    {
        protected VanillaAfterWorld(BeforeWorld beforeWorld, WorldWriter writer) : base(beforeWorld, writer)
        {
        }
    }




    class NewWorldReader : WorldReader {
        public NewWorldReader()
        {
            BeforeWorld = new NewBeforeWorld();
        }
    }

    class LocalWorldReader : WorldReader
    {
        public LocalWorldReader(BeforeWorld beforeWorld)
        {
            BeforeWorld = beforeWorld;
        }
    }
    class GitWorldReader : WorldReader {
        public GitWorldReader(BeforeWorld beforeWorld)
        {
            BeforeWorld = beforeWorld;
        }
    }




    class LocalWorldWriter : WorldWriter{ }



    class GitWorldWriter : WorldWriter
    {
        
    }


}
