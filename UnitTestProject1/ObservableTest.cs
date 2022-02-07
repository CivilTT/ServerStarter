using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class ObservableTest
    {
        [TestMethod]
        public void ObservableTest1()
        {
            var a = new ObservableCollection<int>();
            a.Add(1);
            a.Add(2);
            a.Add(3);
            a.Add(4);

            var b = a.Where(x => x % 2 == 0).Reverse();

            Console.WriteLine("first");
            foreach (var i in b)
                Console.WriteLine(i);

            a.Add(5);
            a.Add(6);

            Console.WriteLine("second");
            foreach (var i in b)
                Console.WriteLine(i);
        }
    }
}
