using Server_GUI2.Develop.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
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
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using MW = ModernWpf;
using Image = Server_GUI2.Windows.MessageBox.Back.Image;

namespace Server_GUI2.Windows.SystemSettings
{
    /// <summary>
    /// SystemSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSettings : GeneralCB
    {
        public SystemSettings()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            SystemSettingsVM vm = (SystemSettingsVM)DataContext;

            // Windowを閉じるときには一度ポートを必ず閉鎖する（MainWindowに戻ってRunするとは限らないから）
            if (vm.PortStatus.Value.StatusEnum.Value == PortStatus.Status.Open)
            {
                PortSetting portSetting = new PortSetting(vm);
                _ = portSetting.DeletePort();
            }

            if (vm.RemoteAdding.Value)
            {
                CustomMessageBox.Show("リモートの登録中はこの画面を閉じることができません", ButtonType.OK, Image.Infomation);
                e.Cancel = true;
                return;
            }

            if (!vm.Saved)
            {
                string message =
                    "Systemに関する設定を保存しますか？";
                string[] buttons = new string[3] { "Save", "Not Save", "Cancel" };
                string result = CustomMessageBox.Show(message, buttons, MessageBox.Back.Image.Warning);
                switch (result)
                {
                    case "Save":
                        vm.SaveSystemSettings();
                        break;
                    case "Not Save":
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
