//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Server_GUI2.Develop.Util;

//namespace UnitTestProject1
//{
//    [TestClass]
//    public class GitTest
//    {
//        [TestMethod]
//        public void TestMethod1()
//        {
//            var repo = new GitRemoteRepository("txkodo", "GitTest");
//            var branchs = repo.ExistsBranch();

//            var branch = branchs[1];

//            var local = new GitLocal(@"C:\Users\shiyu\OneDrive - 共生情報研究室\趣味\Git\ServerSterterTest", branch);

//            local.InitIfIsNotRepository();

//            local.Pull();
//        }
//    }
//}
