using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Server_GUI2.Develop.Server.World;
using Server_GUI2;
using System.Diagnostics;
using System.Text;

namespace UnitTestProject1
{
    [TestClass]
    public class ProcessTest
    {
        [TestMethod]
        public void ProcessTestMethod()
        {
            var arguments = "help";

            var StartInfo = new ProcessStartInfo("kusa", arguments)
            {
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
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

                Console.WriteLine(stdout.ToString());
                Console.WriteLine(stderr.ToString());
                Console.WriteLine(process.ExitCode);
            }
        }
    }
}
