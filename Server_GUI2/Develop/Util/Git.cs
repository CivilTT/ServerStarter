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
//        public static string ExecuteThrow(string path, string arguments, Exception exception)
//        {
//            var (code, output) = Execute(path, arguments);

//            if (code != 0)
//            {
//                Console.WriteLine(code);
//                Console.WriteLine(output);
//                throw exception;
//            }
//            else
//            {
//                return output;
//            }
//        }

//        public static (int, string) Execute(string path, string arguments)
//        {
//            Console.WriteLine($"-C \"{path}\" " + arguments);
//            var process = new Process()
//            {
//                StartInfo = new ProcessStartInfo("git")
//                {
//                    Arguments = $"-C \"{path}\" " + arguments,
//                    UseShellExecute = false,
//                    RedirectStandardOutput = true,
//                    StandardOutputEncoding = Encoding.UTF8
//                }
//            };
//            process.Start();
//            process.WaitForExit();
//            string output = process.StandardOutput.ReadToEnd();

//            return (process.ExitCode, output);
//        }
//    }


//    public class GitLocal
//    {
//        /// <summary>
//        /// 与えられたパスがGitRepositoryのトップレベルかどうかを確認する
//        /// </summary>
//        public bool IsGitRepository()
//        {
//            var (code, output) = GitCommand.Execute(path, "rev-parse --show-toplevel");
//            return code == 0 && Path.GetFullPath(output).Equals(Path.GetFullPath(path));
//        }

//        /// <summary>
//        /// 現在のローカルブランチ名を取得する
//        /// </summary>
//        public string GetCurrentBranch()
//        {
//            var output = GitCommand.ExecuteThrow(path, "symbolic-ref --short HEAD", new GitException("cannot get current branch name."));
//            return output.Substring(0, output.Length - 1); // 末尾の\nを削除
//        }

//        private string path;
//        private GitRemoteBranch branch;

//        public GitLocal(string path)
//        {
//            this.path = path;
//        }

//        public void Init()
//        {
//            GitCommand.ExecuteThrow(path, $"init", new GitException($"failed to init {path}"));
//        }

//        public void CommitPush(string commitMessage)
//        {
//            CommitAll(commitMessage);
//            Push();
//        }

//        public void Push(string localBranch, string remoteBranch)
//        {
//            GitCommand.ExecuteThrow(path, $"push {branch.Repository.Url} \"{branch.LocalBranchName}:{branch.Name}\"", new GitException($"failed to add branch {branch}"));
//        }

//        public void CommitAll(string message)
//        {
//            GitCommand.ExecuteThrow(path, $"add -A", new GitException($"failed to add {path}"));
//            GitCommand.ExecuteThrow(path, $"commit  -m \"{message}\"", new GitException($"failed to commit {path}"));
//        }

//        public void Pull()
//        {
//            GitCommand.ExecuteThrow(path, $"pull {branch.Repository.Url} \"{branch.Name}:{branch.LocalBranchName}\"", new GitException($"failed to get branch pull from {branch}"));
//        }
//    }

//    public class GitRemoteRepository
//    {
//        public string Account { get; private set; }
//        public string RepoName { get; private set; }
//        public Dictionary<string, GitRemoteBranch> ExistsBranchMap { get; private set; }
//        public string Url
//        {
//            get
//            {
//                return $"https://{Account}@github.com/{Account}/{RepoName}.git";
//            }
//        }

//        public GitRemoteRepository(string account, string repoName)
//        {
//            Account = account;
//            RepoName = repoName;
//            GetExistsBranchs();
//        }

//        private void GetExistsBranchs()
//        {
//            var output = GitCommand.ExecuteThrow("/", $"ls-remote --heads {Url}", new GitException($"failed to get branch list from {Url}"));

//            var lines = output.Substring(0, output.Length - 1).Split('\n');
//            foreach (var i in lines)
//            {
//                if (i.Length == 0)
//                {
//                    continue;
//                }
//                var branchName = i.Substring(52);
//                ExistsBranchMap[branchName] = new GitRemoteBranch(this, branchName);
//            }
//        }

//        public GitRemoteBranch NewBranch(string branchName)
//        {

//        }
//    }

//    public class GitRemoteBranch
//    {
//        public GitRemoteRepository Repository { get; private set; }
//        public string Name { get; private set; }

//        public string LocalBranchName
//        {
//            get
//            {
//                return $"{this.Repository.Account}.{this.Repository.RepoName}.{this.Name}";
//            }
//        }

//        public GitRemoteBranch(GitRemoteRepository repository, string name)
//        {
//            Repository = repository;
//            Name = name;
//        }
//        public override string ToString()
//        {
//            return $"<GitRemoteBranch {Repository.Url} {Name} >";
//        }
//    }
//}
