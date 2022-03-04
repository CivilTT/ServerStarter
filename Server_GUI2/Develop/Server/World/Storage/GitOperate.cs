using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2;
using Server_GUI2.Util;
using Newtonsoft.Json;

namespace Server_GUI2.Develop.Server.World
{
    /// <summary>
    /// Gitリポジトリ管理用クラス
    /// </summary>
    public class GitStorageManager
    {
        public static GitStorageManager Instance { get; } = new GitStorageManager();

        private readonly GitLocal gitLocal;

        private readonly GitStatePath stateDirectory;

        private GitStorageManager()
        {
            stateDirectory = ServerGuiPath.Instance.GitState;
            gitLocal = new GitLocal(stateDirectory.FullName);
            // ディレクトリ生成
            CreateDirectory();
        }

        /// <summary>
        /// ディレクトリが存在しなかった場合生成し初期化
        /// </summary>
        private void CreateDirectory()
        {
            if (stateDirectory.Exists)
                return;
            stateDirectory.Create(true);
            // git init
            gitLocal.Init();
            // git commit --allow-empty -m "empty"
            gitLocal.AddAllAndCommit("first commit");
        }

        /// <summary>
        /// worldstateをpush
        /// </summary>
        public void WriteWorldState(GitRemote remote, string email, Dictionary<string, WorldState> worldState)
        {
            var branch = GetRemoteBranch(remote);
            branch.LocalBranch.Checkout();
            stateDirectory.WorldStateJson.WriteJson(worldState);
            branch.LocalBranch.Local.SetUser(remote.Account, email);
            branch.CommitPush("update worldstate.json");
        }

        /// <summary>
        /// worldstateをpull
        /// </summary>
        public Dictionary<string, WorldState> ReadWorldState(GitRemote remote)
        {
            var branch = GetRemoteBranch(remote);
            branch.LocalBranch.Checkout();
            branch.Pull();
            var worldState = stateDirectory.WorldStateJson.ReadJson();
            return worldState;
        }

        public string GetId(GitRemote remote) => $"{remote.Account}.{remote.Repository}";

        public GitNamedRemote NamedRemote(GitRemote remote)
        {
            var id = GetId(remote);
            return new GitNamedRemote(gitLocal, remote, id);
        }

        /// <summary>
        /// worldstateをpull
        /// </summary>
        public void RemoveRemote(GitRemote remote)
        {
            var id = GetId(remote);

            // git checkout "#main"
            var main = gitLocal.GetBranch("#main");
            if (!main.Exists)
                gitLocal.CreateBranch("#main");
            main.Checkout();
            // git remote remove {account}.{repository}
            gitLocal.GetBranch(id).Remove();
            // git branch -d {account}.{repository}
            gitLocal.GetNamedRemotes()[id].Remove();
        }


        /// <summary>
        /// #stateブランチを返す。無ければ生成する
        /// </summary>
        /// <param name="remote"></param>
        private GitLinkedLocalBranch GetRemoteBranch(GitRemote remote)
        {
            var id = GetId(remote);

            // 既に存在する場合
            if (gitLocal.GetBranch(id).Exists)
            {
                var namedRemote = new GitNamedRemote(gitLocal, remote, id);
                var branch = new GitRemoteBranch(namedRemote, "#state", true);
                return new GitLinkedLocalBranch(gitLocal.GetBranch(id), branch);
            }

            // 新規作成する場合
            // git remote add {id}
            var named = gitLocal.AddRemote(remote, id);

            var remoteBranchs = named.GetBranchs();

            // 1. まっさらなリポジトリである場合(#stateブランチがない場合)
            if (!remoteBranchs.Keys.Contains("#state"))
            {
                // 新規ブランチを作成 git branch {account}.{repository}
                var branch = gitLocal.CreateBranch(id);

                // git checkout {account}.{repository}
                branch.Checkout();

                // worldstate.jsonを上書き/生成
                var jsonPath = Path.Combine(gitLocal.Path, "worldstate.json");
                if (!File.Exists(jsonPath))
                {
                    File.Create(jsonPath).Close();
                }

                stateDirectory.WorldStateJson.WriteJson(new Dictionary<string, WorldState>());

                // git add -A
                // git commit -m "initialized"
                // git push -u {account}.{repository} {account}.{repository}:#state
                var linked = branch.CreateLinkedBranch(named, "#state");
                linked.CommitPush("initialized");
                return linked;
            }
            // 2. #stateブランチがある場合
            else
            {
                var statebranch = remoteBranchs["#state"];

                // git branch {account}.{repository} --track {account}.{repository}/#state
                // git checkout {account}.{repository}
                var branch = statebranch.CreateLinkedBranch(id);
                branch.Pull();
                branch.LocalBranch.Checkout();
                return branch;
            }
        }
    }
}


    //public class GitStorageRepository
//    {
//        public GitLinkedLocalBranch Branch;

//        /// <summary>
//        /// 最初のリポジトリ
//        /// </summary>
//        private static void Init(GitLocal local)
//        {
//            // git init
//            local.Init();
//            // git commit --allow-empty -m "empty"
//            local.AddAllAndCommit("first commit");
//        }

//        /// <summary>
//        /// 新規リポジトリ
//        /// </summary>
//        public static GitStorageRepository AddRepository(GitLocal local, GitRemote remote)
//        {
//            var id = $"{remote.Account}.{remote.Repository}";
//            // 新たにrepositoryを紐づける際に起動

//            // フォルダ生成
//            ServerGuiPath.Instance.GitState.Create(true);

//            // 初めてアカウントを紐づける場合(cd/git_worldstateがない場合) Init()

//            // cd/git_worldstate 内でgit remote add {account}.{repository} https://...
//            var namedRemote = local.AddRemote(remote, id);
//            var remoteBranchs = namedRemote.GetBranchs();

//            // 1. まっさらなリポジトリである場合(#stateブランチがない場合)
//            if (! remoteBranchs.Keys.Contains("#state"))
//            {
//                // 新規ブランチを作成 git branch {account}.{repository}
//                var branch = local.CreateBranch(id);
//                // git checkout {account}.{repository}
//                branch.Checkout();

//                // worldstate.jsonを上書き/生成
//                var jsonPath = Path.Combine(local.Path, "worldstate.json");
//                if (!File.Exists(jsonPath))
//                {
//                    File.Create(jsonPath).Close();
//                }

//                var content = "{}";
//                File.WriteAllText(jsonPath, content);

//                // git add -A
//                // git commit -m "initialized"
//                // git push -u {account}.{repository} {account}.{repository}:#state
//                var linked = branch.CreateLinkedBranch(namedRemote, "#state");
//                linked.CommitPush("initialized");
//                return new GitStorageRepository(linked);
//            }
//            // 2. #stateブランチがある場合
//            else
//            {
//                var statebranch = remoteBranchs["#state"];

//                // git branch {account}.{repository} --track {account}.{repository}/#state
//                // git checkout {account}.{repository}
//                var branch = statebranch.CreateLinkedBranch(id);
//                branch.Pull();
//                branch.LocalBranch.Checkout();
//                return new GitStorageRepository(branch);
//            }
//        }

//        public static List<GitStorageRepository> GetAllGitRepositories(GitLocal local)
//        {
//            if (!local.IsGitRepository())
//            {
//                return new List<GitStorageRepository>();
//            }
//            else
//            {
//                return local.GetBranchs().Select(x => x.Value).OfType<GitLinkedLocalBranch>().Select(x => new GitStorageRepository(x)).ToList();
//            }
//        }

//        public GitStorageRepository(GitLinkedLocalBranch branch)
//        {
//            Branch = branch;
//        }

//        public Dictionary<string, WorldState> GetGitWorldstate()
//        {
//            // cd/git_worldstate
//            // git checkout {account}.{repository}
//            Branch.LocalBranch.Checkout();

//            // git pull {account}.{repository}
//            Branch.Pull();

//            return ServerGuiPath.Instance.GitState.WorldStateJson.ReadJson();
//        }
//        public void SaveGitWorldstate(Dictionary<string, WorldState> worldstates)
//        {
//            ServerGuiPath.Instance.GitState.WorldStateJson.WriteJson(worldstates,false,true);

//            Branch.LocalBranch.Checkout();
//            Branch.CommitPush("changed worldstate.json");
//        }

//        /// <summary>
//        /// リモートリポジトリの情報をローカルから削除
//        /// </summary>
//        public void RemoveGitRepository()
//        {
//            // git checkout "#main"
//            Branch.LocalBranch.Local.GetBranch("#main").Checkout();
//            // git remote remove {account}.{repository}
//            Branch.RemoteBranch.NamedRemote.Remove();
//            // git branch -d {account}.{repository}
//            Branch.LocalBranch.Remove();
//        }
//    }
//}

////public class WorldStateJson
////{
////    [JsonProperty("broadcast-rcon-to-ops")]
////}