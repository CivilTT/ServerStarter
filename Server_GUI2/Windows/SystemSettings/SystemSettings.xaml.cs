﻿using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Server_GUI2
{
    /// <summary>
    /// SystemSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSettings : Window
    {
        public SystemSettings()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IOperateWindows vm)
            {
                vm.Close += () =>
                {
                    Close();
                };
                vm.Hide += () =>
                {
                    Hide();
                };
                vm.Show += () =>
                {
                    Show();
                };
            }
        }
    }
}
