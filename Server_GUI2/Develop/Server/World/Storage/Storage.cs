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
    /// <summary>
    /// ワールドの保存先(Gitリポジトリ,Gdrive等)
    /// </summary>
    public abstract class Storage
    {
        public ObservableCollection<RemoteWorld> Worlds = new ObservableCollection<RemoteWorld>();
        public abstract string Id { get; }
    }

    /// <summary>
    /// gitリモートリポジトリをあらわす　インスタンス数はリポジトリの数によって変わる
    /// </summary>
    public class GitStorage: Storage
    {
        public static readonly GitLocal Local = new GitLocal(Path.Combine(SetUp.CurrentDirectory, "git_state"));
        private Dictionary<string, WorldState> worldStates = new Dictionary<string, WorldState>();
        private HashSet<string> usedNames = new HashSet<string>();

        public override string Id => $"git/{Repository.Remote.Remote.Account}/{Repository.Remote.Remote.RepoName}";

        /// <summary>
        /// 使用可能なブランチ名かどうかを返す
        /// </summary>
        public bool IsUsableName(string name)
        {
            // TODO: 使用不可文字が入っていてもfalseを返す
            return ! usedNames.Contains(name);
        }

        public static List<GitStorage> GetStorages()
        {
            var repos = GitStorageRepository.GetAllGitRepositories(Local);
            return repos.Select(x => new GitStorage(x)).ToList();
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
                if ( usedNames.Contains(worldState.Key) )
                {
                    var type = worldState.Value.Type == "vanilla" ? ServerType.Vanilla : ServerType.Spigot;
                    var property = worldState.Value.ServerProperty;
                    var datapacks = new DatapackCollection(worldState.Value.Datapacks);
                    var remoteWorld = new GitRemoteWorld(type, property, datapacks);
                    Worlds.Add(remoteWorld);
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
        public void SaveWorldState()
        {
            Repository.SaveGitWorldstate(worldStates);
        }

        /// <summary>
        /// TODO: ストレージ登録情報を削除
        /// </summary>
        public void Delete()
        {
        }
    }
}
