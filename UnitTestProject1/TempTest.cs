using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class TempTest
    {
        [TestMethod]
        public void TempTestMethod()
        {
            FileSystem.CreateDirectory("testdirectory");
            FileSystem.CreateDirectory(@"testdirectory\testname");
            new FileInfo(@"testdirectory\testname\test.txt").Create().Close();
            var dir = new DirectoryInfo($@"testdirectory\testname");
            dir.MoveTo($@"TEMP");
            FileSystem.DeleteDirectory("testdirectory", DeleteDirectoryOption.DeleteAllContents);
            var tmp = new DirectoryInfo($@"TEMP");
            tmp.MoveTo($@"testdirectory");
        }
    }
}
