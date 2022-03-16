using log4net;
using Server_GUI2.Develop.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        private static string WorldContainerArgument;

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// server.jarを実際に起動する
        /// </summary>
        public static void Start(VersionPath path, string jarName, string log4jArgument, ServerProperty property, string worldContainerArgument = "")
        {
            logger.Info("<Start>");

            logger.Info("<Start> save server.properties");
            path.ServerProperties.WriteAllText(property.ExportProperty());

            Path = path.FullName;
            JarName = jarName;
            Log4jArgument = log4jArgument;
            WorldContainerArgument = worldContainerArgument;

            CheckEula();

            Run();
            logger.Info("</Start>");
        }

        private static void Run()
        {
            var argumnets = $"-Xmx5G -Xms5G{Log4jArgument} -jar {JarName} nogui{WorldContainerArgument}";
            logger.Info("<Run>");
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo("java")
                {
                    Arguments = argumnets,
                    WorkingDirectory = Path,
                    UseShellExecute = false
                }
            };

            logger.Info($"<process/> java {argumnets}");
            process.Start();
            process.WaitForExit();

            switch (process.ExitCode)
            {
                case 0:
                    logger.Info("</Run> success");
                    return;
                default:
                    logger.Info("</Run> failure");
                    throw new ServerException($"server was incorrectly closed. exit code: {process.ExitCode}");
            }
        }

        private static void AgreeEula(List<string> eulaContents)
        {
            logger.Info("<AgreeEula>");

            if (eulaContents[eulaContents.Count - 1] == "eula=true")
            {
                logger.Info("</AgreeEula> already true");
                return;
            }

            string html = eulaContents[0].Substring(eulaContents[0].IndexOf("(") + 1);
            html = html.Replace(").", "");
            
            var result = MW.MessageBox.Show(
                $"以下のURLに示されているサーバー利用に関する注意事項に同意しますか？\n\n" +
                $"【EULAのURL】\n{html}", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (result == MessageBoxResult.Cancel)
            {
                UserSelectException ex = new UserSelectException("User didn't agree eula");
                logger.Info("</AgreeEula> not agreed");
                throw new ServerStarterException<UserSelectException>(ex);
            }

            eulaContents[eulaContents.Count - 1] = "eula=true";

            logger.Info("</AgreeEula>");
        }

        private static void CheckEula()
        {
            logger.Info("<CheckEula>");
            string eulaPath = $@"{Path}\eula.txt";

            // 新規導入の際など、そもそもeula.txtがない場合
            if (!File.Exists(eulaPath))
            {
                logger.Info("</CheckEula> not exists");
                return;
            }

            logger.Info("<CheckEula> load eura.text");
            List<string> eulaContents = new List<string>();
            using (StreamReader sr = new StreamReader(eulaPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    eulaContents.Add(sr.ReadLine());
                }
            }
            
            AgreeEula(eulaContents);

            logger.Info("<CheckEula> save eura.text");
            var stream = new FileInfo(eulaPath).CreateText();
            stream.Write(string.Join("\n", eulaContents));
            stream.Close();

            logger.Info("</CheckEula>");
        }
    }
}
