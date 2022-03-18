using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
        static readonly WebClient wc = new WebClient();
        private static string _localIP;
        private static string _globalIP;
        public static string LocalIP
        {
            get
            {
                if (_localIP == null)
                {
                    string localIP = string.Empty;
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("8.8.8.8", 65530);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint.Address.ToString();
                    }
                    _localIP = localIP;
                }
                return _localIP;
            }
        }
        public static string GlobalIP
        {
            get
            {
                if (_globalIP == null)
                {
                    _globalIP = wc.DownloadString("https://ipv4.icanhazip.com/").Replace("\\r\\n", "").Replace("\\n", "").Trim();
                }
                return _globalIP;
            }
        }

        private static bool? accessible = null;
        private static DateTime? lastGetTime = null;

        // インターネットに接続可能かを再取得するための時間差(秒)
        private static readonly int reCheckTimeSpan = 60;

        public static bool Accessible
        {
            get
            {
                var now = DateTime.Now;
                if (!accessible.HasValue || (now - lastGetTime.Value).TotalSeconds > reCheckTimeSpan)
                {
                    lastGetTime = now;
                    CheckAccess();
                }
                return accessible.Value;
            }
        }

        private static void CheckAccess()
        {
            //インターネットに接続されているか確認する
            string host = "http://www.yahoo.com";

            HttpWebRequest webreq = null;
            HttpWebResponse webres = null;
            try
            {
                //HttpWebRequestの作成
                webreq = (HttpWebRequest)System.Net.WebRequest.Create(host);
                //メソッドをHEADにする
                webreq.Method = "HEAD";
                //受信する
                webres = (HttpWebResponse)webreq.GetResponse();
                //応答ステータスコードを表示
                Console.WriteLine(webres.StatusCode);
                accessible = true;
            }
            catch
            {
                accessible = false;
            }
            finally
            {
                if (webres != null)
                    webres.Close();
            }
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
