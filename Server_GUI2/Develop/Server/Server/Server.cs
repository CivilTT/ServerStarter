﻿using log4net;
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
using Server_GUI2.Util;
using Server_GUI2.Develop.Server.World;

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
            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_Eula);

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


            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_World);

            Path = path;
            JavaPath = javaPath;
            JarName = jarName;
            Log4jArgument = log4jArgument;
            WorldContainerArgument = worldContainerArgument;

            if (!Path.Eula.Exists)
            {
                //Eulaがない場合一度実行し、eula.txtなどの必要ファイルを書き出す
                StartWithoutEula(Path, javaPath, JarName, Log4jArgument, ServerProperty.GetUserDefault());

            }

            logger.Info("save server settings");
            settings.Save(path);
            settings.Ops.WriteLine();

            // リモートのリンク情報一覧を更新する
            WorldCollection.Instance.SaveLinkJson();
            
            var eulaResult = CheckEula();
            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_CheckEula);

            if (eulaResult)
            {
                StartServer.RunProgressBar.Close();
                //StartServer.RunProgressBar.ShowCount();

                Run();

                settings.Load(Path);
            }
            else
            {
                StartServer.RunProgressBar.Close();
                logger.Info("cannot start server without eula agreement");
            }

            // TODO: フラグ数の更新
            StartServer.CloseProgressBar = new Windows.ProgressBar.ProgressBar(Properties.Resources.CloseBar_Title, 5);
            StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_ClosedServer);

            logger.Info("</Start>");
        }

        private static void Run()
        {
            string memorySize = UserSettings.Instance.userSettings.ServerMemorySize.ToString();
            var argumnets = $"-Xmx{memorySize} -Xms{memorySize}{Log4jArgument} -jar {JarName} nogui{WorldContainerArgument}";
            logger.Info("<Run>");
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(JavaPath)
                {
                    Arguments = argumnets,
                    WorkingDirectory = Path.FullName,
                    UseShellExecute = false
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
        }

        private static bool AgreeEula(bool eulaState, string eulaURL)
        {
            logger.Info("<AgreeEula>");

            if ( eulaState )
            {
                logger.Info("</AgreeEula> already true");
                return true;
            }

            int result;
            if (eulaURL == "")
            {
                result = CustomMessageBox.Show(
                    Properties.Resources.Server_EulaMsg1,
                    ButtonType.OKCancel,
                    Image.Infomation
                    );
            }
            else
            {
                result = CustomMessageBox.Show(
                    Properties.Resources.Server_EulaMsg1,
                    ButtonType.OKCancel,
                    Image.Infomation,
                    new LinkMessage(Properties.Resources.Server_EulaMsg2, eulaURL)
                    );
            }

            if (result != 0)
            {
                UserSelectException ex = new UserSelectException("User denied eula");
                logger.Info("</AgreeEula> denied");
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
            
            logger.Info("load eula.text");
            Path.Eula.ReadAllText()
                // eula.txtの読み込みに失敗した場合
                .FailureAction(
                    x => logger.Info("</CheckEula> not exists")
                )
                // eula.txtの読み込みに成功した場合
                .SuccessAction(
                eulacontent =>
                {
                    var eulaValue = false;

                    var eulaValueMatch = Regex.Match(eulacontent, @"[^|\n]\s*eula\s*=\s*(true|True|TRUE|false|False|FALSE)\s*[\n|$]");
                    if (!eulaValueMatch.Success)
                        logger.Info("eula.txt has no segment \"eula=(true|false)\"");
                    else
                    {
                        eulaValue = eulaValueMatch.Groups[1].Value.ToLower() == "true";
                        logger.Info($"eula.txt has segment \"eula={eulaValue.ToString().ToLower()}\"");
                    }

                    var match = Regex.Match(eulacontent, @"https://[a-zA-Z0-9\._/-]+");
                    if (match.Success)
                    {
                        result = AgreeEula(eulaValue, match.Value);
                    }
                    else
                    {
                        result = AgreeEula(eulaValue, "");
                    }

                    if (result != eulaValue)
                    {
                        logger.Info("rewrite eula.txt");
                        Path.Eula.WriteAllText(Regex.Replace(eulacontent,@"(?<=[^|\n]\s*eula\s*=\s*)true|false(?=\s*[\n|$])",result.ToString().ToLower()));
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
