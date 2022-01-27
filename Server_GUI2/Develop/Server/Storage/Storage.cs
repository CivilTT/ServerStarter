using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;

namespace Server_GUI2.Develop.Server.Storage
{
    /// <summary>
    /// ワールドの保存先(ローカルフォルダ,Gitリポジトリ,Gdrive等)
    /// </summary>
    public abstract class Storage
    {
        public abstract WorldWriter GetWorldWriter(Version version);
    }

    class LocalWorldSaveLocation : Storage
    {
        public override WorldWriter GetWorldWriter(Version version)
        {
            return new LocalWorldWriter();
        }
    }
    class GitWorldSaveLocation : Storage
    {
        public override WorldWriter GetWorldWriter(Version version)
        {
            return new GitWorldWriter();
        }
    }
}
