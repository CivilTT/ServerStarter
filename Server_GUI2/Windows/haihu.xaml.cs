using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
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
    /// haihu.xaml の相互作用ロジック
    /// </summary>
    public partial class haihu : Window
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public More_Settings m_settings { get; set; }
        private string data_kinds = "ZIP";
        private readonly Functions func = new Functions();
        private bool folder_pick = false;

        public bool import_haihu { get; set; }
        public string import_path { get; set; }


        public haihu()
        {
            InitializeComponent();

            Version.Text = Data_list.ReadVersion;
            World.Text = Data_list.World;
            import_haihu = false;
            import_path = "";

            zip.IsChecked = true;
        }

        private void Zip_Checked(object sender, RoutedEventArgs e)
        {
            folder.IsChecked = false;
            folder_pick = false;
            data_kinds = "ZIP";
        }

        private void Folder_Checked(object sender, RoutedEventArgs e)
        {
            zip.IsChecked = false;
            folder_pick = true;
            data_kinds = "FOLDERS";
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            import_haihu = true;
            Hide();
            m_settings.Show();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            haihu_path.Text = "(None)";
            import_path = "";
            Hide();
            m_settings.Show();
        }

        private void Import_click(object sender, RoutedEventArgs e)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                InitialDirectory = $@"{MainWindow.Data_Path}\{Version.Text}\{World.Text}\datapacks",
                // フォルダ選択モードにする
                IsFolderPicker = folder_pick,
            })
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                // 配布ワールドとして有効かを確認
                bool? result = Check_valid(cofd.FileName);
                if (result == false)
                {
                    MW.MessageBox.Show($"この{(data_kinds == "ZIP" ? "ファイル" : "フォルダ")}は配布ワールドとして無効です。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if(result == null)
                {
                    return;
                }

                // GUIに値をセットする
                bool over = Check_Override();
                if (!over)
                {
                    Cancel_Click(null, null);
                    return;
                }
                haihu_path.Text = Path.GetFileName(import_path);
            }
        }

        /// <summary>
        /// 配布ワールドとしての有効性を確認する
        /// </summary>
        /// <param name="file_path">確認するファイル（フォルダ）のパス</param>
        /// <returns>有効な場合はtrueを返し、無効な場合はfalseを返す。このメソッド内でユーザーに無効であることを通達済みの場合はnullを返す。</returns>
        private bool? Check_valid(string file_path)
        {
            string file_extension = Path.GetExtension(file_path);
            string extract_path = Path.GetDirectoryName(file_path) + @"\" + Path.GetFileNameWithoutExtension(file_path);
            // そもそもzipやフォルダでないものははじく
            if (file_extension != ".zip" && file_extension != string.Empty)
            {
                return false;
            }
            else if(file_extension == ".zip")
            {
                // Zipを展開
                if (Directory.Exists(extract_path))
                {
                    MessageBoxResult? result = MW.MessageBox.Show($"以下の場所に展開先のフォルダと同名のフォルダが存在しています\n同名のフォルダを展開フォルダで上書きしますか？\n\n【場所（展開先）】\n{extract_path}", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        Directory.Delete(extract_path, true);
                    }
                    else
                    {
                        MW.MessageBox.Show("展開をしなかったため、配布ワールドの導入ができませんでした。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
                        return null;
                    }
                }

                try
                {
                    ZipFile.ExtractToDirectory(file_path, extract_path);
                }
                catch(Exception ex)
                {
                    logger.Warn($"System can not check valid of the custom map. Reason:{ex.Message}");
                    if (!Directory.Exists(extract_path))
                    {
                        string message =
                            "指定したZipファイルの検証中にエラーが発生しました。\n" +
                            "Zipファイルを展開し、展開後のフォルダを追加対象として指定してください。\n\n" +
                            $"【エラー要因】\n{ex.Message}";
                        MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                        logger.Error("System gave up to add the datapack");
                        return false;
                    }
                }
            }

            // フォルダの直下(or一つ下)にadvancementsフォルダが存在しているかを確認する
            if (Directory.Exists($@"{extract_path}\advancements"))
            {
                import_path = extract_path;
                return true;
            }
            foreach(string folder_path in Directory.GetDirectories(extract_path))
            {
                if (Directory.Exists($@"{folder_path}\advancements"))
                {
                    import_path = folder_path;
                    return true;
                }
            }

            return false;
        }

        private bool Check_Override()
        {
            if (!Data_list.Import_NewWorld)
            {
                MessageBoxResult? result = MW.MessageBox.Show($"配布ワールドを導入しようとしているワールドはすでに別のワールドとして存在しています。\n前のワールドデータを上書きして配布ワールドを導入しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.No)
                {
                    MW.MessageBox.Show("配布ワールドの導入を中止しました。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            return true;
        }

        public void Add_data()
        {
            logger.Info("Import the Custom Map");
            try
            {
                // xcopyはディレクトリがないと動かないのか？
                // Worldの名前は先に変えておいても良いが、実行までのどこかのタイミングで変わっていればよい
                // この修正が終わったら一通りデバッグをして、コマンドラインからの実行の作業に移る
                Process p = Process.Start("xcopy", $@"{import_path} {MainWindow.Data_Path}\{Data_list.Version}\{Data_list.World} /E /H /I /Y");
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                string message =
                        "配布ワールドの導入に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new WinCommandException($"Failed to import the custom map (Error Message : {ex.Message})");
            }
        }
    }
}
