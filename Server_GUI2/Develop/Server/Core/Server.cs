using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2
{
    static class Server
    {
        /// <summary>
        /// カレントディレクトリのパス
        /// </summary>
        private static string Path;
        
        private static string JarName;
        private static string Log4jArgument;

        /// <summary>
        /// server.jarを実際に起動する
        /// </summary>
        public static void Start(string path, string jarName, string log4jArgument)
        {
            Path = path;
            JarName = jarName;
            Log4jArgument = log4jArgument;

            CheckEula();

            Run();
        }

        private static void Run()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo("java")
                {
                    Arguments = $"java -Xmx5G -Xms5G{Log4jArgument} -jar {JarName} nogui",
                    WorkingDirectory = Path,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private static void AgreeEula(List<string> eulaContents)
        {
            if (eulaContents[eulaContents.Count - 1] == "eula=true")
                return;

            string html = eulaContents[0].Substring(eulaContents[0].IndexOf("(") + 1);
            html = html.Replace(").", "");
            
            var result = MW.MessageBox.Show(
                $"以下のURLに示されているサーバー利用に関する注意事項に同意しますか？\n\n" +
                $"【EULAのURL】\n{html}", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (result == MessageBoxResult.Cancel)
            {
                UserSelectException ex = new UserSelectException("User didn't agree eula");
                throw new ServerStarterException<UserSelectException>(ex);
            }

            eulaContents[eulaContents.Count - 1] = "eula=true";
        }

        private static void CheckEula()
        {
            string eulaPath = $@"{Path}\eula.txt";

            // 新規導入の際など、そもそもeula.txtがない場合
            if (!File.Exists(eulaPath))
                return;

            List<string> eulaContents = new List<string>();
            using (StreamReader sr = new StreamReader(eulaPath))
            {
                string line;
                while ((line = sr.ReadLine()) != "")
                {
                    eulaContents.Add(sr.ReadLine());
                }
            }
            
            AgreeEula(eulaContents);
        }
    }
}
