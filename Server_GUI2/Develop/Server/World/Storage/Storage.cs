﻿using System;
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

            var storages = ServerGuiPath.Instance.StoragesJson.ReadJson().FailureFunc(x => { x.WriteLine(); return x; }).SuccessOrDefault(new StoragesJson());

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
        public Storage FindStorage(string id)
        {
            var storageValue = Storages.Where(x => x.Id == id).First();
            return storageValue;
        }

        public void Add(Storage storage)
        {
            Storages.Add(storage);
            // Storage削除時にリストから排除
            storage.DeleteEvent += new EventHandler((_,__) => {
                storage.UpdateWorldStates();
                Storages.Remove(storage);
                SaveJson();
            });
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

        /// <summary>
        /// ストレージの持つワールド一覧にワールドを追加
        /// </summary>
        protected virtual void AddWorld(RemoteWorld world)
        {
            RemoteWorlds.Insert(RemoteWorlds.Count < 2 ? 0 : RemoteWorlds.Count-2, world);
            world.DeleteEvent += new EventHandler((_, __) => RemoteWorlds.Remove(world));
        }

        /// <summary>
        /// 与えられたidに該当するリモートワールドがあれば返す
        /// </summary>
        public Either<RemoteWorld,string> FindRemoteWorld(string worldId)
        {
            logger.Info($"<FindRemoteWorld> {AccountName}/{RepositoryName}/{worldId}");
            if (Available)
            {
                logger.Info($"</FindRemoteWorld> available");
                var world = RemoteWorlds.OfType<RemoteWorld>().Where(x => x.Id == worldId).FirstOrDefault();
                if (world != null)
                {
                    logger.Info($"world found");
                    return new Success<RemoteWorld, string>(world);
                }
                else
                {
                    logger.Info($"world not found");
                    return new Failure<RemoteWorld, string>($"remoteworld {AccountName}/{RepositoryName}/{worldId} is missing");
                }
            }
            else
            {
                logger.Info($"</FindRemoteWorld> inavailable");
                return new Failure<RemoteWorld, string>($"network is not available");
            }
        }


        public virtual void Delete()
        {
            logger.Info($"<Delete>");
            DeleteEvent?.Invoke(this,null);
            logger.Info($"</Delete>");
        }
 
        /// <summary>
        ///worldstate.jsonを更新してpush
        /// </summary>
        public abstract void UpdateWorldStates(string name=null,WorldState state = null);

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

        private static bool? GitInstalled_ = null;

        public static bool GitInstalled
        {
            get
            {
                if (GitInstalled_.HasValue)
                {
                    return GitInstalled_.Value;
                }
                else
                {
                    // git help が実行可能かどうかをチェックしてgitがインストールされているかを確認
                    var flag = GitCommand.CheckInstalled();
                    GitInstalled_ = flag;
                    return flag;
                }
            }
        }

        /// <summary>
        /// 使用可能なブランチ名かどうかを返す
        /// </summary>
        public override bool IsUsableName(string name)
        {
            return Regex.IsMatch(name, "^[a-zA-Z0-9_-]+$") && !usedNames.Contains(name);
        }

        /// <summary>
        /// Gitのストレージを全取得
        /// </summary>
        public static List<GitStorage> GetStorages(List<GitStorageJson> gitStorageJsons)
        {
            logger.Info($"<GetStorages>");
            var result = new List<GitStorage>();
            foreach (var json in gitStorageJsons)
            {
                json.Account.WriteLine();
                json.Repository.WriteLine();
                var remote = new GitRemote(json.Account, json.Repository, json.Email);
                GitStorageManager.Instance.ReadWorldState(remote).SuccessAction(
                    worldstate => {
                        var storage = new GitStorage(remote, worldstate, json.Email, true);
                        result.Add(storage);
                   }
                );
            }
            logger.Info($"</GetStorages>");
            return result;
        }

        /// <summary>
        /// 新しいリポジトリを生成、ストレージ一覧に追加する
        /// </summary>
        public static Either<GitStorage,Exception> AddStorage(string account, string repository, string email)
        {
            logger.Info($"<AddStorage>");
            var remote = new GitRemote(account,repository, email);

            return GitStorageManager.Instance.ReadWorldState(remote).SuccessFunc<GitStorage>(
                state => {
                    // 登録済みの場合
                    var exists = StorageCollection.Instance.Storages.Any(s => s.RepositoryName == repository && s.AccountName == account);
                    if (exists)
                    {
                        return new Failure<GitStorage, Exception>(new ServerException("this remote is arleady added to remote list."));
                    }
                    // リポジトリが存在しない場合
                    if (! remote.Exists())
                    {
                        return new Failure<GitStorage, Exception>(new ServerException($"git remote {account}/{repository} is not exists."));
                    }

                    var storage = new GitStorage(remote, state, email, true);
                    StorageCollection.Instance.Add(storage);
                    logger.Info($"</AddStorage>");
                    return new Success<GitStorage, Exception>(storage);
               });


        }

        /// <summary>
        /// 新規リモートワールドを生成
        /// </summary>
        public override RemoteWorld CreateRemoteWorld(string worldName)
        {

            logger.Info($"<CreateRemoteWorld>");

            // ストレージにアクセスできない場合はエラー
            if (!Available) throw new RemoteWorldException($"git storage \"{Remote.Expression}\" is not accessible.");

            // ワールド設定データ
            var prop = new ServerSettings();

            // ブランチ名をuuidに
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
                    AddWorld(remoteWorld);
                }
            }

            RemoteWorlds.Add(new NewRemoteWorld(this));

            // 使用済みの名前一覧を保存
            usedNames.UnionWith(newUsedNames);
            logger.Info($"</GitStorage>");
        }

        /// <summary>
        /// 特定のワールドのWorldstateを#state/worldstate.jsonとマージ
        /// </summary>
        public override void UpdateWorldStates(string name = null,WorldState state = null)
        {
            logger.Info($"<SaveWorldStates>");

            // 通信可能な場合のみ実行
            if (!Available)
            {
                logger.Info($"</SaveWorldStates> network inavailable");
                return;
            }

            GitStorageManager.Instance.ReadWorldState(Remote).SuccessAction(
                new_state =>
                {
                    if (name != null && state != null)
                    {
                        new_state[name] = state;
                    }
                    new_state.WriteLine();
                    // 存在しているリモートだけをフィルタして保存
                    GitStorageManager.Instance.WriteWorldState
                        (
                            Remote,
                            Email,
                            new_state
                            ).SuccessAction
                        (
                            x => logger.Info($"</SaveWorldStates> success")
                        ).FailureAction
                        (
                            x => logger.Info($"</SaveWorldStates> failure")
                        );
                    }
                ).FailureAction( error => logger.Info($"</SaveWorldStates> fail reading worldstate {error}"));
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
                foreach (var world in RemoteWorlds.OfType<RemoteWorld>().ToList())
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