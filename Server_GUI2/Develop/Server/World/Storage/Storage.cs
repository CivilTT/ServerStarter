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

namespace Server_GUI2.Develop.Server.World
{
    public class StorageCollection
    {
        public static StorageCollection Instance { get; } = new StorageCollection();

        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();

        private StorageCollection()
        {
            // gitのリポジトリを全取得
            GitStorage.GetStorages().ForEach(x => Add(x));
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
        }
    }


    /// <summary>
    /// ワールドの保存先(Gitリポジトリ,Gdrive等)
    /// </summary>
    public abstract class Storage
    {
        public ObservableCollection<RemoteWorld> RemoteWorlds = new ObservableCollection<RemoteWorld>();
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
            if (Available)
            {
                return RemoteWorlds.Where(x => x.Id == worldId).First();
            }
            else
            {
                return CreateUnavailableRemoteWorld(worldId);
            }
        }

        protected abstract RemoteWorld CreateUnavailableRemoteWorld(string worldId);

        public virtual void Delete()
        {
            if (DeleteEvent != null) DeleteEvent(this,null);
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
            usedNames.Add(name);
        }
    }

    /// <summary>
    /// 各gitリモートリポジトリをあらわす
    /// </summary>
    public class GitStorage: Storage
    {
        public static readonly GitLocal Local = new GitLocal(ServerGuiPath.Instance.GitState.FullName);

        public override string Id => $"git/{Repository.Branch.RemoteBranch.NamedRemote.Remote.Account}/{Repository.Branch.RemoteBranch.NamedRemote.Remote.Repository}";

        public override string AccountName => Repository.Branch.RemoteBranch.NamedRemote.Remote.Account;

        public override string RepositoryName => Repository.Branch.RemoteBranch.NamedRemote.Remote.Repository;

        public override string Email => "dummy.email@dummy.com";
        /// <summary>
        /// 使用可能なブランチ名かどうかを返す
        /// </summary>
        public override bool IsUsableName(string name)
        {
            return Regex.IsMatch(name, "^[a-zA-Z0-9_-]+$") && !usedNames.Contains(name);
        }

        public static List<GitStorage> GetStorages()
        {
            var repos = GitStorageRepository.GetAllGitRepositories(Local);
            return repos.Select(x => new GitStorage(x)).ToList();
        }

        protected override RemoteWorld CreateUnavailableRemoteWorld(string worldId)
        {
            return new GitRemoteWorld(Repository.Branch.RemoteBranch.NamedRemote.Remote, worldId, null, this, true);
        }

        /// <summary>
        /// 新しいリポジトリを生成、ストレージ一覧に追加する
        /// </summary>
        public static Either<GitStorage,string> AddStorage(string account, string repository, string email)
        {
            // TODO: ストレージのアカウント系のエラー処置
            //StorageCollection.Instance.Storages
            var remote = new GitRemote(account,repository);
            var repo = GitStorageRepository.AddRepository(Local,remote);
            var storage = new GitStorage(repo);
            StorageCollection.Instance.Add(storage);
            return Success<GitStorage, string>(storage);
        }

        private static Either<T1, T2> Success<T1, T2>(T1 storage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新規リモートワールドを生成
        /// </summary>
        public override RemoteWorld CreateRemoteWorld(string worldName)
        {
            // ストレージにアクセスできない場合はエラー
            if (!Available) throw new RemoteWorldException($"git storage \"{Repository.Branch.RemoteBranch.Expression}\" is not accessible.");

            var prop = new ServerProperty();

            var id = Guid.NewGuid();

            var remote = Repository.Branch.RemoteBranch.NamedRemote.Remote;
            var datapacks = new DatapackCollection(new List<string>());
            var plugins = new PluginCollection(new List<string>());
            var result = new GitRemoteWorld( remote, this,id.ToString(),worldName,false, null, null, prop, datapacks, plugins, true );
            AddWorld(result);
            return result;
        }

        public override string ToString()
        {
            return $"{ Repository.Branch.LocalBranch.Name}:{ Repository.Branch.RemoteBranch.NamedRemote.Remote.Expression}";
        }

        public GitStorageRepository Repository;
        public GitStorage(GitStorageRepository repository)
        {
            Repository = repository;
            Available = repository.Branch.RemoteBranch.NamedRemote.IsAvailable;

            var newUsedNames = new HashSet<string>();

            foreach ( var kv in Repository.Branch.RemoteBranch.NamedRemote.GetBranchs())
            {
                newUsedNames.Add(kv.Key);
            }

            worldStates = Repository.GetGitWorldstate();

            foreach ( var worldState in worldStates)
            {
                if (newUsedNames.Contains(worldState.Key))
                {
                    var remoteWorld = new GitRemoteWorld(repository.Branch.RemoteBranch.NamedRemote.Remote, worldState.Key, worldState.Value, this, true);
                    RemoteWorlds.Add(remoteWorld);
                    // 削除イベントを追加
                    remoteWorld.DeleteEvent += new EventHandler((_, __) => RemoteWorlds.Remove(remoteWorld));
                }
                else
                {
                    // 存在しなくなったブランチはworldstateから削除
                    worldStates.Remove(worldState.Key);
                }
            }

            usedNames.UnionWith(newUsedNames);
        }

        /// <summary>
        /// Worldstateを#state/worldstate.jsonに保存
        /// </summary>
        public override void SaveWorldStates()
        {
            // 通信可能な場合のみ発動
            if (Available)
            {
                // 存在しているリモートだけをフィルタして保存
                Repository.SaveGitWorldstate(RemoteWorlds.Where(x => x.Exist == true).ToDictionary(x => x.Id, x => x.ExportWorldState()));
            }
        }

        /// <summary>
        /// TODO: ストレージ登録情報を削除
        /// </summary>
        public override void Delete()
        {
            // リポジトリの登録を削除
            Repository.RemoveGitRepository();

            // 紐づいたリモートワールドを削除
            foreach(var world in RemoteWorlds)
            {
                world.Delete();
            }

            base.Delete();
        }
    }
}