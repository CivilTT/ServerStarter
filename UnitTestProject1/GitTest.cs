using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2;
using System;
using System.IO;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Util;
using System.Diagnostics;

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

        [TestMethod]
        public void ConstructGitignore()
        {
            using (var process = new Process())
            {
                var path = "";
                process.StartInfo = new ProcessStartInfo("forfiles")
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    WorkingDirectory = path,
                    Arguments = $"/s /c \"cmd /q /c if @fsize GTR 100000000 echo @relpath\">{Path.Combine(path,".gitignore")}"
                };
                process.Start();
                process.WaitForExit();
                process.ExitCode.WriteLine();
                process.StandardError.ReadToEnd().WriteLine();
            }
        }
    }
}
