using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using MW = ModernWpf;


namespace Server_GUI2
{
    /// <summary>
    /// Spigot.xaml の相互作用ロジック
    /// </summary>
    public partial class Spigot : Window
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public More_Settings MoreSetting { get; set; }
        public bool Import_plugin { get; set; }


        private readonly List<string> existed_list = new List<string>();
        private readonly List<string> add_list = new List<string>();
        private readonly List<string> remove_list = new List<string>();


        public Spigot()
        {
            InitializeComponent();

            Version.Text = Data_list.ReadVersion;
            World.Text = Data_list.World;
            Import_plugin = false;

            Set_imported();
        }

        private void Set_imported()
        {
            string plugin_path = $@"{MainWindow.Data_Path}\{Data_list.ReadCopy_Version}\plugins\";

            if (!Directory.Exists(plugin_path))
            {
                Imported.Items.Add("(None)");
                Remove.IsEnabled = false;
                return;
            }

            string[] plugins = Directory.GetFiles(plugin_path);

            foreach (string key in plugins)
            {
                string _key = Path.GetFileName(key);
                existed_list.Add(_key);
            }
        }

        private void Import_click(object sender, RoutedEventArgs e)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "jarファイルを選択してください",
                InitialDirectory = $@"{MainWindow.Data_Path}",
                // フォルダ選択モードにしない
                IsFolderPicker = false,
            })
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                if(Path.GetExtension(cofd.FileName) != ".jar")
                {
                    MW.MessageBox.Show("Pluginとして無効なファイルです。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // FileNameで選択されたフォルダを取得する
                add_list.Add(cofd.FileName);
                string datapack_name = Path.GetFileName(cofd.FileName);
                Imported.Items.Remove("(None)");
                Remove.IsEnabled = true;
                Imported.Items.Add("【new】" + datapack_name);
            }
        }

        private void Remove_click(object sender, RoutedEventArgs e)
        {
            object selected_data = Imported.SelectedItem;
            if (selected_data == null)
            {
                MW.MessageBox.Show($"Importedより削除したいpluginを選択してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected_data.ToString() == "(None)")
            {
                return;
            }

            MessageBoxResult? result = MW.MessageBox.Show($"{selected_data} を削除しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Imported.Items.Remove(selected_data);
                if (selected_data.ToString().Contains("【new】"))
                {
                    // add_listの構成要素がdata_pathに順に入るため、これの中から削除しようとしているpluginの名前を探し、削除する
                    add_list.RemoveAll(data_path => data_path.Contains(selected_data.ToString().Substring(4)));
                }
                else
                {
                    remove_list.Add(selected_data.ToString());
                }
            }

            if (Imported.Items.Count == 0)
            {
                Imported.Items.Add("(None)");
                Remove.IsEnabled = false;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Click the OK Button");
            // Okを押したときにはpluginsの作業は行わず、Runが入り、propertiesの編集が終わったあたりで、コピーなどの作業を行う
            Import_plugin = true;

            // 再表示のためにGUI上の表示はリセットしておく
            Imported.Items.Clear();

            Hide();
            MoreSetting.Show();
        }

        public void Add_data()
        {
            logger.Info("Import new plugins");
            foreach (string key in add_list)
            {
                try
                {
                    File.Copy(key, $@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\plugins\{Path.GetFileName(key)}");
                }
                catch (Exception ex)
                {
                    string message =
                        "プラグインの導入に失敗しました。\n" +
                        "導入を行わない状態でサーバーを起動します。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    logger.Warn($"Failed to import the plugin (Error Message : {ex.Message})");
                }
            }
        }

        public void Remove_data()
        {
            logger.Info("Remove the plugins");
            foreach (string key in remove_list)
            {
                File.Delete($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\plugins\{key}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Click the Cancel Button");
            Hide();

            // 処理をリセットして戻す
            Imported.Items.Clear();
            add_list.Clear();
            remove_list.Clear();

            MoreSetting.Show();
        }

        public void Display()
        {
            foreach (string key in existed_list)
            {
                Imported.Items.Add(key);
            }
            foreach (string key in add_list)
            {
                Imported.Items.Add("【new】" + Path.GetFileName(key));
            }
            foreach (string key in remove_list)
            {
                Imported.Items.Remove(key);
            }

            if (Imported.Items.Count == 0)
            {
                Imported.Items.Add("(None)");
                Remove.IsEnabled = false;
            }
            ShowDialog();
        }
    }
}
