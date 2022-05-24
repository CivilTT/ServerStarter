using log4net;
using Open.Nat;
using Server_GUI2.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    public class PortMapping
    {
        static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int LocalPort => 25565;

        public static async Task<bool> AddPort(int portNum)
        {
            logger.Info($"Start opening port {portNum}");

            NatDiscoverer discoverer = new NatDiscoverer();
            var searchDevice = discoverer.DiscoverDeviceAsync();
            var deviceResult = await Task.WhenAny(searchDevice, Task.Delay(5000));
            NatDevice device = searchDevice.Result;
            if (deviceResult != searchDevice || device == null)
            {
                logger.Info("Failed to searchDevice");
                return false;
            }

            Mapping mapping = new Mapping(Protocol.Tcp, LocalPort, portNum, "Server Starter");

            // WhenAnyは引数のTaskのうち、１つでも終了したらawaitを抜ける
            // Delayは非同期処理によって処理を止める（GUIのフリーズ防止）
            // この二つを組み合わせ、目的TaskのTimeout処理（目的Taskが終わる or timeoutまで待つ）を実装
            // 開放しようとするポートがUpnp以外で開放されていた場合、無限に追加しようとしてしまう状態を回避
            logger.Info("Start searching mappings");
            await Task.WhenAny(device.CreatePortMapAsync(mapping), Task.Delay(5000));
            var getMappings = device.GetAllMappingsAsync();
            var mappingsResult = await Task.WhenAny(getMappings, Task.Delay(5000));

            if (mappingsResult != getMappings)
            {
                logger.Info("Failed to search mappings");
                return false;
            }

            foreach(var mapping1 in await getMappings)
            {
                //Console.WriteLine(mapping1.PublicPort);
                if (mapping1.PublicPort == portNum)
                {
                    logger.Info($"Success to port mapping! ({portNum})");
                    return true;
                }
            }

            logger.Info($"Failed to open port {portNum}");
            return false;
        }

        public static async Task<bool> DeletePort(int portNum)
        {
            logger.Info($"Start closing port {portNum}");

            NatDiscoverer discoverer = new NatDiscoverer();
            var searchDevice = discoverer.DiscoverDeviceAsync();
            var deviceResult = await Task.WhenAny(searchDevice, Task.Delay(5000));
            if (deviceResult != searchDevice)
            {
                logger.Info("Failed to searchDevice");
                return false;
            }

            NatDevice device = await searchDevice;

            Mapping mapping = new Mapping(Protocol.Tcp, LocalPort, portNum, "Server Starter");

            logger.Info("Start searching mappings");
            await device.DeletePortMapAsync(mapping);
            var getMappings = device.GetAllMappingsAsync();
            var mappingsResult = await Task.WhenAny(getMappings, Task.Delay(5000));

            if (mappingsResult != getMappings)
            {
                logger.Info("Failed to search mappings");
                return false;
            }

            //Console.WriteLine("The external IP Address is: {0} ", await device.GetExternalIPAsync());
            foreach (var mapping1 in await getMappings)
            {
                //Console.WriteLine(mapping1.PublicPort);
                if (mapping1.PublicPort == portNum)
                {
                    logger.Info($"Success to port mapping! ({portNum})");
                    return true;
                }
            }

            logger.Info($"Failed to close port {portNum}");
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
