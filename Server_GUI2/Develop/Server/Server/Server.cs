using log4net;
using Server_GUI2.Develop.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using MW = ModernWpf;

namespace Server_GUI2
{
    static class Server
    {
        /// <summary>
        /// カレントディレクトリのパス
        /// </summary>
        private static VersionPath Path;
        
        private static string JavaPath;
        private static string JarName;
        private static string Log4jArgument;
        private static string WorldContainerArgument;

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// server.jarをeula.txtなしで起動する
        /// 実行は失敗しeula.txt等のファイルが生成される
        /// </summary>
        public static void StartWithoutEula(VersionPath path, string javaPath, string jarName, string log4jArgument, ServerProperty property, string worldContainerArgument = "")
        {
            logger.Info("<StartWithoutEula>");
            StartServer.RunProgressBar.AddMessage("Temporary Run Server to Generate Eula.");

            logger.Info("save server.properties");
            path.ServerProperties.WriteAllText(property.ExportProperty());

            Path = path;
            JavaPath = javaPath;
            JarName = jarName;
            Log4jArgument = log4jArgument;
            WorldContainerArgument = worldContainerArgument;

            logger.Info("delete eula.txt");
            Path.Eula.Delete(true);
            
            Run();
            logger.Info("</StartWithoutEula>");
        }

        /// <summary>
        /// server.jarを実際に起動する
        /// </summary>
        public static void Start(VersionPath path, string javaPath, string jarName, string log4jArgument, ServerSettings settings, string worldContainerArgument = "")
        {
            logger.Info("<Start>");

            logger.Info("save server settings");
            settings.Save(path);
            StartServer.RunProgressBar.AddMessage("Reflected World Settings.");

            Path = path;
            JavaPath = javaPath;
            JarName = jarName;
            Log4jArgument = log4jArgument;
            WorldContainerArgument = worldContainerArgument;

            var euraResult = CheckEula();
            StartServer.RunProgressBar.AddMessage("Checked Eura.");

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
            StartServer.RunProgressBar.Close();
            StartServer.RunProgressBar.ShowCount();
            var argumnets = $"-Xmx5G -Xms5G{Log4jArgument} -jar {JarName} nogui{WorldContainerArgument}";
            logger.Info("<Run>");
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(JavaPath)
                {
                    Arguments = argumnets,
                    WorkingDirectory = Path.FullName,
                    UseShellExecute = false
                }
            };

            logger.Info($"<process/> {JavaPath} {argumnets}");
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

            string result = CustomMessageBox.Show(
                $"以下のURLに示されているサーバー利用に関する注意事項に同意しますか？\n\n" +
                $"【EULAのURL】\n{euraURL}", ButtonType.OKCancel, Image.Infomation);

            if (result == "Cancel")
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
            
            logger.Info("load eura.text");
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

                    var eulaValueMatch = Regex.Match(euracontent, @"[^|\n]\s*eula\s*=\s*(true|True|TRUE|false|False|FALSE)\s*[\n|$]");
                    if (!eulaValueMatch.Success)
                        logger.Info("eula.txt has no segment \"eura=(true|false)\"");
                    else
                    {
                        eulaValue = eulaValueMatch.Groups[1].Value.ToLower() == "true";
                        logger.Info($"eula.txt has segment \"eura={eulaValue.ToString().ToLower()}\"");
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
                        logger.Info("rewrite eula.txt");
                        Path.Eula.WriteAllText(Regex.Replace(euracontent,@"(?<=[^|\n]\s*eula\s*=\s*)true|false(?=\s*[\n|$])",result.ToString().ToLower()));
                    }
                    else
                    {
                        logger.Info("eula.txt not changes");
                    }
                    logger.Info("</CheckEula>");
                }
                );
            return result;
        }
    }
}
