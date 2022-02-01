using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Server_GUI2.Develop.Server.Storage;

namespace UnitTestProject1
{
    [TestClass]
    public class StorageTest
    {
        [TestMethod]
        public void StorageTestMethod()
        {
            Console.WriteLine(StorageFactory.Instance.Storages);

            foreach (var i in GitStorage.GetStorages())
            {
            }
        }
    }
}
