using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MW = ModernWpf;


namespace Server_GUI2
{
    public partial class Functions : Window
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> info2 = new List<string>();

        private readonly List<string> Version_Folders = new List<string>();
        private readonly List<string> World_Folders = new List<string>();
        private dynamic root;

        public MainWindow Main { get; set; }
        private readonly Git git = new Git();
        readonly WebClient wc = new WebClient();
        readonly Data_list data = new Data_list();

        //public void Build_info()
        //{
        //    logger.Info("Check the info.txt");
        //    var window = new info_builder();

        //    //本来あるべきinfoの行数
        //    int all_line_num = Data_list.Info_index.Count;

        //    //ファイルが存在し、かつ、ファイルの行数が正しいときはbuildしない
        //    if (File.Exists($@"{MainWindow.Data_Path}\info.txt"))
        //    {
        //        string[] lines = File.ReadAllLines($@"{MainWindow.Data_Path}\info.txt");
        //        if (lines.Length == all_line_num)
        //        {
        //            return;
        //        }
        //    }

        //    bool? res = window.ShowDialog();
        //    if (res == false)
        //    {
        //        throw new UserSelectException("Stop building the info.txt by user");
        //    }
        //}

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
            logger.Info("Read the local NewVersions");

            foreach (KeyValuePair<string, List<string>> kvp in Data_list.VerWor_list)
            {
                Version.Items.Add(kvp.Key);
            }

            if (MainWindow.Set_new_Version)
            {
                Version.Items.Add("【new Version】");
            }
            Version = Check_index(Version, Properties.Settings.Default.Version);

            return Version;
        }

        public System.Windows.Controls.ComboBox Init_world(System.Windows.Controls.ComboBox World)
        {
            logger.Info("Read the local Worlds");

            foreach (KeyValuePair<string, List<string>> kvp in Data_list.VerWor_list)
            {
                foreach (string world_name in kvp.Value)
                {
                    if (world_name == "ShareWorld")
                        continue;

                    World.Items.Add($"{kvp.Key}/{world_name}");
                }
            }

            if (Data_list.Avail_sw)
            {
                World.Items.Add("ShareWorld");
            }
            World.Items.Add("【new World】");
            // string index_name = (Data_list.Info[3] == "ShareWorld") ? Data_list.Info[3] : $"{Properties.Settings.Default.Version}/{Data_list.Info[3]}";
            string index_name = (Properties.Settings.Default.World == "ShareWorld") ? Properties.Settings.Default.World : $"{Properties.Settings.Default.CopyVersion}/{Properties.Settings.Default.World}";
            World = Check_index(World, index_name);

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

        /// <summary>
        /// ワールド名に問題がないかを確認する
        /// </summary>
        /// <param name="world"></param>
        /// <param name="GUI"></param>
        /// <returns>問題がない場合にtrueを返し、GUIを再表示する必要がある場合にfalseを返す</returns>
        public bool Define_OpenWorld(System.Windows.Controls.ComboBox world, bool GUI)
        {
            logger.Info("Define_OpenWorld");
            if (world.SelectedIndex == -1 && GUI)
            {
                if (Data_list.World == "ShareWorld" || Data_list.World == "logs" || Data_list.World == "")
                {
                    MW.MessageBox.Show(
                        "新しく作るワールド名が不正なものとなっています。\r\n" +
                        "ワールド名に「(空欄)」、「ShareWorld」、「logs」は指定できません。\r\n" +
                        "これら以外の名称で再登録してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    logger.Error($"User nmae the new world for {Data_list.World}");
                    return false;
                }
                if (world.Items.Contains(Data_list.World))
                {
                    MW.MessageBox.Show(
                        $"{Data_list.World}はすでに存在するワールドです。\n" +
                        $"存在するワールドを新しく起動することはできません。\n" +
                        $"同じ名前でワールドを作り直す場合は、メイン画面にてRecreateにチェックを入れてください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    logger.Error($"'{Data_list.World}' (World) already existed");
                    return false;
                }
            }

            return true;
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
            //if (File.Exists($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\start.bat"))
            // log4j対応のbatをインストールするために緊急でこのようにしている
            // 本来は特定のバージョンにおいて、batの内容に応じて処理を行うか判断するべき
            if (false)
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
                    using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\start.bat", false))
                    {
                        foreach (string line in start)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message =
                        "サーバーの実行ファイル(start.bat)の作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new IOException($"Failed to write start.bat (Error Message : {ex.Message})");
                }
            }
        }

        [Obsolete]
        private void Starter_versionUP(string url)
        {
            logger.Info("Delete old .exe and download new .exe");
            ////MainWindow.Pd.Message = "---START Version-up this system---";
            ////MainWindow.Pd.Message = "Delete old .exe and download new .exe";
            string self_path = Directory.GetCurrentDirectory();

            if (File.Exists(@".\tmp.bat"))
            {
                Process.Start(@".\tmp.bat");
                Environment.Exit(0);
            }

            wc.DownloadFile(url, @".\..\Server_GUI2.zip");

            ////MainWindow.Pd.Close();
            
            // コマンド実行による引数をここで受け取り、再実行する。
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
                $"Powershell -Command \"Expand-Archive -Path "+$@"{self_path}\..\Server_GUI2.zip -DestinationPath {self_path} -Force"+"\"",
                // 更新後に再起動する
                $@"start Server_GUI2.exe{args_list}",
                $@"del {self_path}\..\Server_GUI2.zip",
                //自分自身を削除する
                "del /f \"%~dp0%~nx0\""
            };
            
            try
            {
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
                string message =
                        "Server Starterの更新ファイルの作成に失敗しました。\n" +
                        "システムの更新をせずに実行します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Warn($"Failed to write tmp.bat (Error Message : {ex.Message})");
                Change_info(1, new_system_vesion: Data_list.Starter_Version);
                return;
            }

            Task.Run(() => Process.Start(@".\tmp.bat", @" > .\log\tmp(versionUP)_log.txt 2>&1"));
            Environment.Exit(0);
        }

        private void StarterVersionUP(string url)
        {
            logger.Info("Delete old .exe and download new .exe");
            ////MainWindow.Pd.Message = "---START Version-up this system---";
            ////MainWindow.Pd.Message = "Delete old .exe and download new .exe";

            if (File.Exists(@".\tmp.bat"))
            {
                Process.Start(@".\tmp.bat");
                Environment.Exit(0);
            }

            wc.DownloadFile(url, @".\Setup_ServerStarter.msi");

            ////MainWindow.Pd.Close();

            // コマンド実行による引数をここで受け取り、再実行する。
            string args_list = "";
            if (App.Args != null)
            {
                foreach (string key in App.Args)
                {
                    args_list += $" {key}";
                }
            }

            string[] versionUP = new string[]
            {
                "@echo off",
                $"call msiexec /i Setup_ServerStarter.msi TARGETDIR=\"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\" /passive /li .\\log\\install.log",
                // 更新後に再起動する
                $@"if not %errorlevel%==1602 (start Server_GUI2.exe{args_list})",
                "del Setup_ServerStarter.msi",
                //自分自身を削除する
                "del /f \"%~dp0%~nx0\""
            };

            try
            {
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
                string message =
                        "Server Starterの更新ファイルの作成に失敗しました。\n" +
                        "システムの更新をせずに実行します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Warn($"Failed to write tmp.bat (Error Message : {ex.Message})");
                Change_info(1, new_system_vesion: Data_list.Starter_Version);
                return;
            }

            string mes =
                "Server Starterの更新を開始します。\n" +
                "Setup_ServerStarter.msiによってシステムを更新するため、次に表示される確認画面で許可してください。";
            MW.MessageBox.Show(mes, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
            Process.Start(@".\tmp.bat", @" > .\log\tmp(versionUP)_log.txt 2>&1");
            Environment.Exit(0);
        }

        /// <summary>
        /// info.txtの書き換えを行います。
        /// </summary>
        /// <param name="index">1:Starter_Version, 2:Version, 3:World, 4: Server_Opening (bool) を書き換えます。</param>
        /// <param name="new_system_vesion">index = 1 を書き換える際に必要です。</param>
        /// <param name="Opening_Server">index = 4 を書き換える際に必要です。</param>
        public void Change_info(int index, string new_system_vesion = "", bool Opening_Server = true)
        {
            string info_path = $@"{MainWindow.Data_Path}\info.txt";

            switch (index)
            {
                case 1:
                    Change_info1(new_system_vesion);
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
                string message =
                        "Server Starterの保管ファイル(info.txt)の書き換えに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to write info.txt (Error Message : {ex.Message})");
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
            Data_list.Info[2] = Data_list.ReadVersion;
        }
        private void Change_info1(string version)
        {
            //Server_Starterのバージョンの項目について書き換え
            logger.Info("Change the lateset Server Starter's version number");
            Data_list.Info[1] = version;
        }

        //public void Check_copy_world()
        //{
        //    bool world_copy = Check_Vdown();
        //    logger.Info($"Check copy world (Copy is '{world_copy}')");
        //    if (!world_copy)
        //    {
        //        return;
        //    }

        //    Copy_World();
        //}

        public virtual void Copy_World()
        {
            try
            {
                //ワールドデータをコピー
                if (Data_list.CopyVer_IsSpigot)
                {
                    // Spigot -> Vanila
                    StoV();
                }
                else
                {
                    // Vanila -> Vanila
                    VtoV();
                }
            }
            catch (Exception ex)
            {
                string message =
                        "サーバー開設に必要なワールドデータの操作に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new WinCommandException($"Failed to copy wrold data (Error Message : {ex.Message})");
            }
        }

        public void StoV()
        {
            DirectoryCopy($@"{MainWindow.Data_Path}\{Data_list.ReadCopy_Version}\{Data_list.World}", $@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\{Data_list.World}");
            File.Delete($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\{Data_list.World}\uid.dat");
            DirectoryCopy($@"{MainWindow.Data_Path}\{Data_list.ReadCopy_Version}\{Data_list.World}_nether\DIM-1", $@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\{Data_list.World}\DIM-1");
            DirectoryCopy($@"{MainWindow.Data_Path}\{Data_list.ReadCopy_Version}\{Data_list.World}_the_end\DIM1", $@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\{Data_list.World}\DIM1");
        }

        public void VtoV()
        {
            DirectoryCopy($@"{MainWindow.Data_Path}\{Data_list.Copy_version}\{Data_list.World}", $@"{MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World}");
        }

        /// <summary>
        /// ワールドのバージョンダウンに関するチェックを行う
        /// </summary>
        /// <returns>バージョンダウン（コピー）を行う場合はtrueを返す</returns>
        public bool Check_Vdown()
        {
            // 新規ワールド作成時（開くバージョンと同じワールドの時）の Copy_version="" はコピーの必要性なし
            if (Data_list.ReadVersion == Data_list.ReadCopy_Version || Data_list.World == "ShareWorld")
            {
                return false;
            }

            string re_V = Regex.IsMatch(Data_list.Version, @"[1-9]\.[0-9]+") ? $"release {Data_list.Version}" : $"snapshot {Data_list.Version}";
            string re_CV = Regex.IsMatch(Data_list.Copy_version, @"[1-9]\.[0-9]+") ? $"release {Data_list.Copy_version}" : $"snapshot {Data_list.Copy_version}";

            int re_V_num = Main.All_versions.IndexOf(re_V);
            int re_CV_num = Main.All_versions.IndexOf(re_CV);

            if (re_V_num > re_CV_num)
            {
                logger.Warn($"The World-Data will be recreated by {Data_list.Version} from {Data_list.Copy_version}");
                var result = MW.MessageBox.Show(
                    $"ワールドデータを{Data_list.ReadCopy_Version}から{Data_list.ReadVersion}へバージョンダウンしようとしています。\n" +
                    $"データが破損する可能性が極めて高い操作ですが、危険性を理解したうえで実行しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.No)
                {
                    throw new DowngradeException("User reject downgrading");
                }
            }

            return true;
        }

        public void Check_server_open()
        {
            logger.Info("Check the ShareWorld's info (There are already started Server or not)");
            if (info2[4] == "True" && Data_list.Info[0] != info2[0])
            {
                MW.MessageBox.Show(
                    $"ShareWorldのサーバーはすでに{info2[0]}によって起動されています。\r\n" +
                    $"{info2[0]}のサーバーが閉じたことを確認したうえでサーバーを再起動してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                
                throw new ServerException("There are already opened server so system is over");
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
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\server.properties", false))
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
                string message =
                        "server.propertiesの書き込みに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to write server.properties (Error Code : {ex.Message})");
            }
        }

        public void Alert_version()
        {
            //起動バージョンが前回と違う場合は警告を出す
            if (info2[2] != Data_list.Version)
            {
                logger.Warn("The Version is Different of latest open by ShareWorld.");
                MessageBoxResult? result = MW.MessageBox.Show($"前回のShareWorldでのサーバー起動バージョンは{info2[2]}です。\r\nバージョン{Data_list.Version}で起動を続けますか？\r\n（「いいえ(N)」を選択した場合はもう一度起動をやり直してください。）", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    throw new UserSelectException("User chose NO");
                }
            }
        }

        public void Change_eula()
        {
            logger.Info("Start modifying eula.txt");

            //eula.txtの読み取り
            string line;
            List<string> eula_lines = new List<string>();

            logger.Info("Read the eula.txt");
            if (!File.Exists($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\eula.txt"))
            {
                string message =
                    "server.jarより有効なeula.txtが生成されませんでした。\n" +
                    $"{Data_list.ReadVersion}フォルダ内を確認してください。";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException("Was not created eula.txt");
            }

            using (StreamReader sr = new StreamReader($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\eula.txt", Encoding.GetEncoding("Shift_JIS")))
            {
                while ((line = sr.ReadLine()) != "eula=false")
                {
                    eula_lines.Add(line);
                }
            }
            eula_lines.Add("eula=true");

            Agree(eula_lines);

            try
            {
                //書き込み
                logger.Info("Write the eula.txt");
                using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\eula.txt", false))
                {
                    foreach (string key in eula_lines)
                    {
                        writer.WriteLine(key);
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                        "eula.txtの書き込みに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to write eula.txt (Error Message : {ex.Message})");
            }

        }

        private void Agree(List<string> eula_lines)
        {
            logger.Info("Check user agreement");

            string html = eula_lines[0].Substring(eula_lines[0].IndexOf("(") + 1);
            html = html.Replace(").", "");
            var result = MW.MessageBox.Show($"以下のURLに示されているサーバー利用に関する注意事項に同意しますか？\n\n【EULAのURL】\n{html}", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (result == MessageBoxResult.Cancel)
            {
                throw new UserSelectException("User didn't agree eula");
            }
        }

        public virtual void Check_ShareWorld()
        {
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
            //MainWindow.Pd.Value = 50;


            //Server_bat-files内のinfo.txtの中身を読み取る(ShareWorld起動時のみ使用するためここに記載している)
            logger.Info("Read the ShareWorld > Server_bat-files info");

            try
            {
                if (File.Exists($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\info.txt"))
                {
                    using (StreamReader sr = new StreamReader($@"{MainWindow.Data_Path}\{Data_list.Version}\ShareWorld\info.txt", Encoding.GetEncoding("Shift_JIS")))
                    {
                        info2 = data.Set_info(sr);
                    }
                }
                else
                {
                    info2 = Data_list.Info;
                }
            }
            catch (Exception ex)
            {
                string message =
                        "ShareWorld内のinfo.txtの読み込みに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to read info.txt in ShareWorld (Error Message : {ex.Message})");
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
                string message =
                        "ワールドデータの初期化に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to delete world data (Error Message : {ex.Message})");
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
                string message =
                        "ワールドデータのバックアップ作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new WinCommandException("Failed to make the back up world data");
            }
        }

        //public void Import_datapack(More_Settings m_set_window)
        //{
        //    logger.Info("Check the datapacks");
        //    if (m_set_window.Dp_window == null || !m_set_window.Dp_window.import_dp)
        //    {
        //        logger.Info("There are not datapack necessary");
        //        return;
        //    }

        //    m_set_window.Dp_window.Add_data();
        //    m_set_window.Dp_window.Remove_data();
        //}

        //public void Import_World(More_Settings m_set_window)
        //{
        //    logger.Info("Check the Custom Map");
        //    if (m_set_window.Haihu_window == null || !m_set_window.Haihu_window.import_haihu)
        //    {
        //        logger.Info("There are not Custom Map necessary");
        //        return;
        //    }

        //    m_set_window.Haihu_window.Add_data();
        //}

        //public void Import_plugins(More_Settings m_set_window)
        //{
        //    logger.Info("Check the plugins");
        //    if (m_set_window.Spigot_window == null || !m_set_window.Spigot_window.Import_plugin)
        //    {
        //        logger.Info("There are not plugins necessary");
        //        return;
        //    }

        //    if (!Directory.Exists($@"{MainWindow.Data_Path}Spigot_{Data_list.Version}\plugins"))
        //    {
        //        logger.Info("Create plugins folder");
        //        Directory.CreateDirectory($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\plugins");
        //    }
        //    m_set_window.Spigot_window.Remove_data();
        //    m_set_window.Spigot_window.Add_data();
        //}

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

        public virtual void Start_server(bool first_launch = false)
        {
            // server.jarの起動に必要なstart.batを作成
            Create_bat_start();

            // eula.txtの存在確認
            var app = new ProcessStartInfo
            {
                FileName = "start.bat",
                UseShellExecute = true,
                WorkingDirectory = $@"{MainWindow.Data_Path}\{Data_list.ReadVersion}"
            };

            logger.Info($"Start the server of {Data_list.ReadVersion}");
            Process p = Process.Start(app);
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                string message =
                    "サーバーの実行途中で予期せぬエラーが発生しました。\n\n" +
                    $"【エラーコード】　{p.ExitCode}";
                if (p.ExitCode == 1)
                {
                    message =
                        "サーバーの実行途中で予期せぬエラーが発生しました。\n" +
                        "インストールされているJavaのバージョンが古い可能性があります。\n" +
                        $"【エラーコード】　{p.ExitCode}";
                }
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                if (first_launch)
                {
                    // 中途半端にserver.jarとstart.batのみ残ることを防止
                    Directory.Delete($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}", true);
                }
                throw new ServerException("Failed to process the server");
            }
        }

        public virtual void Upload_ShareWorld()
        {
            ////MainWindow.Pd = new ProgressDialog
            //{
            //    Title = "push ShareWorld"
            //};
            //MainWindow.Pd.Show();

            //info.txtのなかのserver_openをFalseに戻す
            //MainWindow.Pd.Value = 50;
            Change_info(4, Opening_Server: false);

            //push
            git.Push();
            //MainWindow.Pd.Close();
            // //MainWindow.Pd.Dispose();
        }

        public void Check_versionUP()
        {
            logger.Info("Check the versionUP about this system, Server Starter");
            if (File.Exists($@"{MainWindow.Data_Path}\Starter_Version.json"))
                File.Delete($@"{MainWindow.Data_Path}\Starter_Version.json");
            string[] result = Read_json_forGit();

            // このように宣言と代入を分けておかないとうまくjsonの中身を読み込むことができない
            if (result == null)
                return;

            if (Data_list.Starter_Version != result[0])
            {
                //Server Starterのバージョンを書き直し
                Change_info(1, new_system_vesion:result[0]);

                //.exeをアップデート
                StarterVersionUP(result[1]);
            }
        }

        [Obsolete]
        private void Read_Starter_json()
        {
            // 最新のjsonをダウンロード
            string url = "https://drive.google.com/uc?id=1Z9o-1SZpJESlNolOqX6ET5FgPItqXoH0";
            try
            {
                wc.DownloadFile(url, $@"{MainWindow.Data_Path}\Starter_Version.json");
            }
            catch (Exception ex)
            {
                string message =
                        "Server Starterの更新データの取得に失敗しました。\n" +
                        "最新バージョンの確認を行わずに起動します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // jsonの全文読み込み
            string jsonStr = "";
            using (var reader = new StreamReader($@"{MainWindow.Data_Path}\Starter_Version.json"))
            {
                jsonStr = reader.ReadToEnd();
            }

            root = JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// GitHubのreleaseから最新バージョンについて取得する
        /// </summary>
        /// <returns>0番目に最新バージョン、1番目にダウンロードurlを返す</returns>
        private string[] Read_json_forGit()
        {
            string url = "https://api.github.com/repos/CivilTT/ServerStarter/releases";
            string jsonStr;
            try
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                jsonStr = wc.DownloadString(url);
                root = JsonConvert.DeserializeObject(jsonStr);
            }
            catch (Exception ex)
            {
                string message =
                        "Server Starterの更新データの取得に失敗しました。\n" +
                        "最新バージョンの確認を行わずに起動します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            // このように宣言と代入を分けておかないとうまくjsonの中身を読み込むことができない
            if (root == null)
                return null;

            string name = root[0].name;
            name = name.Replace("version ", "");
            string downloadurl = root[0].assets[1].browser_download_url;

            string[] result = new string[2]
            {
                name,
                downloadurl
            };

            return result;
        }

        public void Check_data_folder()
        {
            //World_Dataフォルダの確認
            logger.Info("Check existence of World_Data Folder");
            try
            {
                if (!Directory.Exists(MainWindow.Data_Path))
                {
                    Directory.CreateDirectory(MainWindow.Data_Path);
                    logger.Info("Created the Directory of World_Data");
                }
            }
            catch (Exception ex)
            {
                string message =
                        "サーバーデータを保管するフォルダ(World_Data)の作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to create World_Data folder (Error Message : {ex.Message})");
            }

        }

        //public void Check_op()
        //{
        //    if (Main.Get_op)
        //    {
        //        Make_op();
        //    }
        //}

        //private string Get_uuid(string username)
        //{
        //    string url = $@"https://api.mojang.com/users/profiles/minecraft/{username}";
        //    string jsonStr = wc.DownloadString(url);

        //    dynamic root = JsonConvert.DeserializeObject(jsonStr);
        //    string uuid = root.id;

        //    string uuid_1 = uuid.Substring(0, 8);
        //    string uuid_2 = uuid.Substring(8, 4);
        //    string uuid_3 = uuid.Substring(12, 4);
        //    string uuid_4 = uuid.Substring(16, 4);
        //    string uuid_5 = uuid.Substring(20);
        //    uuid = uuid_1 + "-" + uuid_2 + "-" + uuid_3 + "-" + uuid_4 + "-" + uuid_5;

        //    return uuid;
        //}

        //private void Make_op(int level = 4, bool BypassesPlayerLimit = false)
        //{
        //    logger.Info($"Check ops.josn ({Data_list.Info[0]} has op rights, or not)");
        //    string ops_path = $@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\ops.json";
        //    string ops_line = (File.Exists(ops_path)) ? File.ReadAllText(ops_path) : "[]";
        //    if (ops_line != "[]")
        //    {
        //        // すでにopsが登録されている場合は、処理を行わずに抜ける
        //        return;
        //    }

        //    logger.Info($"Give op rights for {Data_list.Info[0]}");
        //    string username = Data_list.Info[0];
        //    string uuid = Get_uuid(username);
        //    Ops ops = new Ops
        //    {
        //        Uuid = uuid,
        //        Name = username,
        //        Level = level,
        //        BypassesPlayerLimit = BypassesPlayerLimit
        //    };

        //    try
        //    {
        //        var jsonData = JsonConvert.SerializeObject(ops);
        //        jsonData = "[" + jsonData + "]";

        //        using (var sw = new StreamWriter(ops_path, false, Encoding.UTF8))
        //        {
        //            sw.Write(jsonData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Warn($"Failed op process (Error Code : {ex.Message})");
        //        MessageBoxResult? result = MW.MessageBox.Show($"op権限の処理に失敗しました。\nこのままサーバーを開設して良いですか？", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        //        if (result == MessageBoxResult.Cancel)
        //        {
        //            throw new UserSelectException("User interrupt the opening server by op process failed");
        //        }
        //    }
        //}

        //public void Write_VW()
        //{
        //    if (!Properties.Settings.Default.Output_VW)
        //    {
        //        return;
        //    }


        //    All_VW vw = new All_VW
        //    {
        //        Format = "gamma"
        //    };

        //    List<All_VW_Ver> ver_list = new List<All_VW_Ver>();
        //    foreach (KeyValuePair<string, List<string>> kvp in Data_list.VerWor_list)
        //    {
        //        All_VW_Ver ver = new All_VW_Ver
        //        {
        //            Version = kvp.Key
        //        };

        //        List<All_VW_Wor> wor_list = new List<All_VW_Wor>();
        //        foreach (string key in kvp.Value)
        //        {
        //            All_VW_Wor wor = new All_VW_Wor
        //            {
        //                Name = key
        //            };
        //            wor_list.Add(wor);
        //        }
        //        ver.Worlds = wor_list;
        //        ver_list.Add(ver);
        //    }
        //    vw.Versions = ver_list;

        //    vw.All_vers = Main.All_versions;

        //    string jsonData = JsonConvert.SerializeObject(vw);
        //    using (var sw = new StreamWriter($@"{Directory.GetCurrentDirectory()}\All-VerWor.json", false, Encoding.UTF8))
        //    {
        //        sw.Write(jsonData);
        //    }
        //}

        public void Shutdown()
        {
            if (!Properties.Settings.Default.Shutdown)
            {
                return;
            }

            DialogResult result = AutoClosingMessageBox.Show(
                "PCを30秒後にシャットダウンしようとしています。\n" +
                "シャットダウンしない場合は「キャンセル」を押してください。", "Server Starter", 30000, MessageBoxButtons.OKCancel);

            if(result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = "/s /f /t 0",
                // ウィンドウを表示しない
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs=true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
