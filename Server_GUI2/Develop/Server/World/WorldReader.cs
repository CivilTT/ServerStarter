using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{

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

    class NewWorldReader : WorldReader
    {
        public override void ReadTo(string path)
        {
            // TODO: 指定パスに新規ワールドを生成する
            // world,world-nether,world-endフォルダがあったら削除
        }
    }

    class LocalWorldReader : WorldReader
    {
        private string path;

        public LocalWorldReader(string path)
        {
            this.path = path;
        }

        public override void ReadTo(string path)
        {
            // TODO: 指定パスに既存ワールドを移動する
            // 同じパスだった場合何もしない
        }
    }

    class GitWorldReader : WorldReader
    {
        private string path;

        public GitWorldReader(string path)
        {
            this.path = path;
        }

        public override void ReadTo(string path)
        {
            // TODO: 指定パスにGitワールドをPullする
            // すでにローカルにそのGitワールドが存在した場合はそのフォルダを移動してPull
        }
    }

}
