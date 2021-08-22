using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Server_GUI2
{
    class Spigot_Function : Functions
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void Check_copy_world()
        {
            bool world_copy = Check_Vdown();
            logger.Info($"Check copy world (Copy is '{world_copy}')");
            if (!world_copy)
            {
                return;
            }
            try
            {
                //ワールドデータをコピー
                if (Data_list.CopyVer_IsSpigot)
                {
                    // Spigot -> Spigot
                    Process p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World} {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World} /E /H /I /Y");
                    p.WaitForExit();
                    p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}_nether {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_nether /E /H /I /Y");
                    p.WaitForExit();
                    p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}_the_end {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_the_end /E /H /I /Y");
                    p.WaitForExit();
                }
                else
                {
                    // Vanila -> Spigot
                    Process p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\{Data_list.Copy_version}\{Data_list.World} {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World} /E /H /I /Y");
                    p.WaitForExit();
                    Directory.CreateDirectory($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_nether");
                    Directory.CreateDirectory($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_the_end");
                    Directory.Move($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}\DIM-1", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_nether\DIM-1");
                    Directory.Move($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}\DIM1", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_the_end\DIM1");
                    string[] dims = new string[2] { "nether", "the_end" };
                    foreach(string dim in dims)
                    {
                        File.Copy($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}\level.dat", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_{dim}\level.dat");
                        File.Copy($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}\level.dat_old", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_{dim}\level.dat_old");
                        File.Copy($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}\session.lock", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_{dim}\session.lock");
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                        "サーバーの移行に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new WinCommandException($"Failed to switch the server system (Error Message : {ex.Message})");
            }
        }

        public override void Reset_world_method(bool reset_world, bool save_world)
        {
            if (!reset_world)
            {
                return;
            }

            if (save_world)
            {
                Save_Data();
            }

            try
            {
                Directory.Delete($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}", true);
                Directory.Delete($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_nether", true);
                Directory.Delete($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_the_end", true);
            }
            catch (Exception ex)
            {
                string message =
                        "ワールドデータの初期化に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException($"Failed to delete world data (Error Message : {ex.Message})");
            }
        }

        private void Save_Data()
        {
            int num = 1;
            logger.Info("Reset World before saving World");
            //以前に作成したバックアップがないかを確認
            while (Directory.Exists($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_old({num})\"))
            {
                num++;
            }

            try
            {
                Process p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World} {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_old({num}) / E /H /I /Y");
                p.WaitForExit();
                p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_nether {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_nether_old({num}) / E /H /I /Y");
                p.WaitForExit();
                p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_the_end {MainWindow.Data_Path}\Spigot_{Data_list.Version}\{Data_list.World}_the_end_old({num}) / E /H /I /Y");
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                string message =
                        "ワールドデータのバックアップ作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                System.Windows.Forms.MessageBox.Show(message, "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new WinCommandException("Failed to make the back up world data");
            }
        }

        /// <summary>
        /// ビルド時に生成される不要なディレクトリを削除する
        /// </summary>
        public void Delete_dir(string ver_folder)
        {
            foreach (var path in Directory.GetDirectories($@"{MainWindow.Data_Path}\{ver_folder}\"))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                RemoveReadonlyAttribute(di);
                di.Delete(true);
            }

            try
            {
                File.Delete($@"{MainWindow.Data_Path}\{ver_folder}\build.bat");
                File.Delete($@"{MainWindow.Data_Path}\{ver_folder}\BuildTools.jar");
            }
            catch(Exception ex)
            {
                logger.Info($"Failed to delete build.bat and BuildTools.jar ({ex.Message})");
            }
        }

        private void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
                if ((fi.Attributes & FileAttributes.ReadOnly) ==
                    FileAttributes.ReadOnly)
                    fi.Attributes = FileAttributes.Normal;
            //サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                RemoveReadonlyAttribute(di);
        }
    }
}
