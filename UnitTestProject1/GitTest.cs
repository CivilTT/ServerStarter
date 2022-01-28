using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2;
using System;
using System.IO;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Server.Storage;
using Server_GUI2.Develop.Util;

namespace UnitTestProject1
{
    [TestClass]
    public class GitTest
    {
        [TestMethod]
        public void GitTestMethod()
        {
            var path = Environment.GetEnvironmentVariable("SERVER_STERTER_TEST");

            var gitpath = Path.Combine(path, "git_state");

            var local = new GitLocal(gitpath);
            var remote = new GitRemote("txkodo", "GitTest");
            var reader = GitStorageRepository.AddRepository(local, remote);

            var remote2 = new GitRemote("txkodo", "GitTest2");

            var reader2 = GitStorageRepository.AddRepository(local, remote2);
        }
    }
}
