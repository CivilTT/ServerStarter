using Server_GUI2.Windows.ViewModels;
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
        public string TmpMemory { get { return "test"; } }

        private int OldSelectedIndex;

        public SystemSettings()
        {
            InitializeComponent();
            Loaded += WindowLoaded;


            // 初期設定
            //SideMenuLV.SelectedIndex = 0;

            //DataContext = new
            //{
            //    Env_list = Data_list.Env_list,
            //    TmpMemory,
            //    StarterVersion = Data_list.Starter_Version
            //};
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

        //private void ShowMenuContents(object sender, MouseButtonEventArgs e)
        //{
        //    switch (OldSelectedIndex)
        //    {
        //        case 0:
        //            ShareWorldContents.Visibility = Visibility.Hidden;
        //            break;
        //        case 1:
        //            ServerContents.Visibility = Visibility.Hidden;
        //            break;
        //        case 2:
        //            NetworkContents.Visibility = Visibility.Hidden;
        //            break;
        //        case 3:
        //            SystemContents.Visibility = Visibility.Hidden;
        //            break;
        //        case 4:
        //            InformationContents.Visibility = Visibility.Hidden;
        //            break;
        //        default:
        //            ShareWorldContents.Visibility = Visibility.Hidden;
        //            break;
        //    }

        //    int NewselectedIndex = (sender as ListView).SelectedIndex;
        //    switch (NewselectedIndex)
        //    {
        //        case 0:
        //            ShareWorldContents.Visibility = Visibility.Visible;
        //            break;
        //        case 1:
        //            ServerContents.Visibility = Visibility.Visible;
        //            break;
        //        case 2:
        //            NetworkContents.Visibility = Visibility.Visible;
        //            break;
        //        case 3:
        //            SystemContents.Visibility = Visibility.Visible;
        //            break;
        //        case 4:
        //            InformationContents.Visibility = Visibility.Visible;
        //            break;
        //        default:
        //            break;
        //    }

        //    OldSelectedIndex = NewselectedIndex;
        //}

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class MemoryAll: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Data_list.Env_list["Memory_All"]} KB  ({Math.Round(double.Parse(Data_list.Env_list["Memory_All"]) / (1000 * 1000), 1)} GB)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MemoryAva : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Data_list.Env_list["Memory_Ava"]} KB  ({Math.Round(double.Parse(Data_list.Env_list["Memory_Ava"]) / (1000 * 1000), 1)} GB)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
