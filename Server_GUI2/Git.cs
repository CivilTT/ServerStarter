using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Server_GUI2
{
    public partial class Git
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static List<string> log_text { get; set; } = new List<string>();

        public void Pull(string version)
        {
            //かつて生成していたNOTpauseの名称を変更し、pauseするタイプのbatを削除
            logger.Info("Delete the bat files about pull_NOTpause");
            if (File.Exists($@"{MainWindow.Data_Path}\{version}\ShareWorld\pull_NOTpause.bat"))
            {
                File.Delete($@"{MainWindow.Data_Path}\{version}\ShareWorld\pull.bat");
                File.Move($@"{MainWindow.Data_Path}\{version}\ShareWorld\pull_NOTpause.bat", $@"{MainWindow.Data_Path}\{version}\ShareWorld\pull.bat");
            }

            logger.Info("Start the pull ShareWorld");
            MainWindow.Pd.Message = "\n---Start the pull ShareWorld---\n";

            // Process p = Process.Start($@"{MainWindow.Data_Path}\{version}\ShareWorld\pull.bat");
            // %{ "$_" }　によって通常、powershellのエラーによって欠落してしまう文言を補完している
            // \"\"\"はダブルクォーテーションのエスケープをダブルクォーテーションで行っているため、外側二つがエスケープ用で真ん中が入力として必要なダブルクォーテーションとなっている
            // string command = $@"{MainWindow.Data_Path}\{version}\ShareWorld\pull.bat 2>&1 | " + "%{ \"\"\"$_\"\"\" }" + @" | Set-Content -Path .\log\pull_log.txt -PassThru";
            // Process p = Process.Start("powershell", command);
            // p.WaitForExit();
            // if (p.ExitCode != 0 && p.ExitCode != 1)
            // {
            //     Error($"Git pull に失敗しました。(エラーコード：{p.ExitCode})");
            // }

            Bat_start($@"{MainWindow.Data_Path}\{version}\ShareWorld\pull.bat", "pull");

        }

        public void Push()
        {
            //かつて生成していたNOTpauseの名称を変更し、pauseするタイプのbatを削除
            logger.Info("Delete the bat files about push_NOTpause");
            if (File.Exists($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push_NOTpause.bat"))
            {
                File.Delete($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push.bat");
                File.Move($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push_NOTpause.bat", $@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push.bat");
            }

            logger.Info("Start the push ShareWorld");
            MainWindow.Pd.Message = "\n---Start the push ShareWorld---\n";

            // Process p = Process.Start($@"{MainWindow.Data_Path}\{version}\ShareWorld\push.bat");
            // string command = $@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push.bat 2>&1 | " + "%{ \"\"\"$_\"\"\" }" + @" | Set-Content -Path .\log\push_log.txt -PassThru";
            // Process p = Process.Start("powershell", command);
            // p.WaitForExit();
            // if (p.ExitCode != 0 && p.ExitCode != 1)
            // {
            //     Error($"Git push に失敗しました。(エラーコード：{p.ExitCode})");
            // }

            Bat_start($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push.bat", "push");

        }

        public void Clone(string version)
        {
            //clone.batの作成
            Create_bat_clone(version);

            logger.Info("Start the cloning ShareWorld");
            MainWindow.Pd.Message = "\n---Start the cloning ShareWorld---\n";

            // ProcessStartInfo info = new ProcessStartInfo();
            // info.FileName = "clone.bat";
            // info.WorkingDirectory = $@"{MainWindow.Data_Path}\{version}\";
            // info.Arguments = @" > .\log\clone_log.txt 2>&1";

            // Process p = Process.Start(info);
            // Process p = Process.Start($@"{MainWindow.Data_Path}\{version}\clone.bat");

            // string command = $@"{MainWindow.Data_Path}\{version}\clone.bat 2>&1 | " + "%{ \"\"\"$_\"\"\" }" + @" | Set-Content -Path .\log\clone_log.txt -PassThru";
            // Process p = Process.Start("powershell", command);
            // p.WaitForExit();
            // if (p.ExitCode != 0 && p.ExitCode != 1)
            // {
            //     Error($"Git clone に失敗しました。(エラーコード：{p.ExitCode})");
            // }

            Bat_start($@"{MainWindow.Data_Path}\{version}\clone.bat", "clone");

            logger.Info("Delete clone.bat");
            File.Delete($@"{MainWindow.Data_Path}\{version}\clone.bat");
        }

        private void Bat_start(string file_path, string git_type)
        {
            Process p = new Process();
            ProcessStartInfo psi = p.StartInfo;
            psi.FileName = file_path;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            //OutputDataReceivedイベントハンドラを追加
            p.OutputDataReceived += p_OutputDataReceived;
            p.ErrorDataReceived += p_OutputDataReceived;

            p.Start();

            //非同期で出力の読み取りを開始
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();

            if (p.ExitCode != 0 && p.ExitCode != 1)
            {
                string message = $"Git {git_type} に失敗しました。(エラーコード：{p.ExitCode})";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new GitException($"Failed to process 'Git {git_type}' (Error Code : {p.ExitCode})");
            }

            Write_log(git_type);
        }

        //OutputDataReceivedイベントハンドラ
        //行が出力されるたびに呼び出される
        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //出力された文字列を表示する
            // Console.WriteLine(e.Data);
            MainWindow.Pd.Message = e.Data;
            log_text.Add(e.Data);
        }

        private void Write_log(string git_type)
        {
            try
            {
                using (var writer = new StreamWriter($@".\{git_type}_log.txt", false))
                {
                    foreach (string line in log_text)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn($"Failed to make {git_type}_log.txt (Error Message : {ex.Message})");
            }
        }

        public void Create_bat_push()
        {
            logger.Info($@"Create push.bat at {MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push.bat");
            List<string> push = new List<string>
            {
                "@echo off",
                "git config --local user.name %info[6]%",
                "git config --local user.email %info[7]%",
                "for /f \"tokens=1,3 delims=>\" %%a in (%~dp0\\..\\..\\info.txt) do (set info[%%a]=%%b)",
                "git -C %~dp0 add -A",
                "git -C %~dp0 commit -m \"%info[1]%\"",
                "git -C %~dp0 push --progress"
            };

            try
            {
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\push.bat", false))
                {
                    foreach (string line in push)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                    "ShareWorldの処理に必要なファイル(push.bat)の生成に失敗しました。\n\n" +
                    $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException("Failed to make push.bat");
            }
        }

        public void Create_bat_pull()
        {
            logger.Info($@"Create pull.bat at {MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\pull.bat");
            List<string> pull = new List<string>
            {
                "@echo off",
                "for /f \"tokens=1,3 delims=>\" %%a in (%~dp0\\..\\..\\info.txt) do (set info[%%a]=%%b)",
                "git config --local user.name %info[6]%",
                "git config --local user.email %info[7]%",
                "git -C %~dp0 pull --progress"
            };
            try
            {
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\pull.bat", false))
                {
                    foreach (string line in pull)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                    "ShareWorldの処理に必要なファイル(pull.bat)の生成に失敗しました。\n\n" +
                    $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException("Failed to make pull.bat");
            }
        }

        public void Create_bat_clone(string version)
        {
            logger.Info($@"Create clone.bat at {MainWindow.Data_Path}\{version}\clone.bat");
            List<string> clone = new List<string>
            {
                "@echo off",
                "for /f \"tokens=1,3 delims=>\" %%a in (%~dp0\\..\\info.txt) do (set info[%%a]=%%b)",
                "git config --global user.name %info[6]%",
                "git config --global user.email %info[7]%",
                $@"cd {MainWindow.Data_Path}\{version}",
                "git clone https://github.com/%info[6]%/ShareWorld --depth 1 --progress"
            };
            try
            {
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{version}\clone.bat", false))
                {
                    foreach (string line in clone)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                    "ShareWorldの処理に必要なファイル(clone.bat)の生成に失敗しました。\n\n" +
                    $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException("Failed to make clone.bat");
            }
        }
    }
}
