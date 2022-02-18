using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2;
using System;
using System.IO;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Util;

namespace UnitTestProject1
{
    [TestClass]
    public class GitTest
    {
        [TestMethod]
        public void GitTestMethod()
        {
            var path = Environment.GetEnvironmentVariable("SERVER_STERTER_TEST");

            var gitpath = Path.Combine(path, "gittest");

            // ディレクトリを空にする
            Directory.Delete(gitpath, true);
            Directory.CreateDirectory(gitpath);

            var local = new GitLocal(gitpath);
            local.Init();
            var remote = new GitRemote("txkodo", "GitTest");
            var namedRemote = local.AddRemote(remote, "GITTEST");

            foreach ( var i in local.GetNamedRemotes())
            {
                Console.WriteLine(i.Key);
                Console.WriteLine(i.Value);
            }
        }
    }
}
