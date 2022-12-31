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
            var versionFactory = VersionFactory.Instance;

            CollectionViewSource view = new CollectionViewSource()
            {
                Source = versionFactory.Versions
            };
            FilterEventHandler releaseFilter = new FilterEventHandler(filter);
            view.Filter += releaseFilter;

            ListCollectionView _view = (ListCollectionView)view.View;

            _view.MoveCurrentToFirst();
            do
            {
                if (_view.CurrentItem is Server_GUI2.Version ver)
                {
                    Console.Write($"{ver.Name}, ");
                }

            } while (_view.MoveCurrentToNext());

            Console.WriteLine("----");

            view.Filter -= releaseFilter;
            view.Filter += new FilterEventHandler(filter2);

            _view = (ListCollectionView)view.View;

            _view.MoveCurrentToFirst();
            do
            {
                if (_view.CurrentItem is Server_GUI2.Version ver)
                {
                    Console.WriteLine($"{ver.Name}, ");
                }

            } while (_view.MoveCurrentToNext());
        }

        private void filter(object sender, FilterEventArgs e)
        {
            Server_GUI2.Version version = e.Item as Server_GUI2.Version;

            e.Accepted = version is VanillaVersion vanilla && vanilla.IsRelease;
        }

        private void filter2(object sender, FilterEventArgs e)
        {
            Server_GUI2.Version version = e.Item as Server_GUI2.Version;

            e.Accepted = version is SpigotVersion;
        }
    }
}
