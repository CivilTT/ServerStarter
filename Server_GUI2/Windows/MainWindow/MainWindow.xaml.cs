using log4net;
using Server_GUI2.Windows;
using Server_GUI2.Windows.MainWindow;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.WorldSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;


/// <summary>
/// バージョンアップの際に行うべきこと
/// ・Assembly.csのバージョンを変更
/// ・ビルド
/// ・インストーラーのバージョンを変更
/// ・ビルド
/// ・新たにzip化してアップデート
/// </summary>

/// <summary>
/// Mainでの変更をChangeSystemに取り込む方法
/// ・Current Branchをこのブランチにする
/// ・mainを右クリックし、「Current Branch にマージ」を選択
/// ・「main を ChangeSystem にマージしますか？」と聞かれるため「はい」を選択
/// ・適当なファイルを確認し、読み込まれなくなった場合はVisual Studioを再起動
/// </summary>


namespace Server_GUI2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : GeneralCB
    {
        public static string Data_Path { get { return @".\World_Data"; } }

        //インスタンス変数を宣言
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public List<string> All_versions { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            var systemSettingWindow = new ShowNewWindow<SystemSettings, SystemSettingsVM>()
            {
                Owner = this
            };
            var worldSettingWindow = new ShowNewWindow<WorldSettings, WorldSettingsVM>()
            {
                Owner = this
            };
            DataContext = new MainWindowVM(systemSettingWindow, worldSettingWindow);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
