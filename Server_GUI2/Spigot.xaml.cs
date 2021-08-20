using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace Server_GUI2
{
    /// <summary>
    /// Spigot.xaml の相互作用ロジック
    /// </summary>
    public partial class Spigot : Window
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Functions func = new Functions();

        public More_Settings m_settings { get; set; }
        public bool import_plugin { get; set; }

        private List<string> existed_list = new List<string>();
        private List<string> add_list = new List<string>();
        private List<string> remove_list = new List<string>();


        public Spigot()
        {
            InitializeComponent();

            Version.Text = Data_list.Version;
            World.Text = Data_list.World;
            import_plugin = false;

            Set_imported();
        }

        private void Set_imported()
        {
            if (!Directory.Exists($@"{MainWindow.Data_Path}\" + $"\"Spigot_{Data_list.Version}\"" + @"\plugins\"))
            {
                Imported.Items.Add("(None)");
                return;
            }

            string[] plugins = Directory.GetFiles($@"{MainWindow.Data_Path}\" + $"\"Spigot_{Data_list.Version}\"" + @"\plugins\");

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

                // FileNameで選択されたフォルダを取得する
                add_list.Add(cofd.FileName);
                string datapack_name = Path.GetFileName(cofd.FileName);
                Imported.Items.Remove("(None)");
                Imported.Items.Add("【new】" + datapack_name);
            }
        }

        private void Remove_click(object sender, RoutedEventArgs e)
        {
            object selected_data = Imported.SelectedItem;
            if (selected_data == null)
            {
                System.Windows.Forms.MessageBox.Show($"Importedより削除したいpluginを選択してください。", "Server Starter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (selected_data.ToString() == "(None)")
            {
                return;
            }

            DialogResult result = System.Windows.Forms.MessageBox.Show($"{selected_data} を削除しますか？", "Server Starter", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
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
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Click the OK Button");
            // Okを押したときにはpluginsの作業は行わず、Runが入り、propertiesの編集が終わったあたりで、コピーなどの作業を行う
            import_plugin = true;
            Hide();
            m_settings.Show();
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
                    func.Error(ex.Message);
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

            m_settings.Show();
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
            }
            ShowDialog();
        }
    }
}
