using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Server_GUI2.Develop.Server.World;
using Server_GUI2;
using System.Net.NetworkInformation;

namespace UnitTestProject1
{
    [TestClass]
    public class LinkTest
    {
        [TestMethod]
        public void LinkTestMethod()
        {
            //foreach (var i in WorldFactory.Instance.Worlds)
            //{
            //    Console.WriteLine(i.DisplayName);
            //}
            //WorldLink.Instance.SaveJson();
        }

        [TestMethod]
        public void ShowInterfaceSpeedAndQueue()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                IPv4InterfaceStatistics stats = adapter.GetIPv4Statistics();
                Console.WriteLine(adapter.Description);
                Console.WriteLine("     Speed .................................: {0} [bps]",
                    adapter.Speed);
                Console.WriteLine("     Output queue length....................: {0}",
                    stats.OutputQueueLength);
            }
        }
    }
}