using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MW = ModernWpf;

namespace Server_GUI2.Windows.WorldSettings
{
    /// <summary>
    /// WorldSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class WorldSettings : GeneralCB
    {
        public WorldSettings()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            WorldSettingsVM vm = (WorldSettingsVM)DataContext;

            if (!vm.Saved)
            {
                string message =
                    "World Settingsに対する変更を保存せずにMain Windowへ戻りますか？\n" +
                    "変更を保存する場合は「いいえ」を選択したのちに左下の「Save」ボタンを押してください。";
                var result = MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }
    }
}
