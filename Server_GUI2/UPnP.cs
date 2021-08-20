using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

using NATUPNPLib;

using log4net;
using System.Net.NetworkInformation;

namespace Server_GUI2
{
    public partial class UPnP
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Functions func = new Functions();

        UPnPNATClass upnpnat = new UPnPNATClass();
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();



        
        public UPnP()
        {
            logger.Info("Start the check of NAT (Port mapping)");
        }

        public bool Check_port(int port)
        {
            // TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint tcpi in tcpConnInfoArray)
            {
                Console.WriteLine(tcpi.Port);
                if (tcpi.Port == port)
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool Regist_port(int port = 25565)
        {
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            string local_ip = Get_ip();
            Console.WriteLine(local_ip);
            Console.WriteLine(port);
            mappings.Add(25565, "TCP", 5358, "192.168.3.7", true, "Server Starter");


            return true;
        }

        private string Get_ip()
        {
            string ip_address = "";
            
            // ホスト名を取得する
            string hostname = Dns.GetHostName();

            // ホスト名からIPアドレスを取得する
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);
            foreach (IPAddress address in adrList)
            {
                ip_address = address.ToString();
                Console.WriteLine(address.ToString());
            }

            return ip_address;
        }



    }
}
