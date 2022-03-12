using Open.Nat;
using Server_GUI2.Windows.SystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW = ModernWpf;
using System.Windows;
using System.ComponentModel;
using System.Threading;
using Server_GUI2.Windows;

namespace Server_GUI2.Develop.Util
{
    public class PortMapping
    {
        public static async Task<bool> AddPort(int portNum)
        {
            NatDiscoverer discoverer = new NatDiscoverer();
            NatDevice device = await discoverer.DiscoverDeviceAsync();

            Mapping mapping = new Mapping(Protocol.Tcp, 2869, portNum, "Server Starter");

            // WhenAnyは引数のTaskのうち、１つでも終了したらawaitを抜ける
            // Delayは非同期処理によって処理を止める（GUIのフリーズ防止）
            // この二つを組み合わせ、目的TaskのTimeout処理（目的Taskが終わる or timeoutまで待つ）を実装
            // 開放しようとするポートがUpnp以外で開放されていた場合、無限に追加しようとしてしまう状態を回避
            await Task.WhenAny(device.CreatePortMapAsync(mapping), Task.Delay(5000));

            foreach(var mapping1 in await device.GetAllMappingsAsync())
            {
                //Console.WriteLine(mapping1.PublicPort);
                if (mapping1.PublicPort == portNum)
                    return true;
            }

            return false;
        }

        public static async Task<bool> DeletePort(int portNum)
        {
            NatDiscoverer discoverer = new NatDiscoverer();
            NatDevice device = await discoverer.DiscoverDeviceAsync();

            Mapping mapping = new Mapping(Protocol.Tcp, 2869, portNum, "Server Starter");

            await device.DeletePortMapAsync(mapping);

            //Console.WriteLine("The external IP Address is: {0} ", await device.GetExternalIPAsync());
            foreach (var mapping1 in await device.GetAllMappingsAsync())
            {
                //Console.WriteLine(mapping1.PublicPort);
                if (mapping1.PublicPort == portNum)
                    return false;
            }

            return true;
        }

        public static async Task<List<Mapping>> GetPorts()
        {
            NatDiscoverer discoverer = new NatDiscoverer();
            NatDevice device = await discoverer.DiscoverDeviceAsync();
            List<Mapping> mappings = new List<Mapping>(await device.GetAllMappingsAsync());

            return mappings;
        }
    }

    public class PortStatus : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum Status
        {
            Open,
            Close,
            Registering,
            Failed,
            Ready
        }

        public BindingValue<Status> StatusEnum { get; private set; }

        public string DisplayStatus
        {
            get
            {
                string display;
                switch (StatusEnum.Value)
                {
                    case Status.Open:
                        display = "Opened !";
                        break;
                    case Status.Close:
                        display = "Closed";
                        break;
                    case Status.Registering:
                        display = "Registering ...";
                        break;
                    case Status.Failed:
                        display = "Failed";
                        break;
                    case Status.Ready:
                        display = "Ready ...";
                        break;
                    default:
                        display = "";
                        break;
                }

                return display;
            }
        }

        public int PortNumber { get; }

        public PortStatus(int portNumber, Status status)
        {
            PortNumber = portNumber;
            StatusEnum = new BindingValue<Status>(status, () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayStatus")));
        }
    }
}
