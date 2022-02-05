using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Util;
namespace Server_GUI2.Develop.Server.World
{
    public class WorldFactory
    {
        public static WorldFactory Instance = new WorldFactory(Path.Combine(SetUp.CurrentDirectory, "World_Data"));

        /// <summary>
        /// データ整合性のためリスト変換はしない。
        /// インデックスアクセスはプロパティ化して常に更新が反映されるようにする。
        /// </summary>
        public ObservableCollection<LocalWorld> Worlds { get; } = new ObservableCollection<LocalWorld>();
        private WorldFactory(string path)
        {
            //　ディレクトリを走査し既存ワールド一覧を取得
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
                        Worlds.Add(new LocalWorld(worldDir.Name,version));
                    }
                }
                catch (KeyNotFoundException)
                {
                    //version名でないディレクトリは無視
                    continue;
                }
            }
            // [new world]を追加
            Worlds.Add(new LocalWorld("new world"));
        }

        /// <summary>
        /// 以下の挙動をするGetterを返す<br/>
        /// | バージョンとワールド名を文字列で受け取り該当するワールドを返す<br/>
        /// | 該当しなかった場合nullを返す
        /// </summary>
        public ReadOnlyProperty<LocalWorld> LocalWorldFromId(string id)
        {
            return new ReadOnlyProperty<LocalWorld>( () => Worlds.Where( x => x.Id == id ).FirstOrDefault());
        }
    }
}
