using log4net;
using Server_GUI2.Develop.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2
{
    static class Server
    {
        /// <summary>
        /// カレントディレクトリのパス
        /// </summary>
        private static VersionPath Path;
        
        private static string JarName;
        private static string Log4jArgument;
        private static string WorldContainerArgument;

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// server.jarをeula.txtなしで起動する
        /// 実行は失敗しeula.txt等のファイルが生成される
        /// </summary>
        public static void StartWithoutEula(VersionPath path, string jarName, string log4jArgument, ServerProperty property, string worldContainerArgument = "")
        {
            logger.Info("<Start>");

            logger.Info("<Start> save server.properties");
            path.ServerProperties.WriteAllText(property.ExportProperty());

            Path = path;
            JarName = jarName;
            Log4jArgument = log4jArgument;
            WorldContainerArgument = worldContainerArgument;

            logger.Info("delete eula.txt");
            Path.Eula.Delete(true);
            
            Run();
            logger.Info("</Start>");
        }

        /// <summary>
        /// server.jarを実際に起動する
        /// </summary>
        public static void Start(VersionPath path, string jarName, string log4jArgument, ServerSettings settings, string worldContainerArgument = "")
        {
            logger.Info("<Start>");

            logger.Info("<Start> save server settings");
            settings.Save(path);

            Path = path;
            JarName = jarName;
            Log4jArgument = log4jArgument;
            WorldContainerArgument = worldContainerArgument;

            var euraResult = CheckEula();

            if (euraResult)
            {
                Run();
            }
            else
            {
                logger.Info("cannot start server without eula agreement");
            }

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
                    WorkingDirectory = Path.FullName,
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

        private static bool AgreeEula(bool eulaState,string euraURL)
        {
            logger.Info("<AgreeEula>");

            if ( eulaState )
            {
                logger.Info("</AgreeEula> already true");
                return true;
            }

            var result = MW.MessageBox.Show(
                $"以下のURLに示されているサーバー利用に関する注意事項に同意しますか？\n\n" +
                $"【EULAのURL】\n{euraURL}", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (result == MessageBoxResult.Cancel)
            {
                UserSelectException ex = new UserSelectException("User didn't agree eula");
                logger.Info("</AgreeEula> not agreed");
                return false;
            }
            logger.Info("</AgreeEula>");
            return true;
        }

        private static bool CheckEula()
        {
            logger.Info("<CheckEula>");
            string eulaPath = $@"{Path}\eula.txt";

            var result = false;
            
            logger.Info("<CheckEula> load eura.text");
            Path.Eula.ReadAllText()
                // eula.txtがない場合
                .FailureAction(
                x => logger.Info("</CheckEula> not exists")
                )
                // eula.txtがある場合
                .SuccessAction(
                euracontent =>
                {
                    var eulaValue = false;

                    var eulaValueMatch = Regex.Match(euracontent, @"(?<=[^|\n]\s*eula\s*=\s*)true|True|TRUE|false|False|FALSE(?=\s*[\n|$])");
                    if (!eulaValueMatch.Success)
                        logger.Info("<CheckEula> eula.txt has no segment \"eura=(true|false)\"");
                    else
                    {
                        eulaValue = eulaValueMatch.Value.ToLower() == "true";
                        logger.Info($"<CheckEula> eula.txt has segment \"eura={eulaValue.ToString().ToLower()}\"");
                    }

                    var match = Regex.Match(euracontent, @"https://[a-zA-Z0-9\._/-]+");
                    if (match.Success)
                    {
                        result = AgreeEula(eulaValue, match.Value);
                    }
                    else
                    {
                        result = AgreeEula(eulaValue, "リンクが見つかりません");
                    }

                    if (result != eulaValue)
                    {
                        logger.Info("<CheckEula> rewrite eula.txt");
                        Path.Eula.WriteAllText(Regex.Replace(euracontent,@"(?<=[^|\n]\s*eula\s*=\s*)true|false(?=\s*[\n|$])",result.ToString().ToLower()));
                    }
                    else
                    {
                        logger.Info("<CheckEula> eula.txt not changes");
                    }
                    logger.Info("</CheckEula>");
                }
                );
            return result;
        }
    }
}
