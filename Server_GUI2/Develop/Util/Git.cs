using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    class GitLocal
    {
        public static GitLocal Clone(GitBranch branch, string path)
        {
            return new GitLocal(branch, path);
        }

        public string Path { get; private set;}
        public GitBranch Branch { get; private set;}

        public GitLocal(GitBranch branch, string path)
        {
            Path = path;
            Branch = branch;
        }

        public void CheckOut(GitBranch branch)
        {
            Branch = branch;
        }

        public void push() { }
        public void pull() { }
    }

    class GitRepository
    {
        public GitBranch GetBranch() { }
    }

    class GitBranch
    {
        public GitRepository Repository { get; private set; }
        public GitBranch(GitRepository repository)
        {
            Repository = repository;
        }
    }
}
