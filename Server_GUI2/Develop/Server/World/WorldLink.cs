using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Server_GUI2.Develop.Util;
using Server_GUI2.Develop.Server.Storage;

namespace Server_GUI2.Develop.Server.World
{
    /// <summary>
    /// リモートとローカルワールドのリンクを受け持つシングルトン
    /// </summary>
    public class WorldLink
    {
        public static WorldLink Instance = new WorldLink();
        private Dictionary<string, string> local2remote;
        private Dictionary<string, string> remote2local;
        private string jsonPath = Path.Combine(SetUp.CurrentDirectory, "remotes.json");
        private WorldLink()
        {
            local2remote = GetRemoteLinkJson().ToDictionary(x => x.Key,x => x.Value.Id);
            remote2local = local2remote.ToDictionary(x => x.Value, x => x.Key);
        }

        public ReadOnlyProperty<RemoteWorld> GetLinkedRemote(LocalWorld local)
        {
            RemoteWorld target()
            {
                string remote;
                if (local2remote.TryGetValue(local.Id,out remote))
                {
                    var result = StorageFactory.Instance.RemoteWorldFromId(remote).Value;
                    // リンクされていたワールドが存在しない場合リンクを解除
                    if (result == null)
                        UnLink(local);
                    return result;
                }
                return null;
            }
            return new ReadOnlyProperty<RemoteWorld>(target);
        }

        /// <summary>
        /// リンクされているワールドを取得するPropertyを提供する<br/>
        /// ワールドのリンク情報に変化があると自動更新される
        /// </summary>
        public ReadOnlyProperty<LocalWorld> GetLinkedLocal(RemoteWorld remote)
        {
            LocalWorld target()
            {
                string local;
                if (remote2local.TryGetValue(remote.Id,out local))
                {
                    var result = WorldFactory.Instance.LocalWorldFromId(local).Value;
                    // リンクされていたワールドが存在しない場合リンクを解除
                    if (result == null)
                        UnLink(remote);
                    return result;
                }
                else
                    return null;
            }
            return new ReadOnlyProperty<LocalWorld>(target);
        }

        /// <summary>
        /// ローカルとリモートをリンクする
        /// </summary>
        public void Link(LocalWorld local, RemoteWorld remote)
        {
            var localId = local.Id;
            var remoteId = remote.Id;
            if (local2remote.ContainsKey(localId))
                throw new ArgumentException($"local world {localId} is already connected");
            if (remote2local.ContainsKey(remoteId))
                throw new ArgumentException($"remote world {remoteId} is already connected");
            remote2local[remoteId] = localId;
            local2remote[localId] = remoteId;
        }

        /// <summary>
        /// ローカルとリモートのリンクを解除する
        /// </summary>
        public void UnLink(LocalWorld local,bool ignoreWhenNotLinked = false)
        {
            var localId = local.Id;
            if ( ! local2remote.ContainsKey(localId))
            {
                if (ignoreWhenNotLinked)
                    throw new ArgumentException($"local world {localId} is not linked");
                else
                    return;
            }
            remote2local.Remove(local2remote[localId]);
            local2remote.Remove(localId);
        }

        /// <summary>
        /// ローカルワールドのID("version/name")を変更する
        /// </summary>
        public void ChangeLocalId(string oldId, string newId)
        {
            var remote = local2remote[oldId];
            local2remote.Remove(oldId);
            local2remote[newId] = remote;
            remote2local[remote] = newId;
        }

        /// <summary>
        /// ローカルとリモートのリンクを解除する
        /// </summary>
        public void UnLink(RemoteWorld remote, bool ignoreWhenNotLinked = false)
        {
            var remoteId = remote.Id;
            if (!remote2local.ContainsKey(remoteId))
            {
                if (ignoreWhenNotLinked)
                    throw new ArgumentException($"remote world {remoteId} is not linked");
                else
                    return;
            }
            local2remote.Remove(remote2local[remoteId]);
            remote2local.Remove(remoteId);
        }

        private Dictionary<string,RemoteLinkJson> GetRemoteLinkJson()
        {
            var json = File.ReadAllText(jsonPath);
            var result = JsonConvert.DeserializeObject<Dictionary<string, RemoteLinkJson>>(json);
            return result;
        }

        /// <summary>
        /// ローカルとリモートのリンク情報を保存(サーバー終了後に使う)
        /// </summary>
        public void SaveJson()
        {
            var json = local2remote.ToDictionary(x => x.Key, x => new RemoteLinkJson(x.Value, false));
            var content = JsonConvert.SerializeObject(json);
            File.WriteAllText(jsonPath, content);
        }
        /// <summary>
        /// ローカルとリモートのリンク情報を保存(引数に今から起動するワールドを渡す)
        /// </summary>
        public void SaveJson(LocalWorld runningWorld)
        {
            var id = runningWorld.Id;
            var json = local2remote.ToDictionary(x => x.Key, x => new RemoteLinkJson(x.Value, x.Value == id));
            var content = JsonConvert.SerializeObject(json);
            File.WriteAllText(jsonPath,content);
        }
    }

    public class RemoteLinkJson
    {
        [JsonProperty("id")]
        public string Id;
        
        [JsonProperty("using")]
        public bool Using;

        public RemoteLinkJson(string id,bool isUsing)
        {
            Using = isUsing;
            Id = id;
        }
    }
}

