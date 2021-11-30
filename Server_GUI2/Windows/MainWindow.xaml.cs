using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using MW = ModernWpf;



/// <summary>
/// バージョンアップの際に行うべきこと
/// ・Assembly.csのバージョンを変更
/// ・インストーラーのバージョンを変更
/// ・それぞれのビルド
/// ・新たにzip化してアップデート
/// </summary>


namespace Server_GUI2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Data_Path { get { return @".\World_Data"; } }

        //インスタンス変数を宣言
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool GUI = true;

        public static ProgressDialog Pd { get; set; }
        public static bool Set_new_Version { get; set; } = true;


        private readonly List<string> release_versions = new List<string>();
        private readonly List<string> snapshot_versions = new List<string>();
        private readonly List<string> spigot_versions = new List<string>();
        public List<string> All_versions { get; set; } = new List<string>();
        private readonly Functions func = new Functions();
        private readonly ReadJson jsonReader = new ReadJson();
        private readonly ReadHTML htmlReader = new ReadHTML();

        public bool Reset_world { get; set; } = false;
        public bool Save_world { get; set; } = false;
        public bool Get_op { get; set; }


        private readonly Data_list data = new Data_list();
        private readonly ShareWorld shareworld = new ShareWorld();
        private readonly Spigot_Function spi_func = new Spigot_Function();
        private More_Settings m_set_window = new More_Settings();


        public MainWindow(bool gui)
        {
            // 想定外のエラーを処理する
            AppDomain.CurrentDomain.UnhandledException += (sender, eventargs) =>
            {
                var separator = new[] { Environment.NewLine };
                string error_message = eventargs.ExceptionObject.ToString();
                if (!error_message.Contains("UserSelectException") && !error_message.Contains("ServerException"))
                {
                    string message_box = 
                        "予期せぬエラーが発生しました。\n" +
                        "エラー内容を確認し、システムバグが疑われる場合は制作者（CivilTT）にお問い合わせください。\n\n" +
                        $"【エラー内容】\n{error_message.Split(separator, StringSplitOptions.None)[0]}\n\n" +
                        $"【ログファイルの場所】\n{Path.GetFullPath(@".\log\Server_Starter.log")}";
                    MW.MessageBox.Show(message_box, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error(error_message);
                }
            };

            logger.Info("The system of Server Starter is started.");
            InitializeComponent();
            GUI = gui;

            Pd = new ProgressDialog
            {
                //ダイアログのタイトルを設定
                Title = "Server Starter"
            };
            //進行状況ダイアログを表示する
            Pd.Show();

            func.Check_data_folder();
            Pd.Message = "Check existence of World_Data Folder";
            Pd.Value = 10;

            func.Build_info();
            Pd.Message = "Check the info.txt";
            Pd.Value = 20;

            //info.txtの読み取り
            using (StreamReader sr = new StreamReader($@"{Data_Path}\info.txt", Encoding.GetEncoding("Shift_JIS")))
            {
                Data_list.Info = data.Set_info(sr);
            }
            Pd.Message = "Read the local info data";
            Pd.Value = 30;

            // 読み取ったinfo.txtの情報をもとにバージョンアップの必要性があるか否かを判別
            func.Check_versionUP();
            Pd.Message = "Check the versionUP about this system, Server Starter";
            Pd.Value = 40;

            //右上 & opの仕様の変更
            name.Text = Data_list.Info[0];
            op.Content = Data_list.Info[0] + " has op rights in this version's server";
            info_version.Text = $"ver {Data_list.Starter_Version}";
            Get_op = true;
            shutdown.IsChecked = Properties.Settings.Default.Shutdown;
            Pd.Message = "Set the value of GUI";
            Pd.Value = 50;

            // インストールされているVersionとWorldの連想配列を作成
            data.Set_VerWor();
            Pd.Message = "Check the all Directories of Server Version and World";
            Pd.Value = 60;

            // 環境の確認
            data.Set_env();
            Pd.Message = "Check the environment of this PC";
            Pd.Value = 70;

            //追加バージョンの読み込み
            logger.Info("Read the new Versions");
            release_versions = jsonReader.Import_version("release");
            All_versions = jsonReader.Import_version("all");
            spigot_versions = htmlReader.Get_SpigotVers();
            
            new_Version = func.Init_new_Versions(new_Version, release_versions);
            new_Version.SelectedIndex = 0;
            Pd.Message = "Read the new Versions";
            Pd.Value = 80;

            if (GUI)
            {
                //Versionの選択
                Version = func.Init_version(Version);
                World_reload();
                // if (Version.Text == "【new Version】")
                // {
                //     version_main.Visibility = Visibility.Hidden;
                //     select_version.Visibility = Visibility.Visible;
                //     version_hide.Visibility = Visibility.Visible;
                //     Version2.SelectedIndex = Version2.Items.IndexOf("【new Version】");
                // }
                // data.Set_Version(Version, new_Version.Text);
                Pd.Message = "Read the local Versions";
                Pd.Value = 90;

                //Worldの選択
                data.Set_SW();
                World = func.Init_world(World);
                Name_reload(null, null);
                // data.Set_World(World);
                // if (World.Text == "【new World】")
                // {
                //     world_hide.Visibility = Visibility.Visible;
                //     World2.SelectedIndex = World2.Items.IndexOf("【new World】");
                //     world_main.Visibility = Visibility.Hidden;
                // }
                Pd.Message = "Read the local Worlds";
            }
            Pd.Value = 100;
            Pd.Close();
        }

        public void Start(bool gui=true)
        {
            logger.Debug("------------------------------------------------------------");
            logger.Info("Start the Server Opening");
            Pd = new ProgressDialog
            {
                Title = $"the server {Data_list.ReadVersion}/{Data_list.World}"
            };
            Pd.Show();

            dynamic start_func;
            if (Data_list.Import_spigot)
            {
                start_func = new Spigot_Function();
            }
            else
            {
                start_func = new Functions();
            }

            start_func.Main = this;

            bool show = start_func.Define_OpenWorld(World, gui);
            if(!show && GUI)
            {
                Pd.Close();
                logger.Info("Show MainWindow again");
                Show();
                return;
            }
            Pd.Value = 10;
            Pd.Message = "Define opening World";

            //バージョンについて分岐
            if (Data_list.Import_NewVersion)
            {
                // GUIでは取得していない場合に新規バージョンを追加できないようになっているが、コマンド側から実行された際にエラーを吐くようにしている
                if (!Set_new_Version)
                {
                    string message =
                        "Minecraftのバージョン一覧を取得していないため、新規バージョンの導入ができません。\n" +
                        $"新規バージョンの導入を行うためにはServer Starterを再起動してください。\n\n";
                    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentException("Can not import new Version's server");
                }
                Pd.Message = "Download server.jar";
                jsonReader.Import_server();
            }
            else if (Data_list.ReadVersion != Data_list.Info[2])
            {
                //infoのVersionを書き換え
                // ShareWorld内のinfoにバージョンを記録する作業はCheck_ShareWorld()で行う
                start_func.Change_info(2);
            }
            Pd.Value = 20;

            //start_func.Check_copy_world();
            bool world_copy = start_func.Check_Vdown();
            logger.Info($"Check copy world (Copy is '{world_copy}')");
            if (world_copy)
                start_func.Copy_World();
            Pd.Value = 30;
            Pd.Message = "Check copy world";

            //新バージョン導入時は必ずpropertiesの書き換えが入るため、バージョンの分岐とは関係なく設置
            start_func.Wirte_properties();
            Pd.Value = 40;
            Pd.Message = "Write Properties for server.properties";

            //ShareWorldの存在確認や起動済みのサーバーがないかなどを確認
            if (Data_list.World == "ShareWorld")
            {
                shareworld.Check_ShareWorld();
                //start_func.Check_ShareWorld();
                Pd.Value = 60;
                Pd.Message = "Finish the process of ShareWorld";
            }

            start_func.Reset_world_method(Reset_world, Save_world);
            Pd.Value = 70;
            Pd.Message = "Check reset and save the world data";


            start_func.Import_datapack(m_set_window);
            start_func.Import_World(m_set_window);
            start_func.Import_plugins(m_set_window);
            Pd.Value = 80;
            Pd.Message = "Check the Datapack, CustomWorld and plugins";

            start_func.Check_op();
            Pd.Value = 90;
            Pd.Message = $"Check ops.josn ({Data_list.Info[0]} has op rights, or not)";

            Pd.Value = 100;
            Pd.Message = $"Start the Server {Data_list.Version}/{Data_list.World}";
            Pd.Close();
            // pd.Dispose();

            // これをStart_server()の中に入れると、新規バージョン導入の際にワールドの設定を反映しないままサーバーが起動してしまう
            if (!File.Exists($@"{Data_Path}\{Data_list.ReadVersion}\eula.txt"))
            {
                start_func.Start_server();
                start_func.Change_eula();
            }
            start_func.Start_server();

            if (Data_list.World == "ShareWorld")
                shareworld.Upload_ShareWorld();
                // start_func.Upload_ShareWorld();

            start_func.Write_VW();

            //GUIの終了
            logger.Info("This System is successfully over");
            start_func.Shutdown();
            Close();
            Application.Current.Shutdown();
        }

        private void START_Click(object sender, RoutedEventArgs e)
        {
            //MAINを閉じる
            Hide();
            Start();
        }

        private void More_Settings_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            m_set_window.Main = this;
            m_set_window.Set_value();
            m_set_window.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Starter was pushed Close Button");
            Application.Current.Shutdown();
            logger.Info("This System is successfully over");
        }

        private void World_reload(object sender, EventArgs e)
        {
            // 新バージョンがインストールされ、propertiesに不足項目があったとしても自動補完されるため、More_Settings_buttonを常に表示する仕様に変更

            World_reload();

            // ShareWorldなど、バージョンの変更を反映する必要がある設定が存在するため
            data.Set_World(World, input_box_world.Text);
        }

        /// <summary>
        /// 初期読み込み用
        /// </summary>
        private void World_reload()
        {
            if (Version.Text == "【new Version】" || Version2.Text == "【new Version】")
            {
                version_hide.Visibility = Visibility.Visible;
                Version2.SelectedIndex = Version2.Items.IndexOf("【new Version】");
                //new Versionでないほうの表示項目は仮置きしておき、選択された際に決定する
                Version.SelectedIndex = -1;
                version_main.Visibility = Visibility.Hidden;
            }
            else if (Version.SelectedIndex == -1)
            {
                version_main.Visibility = Visibility.Visible;
                Version.SelectedIndex = Version2.SelectedIndex;
                version_hide.Visibility = Visibility.Hidden;
            }

            if (world_hide.Visibility == Visibility.Visible)
            {
                World2.SelectedIndex = World2.Items.IndexOf("【new World】");
            }

            data.Set_Version(Version, new_Version.Text);
            m_set_window = new More_Settings();
            m_set_window.Read_properties();
        }

        private void Name_reload(object sender, EventArgs e)
        {
            if (World.Text == "【new World】")
            {
                world_hide.Visibility = Visibility.Visible;
                World2.SelectedIndex = World2.Items.IndexOf("【new World】");
                //new Worldでないほうの表示項目は仮置きしておき、選択された際に決定する
                World.SelectedIndex = -1;
                world_main.Visibility = Visibility.Hidden;
            }
            else if (World2.Text != "【new World】")
            {
                world_hide.Visibility = Visibility.Hidden;
                world_main.Visibility = Visibility.Visible;
                //new Worldを介した操作に限定
                if (World.SelectedIndex == -1)
                {
                    //仮置きの内容をWorld2で選択されたものに決定
                    World.SelectedIndex = World.Items.IndexOf(World2.Text);
                }

                // ワールド名を新規でないものにする場合はRUNボタンなどを有効に戻す
                if (!Run_button.IsEnabled)
                {
                    Run_button.IsEnabled = true;
                    More_Settings_button.IsEnabled = true;
                    input_box_world.Text = "input_name";
                }
            }

            data.Set_World(World, input_box_world.Text);
        }

        // private void Version_Click(object sender, RoutedEventArgs e)
        // {
        //     // 選択がなくなってしまった場合に落ちるバグを修正
        //     if (release.IsChecked == false && snapshot.IsChecked == false)
        //     {
        //         release.IsChecked = true;
        //     }

        //     string selected_item = new_Version.SelectedItem.ToString();
        //     new_Version.Items.Clear();

        //     //Versionのチェックボックスで選択されているものに応じて追加バージョン候補の表示を変える
        //     if (release.IsChecked == true && snapshot.IsChecked == false)
        //     {
        //         new_Version = func.Init_new_Versions(new_Version, release_versions);
        //     }
        //     if (snapshot.IsChecked == true && release.IsChecked == false)
        //     {
        //         new_Version = func.Init_new_Versions(new_Version, snapshot_versions);
        //     }
        //     if (release.IsChecked == true && snapshot.IsChecked == true)
        //     {
        //         new_Version = func.Init_new_Versions(new_Version, All_versions);
        //     }

        //     new_Version.SelectedIndex = (new_Version.Items.IndexOf(selected_item) == -1) ? 0 : new_Version.Items.IndexOf(selected_item);
        // }

        private void VersionToggle(object sender, RoutedEventArgs e)
        {
            string selected_item = new_Version.SelectedItem.ToString();
            new_Version.Items.Clear();

            if (toggleVersions.IsOn)
            {
                new_Version = func.Init_new_Versions(new_Version, All_versions);
            }
            else
            {
                new_Version = func.Init_new_Versions(new_Version, release_versions);
            }

            new_Version.SelectedIndex = (new_Version.Items.IndexOf(selected_item) == -1) ? 0 : new_Version.Items.IndexOf(selected_item);
        }

        private void Delete_version(object sender, RoutedEventArgs e)
        {
            MessageBoxResult? result = MW.MessageBox.Show($"このバージョンを削除しますか？\r\n「{Data_list.ReadVersion}」とその内部に保管されたワールドデータは完全に削除され、復元ができなくなります。", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            logger.Warn("Warning the delete Version data");
            if (result == MessageBoxResult.OK)
            {
                Directory.Delete($@"{Data_Path}\{Data_list.ReadVersion}\", true);
                Version.Items.Remove(Data_list.ReadVersion);

                // 削除したバージョンのワールドも表示（＆内部データ）から消す
                Data_list.VerWor_list[Data_list.ReadVersion].ForEach(t => World.Items.Remove($"{Data_list.ReadVersion}/{t}"));
                Data_list.VerWor_list.Remove(Data_list.ReadVersion);

                logger.Info($"The Version {Data_list.ReadVersion} was successfully deleted");


                int before_index_ver = Version.SelectedIndex;
                int before_index_wor = World.SelectedIndex;
                Version.SelectedIndex = (before_index_ver - 1 >= 0) ? before_index_ver - 1 : 0;
                World.SelectedIndex = (before_index_wor - 1 >= 0) ? before_index_wor - 1 : 0;

                World_reload(null, null);
                Name_reload(null, null);
            }
        }

        private void Delete_world(object sender, RoutedEventArgs e)
        {
            MessageBoxResult? result;
            if (Data_list.World == "ShareWorld")
            {
                logger.Warn("Warning the delete ShareWorld");
                result = MW.MessageBox.Show("ShareWorldの内容を変更する場合は削除する必要はなく、メイン画面のRecreateにチェックを入れてください。\r\nこれを理解したうえで削除しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                logger.Warn("Warning the delete World data");
                result = MW.MessageBox.Show($"このワールドを削除しますか？\r\n「{Data_list.World}」は完全に削除され、復元ができなくなります。", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }

            if (result == MessageBoxResult.OK)
            {
                int before_index = World.SelectedIndex;
                if (Data_list.CopyVer_IsSpigot)
                {
                    Directory.Delete($@"{Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}\", true);
                    Directory.Delete($@"{Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}_nether\", true);
                    Directory.Delete($@"{Data_Path}\Spigot_{Data_list.Copy_version}\{Data_list.World}_the_end\", true);
                }
                else
                {
                    // 削除するワールドのバージョンをVersion.Textにしていると、上記の1.17.1ではないバージョンのワールドが削除される
                    //  ->copy_versionを使う
                    Directory.Delete($@"{Data_Path}\{Data_list.Copy_version}\{Data_list.World}\", true);
                }

                World.Items.Remove($"{Data_list.ReadCopy_Version}/{Data_list.World}");
                Data_list.VerWor_list[Data_list.ReadCopy_Version].Remove(Data_list.World);

                logger.Info($"The World {Data_list.World} was successfully deleted");

                World.SelectedIndex = (before_index - 1 >= 0) ? before_index - 1 : 0;
            }
        }

        private void Check_WorldName(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // 起動時にMainWindow.xamlを上から読んでいくと、RunボタンはWorld_nameよりも後の一番最後に読み込まれるため、NullExeptionを回避するために入れている
            if (Run_button == null)
            {
                return;
            }

            if(!Regex.IsMatch(input_box_world.Text, @"^[0-9a-zA-Z-_]+$"))
            {
                More_Settings_button.IsEnabled = false;
                Run_button.IsEnabled = false;
            }
            else
            {
                More_Settings_button.IsEnabled = true;
                Run_button.IsEnabled = true;
            }

            Data_list.World = input_box_world.Text;
        }

        private void Op_Click(object sender, RoutedEventArgs e)
        {
            if (op.IsChecked == true)
            {
                Get_op = true;
            }
            else
            {
                Get_op = false;
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            if (shutdown.IsChecked == true)
            {
                Properties.Settings.Default.Shutdown = true;
            }
            else
            {
                Properties.Settings.Default.Shutdown = false;
            }
            Properties.Settings.Default.Save();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Infomation _info = new Infomation();
            _info.ShowDialog();
            Show();
        }

        private void Save_world_Click(object sender, RoutedEventArgs e)
        {
            //そもそもresetにチェックがついていないときはバックアップを生成しない。
            Save_world = sa_world.IsEnabled != false && Save_world;
        }

        private void Reset_world_Click(object sender, RoutedEventArgs e)
        {
            Reset_world = re_world.IsChecked == true;
            sa_world.IsEnabled = re_world.IsChecked == true;
            sa_world.Foreground = (re_world.IsChecked == true) ? Brushes.Black : Brushes.LightGray;
            if (re_world.IsChecked == false)
            {
                sa_world.IsChecked = false;
                Save_world = false;
            }
        }

        private void TF_spigot(object sender, RoutedEventArgs e)
        {
            new_Version.Items.Clear();

            if (Toggle_spigot.IsOn == true)
            {
                Data_list.Import_spigot = true;
                toggleVersions.IsEnabled = false;
                foreach(var spi_ver in spigot_versions)
                {
                    new_Version.Items.Add(spi_ver);
                }
            }
            else
            {
                Data_list.Import_spigot = false;
                toggleVersions.IsEnabled = true;
                if (toggleVersions.IsOn)
                {
                    new_Version = func.Init_new_Versions(new_Version, All_versions);
                }
                else
                {
                    new_Version = func.Init_new_Versions(new_Version, release_versions);
                }
            }

            new_Version.SelectedIndex = 0;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class DataSource : IDataErrorInfo
    {
        //IDはDataContexのプロパティ
        public String ID { get; set; }

        // 今回は使わないが、IDataErrorInfoインターフェースでは実装しなければならない
        public string Error { get { return null; } }

        // これも実装必須のプロパティで、各プロパティに対応するエラーメッセージを返す
        public string this[string propertyName]
        {
            get
            {
                string result = null;

                switch (propertyName)
                {
                    case "ID":
                        // if (!Regex.IsMatch(ID, @"[1-9]\.[0-9]+\.[0-9]+") && !Regex.IsMatch(ID, "[0-9]+(?i)[A-Z][0-9]+(?i)[A-Z]"))
                        // {
                        //     result = "Please enter the Version as 1.16.5 or 21w10a";
                        //     break;
                        // }
                        if (!Regex.IsMatch(ID, @"^[0-9a-zA-Z_-]+$"))
                        {
                            result = @"You can use only capital and small letters";
                            break;
                        }
                        break;
                }
                return result;
            }
        }
    }

    [JsonObject("Ops")]
    class Ops
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("bypassesPlayerLimit")]
        public bool BypassesPlayerLimit { get; set; }
    }


    [JsonObject("All_VW")]
    class All_VW
    {
        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("versions")]
        public List<All_VW_Ver> Versions { get; set; }
        
        [JsonProperty("all_vers")]
        public List<string> All_vers { get; set; }
    }

    [JsonObject("All_VW_Ver")]
    class All_VW_Ver
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("worlds")]
        public List<All_VW_Wor> Worlds { get; set; }
    }

    [JsonObject("All_VW_Wor")]
    class All_VW_Wor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
