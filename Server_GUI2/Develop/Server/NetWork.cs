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
        private static DateTime? lastGetTime = null;

        // インターネットに接続可能かを再取得ための時間差(秒)
        private static readonly int reCheckTimeSpan = 60;

        public static bool Accessible
        {
            get
            {
                var now = DateTime.Now;
                if (lastGetTime.HasValue && (now - lastGetTime.Value).TotalSeconds > reCheckTimeSpan)
                if (!accessible.HasValue)
                    CheckAccess();
                lastGetTime = now;
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
