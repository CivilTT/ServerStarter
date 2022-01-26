using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
    /// <summary>
    /// ワールドの保存先(ローカルフォルダ,Gitリポジトリ,Gdrive等)
    /// </summary>
    public abstract class WorldSaveLocation
    {
        public abstract WorldWriter GetWorldWriter(Version version);
    }

    class LocalWorldSaveLocation : WorldSaveLocation
    {
        public override WorldWriter GetWorldWriter(Version version)
        {
            return new LocalWorldWriter();
        }
    }
    class GitWorldSaveLocation : WorldSaveLocation
    {
        public override WorldWriter GetWorldWriter(Version version)
        {
            return new GitWorldWriter();
        }
    }
}
