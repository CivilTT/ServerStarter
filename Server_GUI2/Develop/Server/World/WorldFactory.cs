using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
    public class WorldFactory
    {
        public static WorldFactory Instance = new WorldFactory(Path.Combine(SetUp.CurrentDirectory, "World_Data"));
        public ObservableCollection<LocalWorld> Worlds { get; } = new ObservableCollection<LocalWorld>();

        private WorldFactory(string path)
        {
            var versions = new DirectoryInfo(path);
            foreach ( var verDir in versions.EnumerateDirectories())
            {
                try
                {
                    var version = VersionFactory.Instance.GetVersionFromName(verDir.Name);
                    foreach ( var worldDir in verDir.EnumerateDirectories())
                    {
                        //ログフォルダは無視
                        if (worldDir.Name == "logs")
                            continue;
                        Worlds.Add(new LocalWorld(worldDir,version));
                    }
                }
                catch (KeyNotFoundException)
                {
                    //version名でないディレクトリは無視
                    continue;
                }
            }
        }

        // shared world のリポジトリ名は User/repository/WorldName としてバージョン情報は記録しない
        // バージョン情報やブランチ一覧等は特別なブランチを作ってjson管理とかがいいか

        // リモートにあってローカルにない
        // User.repository.WorldNameにclone

        // リモートにあってローカルにある
        // User.repository.WorldNameにpull

        // リモートになくてローカルにある
        // 通常起動

        // リモートになくてローカルにない
        // WorldNameを新規作成

        // push先リポジトリを変えたい
        // CustomMapからどうぞ
    }
}
