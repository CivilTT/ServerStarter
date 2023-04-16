using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Server_GUI2.Util;

namespace UnitTestProject1
{
    [TestClass]
    public class RegexTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pattern = @"jdk-(?<ver>[0-9]+)(?:\.[0-9]+)*";

            var tests = new string[] {
                "jdk-17",
                "jdk-17.0",
                "jdk-17.0.0"
            };

            foreach (var test in tests)
            {
                var match = Regex.Match(test, pattern);
                match.Groups["ver"].Value.WriteLine();
            }
        }
    }
}
