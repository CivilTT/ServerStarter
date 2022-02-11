using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;

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

        // TODO: リモートリポジトリから消えた場合とリモートリポジトリと通信できない場合エラーを吐く
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
        public abstract string Id { get; }

        public virtual void AddWorld(RemoteWorld world)
        {
            RemoteWorlds.Add(world);
            world.DeleteEvent += new EventHandler((_, __) => RemoteWorlds.Remove(world));
        }

        public RemoteWorld FindRemoteWorld(string worldId)
        {
            return RemoteWorlds.Where(x => x.Id == worldId).First();
        }

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
    }

    /// <summary>
    /// 各gitリモートリポジトリをあらわす
    /// </summary>
    public class GitStorage: Storage
    {
        public static readonly GitLocal Local = new GitLocal(ServerGuiPath.Instance.GitState.FullName);
        private HashSet<string> usedNames = new HashSet<string>();

        public override string Id => $"git/{Repository.Remote.Remote.Account}/{Repository.Remote.Remote.RepoName}";

        /// <summary>
        /// 使用可能なブランチ名かどうかを返す
        /// </summary>
        public override bool IsUsableName(string name)
        {
            // TODO: 使用不可文字が入っていてもfalseを返す
            return ! usedNames.Contains(name);
        }

        public static List<GitStorage> GetStorages()
        {
            var repos = GitStorageRepository.GetAllGitRepositories(Local);
            return repos.Select(x => new GitStorage(x)).ToList();
        }

        /// <summary>
        /// 新しいリポジトリを追加する
        /// </summary>
        public static GitStorage AddStorage(string account,string repository)
        {
            var remote = new GitRemote(account,repository);
            var repo = GitStorageRepository.AddRepository(Local,remote);
            var storage = new GitStorage(repo);
            StorageCollection.Instance.Add(storage);
            return storage;
        }

        /// <summary>
        /// 新規リモートワールドを生成
        /// </summary>
        public override RemoteWorld CreateRemoteWorld(string worldName)
        {
            var prop = new ServerProperty();

            var id = Guid.NewGuid();

            var result = new GitRemoteWorld( Repository.Remote.Remote,this,id.ToString(),worldName,false, null, null, prop, new DatapackCollection(new List<string>()));
            AddWorld(result);
            return result;
        }

        public override string ToString()
        {
            return $"{ Repository.LocalBranch.Name}:{ Repository.Remote.Expression}";
        }

        public GitStorageRepository Repository;
        public GitStorage(GitStorageRepository repository)
        {
            Repository = repository;
            foreach ( var kv in Repository.Remote.Remote.GetBranchs())
            {
                usedNames.Add(kv.Key);
            }

            worldStates = Repository.GetGitWorldstate();

            foreach ( var worldState in worldStates)
            {
                if ( usedNames.Contains(worldState.Key))
                {
                    var remoteWorld = new GitRemoteWorld(repository.Remote.Remote, worldState.Key, worldState.Value, this);
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
        }

        /// <summary>
        /// Worldstateを#state/worldstate.jsonに保存
        /// </summary>
        public override void SaveWorldStates()
        {
            // 存在しているリモートだけをフィルタして保存
            Repository.SaveGitWorldstate( RemoteWorlds.Where(x => x.Exist == true).ToDictionary(x => x.Id,x => x.ExportWorldState()));
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