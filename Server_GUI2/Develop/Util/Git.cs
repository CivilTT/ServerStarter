
using log4net;
using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Server_GUI2.Develop.Util
{
    public class GitException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GitException(string message) : base(message)
        {
            logger.Info(message);
            Console.WriteLine(App.end_str);
        }
    }

    class GitCommand
    {
        public static string ExecuteThrow(string arguments, Exception exception,string directory)
        {
            var (code, output) = Execute(arguments, directory);

            if (code != 0)
            {
                Console.WriteLine(output);
                throw exception;
            }
            else
            {
                return output;
            }
        }

        public static (int, string) Execute(string arguments, string directory)
        {
            // TODO: directory引数で実行ディレクトリを変更
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo("git")
                {
                    Arguments = directory == null ? arguments : $"-C \"{directory}\" {arguments}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };
            Console.WriteLine("git " + (directory == null ? arguments : $"-C \"{directory}\" {arguments}"));
            process.Start();
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();

            return (process.ExitCode, output);
        }
    }

    public class GitLocalBranch
    {
        public readonly GitLocal Local;
        public string Name { get; private set; }
        public GitLocalBranch(GitLocal local, string name)
        {
            Local = local;
            Name = name;
        }

        public void Rename(string name)
        {
            GitCommand.ExecuteThrow($"branch -m {Name} {name}", new GitException($"cannot change branch name. {Name} -> {name}"), Local.Path);
            Name = name;
        }

        public void Remove()
        {
            GitCommand.ExecuteThrow($"branch -d {Name}", new GitException($"cannot remove branch. {Name}"), Local.Path);
        }

        public void Checkout()
        {
            GitCommand.ExecuteThrow($"checkout {Name}", new GitException($"cannot checkout branch {Name}."), Local.Path);
        }

        /// <summary>
        /// トラック中のリモートブランチにpush
        /// </summary>
        public void Push()
        {
            GitCommand.ExecuteThrow($"push ", new GitException($"cannot push branch {Name}."), Local.Path);
        }

        /// <summary>
        /// 特定のリモートブランチにpush
        /// </summary>
        public void Push(GitRemoteBranch remoteBranch)
        {
            remoteBranch.Remote.AssertRelated(Local);
            GitCommand.ExecuteThrow($"push \"{remoteBranch.Remote.Expression}\" \"{Name}:{remoteBranch.Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
        }

        /// <summary>
        /// 同名のリモートブランチにpush
        /// </summary>
        public void Push(GitRemote remote)
        {
            remote.AssertRelated(Local);
            GitCommand.ExecuteThrow($"push \"{remote.Expression}\" \"{Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
        }

        /// <summary>
        /// 特定のリモートブランチにpushしtrack
        /// </summary>
        public void PushTrack(GitRemoteBranch remoteBranch)
        {
            remoteBranch.Remote.AssertRelated(Local);
            GitCommand.ExecuteThrow($"push -u \"{remoteBranch.Remote.Expression}\" \"{Name}:{remoteBranch.Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
        }

        /// <summary>
        /// 同名のリモートブランチにpushしtrack
        /// </summary>
        public void PushTrack(GitRemote remote)
        {
            remote.AssertRelated(Local);
            GitCommand.ExecuteThrow($"push -u \"{remote.Expression}\" \"{Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
        }

        /// <summary>
        /// trackしているリモートブランチからPull
        /// </summary>
        public void Pull()
        {
            GitCommand.ExecuteThrow($"pull", new GitException($"cannot pull branch {Name}."), Local.Path);
        }
    }
    public class GitLocal
    {
        /// <summary>
        /// 与えられたパスがGitRepositoryのトップレベルかどうかを確認する
        /// </summary>
        public static bool IsGitLocal(string path)
        {
            var (code, output) = GitCommand.Execute("rev-parse --show-toplevel", path);
            return code == 0 && System.IO.Path.GetFullPath(output).Equals(System.IO.Path.GetFullPath(path));
        }

        public static GitLocal InitGitLocal(string path)
        {
            GitCommand.ExecuteThrow("init", new GitException("cannot initialize local repository."), path);
            return new GitLocal(path);
        }

        public string Path { get; }

        public GitLocal(string path)
        {
            Path = path;
        }

        public bool IsGitLocal()
        {
            return IsGitLocal(Path);
        }

        /// <summary>
        /// 現在のローカルブランチを取得する
        /// </summary>
        public GitLocalBranch GetCurrentBranch()
        {
            var output = GitCommand.ExecuteThrow("symbolic-ref --short HEAD", new GitException("cannot get current branch name."), Path);
            return new GitLocalBranch(this, output.Substring(0, output.Length - 1)); // 末尾の\nを削除
        }

        public Dictionary<string, GitLocalBranch> GetBranchs()
        {
            var output = GitCommand.ExecuteThrow($"branch", new GitException($"failed to get branch list from {Path}"),Path);
            var branchMap = new Dictionary<string, GitLocalBranch>();
            var lines = output.Substring(0, output.Length - 1).Split('\n'); //出力を行に分解  e.g."* master"
            foreach (var i in lines)
            {
                var branchName = i.Substring(2);
                branchMap[branchName] = new GitLocalBranch(this, branchName);
            }
            return branchMap;
        }

        /// <summary>
        /// ローカルブランチを存在確認なしに返す
        /// </summary>
        public GitLocalBranch GetBranch(string name)
        {
            return new GitLocalBranch(this, name);
        }

        public GitLocalBranch CreateBranch(string name)
        {
            GitCommand.ExecuteThrow($"branch \"{name}\"", new GitException($"failed to create branch {name}"), Path);
            return new GitLocalBranch(this, name);
        }

        public GitLocalBranch CreateTrackBranch(string name,GitRemoteBranch track)
        {
            GitCommand.ExecuteThrow($"branch \"{name}\" --track \"{track.Remote.Expression}/{track.Name}\"", new GitException($"failed to create branch {name}"), Path);
            return new GitLocalBranch(this, name);
        }

        public void AddAll()
        { 
            GitCommand.ExecuteThrow($"add -A", new GitException($"failed to add files"), Path);
        }
        
        public void Commit(string message)
        {
            GitCommand.ExecuteThrow($"commit --allow-empty -m \"{message}\"", new GitException($"failed to commit"), Path);
        }

        public GitNamedRemote AddRemote(GitRemote remote,string name)
        {
            GitCommand.ExecuteThrow($"remote add \"{name}\" \"{remote.Expression}\"", new GitException($"failed to add remote repository :{remote.Expression} {Path}"), Path);
            return new GitNamedRemote(this,remote,name);
        }

        public GitLocalBranch Init(string branchName)
        {
            GitCommand.ExecuteThrow($"init", new GitException($"failed to init {Path}"), Path);
            GitCommand.ExecuteThrow($"branch -m \"{branchName}\"", new GitException($"failed to change branch name to {branchName}"), Path);
            return GetBranch(branchName);
        }
        public void Fetch(GitRemoteBranch remoteBranch)
        {
            GitCommand.ExecuteThrow($"fetch \"{remoteBranch.Remote.Expression}\" \"{remoteBranch.Name}\"", new GitException($"failed to fetch {remoteBranch.Remote.Expression}/{remoteBranch.Name} to {Path}"), Path);
        }
        public void Merge()
        {
            GitCommand.ExecuteThrow($"merge", new GitException($"failed to merge path:{Path}"), Path);
        }

        public void Push()
        {
            GitCommand.ExecuteThrow($"push", new GitException($"failed to push path:{Path}"), Path);
        }

        public List<GitNamedRemote> GetRemotes()
        {
            var remotes = new List<GitNamedRemote>();
            var output = GitCommand.ExecuteThrow($"remote -v", new GitException($"failed to get remotelist of {Path}"), Path);


            GitNamedRemote GetNamedRemote(string str)
            {
                var strs = str.Split();
                var urls = strs[1].Split('/').ToList();
                var accountName = urls[urls.Count - 2];
                var repositoryName = urls[urls.Count - 1].Substring(0,urls[urls.Count - 1].Length - 4);
                return new GitNamedRemote(this, new GitRemote(accountName, repositoryName),strs[0]);
            }

            var remoteData = output.Substring(0, output.Length - 1).Split('\n').Where( ( _,i ) => i % 2 == 0 );

            return remoteData.Select(name => GetNamedRemote(name)).ToList();
        }
    }

    public class GitRemoteBranch
    {
        public readonly IGitRemote Remote;
        public readonly string Name;
        public GitRemoteBranch(IGitRemote remote, string name)
        {
            Remote = remote;
            Name = name;
        }
    }

    public interface IGitRemote
    {
        string Expression { get; }
        bool IsMyBranch(GitRemoteBranch branch);
        void AssertRelated(GitLocal local);

        GitRemoteBranch CreateBranch(string name);
    }

    /// <summary>
    /// ローカルリポジトリに紐づいたリモートリポジトリエイリアス( origin的な奴 )
    /// </summary>
    public class GitNamedRemote: IGitRemote
    {
        public readonly GitLocal Local;
        public readonly GitRemote Remote;
        public readonly string Name;
        public GitNamedRemote(GitLocal local, GitRemote remote, string name)
        {
            Local = local;
            Remote = remote;
            Name = name;
        }

        public string Expression
        {
            get
            {
                return Name;
            }
        }

        /// <summary>
        /// 自分が引数のローカルリポジトリのエイリアスかどうかを確認し違ったらエラー
        /// </summary>
        public void AssertRelated(GitLocal local)
        {
            if (! IsRelated(local))
            {
                throw new GitException($"remote repository {Name} is not related to local repository {local.Path}.");
            }
        }

        public GitRemoteBranch CreateBranch(string name)
        {
            return new GitRemoteBranch(this, name);
        }

        public bool IsMyBranch(GitRemoteBranch branch)
        {
            return branch.Remote == this;
        }

        public bool IsRelated(GitLocal local)
        {
            return Local == local;
        }

        /// <summary>
        /// Localとの紐づけを解除する リモートリポジトリは削除しない
        /// </summary>
        public void Remove()
        {
            GitCommand.ExecuteThrow($"remote rm \"{Name}\"", new GitException($"failed to remove remote repository"), Local.Path);
        }
    }
    public class GitRemote : IGitRemote
    {
        public string Account { get; private set; }
        public string RepoName { get; private set; }


        public string Expression
        {
            get
            {
                return $"https://{Account}@github.com/{Account}/{RepoName}.git";
            }
        }

        public GitRemote(string account, string repoName)
        {
            Account = account;
            RepoName = repoName;
        }

        public GitRemoteBranch CreateBranch(string name)
        {
            return new GitRemoteBranch(this, name);
        }

        public Dictionary<string, GitRemoteBranch> GetBranchs()
        {
            var output = GitCommand.ExecuteThrow($"ls-remote --heads {Expression}", new GitException($"failed to get branch list from {Expression}"),null);
            var branchMap = new Dictionary<string, GitRemoteBranch>();
            if (output.Length == 0)
            {
                return branchMap;
            }
            var lines = output.Substring(0, output.Length - 1).Split('\n');
            foreach (var i in lines)
            {
                var branchName = i.Substring(52);
                branchMap[branchName] = new GitRemoteBranch(this, branchName);
            }
            return branchMap;
        }

        public bool IsMyBranch(GitRemoteBranch branch)
        {
            return branch.Remote == this;
        }
        public void AssertRelated(GitLocal local)
        {}
    }
}
