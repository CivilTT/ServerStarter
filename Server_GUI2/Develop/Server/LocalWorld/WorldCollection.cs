using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2.Develop.Server.LocalWorld
{
    class WorldCollection
    {
        public static WorldCollection Instance = new WorldCollection(Path.Combine(SetUp.CurrentDirectory, "World_Data"));

        private string linkJsonPath = Path.Combine(SetUp.CurrentDirectory, "remotes.json");

        /// <summary>
        /// データ整合性のためリスト変換はしない。
        /// インデックスアクセスはプロパティ化して常に更新が反映されるようにする。
        /// </summary>
        public ObservableCollection<LocalWorld> Worlds { get; } = new ObservableCollection<LocalWorld>();
        private WorldCollection(string path)
        {
            // ローカルとリモートの接続情報
            var linkJson = LoadLinkJson();
            //　ディレクトリを走査し既存ワールド一覧を取得
            var versions = new DirectoryInfo(path);
            foreach (var verDir in versions.EnumerateDirectories())
            {
                try
                {
                    var version = VersionFactory.Instance.GetVersionFromName(verDir.Name);
                    foreach (var worldDir in verDir.EnumerateDirectories())
                    {
                        //ログフォルダは無視
                        if (worldDir.Name == "logs")
                            continue;
                        // 接続済みローカルはRemoteWorld化して扱う
                        Worlds.Add(new LocalWorld(worldDir.Name, version));
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
        /// ローカルとリモートの接続情報を取得
        /// </summary>
        private Dictionary<string, RemoteLinkJson> LoadLinkJson()
        {
            var json = File.ReadAllText(linkJsonPath);
            var result = JsonConvert.DeserializeObject<Dictionary<string, RemoteLinkJson>>(json);
            return result;
        }
    }

    public class RemoteLinkJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("using")]
        public bool Using;

        public RemoteLinkJson(string id, bool isUsing)
        {
            Using = isUsing;
            Id = id;
        }
    }
}
