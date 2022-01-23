//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Server_GUI2.Develop.Server.World
//{

//    class BeforeWorld
//    {
//        Version Version { get; }
//    }

//    class AfterWorld
//    {
//        /// <summary>
//        /// ワールド変換(v2v s2s etc...)をコンストラクタに組み込む。
//        /// </summary>
//        /// <param name="beforeWorld"></param>
//        protected AfterWorld(BeforeWorld beforeWorld)
//        {

//        }
//    }

//    class WorldReader
//    {
//        protected BeforeWorld BeforeWorld;

//        /// <summary>
//        /// git pullなりしてワールドデータを用意する
//        /// </summary>
//        public virtual BeforeWorld Read(){}
//    }


//    class WorldWriter
//    {
//        protected string Path { get; }
//        protected AfterWorld AfterWorld { get; }

//        protected WorldWriter(AfterWorld afterWorld)
//        {
//            AfterWorld = afterWorld;
//        }

//        /// <summary>
//        /// git pushなりしてワールドデータを書き出す
//        /// </summary>
//        public virtual void Write() { }

//    }


//    class Use
//    {
//        public static void Test()
//        {
//            var reader = new WorldReader();
//            new LocalWorldWriter( new VanillaAfterWorld(reader.Read()));
//        }
//    }


//    class NewBeforeWorld : BeforeWorld
//    { }

//    class VanillaBeforeWorld: BeforeWorld
//    { }

//    class SpigotBeforeWorld : BeforeWorld
//    { }



//    class VanillaAfterWorld : AfterWorld
//    { }

//    class SpigotAfterWorld : AfterWorld
//    { }




//    class NewWorldReader : WorldReader {
//        public NewWorldReader()
//        {
//            BeforeWorld = new NewBeforeWorld();
//        }
//    }

//    class LocalWorldReader : WorldReader
//    {
//        public LocalWorldReader(BeforeWorld beforeWorld)
//        {
//            BeforeWorld = beforeWorld;
//        }
//    }
//    class GitWorldReader : WorldReader {
//        public GitWorldReader(BeforeWorld beforeWorld)
//        {
//            BeforeWorld = beforeWorld;
//        }
//    }




//    class LocalWorldWriter : WorldWriter{ }



//    class GitWorldWriter : WorldWriter
//    {
        
//    }


//}
