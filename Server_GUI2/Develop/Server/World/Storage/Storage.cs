using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Util;
using Newtonsoft.Json;
using log4net;
using System.Reflection;

namespace Server_GUI2.Develop.Server.World
{
    public class StorageCollection
    {
        //TODO: push pullのアカウント切り替え
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static StorageCollection Instance { get; } = new StorageCollection();

        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();

        private StorageCollection()
        {
            logger.Info($"<StorageCollection>");

            var storages = ServerGuiPath.Instance.StoragesJson.ReadJson().SuccessOrDefault(new StoragesJson());

            // gitのリポジトリを全取得
            GitStorage.GetStorages(storages.Git).ForEach(x => Add(x));

            logger.Info($"</StorageCollection>");
        }

        private void SaveJson()
        {
            var json = new StoragesJson();
            foreach ( var storage in Storages)
            {
                if ( storage is GitStorage s)
                {
                    json.Git.Add(s.ExportStorageJson());
                }
            }
            ServerGuiPath.Instance.StoragesJson.WriteJson(json);
        }

        /// <summary>
        /// 条件に合うリモートリポジトリを返す
        /// </summary>
        public Storage FindStorage(string storage)
        {
            var storageValue = Storages.Where(x => x.Id == storage).First();
            return storageValue;
        }

        public void Add(Storage storage)
        {
            Storages.Add(storage);
            // Storage削除時にリストから排除
            storage.DeleteEvent += new EventHandler((_,__) => Storages.Remove(storage));
            // Storageの状態を保存
            SaveJson();
        }
    }

    public class StoragesJson
    {
        [JsonProperty("git")]
        public List<GitStorageJson> Git = new List<GitStorageJson>();
    }

    /// <summary>
    /// ワールドの保存先(Gitリポジトリ,Gdrive等)
    /// </summary>
    public abstract class Storage
    {
        protected static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ObservableCollection<IRemoteWorld> RemoteWorlds = new ObservableCollection<IRemoteWorld>();
        protected Dictionary<string, WorldState> worldStates = new Dictionary<string, WorldState>();
        public event EventHandler DeleteEvent;

        protected HashSet<string> usedNames = new HashSet<string>();

        public abstract string Id { get; }

        public abstract string AccountName { get; }

        public abstract string RepositoryName { get; }

        // 今後消すかも
        public abstract string Email { get; }

        public bool Available { get; set; }

        public virtual void AddWorld(RemoteWorld world)
        {
            RemoteWorlds.Add(world);
            world.DeleteEvent += new EventHandler((_, __) => RemoteWorlds.Remove(world));
        }

        /// <summary>
        /// 与えられたidに該当するリモートワールドを返す
        /// </summary>
        public RemoteWorld FindRemoteWorld(string worldId)
        {
            logger.Info($"<FindRemoteWorld> {worldId}");
            if (Available)
            {
                logger.Info($"</FindRemoteWorld> available");
                return RemoteWorlds.OfType<RemoteWorld>().Where(x => x.Id == worldId).First();
            }
            else
            {
                logger.Info($"</FindRemoteWorld> inavailable");
                return CreateUnavailableRemoteWorld(worldId);
            }
        }
        
        protected abstract RemoteWorld CreateUnavailableRemoteWorld(string worldId);

        public virtual void Delete()
        {
            logger.Info($"<Delete>");
            DeleteEvent?.Invoke(this,null);
            logger.Info($"</Delete>");
        }
 
        /// <summary>
        ///worldstate.jsonを更新してpush
        /// </summary>
        public abstract void SaveWorldStates();

        /// <summary>
        /// リモートワールドを新規作成
        /// </summary>
        public abstract RemoteWorld CreateRemoteWorld(string worldName);

        public abstract bool IsUsableName(string name);

        public void AddUsedName(string name)
        {
            logger.Info($"<AddUsedName>");
            usedNames.Add(name);
            logger.Info($"</AddUsedName>");
        }
    }

    public class GitStorageJson
    {
        [JsonProperty("account")]
        public string Account;

        [JsonProperty("repository")]
        public string Repository;

        [JsonProperty("email")]
        public string Email;

        [JsonProperty("worldstates")]
        public Dictionary<string,WorldState> WorldStates;

        /// <summary>
        /// Newtonsoft.Json用コンストラクタ
        /// </summary>
        public GitStorageJson() { }
    }

    /// <summary>
    /// 各gitリモートリポジトリをあらわす
    /// </summary>
    public class GitStorage: Storage
    {
        public static readonly GitLocal Local = new GitLocal(ServerGuiPath.Instance.GitState.FullName);

        public override string Id => $"git/{Remote.Account}/{Remote.Repository}";

        public override string AccountName => Remote.Account;

        public override string RepositoryName => Remote.Repository;

        public override string Email { get; }

        /// <summary>
        /// 使用可能なブランチ名かどうかを返す
        /// </summary>
        public override bool IsUsableName(string name)
        {
            return Regex.IsMatch(name, "^[a-zA-Z0-9_-]+$") && !usedNames.Contains(name);
        }

        public static List<GitStorage> GetStorages(List<GitStorageJson> gitStorageJsons)
        {
            logger.Info($"<GetStorages>");
            var result = new List<GitStorage>();
            foreach (var json in gitStorageJsons)
            {
                var remote = new GitRemote(json.Account, json.Repository);
                var storage = GitStorageManager.Instance.ReadWorldState(remote, json.Email).SuccessFunc(
                    worldstate => new GitStorage(remote, worldstate, json.Email, true)
                ).SuccessOrDefault( new GitStorage(remote, json.WorldStates, json.Email, false));
                result.Add(storage);
            }
            logger.Info($"</GetStorages>");
            return result;
        }

        protected override RemoteWorld CreateUnavailableRemoteWorld(string worldId)
        {
            return new GitRemoteWorld(Remote, worldId, null, this, true);
        }

        /// <summary>
        /// 新しいリポジトリを生成、ストレージ一覧に追加する
        /// </summary>
        public static Either<GitStorage,Exception> AddStorage(string account, string repository, string email)
        {
            logger.Info($"<AddStorage>");
            var remote = new GitRemote(account,repository);
            return GitStorageManager.Instance.ReadWorldState(remote, email).SuccessFunc(
                state => {
                    var storage = new GitStorage(remote, state, email, true);
                    StorageCollection.Instance.Add(storage);
                    logger.Info($"</AddStorage>");
                    return storage;
                }
                );
        }

        /// <summary>
        /// 新規リモートワールドを生成
        /// </summary>
        public override RemoteWorld CreateRemoteWorld(string worldName)
        {
            logger.Info($"<CreateRemoteWorld>");

            // ストレージにアクセスできない場合はエラー
            if (!Available) throw new RemoteWorldException($"git storage \"{Remote.Expression}\" is not accessible.");

            var prop = new ServerSettings();

            var id = Guid.NewGuid();

            var remote = Remote;
            var datapacks = new DatapackCollection();
            var plugins = new PluginCollection();
            var result = new GitRemoteWorld( remote, this,id.ToString(),worldName,false, null, null, prop, datapacks, plugins, true );
            AddWorld(result);

            logger.Info($"</CreateRemoteWorld>");
            return result;
        }

        public GitRemote Remote;

        public GitStorage(GitRemote remote, Dictionary<string, WorldState> worldStates,string email,bool available)
        {
            logger.Info($"<GitStorage> {remote.Account}.{remote.Repository}");

            Email = email;
            Remote = remote;
            var named = GitStorageManager.Instance.NamedRemote(Remote);
            Available = available;

            var newUsedNames = new HashSet<string>();

            // リモートと通信できる場合は既に存在するブランチ名は使えないようにする
            if (Available)
            {
                foreach ( var kv in named.GetBranchs())
                {
                    newUsedNames.Add(kv.Key);
                }
            }

            // WorldStatesに登録されたリモートワールド一覧を取得
            foreach ( var worldState in worldStates)
            {
                if (newUsedNames.Contains(worldState.Key))
                {
                    var remoteWorld = new GitRemoteWorld(Remote, worldState.Key, worldState.Value, this, Available);
                    RemoteWorlds.Add(remoteWorld);
                    // 削除イベントを追加
                    remoteWorld.DeleteEvent += new EventHandler((_, __) => RemoteWorlds.Remove(remoteWorld));
                }
            }

            RemoteWorlds.Add(new NewRemoteWorld(this));

            usedNames.UnionWith(newUsedNames);
            logger.Info($"</GitStorage>");
        }

        /// <summary>
        /// Worldstateを#state/worldstate.jsonに保存
        /// </summary>
        public override void SaveWorldStates()
        {
            logger.Info($"<SaveWorldStates>");
            // 通信可能な場合のみ発動
            if (Available)
            {
                // 存在しているリモートだけをフィルタして保存
                GitStorageManager.Instance.WriteWorldState
                    (
                        Remote,
                        Email,
                        ExportWorldStates()
                        ).SuccessAction
                    (
                        x => logger.Info($"</SaveWorldStates> success")
                    ).FailureAction
                    (
                        x => logger.Info($"</SaveWorldStates> failure")
                    );
            }
        }

        private Dictionary<string,WorldState> ExportWorldStates()
        {
            return RemoteWorlds
                    .OfType<RemoteWorld>()
                    .Where(x => x.Exist == true)
                    .ToDictionary(x => x.Id, x => x.ExportWorldState());
        }

        /// <summary>
        /// ストレージ登録情報を削除
        /// </summary>
        public override void Delete()
        {
            // リポジトリの登録を削除
            GitStorageManager.Instance.RemoveRemote(Remote).SuccessAction( x => {

                // 紐づいたリモートワールドを削除
                foreach (var world in RemoteWorlds.OfType<RemoteWorld>())
                {
                    world.Delete();
                }
                base.Delete();
            });
        }

        public GitStorageJson ExportStorageJson()
        {
            var worldstate = ExportWorldStates();
            var json = new GitStorageJson();
            json.Account = Remote.Account;
            json.Repository = Remote.Repository;
            json.Email = Email;
            json.WorldStates = worldstate;
            return json;
        }
    }
}