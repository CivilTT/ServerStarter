using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2;
using Server_GUI2.Develop.Util;
using Newtonsoft.Json;

namespace Server_GUI2.Develop.Server.Storage
{
    /// <summary>
    /// Gitリポジトリ管理用クラス
    /// </summary>
    public class GitStorageRepository
    {
        public GitLocalBranch LocalBranch;
        public GitNamedRemote Remote;

        /// <summary>
        /// 最初のリポジトリ
        /// </summary>
        private static void Init(GitLocal local)
        {
            // git init
            // git branch -m "#main"
            local.Init("#main");
            // git commit --allow-empty -m "empty"
            local.Commit("first commit");
        }

        /// <summary>
        /// 新規リポジトリ
        /// </summary>
        public static GitStorageRepository AddRepository(GitLocal local, GitRemote remote)
        {
            var id = $"{remote.Account}.{remote.RepoName}";
            // 新たにrepositoryを紐づける際に起動

            // 初めてアカウントを紐づける場合(cd/git_worldstateがない場合) Init()
            if ( ! Directory.Exists(local.Path))
            {
                Directory.CreateDirectory(local.Path);
                Init(local);
            }

            // cd/git_worldstate 内でgit remote add {account}.{repository} https://...
            var namedRemote = local.AddRemote(remote, id);
            var remoteBranchs = remote.GetBranchs();

            GitLocalBranch branch;
            GitRemoteBranch remoteBranch = namedRemote.CreateBranch("#state");

            // 1. まっさらなリポジトリである場合(#stateブランチがない場合)
            if (! remoteBranchs.Keys.Contains("#state"))
            {
                // 新規ブランチを作成 git branch {account}.{repository}
                branch = local.CreateBranch(id);
                // git checkout {account}.{repository}
                branch.Checkout();

                // worldstate.jsonを上書き/生成
                var jsonPath = Path.Combine(local.Path, "worldstate.json");
                if (!File.Exists(jsonPath))
                {
                    File.Create(jsonPath).Close();
                }
                var content = "{}";
                File.WriteAllText(jsonPath, content);

                // git add -A
                local.AddAll();
                // git commit -m "initialized"
                local.Commit("initialized");
                // git push -u {account}.{repository} {account}.{repository}:#state
                branch.PushTrack(remoteBranch);
            }
            // 2. #stateブランチがある場合
            else
            {
                // cd/git_worldstate 内でgit branch {account}.{repository} --track {account}.{repository}/#state
                local.Fetch(remoteBranch);
                branch = local.CreateTrackBranch(id, remoteBranch);
                // git checkout {account}.{repository}
            }
            return new GitStorageRepository(branch, namedRemote);
        }

        public static List<GitStorageRepository> GetAllGitRepositories(GitLocal local)
        {
            var repos = new List<GitStorageRepository>();

            if ( ! local.IsGitLocal())
            {
                return repos;
            }
            // cd/git_worldstate
            // 紐づけられたリポジトリを取得
            var remotes = local.GetRemotes();
            var branchs = local.GetBranchs();
            foreach (var remote in remotes)
            {
                repos.Add(new GitStorageRepository(branchs[remote.Name],remote));
            }
            return repos;
        }

        public GitStorageRepository(GitLocalBranch localBranch, GitNamedRemote remote)
        {
            LocalBranch = localBranch;
            Remote = remote;
        }
        public Dictionary<string, WorldState> GetGitWorldstate()
        {
            // cd/git_worldstate
            // git checkout {account}.{repository}
            LocalBranch.Checkout();
            // git pull {account}.{repository}
            LocalBranch.Pull();
            // TODO: worldstate.json を開きリモートワールド情報からWorldインスタンスを生成し返却
            var jsonPath = Path.Combine(LocalBranch.Local.Path, "worldstate.json");
            var jsonContent = File.ReadAllText(jsonPath);
            var worldState = JsonConvert.DeserializeObject<Dictionary<string, WorldState>>(jsonContent, new JsonWorldStateConverter());
            return worldState;
        }
        public void SaveGitWorldstate(Dictionary<string, WorldState> worldstates)
        {
            LocalBranch.Checkout();
            var jsonContent = JsonConvert.SerializeObject(worldstates, new JsonWorldStateConverter());
            var jsonPath = Path.Combine(LocalBranch.Local.Path, "worldstate.json");
            File.WriteAllText(jsonPath, jsonContent);
            LocalBranch.Local.AddAll();
            LocalBranch.Local.Commit("changed worldstate.json");
            LocalBranch.Push();
        }

        public void RemoveGitRepository()
        {
            // 以下の操作は cd/git_worldstate 内で行う
            Remote.Remove();
            // git remote remove {account}.{repository}
            // git checkout "#main"
            LocalBranch.Local.GetBranch("#main").Checkout();
            // git branch -d {account}.{repository}
            LocalBranch.Remove();
            // TODO: WorldFactory.Instance.Wordlsから該当リポジトリに紐づいたワールドをワールドデータとともに削除
        }
    }
}

//public class WorldStateJson
//{
//    [JsonProperty("broadcast-rcon-to-ops")]
//}