using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Nat;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class UPnPTest
    {
        readonly UPnP uPnP = new UPnP();

        [TestMethod]
        public void TestMethod1()
        {
            //uPnP.AddPort();
            uPnP.DeletePort();

            // Port開放が終了するまでタイムラグがあるため、待っている
            // 実際に本体に実装するときには不要
            System.Threading.Thread.Sleep(5000);
            //uPnP.CheckPort();
        }
    }

    class UPnP
    {
        public void AddPort()
        {
            NatUtility.DeviceFound += Add_Port;
            NatUtility.StartDiscovery();
        }

        public void DeletePort()
        {
            NatUtility.DeviceFound += Delete_Port;
            NatUtility.StartDiscovery();
        }

        private void Add_Port(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;

            device.CreatePortMap(new Mapping(Protocol.Tcp, 2869, 25575));


            foreach (Mapping portMap in device.GetAllMappings())
            {
                Console.WriteLine(portMap.ToString());
            }

            NatUtility.StopDiscovery();
        }

        private void Delete_Port(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;

            device.DeletePortMap(new Mapping(Protocol.Tcp, 2869, 25575));

            NatUtility.StopDiscovery();
        }
    }
}
