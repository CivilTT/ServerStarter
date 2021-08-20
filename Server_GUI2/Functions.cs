﻿using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace Server_GUI2
{
    public partial class Functions : Window
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> info2 = new List<string>();

        private List<string> Version_Folders = new List<string>();
        private List<string> World_Folders = new List<string>();
        private dynamic root;

        public MainWindow main { get; set; }
        private Git git = new Git();

        WebClient wc = new WebClient();

        public void Build_info()
        {
            try
            {
                logger.Info("Check the info.txt");
                var window = new info_builder();

                //本来あるべきinfoの行数
                int all_line_num = Data_list.Info_index.Count;

                //ファイルが存在し、かつ、ファイルの行数が正しいときはbuildしない
                if (File.Exists($@"{MainWindow.Data_Path}\info.txt"))
                {
                    string[] lines = File.ReadAllLines($@"{MainWindow.Data_Path}\info.txt");
                    if (lines.Length == all_line_num)
                    {
                        return;
                    }
                }

                bool? res = window.ShowDialog();
                if (res == false)
                {
                    //強制終了
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public System.Windows.Controls.ComboBox Init_new_Versions(System.Windows.Controls.ComboBox new_Version, List<string> list_versions)
        {
            foreach (string version in list_versions)
            {
                new_Version.Items.Add(version);
            }

            return new_Version;
        }

        public System.Windows.Controls.ComboBox Init_version(System.Windows.Controls.ComboBox Version)
        {
            logger.Info("Read the local Versions");

            try
            {
                foreach (KeyValuePair<string, List<string>> kvp in Data_list.VerWor_list)
                {
                    Version.Items.Add(kvp.Key);
                }
                Version.Items.Add("【new Version】");
                Version = Check_index(Version, Properties.Settings.Default.Version);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

            return Version;
        }

        public System.Windows.Controls.ComboBox Init_world(System.Windows.Controls.ComboBox World)
        {
            logger.Info("Read the local Worlds");

            try
            {
                // //以前開いたバージョンが存在しない場合の処理
                // if (!(Directory.Exists($@"{MainWindow.Data_Path}\{read_version}")) || read_version == "")
                // {
                //     string[] subFolders = Directory.GetDirectories(
                //         $@"{MainWindow.Data_Path}\", "*", SearchOption.TopDirectoryOnly);

                //     //World_Dataフォルダに一つもデータが存在しない場合、read_versionを空欄で返す
                //     read_version = (subFolders.Length == 0) ? "" : Path.GetFileName(subFolders[0]);
                // }

                // string[] Worlds = Directory.GetDirectories(
                //     $@"{MainWindow.Data_Path}\{read_version}", "*", SearchOption.TopDirectoryOnly);
                // for (int i = 0; i < Worlds.Length; i++)
                // {
                //     string World_name = Path.GetFileName(Worlds[i]);
                //     World.Items.Add(World_name);
                // }
                // World.Items.Remove("logs");
                foreach (KeyValuePair<string, List<string>> kvp in Data_list.VerWor_list)
                {
                    foreach (string world_name in kvp.Value)
                    {
                        if (world_name != "ShareWorld")
                        {
                            if (kvp.Key.Contains("Spigot") && world_name.Contains("_nether") || world_name.Contains("_the_end") || world_name == "plugins")
                            {
                                continue;
                            }
                            World.Items.Add($"{kvp.Key}/{world_name}");
                        }
                    }
                }
                if (Data_list.Avail_sw)
                {
                    World.Items.Add("ShareWorld");
                }
                World.Items.Add("【new World】");
                string index_name = (Data_list.Info[3] == "ShareWorld") ? Data_list.Info[3] : $"{Properties.Settings.Default.Version}/{Data_list.Info[3]}";
                World = Check_index(World, index_name);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

            return World;
        }

        private System.Windows.Controls.ComboBox Check_index(System.Windows.Controls.ComboBox world_version, string index_name)
        {
            int index_num = world_version.Items.IndexOf(index_name);
            //index_nameに該当するワールド名(バージョン名)が登録されていない場合に-1を返されるため、0番目のcombo_boxのワールド名(バージョン名)を返す
            if (index_num == -1 && world_version.Items.Count != 0)
            {
                index_num = 0;
            }
            world_version.SelectedIndex = index_num;

            return world_version;
        }

        public void Define_OpenWorld(System.Windows.Controls.ComboBox world, bool GUI)
        {
            logger.Info("Define_OpenWorld");
            if (world.SelectedIndex == -1 && GUI)
            {
                if (Data_list.World == "ShareWorld" || Data_list.World == "logs" || Data_list.World == "")
                {
                    System.Windows.Forms.MessageBox.Show(
                        "新しく作るワールド名が不正なものとなっています。\r\n" +
                        "ワールド名に「(空欄)」、「ShareWorld」、「logs」は指定できません。\r\n" +
                        "これら以外の名称で再登録してください。", "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    logger.Error($"User nmae the new world for {Data_list.World}");
                    Console.Write(App.end_str);
                    Environment.Exit(0);
                }
                if (world.Items.Contains(Data_list.World))
                {
                    System.Windows.MessageBox.Show(
                        $"{Data_list.World}はすでに存在するワールドです。\n" +
                        $"存在するワールドを新しく起動することはできません。\n" +
                        $"同じ名前でワールドを作り直す場合は、メイン画面にてRecreateにチェックを入れてください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    logger.Error($"'{Data_list.World}' (World) already existed");
                    Console.Write(App.end_str);
                    Environment.Exit(0);
                }
            }
        }

        

        public List<string> Add_info(StreamReader sr)
        {
            logger.Info($"Read the local info data");

            string line;
            List<string> tmp_info = new List<string>();

            try
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //＞が入っていない行ははじく
                    if (line.IndexOf(">") == -1)
                    {
                        continue;
                    }

                    //-＞の前後をリストとして登録している
                    tmp_info.Add(line.Substring(line.IndexOf("->") + 2));
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

            return tmp_info;
        }

        public void Check_file_directory_SW()
        {
            //batファイルの変更を反映できるよう毎度作成する
            logger.Info("Create bat files (pull & push)");
            git.Create_bat_pull();
            git.Create_bat_push();
        }

        public void Create_bat_start()
        {
            string ver = (Data_list.Import_spigot) ? "Spigot_" + Data_list.Version : Data_list.Version;
            if (File.Exists($@"{MainWindow.Data_Path}\{ver}\start.bat"))
            {
                return;
            }
            else
            {
                logger.Info("Generate start.bat");
                List<string> start = new List<string>
                {
                    "@echo off",
                    "cd %~dp0",
                };
                if (Data_list.Import_spigot)
                {
                    start.Add($"java -Xmx5G -Xms5G -jar spigot-{Data_list.Version}.jar nogui");
                }
                else
                {
                    start.Add("java -Xmx5G -Xms5G -jar server.jar nogui");
                }

                try
                {
                    using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{ver}\start.bat", false))
                    {
                        foreach (string line in start)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
            }
        }

        private void Starter_versionUP(string url)
        {
            logger.Info("Delete old .exe and download new .exe");
            string self_path = Directory.GetCurrentDirectory();


            wc.DownloadFile(url, $@".\..\Server_GUI2.zip");
            try
            {
                string args_list = "";
                if (App.Args != null)
                {
                    foreach (string key in App.Args)
                    {
                        args_list += $" {key}";
                    }
                }


                List<string> versionUP = new List<string>()
                {
                    "@echo off",
                    @"taskkill /IM Server_GUI2.exe /F",
                    $@"@powershell Expand-Archive -Path {self_path}\..\Server_GUI2.zip -DestinationPath {self_path} -Force",
                    // 更新後に再起動する
                    $@"start Server_GUI2.exe{args_list}",
                    $@"del {self_path}\..\Server_GUI2.zip",
                    //自分自身を削除する
                    "del /f \"%~dp0%~nx0\""
                };
                using (var writer = new StreamWriter(@".\tmp.bat", false))
                {
                    foreach (string line in versionUP)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message, "tmp(versionUP)_log.txt");
            }


            Process.Start(@".\tmp.bat", @" > .\log\tmp(versionUP)_log.txt 2>&1");
            Environment.Exit(0);
        }

        /// <summary>
        /// info.txtの書き換えを行います。
        /// </summary>
        /// <param name="index">1:Starter_Version, 2:Version, 3:World, 4: Server_Opening (bool) を書き換えます。</param>
        /// <param name="Opening_Server">index = 4を書き換える際に必要です。</param>
        public void Change_info(int index, bool Opening_Server = true)
        {
            string info_path = $@"{MainWindow.Data_Path}\info.txt";

            switch (index)
            {
                case 1:
                    Change_info1();
                    break;
                case 2:
                    Change_info2();
                    break;
                case 3:
                    Change_info3();
                    break;
                case 4:
                    Change_info4(Opening_Server);
                    info_path = $@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\info.txt";
                    break;
            }

            try
            {
                using (var writer = new StreamWriter(info_path, false))
                {
                    for (int a = 0; a < Data_list.Info.Count; a++)
                    {
                        writer.WriteLine($"{Data_list.Info_index[a]}->{Data_list.Info[a]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public void Change_info4(bool Opening_Server)
        {
            //server_openの項目についてFalseをTrueに書き換え
            logger.Info($"Change info about the server_open -> {Opening_Server}");
            logger.Info($"Change info about the server version -> {Data_list.Version}");
            Data_list.Info[4] = Opening_Server.ToString();
            Data_list.Info[2] = Data_list.Version;
        }

        private void Change_info3()
        {
            //The latest Minecraft World nameの項目について書き換え
            logger.Info("Change info about The latest Minecraft World name");
            Data_list.Info[3] = Data_list.World;
        }

        private void Change_info2()
        {
            //The latest Minecraft Versionの項目について書き換え
            logger.Info("Chenge info about latest Minecraft Version");
            Data_list.Info[2] = (Data_list.Import_spigot) ? "Spigot_" + Data_list.Version : Data_list.Version;
        }

        private void Change_info1()
        {
            //Server_Starterのバージョンの項目について書き換え
            logger.Info("Change the lateset Server Starter's version number");
            Data_list.Info[1] = Data_list.Starter_Version;
        }

        public virtual void Check_copy_world()
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
                    // Spigot -> Vanila
                    // cp -r source dest
                    Process p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World} {MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World} /E /H /I /Y");
                    p.WaitForExit();
                    File.Delete($@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}\uid.dat");
                    p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}_nether\DIM-1 {MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}\DIM-1 /E /H /I /Y");
                    p.WaitForExit();
                    p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}_the_end\DIM1 {MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}\DIM1 /E /H /I /Y");
                    p.WaitForExit();
                }
                else
                {
                    // Vanila -> Vanila
                    Process p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\{Data_list.Copy_version}\{Data_list.World} {MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World} /E /H /I /Y");
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public bool Check_Vdown()
        {
            // 新規ワールド作成時（開くバージョンと同じワールドの時）の Copy_version="" は警告の必要性なし
            if ((Data_list.Version == Data_list.Copy_version && Data_list.Import_spigot == Data_list.CopyVer_IsSpigot) || Data_list.Copy_version == "" || Data_list.World == "ShareWorld")
            {
                return false;
            }

            string re_V = Regex.IsMatch(Data_list.Version, @"[1-9]\.[0-9]+\.[0-9]") ? $"release {Data_list.Version}" : $"snapshot {Data_list.Version}";
            string re_CV = Regex.IsMatch(Data_list.Copy_version, @"[1-9]\.[0-9]+\.[0-9]") ? $"release {Data_list.Copy_version}" : $"snapshot {Data_list.Copy_version}";

            int re_V_num = main.All_versions.IndexOf(re_V);
            int re_CV_num = main.All_versions.IndexOf(re_CV);

            if (re_V_num > re_CV_num)
            {
                logger.Warn($"The World-Data will be recreated by {Data_list.Version} from {Data_list.Copy_version}");
                var result = System.Windows.Forms.MessageBox.Show(
                    $"ワールドデータを{Data_list.Copy_version}から{Data_list.Version}へバージョンダウンしようとしています。\n" +
                    $"データが破損する可能性が極めて高い操作ですが、危険性を理解したうえで実行しますか？", "Server Starter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    logger.Info("User reject downgrading");
                    Environment.Exit(0);
                }
            }

            return true;
        }

        public void Check_server_open()
        {
            logger.Info("Check the ShareWorld's info (There are already started Server or not)");
            if (info2[4] == "True")
            {
                System.Windows.MessageBox.Show(
                    $"ShareWorldのサーバーはすでに{info2[0]}によって起動されています。\r\n" +
                    $"{info2[0]}のサーバーが閉じたことを確認したうえでサーバーを再起動してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                
                logger.Warn("There are already opened server so system is over");
                Environment.Exit(0);
            }
            else
            {
                Change_info(4, Opening_Server: true);

                //変更内容をpush
                git.Push();
            }
        }

        public void Wirte_properties()
        {
            logger.Info("Write Properties");

            Data_list.Server_Properties["level-name"] = Data_list.World;
            Change_info(3);

            //propertiesを該当バージョンのserver.propertiesに書き込む
            logger.Info("Start Writting for server.properties");
            try
            {
                string ver = (Data_list.Import_spigot) ? $"Spigot_{Data_list.Version}" : Data_list.Version;
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{ver}\server.properties", false))
                {
                    foreach (string key in Data_list.Server_Properties.Keys)
                    {
                        if (key.Contains("#"))
                        {
                            writer.WriteLine(key);
                        }
                        else
                        {
                            writer.WriteLine($"{key}={Data_list.Server_Properties[key]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public void Alert_version()
        {
            //起動バージョンが前回と違う場合は警告を出す
            if (info2[2] != Data_list.Version)
            {
                logger.Warn("The Version is Different of latest open by ShareWorld.");
                DialogResult result = System.Windows.Forms.MessageBox.Show($"前回のShareWorldでのサーバー起動バージョンは{info2[2]}です。\r\nバージョン{Data_list.Version}で起動を続けますか？\r\n（「いいえ(N)」を選択した場合はもう一度起動をやり直してください。）", "Server Starter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (result == System.Windows.Forms.DialogResult.No)
                {
                    logger.Info("User chose NO");
                    Environment.Exit(0);
                    Close();
                }
            }
        }

        public void Change_eula()
        {
            logger.Info("Start modifying eula.txt");
            try
            {
                //eula.txtの読み取り
                string line;
                List<string> eula_lines = new List<string>();
                logger.Info("Read the eula.txt");
                string ver = (Data_list.Import_spigot) ? "Spigot_" + Data_list.Version : Data_list.Version;
                using (StreamReader sr = new StreamReader($@"{MainWindow.Data_Path}\{ver}\eula.txt", Encoding.GetEncoding("Shift_JIS")))
                {
                    while ((line = sr.ReadLine()) != "eula=false")
                    {
                        eula_lines.Add(line);
                    }
                }
                eula_lines.Add("eula=true");

                Agree(eula_lines);

                //書き込み
                logger.Info("Write the eula.txt");
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{ver}\eula.txt", false))
                {
                    foreach (string key in eula_lines)
                    {
                        writer.WriteLine(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }

        private void Agree(List<string> eula_lines)
        {
            logger.Info("Check user agreement");

            string html = eula_lines[0].Substring(eula_lines[0].IndexOf("(") + 1);
            html = html.Replace(").", "");
            var result = System.Windows.Forms.MessageBox.Show($"以下のURLに示されているサーバー利用に関する注意事項に同意しますか？\n\n【EULAのURL】\n{html}", "Server Starter", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                logger.Info("User didn't agree eula");
                Environment.Exit(0);
            }
        }

        public void Check_ShareWorld()
        {
            if (Data_list.World != "ShareWorld")
            {
                return;
            }

            Download_ShareWorld();

            //異なるバージョンが指定された場合、初めに確認
            Alert_version();


            //ShareWorldの処理に必要なbatが存在するか否かを確認
            Check_file_directory_SW();

            //起動済みサーバーがあるか否かの確認
            //Server_bat-files\info.txtの中身にてserver_openの項目がTrueであれば起動を中止し、FalseならばTrueに書き換えたうえで起動前にpushを行う
            Check_server_open();
        }

        private void Download_ShareWorld()
        {
            if (!(Directory.Exists($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\")))
            {
                git.Clone(Data_list.Version);
            }
            else
            {
                git.Pull(Data_list.Version);
            }
            MainWindow.Pd.Value = 50;


            //Server_bat-files内のinfo.txtの中身を読み取る(ShareWorld起動時のみ使用するためここに記載している)
            logger.Info("Read the ShareWorld > Server_bat-files info");
            try
            {
                if (File.Exists($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\info.txt"))
                {
                    using (StreamReader sr = new StreamReader($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\info.txt", Encoding.GetEncoding("Shift_JIS")))
                    {
                        info2 = Add_info(sr);
                    }
                }
                else
                {
                    info2 = Data_list.Info;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public virtual void Reset_world_method(bool reset_world, bool save_world)
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
                Delete_folders();
                Delete_files();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void Save_Data()
        {
            int num = 1;
            logger.Info("Reset World before saving World");
            //以前に作成したバックアップがないかを確認
            while (Directory.Exists($@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}_old({num})\"))
            {
                num++;
            }

            try
            {
                Process p = Process.Start("xcopy", $@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World} {MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}_old({num}) /E /H /I /Y");
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            //CopyDirectory($@"{MainWindow.Data_Path}\{version}\{world}\", $@"{MainWindow.Data_Path}\{version}\{world}_old({num})\");

        }

        public void Import_datapack(More_Settings m_set_window)
        {
            logger.Info("Check the datapacks");
            if (!m_set_window.Dp_window.import_dp)
            {
                logger.Info("There are not datapack necessary");
                return;
            }

            m_set_window.Dp_window.Add_data();
            m_set_window.Dp_window.Remove_data();
        }

        public void Import_World(More_Settings m_set_window)
        {
            logger.Info("Check the Custom Map");
            if (!m_set_window.Haihu_window.import_haihu)
            {
                logger.Info("There are not Custom Map necessary");
                return;
            }

            m_set_window.Haihu_window.Add_data();
        }

        public void Import_plugins(More_Settings m_set_window)
        {
            logger.Info("Check the plugins");
            if (!m_set_window.Spigot_window.import_plugin)
            {
                logger.Info("There are not plugins necessary");
                return;
            }

            if (!Directory.Exists($@"{MainWindow.Data_Path}Spigot_{Data_list.Version}\plugins"))
            {
                logger.Info("Create plugins folder");
                Directory.CreateDirectory($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\plugins");
            }
            m_set_window.Spigot_window.Add_data();
            m_set_window.Spigot_window.Remove_data();
        }

        private void Delete_folders()
        {
            //フォルダの削除
            //worldデータ内の全フォルダを取得し、stay_foldersを残す
            logger.Info($"Delete the {Data_list.World} about the directory");
            List<string> stay_folders = new List<string>() { ".git", "Server_bat-files" };
            string[] Folders = Directory.GetDirectories(
                $@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}", "*", SearchOption.TopDirectoryOnly);
            foreach (string name in Folders)
            {
                string rename = Path.GetFileName(name);
                if (stay_folders.Contains(rename))
                {
                    continue;
                }
                Directory.Delete($@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}\{rename}", true);
            }
        }

        private void Delete_files()
        {
            //ファイルの削除
            logger.Info($"Delete the {Data_list.World} about files");
            string[] tmp_stay_files = Directory.GetFiles(
                $@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}", "*.bat", SearchOption.TopDirectoryOnly);
            List<string> stay_files = new List<string>(tmp_stay_files);
            string[] Files = Directory.GetFiles(
                $@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}", "*", SearchOption.TopDirectoryOnly);
            foreach (string name in Files)
            {
                if (stay_files.Contains(name))
                {
                    continue;
                }
                File.Delete(name);
            }
        }

        public virtual void Start_server()
        {
            //server.jarの起動に必要なstart.batを作成
            Create_bat_start();

            string ver_name = (Data_list.Import_spigot) ? $"Spigot_{Data_list.Version}" : Data_list.Version;
            logger.Info($"Start the server of {ver_name}");
            
            Process p = Process.Start($@"{MainWindow.Data_Path}\{ver_name}\start.bat");
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                Error($"サーバーの開設に失敗しました。(エラーコード：{p.ExitCode})");
                Console.Write(App.end_str);
                Environment.Exit(0);
            }
        }

        [Obsolete]
        public void Upload_ShareWorld()
        {
            if (Data_list.World == "ShareWorld")
            {
                MainWindow.Pd = new ProgressDialog();
                MainWindow.Pd.Title = "push ShareWorld";
                MainWindow.Pd.Show();

                //info.txtのなかのserver_openをFalseに戻す
                Change_info(4, Opening_Server: false);
                MainWindow.Pd.Value = 50;

                //push
                git.Push();
                MainWindow.Pd.Close();
                // MainWindow.pd.Dispose();
            }
        }

        public void Check_versionUP()
        {
            logger.Info("Check the versionUP about this system, Server Starter");
            Read_Starter_json();

            // このように宣言と代入を分けておかないとうまくjsonの中身を読み込むことができない
            string latest_ver;
            string url;
            latest_ver = root.latest.name;
            url = root.latest.url;

            if (Data_list.Starter_Version != latest_ver)
            {
                //Server Starterのバージョンを書き直し
                Change_info(1);

                //.exeをアップデート
                Starter_versionUP(url);
            }
        }

        private void Read_Starter_json()
        {
            // 最新のjsonをダウンロード
            string url = "https://drive.google.com/uc?id=1cWaNVXzfXsm-xq3PcQl46WBSgwzIRGoh";
            wc.DownloadFile(url, $@"{MainWindow.Data_Path}\Starter_Version.json");

            // jsonの全文読み込み
            string jsonStr = "";
            using (var reader = new StreamReader($@"{MainWindow.Data_Path}\Starter_Version.json"))
            {
                jsonStr = reader.ReadToEnd();
            }

            root = JsonConvert.DeserializeObject(jsonStr);
        }

        public void Check_data_folder()
        {
            //World_Dataフォルダの確認
            logger.Info("Check existence of World_Data Folder");
            try
            {
                if (!(Directory.Exists(MainWindow.Data_Path)))
                {
                    Directory.CreateDirectory(MainWindow.Data_Path);
                    logger.Info("Created the Directory of World_Data");
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }

        public void Check_op()
        {
            if (main.Get_op)
            {
                Make_op();
            }
        }

        private string Get_uuid(string username)
        {
            string url = $@"https://api.mojang.com/users/profiles/minecraft/{username}";
            string jsonStr = wc.DownloadString(url);

            dynamic root = JsonConvert.DeserializeObject(jsonStr);
            string uuid = root.id;

            string uuid_1 = uuid.Substring(0, 8);
            string uuid_2 = uuid.Substring(8, 4);
            string uuid_3 = uuid.Substring(12, 4);
            string uuid_4 = uuid.Substring(16, 4);
            string uuid_5 = uuid.Substring(20);
            uuid = uuid_1 + "-" + uuid_2 + "-" + uuid_3 + "-" + uuid_4 + "-" + uuid_5;

            return uuid;
        }

        private void Make_op(int level = 4, bool bypassesPlayerLimit = false)
        {
            logger.Info($"Check ops.josn ({Data_list.Info[0]} has op rights, or not)");
            string ver_name = (Data_list.Import_spigot) ? $"Spigot_{Data_list.Version}" : Data_list.Version;
            string ops_path = $@"{MainWindow.Data_Path}\{ver_name}\ops.json";
            string ops_line = (File.Exists(ops_path)) ? File.ReadAllText(ops_path) : "[]";
            if (ops_line != "[]")
            {
                // すでにopsが登録されている場合は、処理を行わずに抜ける
                return;
            }

            logger.Info($"Give op rights for {Data_list.Info[0]}");
            string username = Data_list.Info[0];
            string uuid = Get_uuid(username);
            Ops ops = new Ops
            {
                Uuid = uuid,
                Name = username,
                Level = level,
                BypassesPlayerLimit = bypassesPlayerLimit
            };

            try
            {
                var jsonData = JsonConvert.SerializeObject(ops);
                jsonData = "[" + jsonData + "]";

                using (var sw = new StreamWriter(ops_path, false, Encoding.UTF8))
                {
                    sw.Write(jsonData);
                }
            }
            catch (Exception ex)
            {
                logger.Warn($"Failed op process (Error Code : {ex.Message})");
                DialogResult result1 = System.Windows.Forms.MessageBox.Show($"op権限の処理に失敗しました。\nこのままサーバーを開設して良いですか？", "Server Starter", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result1 == System.Windows.Forms.DialogResult.Cancel)
                {
                    logger.Info("User interrupt the opening server");
                    Environment.Exit(0);
                }
            }
        }

        public void Write_VW()
        {
            if (!Properties.Settings.Default.Output_VW)
            {
                return;
            }


            All_VW vw = new All_VW
            {
                Format = "gamma"
            };

            List<All_VW_Ver> ver_list = new List<All_VW_Ver>();
            foreach (KeyValuePair<string, List<string>> kvp in Data_list.VerWor_list)
            {
                All_VW_Ver ver = new All_VW_Ver
                {
                    Version = kvp.Key
                };

                List<All_VW_Wor> wor_list = new List<All_VW_Wor>();
                foreach (string key in kvp.Value)
                {
                    All_VW_Wor wor = new All_VW_Wor
                    {
                        Name = key
                    };
                    wor_list.Add(wor);
                }
                ver.Worlds = wor_list;
                ver_list.Add(ver);
            }
            vw.Versions = ver_list;

            vw.All_vers = main.All_versions;

            string jsonData = JsonConvert.SerializeObject(vw);
            using (var sw = new StreamWriter($@"{Directory.GetCurrentDirectory()}\All-VerWor.json", false, Encoding.UTF8))
            {
                sw.Write(jsonData);
            }
        }

        public void Shutdown()
        {
            if (!Properties.Settings.Default.Shutdown)
            {
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = "/s",
                // ウィンドウを表示しない
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }

        public void Error(string ex_message, string file_name = "Server_Starter.log")
        {
            System.Windows.Forms.MessageBox.Show(
                $"実行途中でエラーが発生しました。\r\n" +
                $"logファイルとともに開発者にお問い合わせください。\r\n" +
                $"【logファイルの場所】\r\n{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}log\\{file_name}\r\n\n" +
                $"【エラー内容】\r\n{ex_message}", "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            logger.Error(ex_message);
            Console.Write(App.end_str);
            Environment.Exit(0);
        }
    }
}
