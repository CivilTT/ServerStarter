using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2.Develop.Util;
using System.Collections.Generic;
using Open.Nat;
using System;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UPnPTest
    {

        [TestMethod]
        public async Task TestMethod1()
        {
            List<Mapping> mappings = await PortMapping.GetPorts();
            //uPnP.AddPort();
            //uPnP.DeletePort();

            //uPnP.CheckPort();
            Console.WriteLine("TEST");
            foreach (Mapping mapping1 in mappings)
            {
                Console.WriteLine(mapping1.ToString());
            }
        }
    }

    //class UPnP
    //{
    //    public void AddPort()
    //    {
    //        NatUtility.DeviceFound += Add_Port;
    //        NatUtility.StartDiscovery();
    //    }

    //    public void DeletePort()
    //    {
    //        NatUtility.DeviceFound += Delete_Port;
    //        NatUtility.StartDiscovery();
    //    }

    //    private void Add_Port(object sender, DeviceEventArgs args)
    //    {
    //        INatDevice device = args.Device;

    //        device.CreatePortMap(new Mapping(Protocol.Tcp, 2869, 25575));


    //        foreach (Mapping portMap in device.GetAllMappings())
    //        {
    //            Console.WriteLine(portMap.ToString());
    //        }

    //        NatUtility.StopDiscovery();
    //    }

    //    private void Delete_Port(object sender, DeviceEventArgs args)
    //    {
    //        INatDevice device = args.Device;

    //        device.DeletePortMap(new Mapping(Protocol.Tcp, 2869, 25575));

    //        NatUtility.StopDiscovery();
    //    }
    //}
}
