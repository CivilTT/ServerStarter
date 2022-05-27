
using log4net;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Http;
using System.IO;

namespace Server_GUI2.Util
{
    public class GitException : Exception
    {
        public GitException(string message) : base(message)
        {
            Console.WriteLine(App.end_str);
        }
    }


    class GitCommand
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string ExecuteThrow(string arguments, string directory)
        {
             return  ExecuteThrow(arguments, new GitException($"failed to execute \"git {arguments}\""), directory);
        }

        public static string ExecuteThrow(string arguments, Exception exception, string directory)
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
            logger.Info($"<GitCommand> git {arguments} ({directory})");
            var output = new StringBuilder(); ;
            int exitCode;

            var StartInfo = new ProcessStartInfo("git",arguments)
            {
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                WorkingDirectory = directory
            };

            using (var process = Process.Start(StartInfo))
            {

                var stdout = new StringBuilder();
                var stderr = new StringBuilder();

                process.OutputDataReceived += (sender, e) => { if (e.Data != null) { stdout.AppendLine(e.Data); } }; // 標準出力に書き込まれた文字列を取り出す
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) { stderr.AppendLine(e.Data); } }; // 標準エラー出力に書き込まれた文字列を取り出す
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                process.CancelOutputRead();
                process.CancelErrorRead();

                output.AppendLine(stdout.ToString());

                exitCode = process.ExitCode;
            }
            var o = output.ToString();
            logger.Info($"</GitCommand> exitcode: {exitCode},{o}");
            return (exitCode, o);
        }
    }

    public class GitLocal
    {
        public string Path;
        public GitLocal(string path)
        {
            Path = path;
        }

        /// <summary>
        /// gitリポジトリを初期化
        /// </summary>
        public void Init()
        {
            GitCommand.ExecuteThrow("init", new GitException("cannot initialize local repository."), Path);
        }

        /// <summary>
        /// gitリポジトリかどうかを確認
        /// </summary>
        public bool IsGitRepository()
        {
            var (code, output) = GitCommand.Execute("rev-parse --show-toplevel", Path);
            return code == 0 && System.IO.Path.GetFullPath(output).Equals(System.IO.Path.GetFullPath(Path));
        }

        public void SetUser(string account,string email)
        {
            GitCommand.ExecuteThrow($"config --local user.name \"{account}\"", Path);
            GitCommand.ExecuteThrow($"config --local user.email \"{email}\"", Path);
        }

        /// <summary>
        /// ブランチ一覧
        /// </summary>
        public Dictionary<string, IGitLocalBranch> GetBranchs()
        {
            var output = GitCommand.ExecuteThrow($"for-each-ref --format=%(refname:short)...%(upstream:short) refs/heads", new GitException($"failed to get branch list from {Path}"), Path).Trim();
            var branchMap = new Dictionary<string, IGitLocalBranch>();

            if (output.Length == 0) return branchMap;

            var lines = output.Split('\n'); //出力を行に分解

            var remotes = GetNamedRemotes();

            foreach (var i in lines)
            {
                var splitted = i.Trim().Split(new string[]{"..."},StringSplitOptions.RemoveEmptyEntries);

                var branchName = splitted[0];

                var branch = new GitLocalBranch(this, branchName);

                if (splitted.Length == 2)
                {
                    var remoteData = splitted[1].Split('/');
                    var remote = remotes[remoteData[0]];
                    var newBranch = new GitRemoteBranch(remote, remoteData[1],true);                        
                    branchMap[branchName] = new GitLinkedLocalBranch(branch, newBranch,true);
                }
                else
                {
                    branchMap[branchName] = branch;
                }

            }

            Console.WriteLine("GetBranchs");
            foreach (var jk in branchMap)
            {
                Console.WriteLine(jk.Key);
            }

            return branchMap;
        }

        /// <summary>
        /// 名前の付いたリモートの一覧
        /// </summary>
        public Dictionary<string, GitNamedRemote> GetNamedRemotes()
        {
            var remotes = new Dictionary<string, GitNamedRemote>();
            var output = GitCommand.ExecuteThrow($"remote -v", new GitException($"failed to get remotelist of {Path}"), Path).Trim();


            void GetNamedRemote(string str)
            {
                var strs = str.Split();
                var urls = strs[1].Split('/').ToList();
                var accountName = urls[urls.Count - 2];
                var repositoryName = urls[urls.Count - 1].Substring(0, urls[urls.Count - 1].Length - 4);
                remotes[strs[0]] = new GitNamedRemote(this, new GitRemote(accountName, repositoryName), strs[0]);
            }
            
            if (output.Length != 0)
            {
                var remoteData = output.Split('\n').Where((_, i) => i % 2 == 0).Select(x => x.Trim());
                remoteData.WriteLine();

                foreach (var remote in remoteData) GetNamedRemote(remote);

                return remotes;
            }
            else
            {
                return new Dictionary<string, GitNamedRemote>();
            }
        }

        public void AddAllAndCommit(string message)
        {
            // GitHubで扱えないサイズ(100MB+)のファイルをgitignoreに書く
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo("forfiles")
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Path,
                    Arguments = $"/s /c \"cmd /q /c if @fsize GTR 100000000 echo @relpath\""
                };
                process.Start();
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd().Replace(@"\", "/").Replace("./", "/").Replace("\"", "");
                using (var writer = new StreamWriter(System.IO.Path.Combine(Path, ".gitignore")))
                    writer.WriteLine(output);
            }
            // git add -A
            GitCommand.ExecuteThrow($"add -A", new GitException($"falied to 'git add -A'"), Path);
            // git commit -m "message"
            GitCommand.ExecuteThrow($"commit --allow-empty -m \"{message}\"", new GitException($"falied to 'commit --allow-empty -m \"{message}\"'"), Path);
        }


        public void ExecuteThrow(string argument)
        {
            // git add -A
            GitCommand.ExecuteThrow(argument, new GitException($"falied to 'git {argument}'"), Path);
        }


        /// <summary>
        /// ローカルブランチを返す(実際のブランチの生成はしない)
        /// </summary>
        public GitLocalBranch GetBranch(string name)
        {
            return new GitLocalBranch(this, name);
        }

        /// <summary>
        /// ローカルブランチを生成する(実際にブランチを生成)
        /// </summary>
        public GitLocalBranch CreateBranch(string name)
        {
            GitCommand.ExecuteThrow($"branch \"{name}\"", new GitException($"falied to 'git branch \"{name}\"'"), Path);
            return new GitLocalBranch(this, name);
        }

        /// <summary>
        /// リモートリポジトリを紐づける
        /// </summary>
        public GitNamedRemote AddRemote(GitRemote remote, string name)
        {
            GitCommand.ExecuteThrow($"remote add \"{name}\" \"{remote.Expression}\"", new GitException($"failed to add remote repository :{remote.Expression} {Path}"), Path);
            return new GitNamedRemote(this, remote, name);
        }
    }

    public interface IGitLocalBranch { }

    public class GitLocalBranch: IGitLocalBranch
    {
        public GitLocal Local;
        public string Name;
        public bool Exists
        {
            get
            {
                var (code,_) = GitCommand.Execute($"rev-parse --verify {Name}", Local.Path);
                return code == 0;
            }
        }

        public GitLocalBranch(GitLocal local, string name)
        {
            Local = local;
            Name = name;
        }

        public void Checkout()
        {
            GitCommand.ExecuteThrow($"checkout {Name}", new GitException($"falied to 'git checkout {Name}'"), Local.Path);
        }

        /// <summary>
        /// 新しくリモートブランチを作成し追跡する
        /// </summary>
        /// <returns></returns>
        public GitLinkedLocalBranch CreateLinkedBranch(GitNamedRemote remote, string name)
        {
            return new GitLinkedLocalBranch(this, remote.GetBranch(name), false);
        }

        /// <summary>
        /// git branch -D ...
        /// </summary>
        public void Remove()
        {
            GitCommand.ExecuteThrow($"branch -D {Name}", new GitException($"falied to 'branch -D {Name}'"), Local.Path);
        }
    }

    public class GitLinkedLocalBranch: IGitLocalBranch
    {
        public GitLocalBranch LocalBranch;
        public GitRemoteBranch RemoteBranch;
        private bool linked;

        public GitLinkedLocalBranch(GitLocalBranch localBranch, GitRemoteBranch remoteBranch, bool linked = true)
        {
            this.linked = linked;
            LocalBranch = localBranch;
            RemoteBranch = remoteBranch;
        }

        public void Pull()
        {
            // リモートがない場合プルできない
            if (!RemoteBranch.Exists)
                throw new GitException($"remote branch '{RemoteBranch.Expression}' not exists.");

            if (linked)
            {     
                LocalBranch.Checkout();
                // git pull
                GitCommand.ExecuteThrow($"fetch {RemoteBranch.NamedRemote.Name} {RemoteBranch.Name}", new GitException($"falied to 'git fetch { RemoteBranch.Expression }'"), LocalBranch.Local.Path);
                GitCommand.ExecuteThrow("merge", new GitException("falied to 'git merge'"), LocalBranch.Local.Path);
            }
            else
            {
                // ローカルがある場合上書きできない
                if (LocalBranch.Exists)
                    throw new GitException($"local branch '{LocalBranch.Name}' already exists.");

                // git fetch remote_branch
                GitCommand.ExecuteThrow($"fetch {RemoteBranch.NamedRemote.Name} {RemoteBranch.Name}", new GitException($"falied to 'git fetch { RemoteBranch.Expression }'"), LocalBranch.Local.Path);

                // git branch local_branch --t remote_branch
                GitCommand.ExecuteThrow($"branch {LocalBranch.Name} --t {RemoteBranch.Expression}", new GitException($"falied to 'git branch {LocalBranch.Name} --t {RemoteBranch.Expression}'"), LocalBranch.Local.Path);

                LocalBranch.Checkout();
            }
            linked = true;
            RemoteBranch.Exists = true;
        }
        public void CommitPush(string message)
        {
            LocalBranch.Checkout();
            LocalBranch.Local.AddAllAndCommit(message);
            Push();
        }

        public void Push()
        {
            // ローカルがない場合プッシュできない
            if (!LocalBranch.Exists)
                throw new GitException($"local branch '{LocalBranch.Name}' not exists.");

            LocalBranch.Checkout();
            if (linked)
            {
                // git push
                GitCommand.ExecuteThrow($"push", new GitException($"falied to 'git push'"), LocalBranch.Local.Path);
            }
            else
            {
                // リモートがある場合新規作成できない
                if (RemoteBranch.Exists)
                    throw new GitException($"remote branch '{RemoteBranch.Expression}' already exists.");

                // git push -u remote_branch
                GitCommand.ExecuteThrow($"push --set-upstream \"{RemoteBranch.NamedRemote.Name}\" \"{LocalBranch.Name}:{RemoteBranch.Name}\"", new GitException($"falied to 'git push --set-upstream \"{RemoteBranch.NamedRemote.Name}\" \"{LocalBranch.Name}:{RemoteBranch.Name}\"'"), LocalBranch.Local.Path);
            }
            linked = true;
            RemoteBranch.Exists = true;
        }
    }

    public class GitRemote
    {
        public string Account;
        public string Repository;
        public string Expression => $"https://{Account}@github.com/{Account}/{Repository}.git";

        public GitRemote(string account, string repository)
        {
            Account = account;
            Repository = repository;
        }

        public bool Equals(GitRemote value)
        {
            return value != null && Account == value.Account && Repository == value.Repository;
        }
    }

    public class GitNamedRemote
    {
        public GitLocal Local;
        public GitRemote Remote;
        public string Name;

        public GitNamedRemote(GitLocal local, GitRemote remote, string name)
        {
            Local = local;
            Remote = remote;
            Name = name;
        }

        /// <summary>
        /// リポジトリにアクセス可能か
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                var response = new HttpClient().GetAsync(Remote.Expression).Result;
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            } 
        }

        /// <summary>
        /// リモートブランチを返す(実際のブランチの生成はしない)
        /// </summary>
        public GitRemoteBranch GetBranch(string name)
        {
            return new GitRemoteBranch(this, name, false);
        }

        public Dictionary<string, GitRemoteBranch> GetBranchs()
        {
            var output = GitCommand.ExecuteThrow($"ls-remote --heads {Name}", new GitException($"failed to get branch list from {Name}"),Local.Path );
            var branchMap = new Dictionary<string, GitRemoteBranch>();
            if (output.Length == 0)
            {
                return branchMap;
            }
            var lines = output.Trim().Split('\n');
            foreach (var i in lines)
            {
                Console.WriteLine("+++");
                Console.WriteLine(i.Trim().Substring(52));
                Console.WriteLine("+++");
                var branchName = i.Trim().Substring(52);
                branchMap[branchName] = new GitRemoteBranch(this, branchName, true);
            }
            return branchMap;
        }

        /// <summary>
        /// Localとの紐づけを解除する リモートリポジトリは削除しない
        /// </summary>
        public void Remove()
        {
            GitCommand.ExecuteThrow($"remote rm \"{Name}\"", new GitException($"failed to remove remote repository"), Local.Path);
        }
    }

    public class GitRemoteBranch
    {
        public GitNamedRemote NamedRemote;
        public string Name;
        public bool Exists;

        public string Expression => $"\"{NamedRemote.Name}/{Name}\"";

        public GitRemoteBranch(GitNamedRemote namedRemote, string name, bool exists)
        {
            NamedRemote = namedRemote;
            Name = name;
            Exists = exists;
        }

        /// <summary>
        /// このリモートブランチを追跡するローカルブランチを作成
        /// </summary>
        /// <returns></returns>
        public GitLinkedLocalBranch CreateLinkedBranch(string name)
        {
            return new GitLinkedLocalBranch(NamedRemote.Local.GetBranch(name), this,false);
        }

        /// <summary>
        /// リモートブランチを削除
        /// </summary>
        public void Delete()
        {
            GitCommand.ExecuteThrow($"push {NamedRemote.Name} :{Name}", new GitException($"failed to remove remote repository"), NamedRemote.Local.Path);
        }
    }
}

//using log4net;
//using System.Reflection;
//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace Server_GUI2.Develop.Util
//{
//    public class GitException : Exception
//    {
//        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        public GitException(string message) : base(message)
//        {
//            logger.Info(message);
//            Console.WriteLine(App.end_str);
//        }
//    }

//    class GitCommand
//    {
//        public static string ExecuteThrow(string arguments, Exception exception,string directory)
//        {
//            var (code, output) = Execute(arguments, directory);

//            if (code != 0)
//            {
//                Console.WriteLine(output);
//                throw exception;
//            }
//            else
//            {
//                return output;
//            }
//        }

//        public static (int, string) Execute(string arguments, string directory)
//        {
//            var process = new Process()
//            {
//                StartInfo = new ProcessStartInfo("git")
//                {
//                    Arguments = arguments,
//                    UseShellExecute = false,
//                    RedirectStandardOutput = true,
//                    StandardOutputEncoding = Encoding.UTF8,
//                    WorkingDirectory = directory
//                }
//            };
//            Console.WriteLine("git " + (directory == null ? arguments : $"-C \"{directory}\" {arguments}"));
//            process.Start();
//            process.WaitForExit();
//            string output = process.StandardOutput.ReadToEnd();

//            return (process.ExitCode, output);
//        }
//    }

//    public class GitLocalBranch
//    {
//        public readonly GitLocal Local;
//        public string Name { get; private set; }
//        public GitLocalBranch(GitLocal local, string name)
//        {
//            Local = local;
//            Name = name;
//        }

//        public void Rename(string name)
//        {
//            GitCommand.ExecuteThrow($"branch -m {Name} {name}", new GitException($"cannot change branch name. {Name} -> {name}"), Local.Path);
//            Name = name;
//        }

//        public void Remove()
//        {
//            GitCommand.ExecuteThrow($"branch -d {Name}", new GitException($"cannot remove branch. {Name}"), Local.Path);
//        }

//        public void Checkout()
//        {
//            GitCommand.ExecuteThrow($"checkout {Name}", new GitException($"cannot checkout branch {Name}."), Local.Path);
//        }

//        / <summary>
//        / トラック中のリモートブランチにpush
//        / </summary>
//        public void Push()
//        {
//            GitCommand.ExecuteThrow($"push ", new GitException($"cannot push branch {Name}."), Local.Path);
//        }

//        / <summary>
//        / 特定のリモートブランチにpush
//        / </summary>
//        public void Push(GitRemoteBranch remoteBranch)
//        {
//            remoteBranch.Remote.AssertRelated(Local);
//            GitCommand.ExecuteThrow($"push \"{remoteBranch.Remote.Expression}\" \"{Name}:{remoteBranch.Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
//        }

//        / <summary>
//        / 同名のリモートブランチにpush
//        / </summary>
//        public void Push(GitRemote remote)
//        {
//            remote.AssertRelated(Local);
//            GitCommand.ExecuteThrow($"push \"{remote.Expression}\" \"{Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
//        }

//        / <summary>
//        / 特定のリモートブランチにpushしtrack
//        / </summary>
//        public void PushTrack(GitRemoteBranch remoteBranch)
//        {
//            remoteBranch.Remote.AssertRelated(Local);
//            GitCommand.ExecuteThrow($"push -u \"{remoteBranch.Remote.Expression}\" \"{Name}:{remoteBranch.Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
//        }

//        / <summary>
//        / 同名のリモートブランチにpushしtrack
//        / </summary>
//        public void PushTrack(GitRemote remote)
//        {
//            remote.AssertRelated(Local);
//            GitCommand.ExecuteThrow($"push -u \"{remote.Expression}\" \"{Name}\"", new GitException($"cannot push branch {Name}."), Local.Path);
//        }

//        / <summary>
//        / trackしているリモートブランチからPull
//        / </summary>
//        public void Pull()
//        {
//            GitCommand.ExecuteThrow($"pull", new GitException($"cannot pull branch {Name}."), Local.Path);
//        }
//    }
//    public class GitLocal
//    {
//        / <summary>
//        / 与えられたパスがGitRepositoryのトップレベルかどうかを確認する
//        / </summary>
//        public static bool IsGitLocal(string path)
//        {
//            var (code, output) = GitCommand.Execute("rev-parse --show-toplevel", path);
//            return code == 0 && System.IO.Path.GetFullPath(output).Equals(System.IO.Path.GetFullPath(path));
//        }

//        public static GitLocal InitGitLocal(string path)
//        {
//            GitCommand.ExecuteThrow("init", new GitException("cannot initialize local repository."), path);
//            return new GitLocal(path);
//        }

//        public string Path { get; }

//        public GitLocal(string path)
//        {
//            Path = path;
//        }

//        public bool IsGitLocal()
//        {
//            return IsGitLocal(Path);
//        }

//        / <summary>
//        / 現在のローカルブランチを取得する
//        / </summary>
//        public GitLocalBranch GetCurrentBranch()
//        {
//            var output = GitCommand.ExecuteThrow("symbolic-ref --short HEAD", new GitException("cannot get current branch name."), Path);
//            return new GitLocalBranch(this, output.Substring(0, output.Length - 1)); // 末尾の\nを削除
//        }

//        public Dictionary<string, GitLocalBranch> GetBranchs()
//        {
//            var output = GitCommand.ExecuteThrow($"branch", new GitException($"failed to get branch list from {Path}"),Path);
//            var branchMap = new Dictionary<string, GitLocalBranch>();
//            var lines = output.Substring(0, output.Length - 1).Split('\n'); //出力を行に分解  e.g."* master"
//            foreach (var i in lines)
//            {
//                var branchName = i.Substring(2);
//                branchMap[branchName] = new GitLocalBranch(this, branchName);
//            }
//            return branchMap;
//        }

//        / <summary>
//        / ローカルブランチを存在確認なしに返す
//        / </summary>
//        public GitLocalBranch GetBranch(string name)
//        {
//            return new GitLocalBranch(this, name);
//        }

//        public GitLocalBranch CreateBranch(string name)
//        {
//            GitCommand.ExecuteThrow($"branch \"{name}\"", new GitException($"failed to create branch {name}"), Path);
//            return new GitLocalBranch(this, name);
//        }

//        public GitLocalBranch CreateTrackBranch(string name,GitRemoteBranch track)
//        {
//            GitCommand.ExecuteThrow($"branch \"{name}\" --track \"{track.Remote.Expression}/{track.Name}\"", new GitException($"failed to create branch {name}"), Path);
//            return new GitLocalBranch(this, name);
//        }

//        public void AddAll()
//        { 
//            GitCommand.ExecuteThrow($"add -A", new GitException($"failed to add files"), Path);
//        }

//        public void Commit(string message)
//        {
//            GitCommand.ExecuteThrow($"commit --allow-empty -m \"{message}\"", new GitException($"failed to commit"), Path);
//        }

//        public GitNamedRemote AddRemote(GitRemote remote,string name)
//        {
//            GitCommand.ExecuteThrow($"remote add \"{name}\" \"{remote.Expression}\"", new GitException($"failed to add remote repository :{remote.Expression} {Path}"), Path);
//            return new GitNamedRemote(this,remote,name);
//        }

//        public GitLocalBranch Init(string branchName)
//        {
//            GitCommand.ExecuteThrow($"init", new GitException($"failed to init {Path}"), Path);
//            GitCommand.ExecuteThrow($"branch -m \"{branchName}\"", new GitException($"failed to change branch name to {branchName}"), Path);
//            return GetBranch(branchName);
//        }
//        public void Fetch(GitRemoteBranch remoteBranch)
//        {
//            GitCommand.ExecuteThrow($"fetch \"{remoteBranch.Remote.Expression}\" \"{remoteBranch.Name}\"", new GitException($"failed to fetch {remoteBranch.Remote.Expression}/{remoteBranch.Name} to {Path}"), Path);
//        }
//        public void Merge()
//        {
//            GitCommand.ExecuteThrow($"merge", new GitException($"failed to merge path:{Path}"), Path);
//        }

//        public void Push()
//        {
//            GitCommand.ExecuteThrow($"push", new GitException($"failed to push path:{Path}"), Path);
//        }

//        public List<GitNamedRemote> GetRemotes()
//        {
//            var remotes = new List<GitNamedRemote>();
//            var output = GitCommand.ExecuteThrow($"remote -v", new GitException($"failed to get remotelist of {Path}"), Path);

//            GitNamedRemote GetNamedRemote(string str)
//            {
//                var strs = str.Split();
//                var urls = strs[1].Split('/').ToList();
//                var accountName = urls[urls.Count - 2];
//                var repositoryName = urls[urls.Count - 1].Substring(0,urls[urls.Count - 1].Length - 4);
//                return new GitNamedRemote(this, new GitRemote(accountName, repositoryName),strs[0]);
//            }

//            var remoteData = output.Substring(0, output.Length - 1).Split('\n').Where( ( _,i ) => i % 2 == 0 );

//            return remoteData.Select(name => GetNamedRemote(name)).ToList();
//        }
//    }

//    public class GitRemoteBranch
//    {
//        public readonly IGitRemote Remote;
//        public readonly string Name;
//        public GitRemoteBranch(IGitRemote remote, string name)
//        {
//            Remote = remote;
//            Name = name;
//        }
//    }

//    public interface IGitRemote
//    {
//        string Expression { get; }
//        bool IsMyBranch(GitRemoteBranch branch);
//        void AssertRelated(GitLocal local);

//        GitRemoteBranch CreateBranch(string name);
//    }

//    / <summary>
//    / ローカルリポジトリに紐づいたリモートリポジトリエイリアス( origin的な奴 )
//    / </summary>
//    public class GitNamedRemote: IGitRemote
//    {
//        public readonly GitLocal Local;
//        public readonly GitRemote Remote;
//        public readonly string Name;
//        public GitNamedRemote(GitLocal local, GitRemote remote, string name)
//        {
//            Local = local;
//            Remote = remote;
//            Name = name;
//        }

//        public string Expression
//        {
//            get
//            {
//                return Name;
//            }
//        }

//        / <summary>
//        / 自分が引数のローカルリポジトリのエイリアスかどうかを確認し違ったらエラー
//        / </summary>
//        public void AssertRelated(GitLocal local)
//        {
//            if (! IsRelated(local))
//            {
//                throw new GitException($"remote repository {Name} is not related to local repository {local.Path}.");
//            }
//        }

//        public GitRemoteBranch CreateBranch(string name)
//        {
//            return new GitRemoteBranch(this, name);
//        }

//        public bool IsMyBranch(GitRemoteBranch branch)
//        {
//            return branch.Remote == this;
//        }

//        public bool IsRelated(GitLocal local)
//        {
//            return Local == local;
//        }

//        / <summary>
//        / Localとの紐づけを解除する リモートリポジトリは削除しない
//        / </summary>
//        public void Remove()
//        {
//            GitCommand.ExecuteThrow($"remote rm \"{Name}\"", new GitException($"failed to remove remote repository"), Local.Path);
//        }
//    }
//    public class GitRemote : IGitRemote
//    {
//        public string Account { get; private set; }
//        public string RepoName { get; private set; }


//        public string Expression
//        {
//            get
//            {
//                return $"https://{Account}@github.com/{Account}/{RepoName}.git";
//            }
//        }

//        public GitRemote(string account, string repoName)
//        {
//            Account = account;
//            RepoName = repoName;
//        }

//        public GitRemoteBranch CreateBranch(string name)
//        {
//            return new GitRemoteBranch(this, name);
//        }

//        public Dictionary<string, GitRemoteBranch> GetBranchs()
//        {
//            var output = GitCommand.ExecuteThrow($"ls-remote --heads {Expression}", new GitException($"failed to get branch list from {Expression}"),null);
//            var branchMap = new Dictionary<string, GitRemoteBranch>();
//            if (output.Length == 0)
//            {
//                return branchMap;
//            }
//            var lines = output.Substring(0, output.Length - 1).Split('\n');
//            foreach (var i in lines)
//            {
//                var branchName = i.Substring(52);
//                branchMap[branchName] = new GitRemoteBranch(this, branchName);
//            }
//            return branchMap;
//        }

//        public bool IsMyBranch(GitRemoteBranch branch)
//        {
//            return branch.Remote == this;
//        }
//        public void AssertRelated(GitLocal local)
//        {}
//    }
//}
