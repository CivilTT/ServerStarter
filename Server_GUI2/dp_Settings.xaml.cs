using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using MW = ModernWpf;


namespace Server_GUI2
{
    /// <summary>
    /// dp_Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class Dp_Settings : Window
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Functions func = new Functions();

        private readonly string Data_Path = MainWindow.Data_Path;
        private string data_kinds = "ZIP";
        private List<string> existed_list = new List<string>();
        private List<string> add_list = new List<string>();
        private List<string> remove_list = new List<string>();

        public More_Settings m_settings { get; set; }
        public bool import_dp { get; set; }


        public Dp_Settings()
        {
            InitializeComponent();

            Version.Text = Data_list.Version;
            World.Text = Data_list.World;
            import_dp = false;

            Set_imported();

            zip.IsChecked = true;
        }

        private void Set_imported()
        {
            if (!Directory.Exists($@"{Data_Path}\{Data_list.Version}\{Data_list.World}\datapacks\"))
            {
                Imported.Items.Add("(None)");
                return;
            }
            
            string[] datapacks_zip = Directory.GetFiles($@"{Data_Path}\{Data_list.Version}\{Data_list.World}\datapacks\");
            string[] datapacks_dir = Directory.GetDirectories($@"{Data_Path}\{Data_list.Version}\{Data_list.World}\datapacks\");

            foreach (string key in datapacks_zip)
            {
                string _key = Path.GetFileName(key);
                existed_list.Add(_key);
            }
            foreach (string key in datapacks_dir)
            {
                string _key = Path.GetFileName(key);
                existed_list.Add(_key);
            }
        }

        private void Zip_Checked(object sender, RoutedEventArgs e)
        {
            folder.IsChecked = false;
            data_kinds = "ZIP";
        }

        private void Folder_Checked(object sender, RoutedEventArgs e)
        {
            zip.IsChecked = false;
            data_kinds = "FOLDERS";
        }

        private void Import_click(object sender, RoutedEventArgs e)
        {
            bool folder_pick = false;
            if (data_kinds == "ZIP")
            {
                folder_pick = false;
            }
            else if (data_kinds == "FOLDERS")
            {
                folder_pick = true;
            }
            
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                InitialDirectory = $@"{Data_Path}\{Version.Text}\{World.Text}\datapacks",
                // フォルダ選択モードにする
                IsFolderPicker = folder_pick,
            })
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                // datapackとして有効かを確認
                bool? result = Check_valid(cofd.FileName);
                if (result == false)
                {
                    MW.MessageBox.Show($"この{(data_kinds == "ZIP" ? "ファイル" : "フォルダ")}はデータパックとして無効です。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (result == null)
                {
                    return;
                }

                // FileNameで選択されたフォルダを取得する
                string datapack_name = Path.GetFileName(cofd.FileName);
                Imported.Items.Remove("(None)");
                Imported.Items.Add("【new】"+datapack_name);
                // MessageBox.Show($"{cofd.FileName}を選択しました");
            }
        }

        private bool? Check_valid(string file_path)
        {
            // // フォルダ（ファイル）の直下にdataフォルダとpack.mcmetaが存在しているかを確認する
            // if (Directory.Exists($@"{file_path}\data") && File.Exists($@"{file_path}\pack.mcmeta"))
            // {
            //     logger.Info($"{file_path} is a valid folder as Datapack");
            //     return true;
            // }
            // if (System.IO.Path.GetExtension(file_path) != ".zip")
            // {
            //     logger.Info($"{file_path} is a invalid file as Datapack");
            //     return false;
            // }

            // string file_name = System.IO.Path.GetFileNameWithoutExtension(file_path);
            // using (ZipArchive a = ZipFile.OpenRead(file_path))
            // {
            //     //「dir/1.txt」のZipArchiveEntryを取得する
            //     ZipArchiveEntry e = a.GetEntry("pack.mcmeta");
            //     ZipArchiveEntry f = a.GetEntry($"{file_name}/pack.mcmeta");
            //     if (e != null || f != null)
            //     {
            //         logger.Info($"{file_path} is a valid file as Datapack");
            //         return true;
            //     }
            // }

            string file_extension = Path.GetExtension(file_path);
            // そもそもzipやフォルダでないものははじく
            bool isDirectory = File.GetAttributes(file_path).HasFlag(FileAttributes.Directory)
;
            if (file_extension != ".zip" && !isDirectory)
            {
                logger.Info($"{file_path} is a invalid file as Datapack (Extent : {file_extension} is invalid)");
                return false;
            }
            else if (file_extension == ".zip")
            {
                // Zipを展開
                string extract_path = Path.GetDirectoryName(file_path) + @"\" + Path.GetFileNameWithoutExtension(file_path);
                if (Directory.Exists(extract_path))
                {
                    MessageBoxResult? result = MW.MessageBox.Show($"以下の場所に展開先のフォルダと同名のフォルダが存在しています。\n同名のフォルダを展開フォルダで上書きしますか？\n\n【場所（展開先）】\n{extract_path}", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Directory.Delete(extract_path, true);
                    }
                    else
                    {
                        MW.MessageBox.Show("展開をしなかったため、データパックの導入ができませんでした。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
                        return null;
                    }
                }
                ZipFile.ExtractToDirectory(file_path, extract_path);
                file_path = extract_path;
            }

            // フォルダの直下(or一つ下)に pack.mcmeta & dataフォルダ が存在しているかを確認する
            if (Directory.Exists($@"{file_path}\data") && File.Exists($@"{file_path}\pack.mcmeta"))
            {
                logger.Info($"{file_path} is a valid file as Datapack");
                add_list.Add(file_path);
                return true;
            }
            foreach (string folder_path in Directory.GetDirectories(file_path))
            {
                if (Directory.Exists($@"{folder_path}\data") && File.Exists($@"{folder_path}\pack.mcmeta"))
                {
                    logger.Info($"{folder_path} is a valid file as Datapack");
                    add_list.Add(folder_path);
                    return true;
                }
            }

            logger.Info($"{file_path} is a invalid zip file as Datapack (There are not data folder & pack.mcmeta)");
            return false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Click the OK Button");
            // Okを押したときにはdatapackの作業は行わず、Runが入り、propertiesの編集が終わったあたりで、コピーなどの作業を行う
            // ShareWorldの新規導入などで衝突が起こったとしても、ファイルが上書きされるだけのため、問題なし
            import_dp = true;

            // 再表示のためにGUI上の表示はリセットしておく
            Imported.Items.Clear();

            Hide();
            m_settings.Show();
        }

        public void Add_data()
        {
            logger.Info("Import new datapack");
            foreach(string key in add_list)
            {
                Process p = Process.Start("xcopy", $@"{key} {Data_Path}\{Data_list.Version}\{Data_list.World}\datapacks\{Path.GetFileName(key)} /E /H /I /Y");
                p.WaitForExit();

                if(p.ExitCode != 0)
                {
                    string message = 
                        "datapackの導入に失敗しました。\n" +
                        "このdatapackの導入をせずにサーバーを起動します。\n\n" +
                        $"【失敗したデータパック名】  {Path.GetFileName(key)}";
                    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    logger.Warn($"Failed to import datapack ({Path.GetFileName(key)})");
                }
            }
        }

        public void Remove_data()
        {
            logger.Info("Remove the datapack");
            foreach(string key in remove_list)
            {
                string _path = $@"{Data_Path}\{Data_list.Version}\{Data_list.World}\datapacks\{key}";
                string extend = Path.GetExtension(key);
                if (extend == string.Empty)
                {
                    DirectoryInfo di = new DirectoryInfo(_path);
                    di.Delete();
                }
                else
                {
                    File.Delete(_path);
                }
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

        private void Remove_click(object sender, RoutedEventArgs e)
        {
            object selected_data = Imported.SelectedItem;
            if(selected_data == null)
            {
                MW.MessageBox.Show($"Importedより削除したいdatapackを選択してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selected_data.ToString() == "(None)")
            {
                return;
            }

            MessageBoxResult? result = MW.MessageBox.Show($"{selected_data} を削除しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                Imported.Items.Remove(selected_data);
                if (selected_data.ToString().Contains("【new】"))
                {
                    // add_listの構成要素がdata_pathに順に入るため、これの中から削除しようとしているdatapackの名前を探し、削除する
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

        public void Display()
        {
            foreach (string key in existed_list)
            {
                Imported.Items.Add(key);
            }
            foreach(string key in add_list)
            {
                Imported.Items.Add("【new】" + Path.GetFileName(key));
            }
            foreach(string key in remove_list)
            {
                Imported.Items.Remove(key);
            }

            if(Imported.Items.Count == 0)
            {
                Imported.Items.Add("(None)");
            }
            ShowDialog();
        }
    }
}
