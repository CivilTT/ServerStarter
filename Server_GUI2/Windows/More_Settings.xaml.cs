using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Server_GUI2
{
    /// <summary>
    /// More_Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class More_Settings : Window
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow Main { get; set; }
        // private readonly Functions func = new Functions();

        public Dp_Settings Dp_window { get; set; }
        public haihu Haihu_window { get; set; }
        public Spigot Spigot_window { get; set; }
        private readonly Data_list data = new Data_list();

        private SortedDictionary<string, string> tmp_properites;


        public More_Settings()
        {
            InitializeComponent();
        }

        public void Read_properties()
        {
            //Worldのバージョンを渡してパスを書き直す
            string read_version = Data_list.ReadVersion;

            // new Versionが選択された際には、前回の起動バージョンのpropertiesを読み込む
            if (!Directory.Exists($@"{MainWindow.Data_Path}\{read_version}"))
            {
                logger.Info($"Read the server.properties at {Data_list.Info[2]}");
                read_version = Data_list.Info[2];
            }
            if (!File.Exists($@"{MainWindow.Data_Path}\{read_version}\server.properties"))
            {
                // それでもファイルが存在していない場合は、Data_listに初期登録されたserver.propertiesを使う
                logger.Warn("Read the Default server.properties in this system.");
                return;
            }
            
            logger.Info($@"Read the server.properties at {MainWindow.Data_Path}\{read_version}\server.properties");
            using (StreamReader sr = new StreamReader($@"{MainWindow.Data_Path}\{read_version}\server.properties", Encoding.GetEncoding("Shift_JIS")))
            {
                data.Set_properties(sr);

                //【デバッグ】
                string moji = "";
                //Dictionaryをまわす
                foreach (string key in Data_list.Server_Properties.Keys)
                {
                    moji = moji + "key=" + key + ",val=" + Data_list.Server_Properties[key] + "  ";
                }
                //結果をコンソールに出力する
                // Console.WriteLine("propertiesの中身は「{0}」です。", moji);

            }
        }

        public void Set_value()
        {
            logger.Info("set_value method");

            SetGUI();


            // GUIでの変更を保存するための仕掛け
            // 何も考えずにtmp_properties = Data_list.Server_Propertiesとすると「参照渡し」になってしまい、tmp_propertiesの変更がData_list.Server_Propertiesにも反映されてしまう。
            // 「参照渡し」ではなく通常の代入のようなものは「値渡し」という
            tmp_properites = new SortedDictionary<string, string>(Data_list.Server_Properties);

            string[] difficulty_list = new string[] { "easy", "normal", "hard", "peaceful" };
            string[] gamemode_list = new string[] { "adventure", "creative", "spectator", "survival" };
            string[] true_false_list = new string[] { "true", "false" };
            //other_settingsのリストを作る
            List<string> other_settings_TF_1 = new List<string>();
            List<string> other_settings_last_1 = new List<string>();

            foreach (string key in Data_list.Server_Properties.Keys)
            {
                if (key == "level-name" || key == "difficulty" || key == "gamemode" || key == "hardcore" || key == "force-gamemode" || key == "white-list" || key == "enforce-whitelist" || key.Contains("#"))
                {
                    continue;
                }
                if (Data_list.Server_Properties[key] == "true" || Data_list.Server_Properties[key] == "false")
                {
                    other_settings_TF_1.Add(key);
                }
                else
                {
                    other_settings_last_1.Add(key);
                }
            }
            //型を合わせるための処理
            string[] other_settings_TF = new string[other_settings_TF_1.Count];
            string[] other_settings_last = new string[other_settings_last_1.Count];
            for (int i = 0; i < other_settings_TF_1.Count; i++)
            {
                other_settings_TF[i] = other_settings_TF_1[i];
            }
            for (int i = 0; i < other_settings_last_1.Count; i++)
            {
                other_settings_last[i] = other_settings_last_1[i];
            }

            //MAIN Settings
            Register_combo(difficulty, difficulty_list, Data_list.Server_Properties["difficulty"]);
            Register_combo(hardcore, true_false_list, Data_list.Server_Properties["hardcore"]);
            Register_combo(gamemode, gamemode_list, Data_list.Server_Properties["gamemode"]);
            Register_combo(force_gamemode, true_false_list, Data_list.Server_Properties["force-gamemode"]);
            Register_combo(white_list, true_false_list, Data_list.Server_Properties["white-list"]);
            Register_combo(enforce_white_list, true_false_list, Data_list.Server_Properties["enforce-whitelist"]);

            //OTHER Settings
            Register_combo(true_false, other_settings_TF, other_settings_TF[0]);
            Register_combo(true_false_combo, true_false_list, Data_list.Server_Properties[other_settings_TF[0]]);
            Register_combo(input_text, other_settings_last, other_settings_last[0]);
            input_text_txt.Text = Data_list.Server_Properties[other_settings_last[0]];
        }

        private void SetGUI()
        {
            // GUIでの表示を設定
            m_Version.Text = Data_list.Version;
            m_World.Text = Data_list.World;
            Json.Content = $@"Get All-VerWor.json at {Directory.GetCurrentDirectory()}\All-VerWor.json";
            Json.IsChecked = Properties.Settings.Default.Output_VW;
            import_World.IsEnabled = Data_list.Import_NewWorld;
            Plugins.IsEnabled = Data_list.Import_spigot;
            TF_Spigot.Text = Data_list.Import_spigot ? "Yes" : "No";
            // Toggle_spigot.IsOn = Data_list.Import_spigot;
        }

        private void Register_combo(System.Windows.Controls.ComboBox name_combo, string[] index_list, object IndexOf)
        {
            // More_Settingsを複数回呼び出した場合、すでにCombo_Boxには値が入っているため、項目を追加しない
            if(name_combo.Items.Count == 0)
            {
                foreach (string list in index_list)
                {
                    name_combo.Items.Add(list);
                }
            }

            name_combo.SelectedIndex = name_combo.Items.IndexOf(IndexOf);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Click the Cancel Button");

            //変数の破棄（これにより再度More_Settingsを開いても変数被りのエラーを回避する）
            // properties = null;
            Hide();
            Main.Show();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Click the OK Button");

            // GUIでの変更を登録する
            Data_list.Server_Properties = tmp_properites;
            
            //startを呼び出す
            Hide();
            Main.Show();
        }

        private void TF_reload(object sender, EventArgs e)
        {
            true_false_combo.SelectedIndex = true_false_combo.Items.IndexOf(Data_list.Server_Properties[true_false.Text]);
            logger.Info($"Change the display of propertie's selection about [{true_false.Text} = {true_false_combo.Text}]");
        }

        private void IT_reload(object sender, EventArgs e)
        {
            input_text_txt.Text = Data_list.Server_Properties[input_text.Text];
            logger.Info($"Change the display of propertie's selection about [{input_text.Text} = {input_text_txt.Text}]");
        }

        private void Difficulty_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [difficulty = {difficulty.Text}]");
            tmp_properites["difficulty"] = difficulty.Text;
        }

        private void Hardcore_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [hardcore = {hardcore.Text}]");
            tmp_properites["hardcore"] = hardcore.Text;
        }

        private void Gamemode_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [gamemode = {gamemode.Text}]");
            tmp_properites["gamemode"] = gamemode.Text;
        }

        private void Force_gamemode_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [force-gamemode = {force_gamemode.Text}]");
            tmp_properites["force-gamemode"] = force_gamemode.Text;
        }

        private void White_list_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [white-list = {white_list.Text}]");
            tmp_properites["white-list"] = white_list.Text;
        }

        private void Enforece_gamemode_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [enfoce-whitelist = {enforce_white_list.Text}]");
            tmp_properites["enforce-whitelist"] = enforce_white_list.Text;
        }

        private void TF_CB(object sender, EventArgs e)
        {
            logger.Info($"Change the propertie about [{true_false.Text} = {true_false_combo.Text}]");
            tmp_properites[true_false.Text] = true_false_combo.Text;
        }

        private void IT_TB(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            logger.Info($"Change the propertie about [{input_text.Text} = {input_text_txt.Text}]");
            tmp_properites[input_text.Text] = input_text_txt.Text;
        }

        private void Import_click(object sender, RoutedEventArgs e)
        {
            Hide();
            if (Haihu_window == null || Haihu_window.Version.Text != Data_list.ReadVersion || Haihu_window.World.Text != Data_list.World)
                Haihu_window = new haihu();

            Haihu_window.m_settings = this;
            Haihu_window.ShowDialog();
        }

        private void Dp_click(object sender, RoutedEventArgs e)
        {
            Hide();
            if (Dp_window == null || Dp_window.Version.Text != Data_list.ReadVersion || Dp_window.World.Text != Data_list.World)
                Dp_window = new Dp_Settings();

            Dp_window.m_settings = this;
            Dp_window.Display();
        }

        private void Spigot_click(object sender, RoutedEventArgs e)
        {
            Hide();
            // Pluginはワールドに関係ないため他とは変えている
            if (Spigot_window == null || Spigot_window.Version.Text != Data_list.ReadVersion)
                Spigot_window = new Spigot();

            Spigot_window.MoreSetting = this;
            Spigot_window.Display();
        }

        private void Json_Click(object sender, RoutedEventArgs e)
        {
            if (Json.IsChecked == true)
            {
                Properties.Settings.Default.Output_VW = true;
            }
            else
            {
                Properties.Settings.Default.Output_VW = false;
            }
            Properties.Settings.Default.Save();
        }

        // private void TF_spigot(object sender, RoutedEventArgs e)
        // {
        //     if (Toggle_spigot.IsOn == true)
        //     {
        //         Plugins.IsEnabled = true;
        //         Data_list.Import_spigot = true;
        //     }
        //     else
        //     {
        //         Plugins.IsEnabled = false;
        //         Data_list.Import_spigot = false;
        //     }
        // }
    }
}
