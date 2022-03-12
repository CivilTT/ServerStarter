﻿using Server_GUI2.Develop.Util;
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
using MW = ModernWpf;

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
            _ = PortMapping.DeletePort(int.Parse(vm.PortNumber.Value));

            if (!vm.Saved)
            {
                string message =
                    "System Settingsに対する変更を保存せずにMain Windowへ戻りますか？\n" +
                    "変更を保存する場合は「いいえ」を選択したのちに左下の「Save」ボタンを押してください。";
                var result = MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }
    }
}
