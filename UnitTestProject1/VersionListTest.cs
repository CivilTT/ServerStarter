using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Windows.Data;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class VersionListTest
    {
        [TestMethod]
        public void GetVersionList()
        {
            var a = VersionFactory.Instance.Versions;

            //CollectionViewSource view = new CollectionViewSource()
            //{
            //    Source = versionFactory.Versions
            //};
            //view.Filter += new FilterEventHandler(test);
            //view.SortDescriptions.Add(new SortDescription("Salary", ListSortDirection.Descending));

            //var list = versionFactory.Versions.Select(x => x is VanillaVersion);

            foreach (var item in a)
            {
                Console.WriteLine(item.Name);
            }

        }

        private void test(object sender, FilterEventArgs e)
        {
            Server_GUI2.Version version = e.Item as Server_GUI2.Version;

            e.Accepted = version is VanillaVersion;
        }
    }
}
