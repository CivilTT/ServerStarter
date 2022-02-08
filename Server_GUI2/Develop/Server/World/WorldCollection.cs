using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2.Develop.Server.World
{
    public class WorldCollection
    {
        public static WorldCollection Instance = new WorldCollection(ServerGuiPath.Instance.WorldData);

        private string linkJsonPath = Path.Combine(SetUp.CurrentDirectory, "remotes.json");

        /// <summary>
        /// データ整合性のためリスト変換はしない。
        /// インデックスアクセスはプロパティ化して常に更新が反映されるようにする。
        /// </summary>
        public ObservableCollection<WorldWrapper> WorldWrappers { get; } = new ObservableCollection<WorldWrapper>();
        private WorldCollection(WorldDataPath path)
        {
            // ローカルとリモートの接続情報
            var linkJson = LoadLinkJson();
            //　ディレクトリを走査し既存ワールド一覧を取得
            foreach (var verDir in path.GetVersionDirectories())
            {
                try
                {
                    var version = VersionFactory.Instance.GetVersionFromName(verDir.Name);
                    foreach (var worldDir in verDir.GetWorldDirectories())
                    {
                        // ログフォルダは無視
                        if (worldDir.Name == "logs")
                            continue;

                        var key = $"{verDir.Name}/{worldDir.Name}";
                        // 接続済みローカルはLinkedRemoteWorldWrapperとして扱う
                        // TODO: リモートとの通信ができなかった場合のフォールバック
                        // 通信できないワールドは一覧に追加しないorグレーアウトして選択できないように
                        if (linkJson.ContainsKey(key))
                        {
                            var linkData = linkJson[key];
                            var remote = StorageCollection.Instance.FindRemoteWorld(linkData.Storage,linkData.World);
                            WorldWrappers.Add(new LinkedRemoteWorldWrapper(remote, worldDir));
                        }
                        // 未接続ローカルはUnLinkedLocalWorldWrapperとして扱う
                        else
                        {
                            WorldWrappers.Add(new UnLinkedLocalWorldWrapper(new LocalWorld(worldDir, version)));
                        }
                    }
                }
                catch (KeyNotFoundException)
                {
                    //version名でないディレクトリは無視
                    continue;
                }
            }
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
        [JsonProperty("storage")]
        public string Storage;
        
        [JsonProperty("world")]
        public string World;

        [JsonProperty("using")]
        public bool Using;

        public RemoteLinkJson(string storage, string world, bool isUsing)
        {
            Using = isUsing;
            Storage = storage;
            World = world;
        }
    }
}
