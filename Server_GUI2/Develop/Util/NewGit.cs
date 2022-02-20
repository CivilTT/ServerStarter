using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Util
{
    class GitLocal
    {
        public string Path;
        public GitLocal(string path)
        {
            Path = path;
        }

        public bool IsGitRepository()
        {

        }


        /// <summary>
        /// ローカルブランチを返す(実際のブランチの生成はしない)
        /// </summary>
        public GitLocalBranch GetBranch(string name)
        {
            return new GitLocalBranch(this, name);
        }
    }

    class GitLocalBranch
    {
        public GitLocal Local;
        public string Name;
        public GitLocalBranch(GitLocal local,string name)
        {
            Name = name;
        }

        public void Checkout() { }

        /// <summary>
        /// 新しくリモートブランチを作成し追跡する
        /// </summary>
        /// <returns></returns>
        public GitLinkedLocalBranch CreateLinkedBranch(GitNamedRemote remote,string name)
        {
            return new GitLinkedLocalBranch(this,remote.GetBranch(name),false);
        }
    }

    class GitLinkedLocalBranch
    {
        public GitLocalBranch LocalBranch;
        public GitRemoteBranch RemoteBranch;
        private bool linked;

        public GitLinkedLocalBranch(GitLocalBranch localBranch, GitRemoteBranch remoteBranch,bool linked = true)
        {
            this.linked = linked;
            LocalBranch = localBranch;
            RemoteBranch = remoteBranch;
        }

        public void Pull() { }
        public void Push() { }
    }

    class GitAccount
    {
        public string Name;
        public GitAccount(string name)
        {
            Name = name;
        }
    }

    class GitRemote
    {
        public GitAccount Account;
        public string Repository;
        public GitRemote(GitAccount account,string repository)
        {
            Account = account;
            Repository = repository;
        }
    }

    class GitNamedRemote
    {
        public GitLocal Local;

        /// <summary>
        /// リモートブランチを返す(実際のブランチの生成はしない)
        /// </summary>
        public GitRemoteBranch GetBranch(string name)
        {
            return new GitRemoteBranch(this, name);
        }
    }

    class GitRemoteBranch
    {
        public GitNamedRemote NamedRemote;
        public string Name;

        public GitRemoteBranch(GitNamedRemote namedRemote,string name)
        {
            NamedRemote = namedRemote;
            Name = name;
        }

        /// <summary>
        /// このリモートブランチを追跡するローカルブランチを作成
        /// </summary>
        /// <returns></returns>
        public GitLinkedLocalBranch CreateLinkedBranch(string name)
        {
            return new GitLinkedLocalBranch(this.NamedRemote.Local.GetBranch(name),this);
        }
    }
}
