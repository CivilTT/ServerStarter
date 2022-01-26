using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
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
}
