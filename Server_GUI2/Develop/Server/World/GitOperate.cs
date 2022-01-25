using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2;
using Server_GUI2.Develop.Util;

namespace Server_GUI2.Develop.Server.World
{
    public class GitStateReader
    {
        GitLocalBranch LocalBranch;
        GitNamedRemote Remote;

        /// <summary>
        /// 最初のリポジトリ
        /// </summary>
        public static void Init(GitLocal local)
        {
            // git init
            // git branch -m "#main"
            local.Init("#main");
            // git commit --allow-empty -m "empty"
            local.Commit("empty");
            local.Commit("empty");
            local.Commit("empty");
        }

        /// <summary>
        /// 新規リポジトリ
        /// </summary>
        public static GitStateReader AddRepository(GitLocal local, GitRemote remote)
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
                var content = "{" + string.Join(",", remoteBranchs.Keys.Select(str => $"\"{str}\":{{\"type\":\"New\"}}")) + "}" ;
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
            return new GitStateReader(branch, namedRemote);
        }

        public static void GetAllGitWorlds(GitLocal local)
        {
            if ( ! local.IsGitLocal())
            {
                return;
            }
            // cd/git_worldstate
            // 紐づけられたリポジトリを取得
            var remotes = local.GetRemotes();
            var branchs = local.GetBranchs();
            foreach (var remote in remotes)
            {
                Console.WriteLine(remote.Name);
                new GitStateReader(branchs[remote.Name],remote);
            }
        }

        /// <summary>
        /// リモートリポジトリの #state ブランチを確認する
        /// </summary>
        public GitStateReader(GitLocalBranch localBranch, GitNamedRemote remote)
        {
            LocalBranch = localBranch;
            Remote = remote;
        }
        public void GetGitWorlds()
        {
            // cd/git_worldstate
            // git checkout {account}.{repository}
            LocalBranch.Checkout();
            // git pull {account}.{repository}
            LocalBranch.Pull();
            // TODO: worldstate.json を開きリモートワールド情報からGitWorldReaderインスタンスを生成しWorldFactory.Instance.Wordlsに追加
            var jsonPath = Path.Combine(LocalBranch.Local.Path, "worldstate.json");
            File.ReadAllText(jsonPath);
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
