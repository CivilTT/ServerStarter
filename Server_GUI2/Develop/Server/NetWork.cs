using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Develop.Server
{
    class NetWorkException : Exception
    {
        public NetWorkException(string message = "network not accessible") : base(message) { }
    }

    public static class NetWork
    {
        private static bool? accessible = null;
        public static bool Accessible
        {
            get
            {
                if (!accessible.HasValue)
                    CheckAccess();

                return accessible.Value;
            }
        }

        private static void CheckAccess()
        {
            accessible = NetworkInterface.GetIsNetworkAvailable();
            //if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            //{
            //    string message =
            //        "本システムはインターネット環境下のみで動作します。\n" +
            //        "インターネットに接続したうえで、再度起動してください。";
            //    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}
        }
    }
}
