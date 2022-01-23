using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

namespace Server_GUI2
{
    /// <summary>
    /// Infomation.xaml の相互作用ロジック
    /// </summary>
    public partial class Infomation : Window
    {
        public string TmpMemory { get { return "test"; } }

        public Infomation()
        {
            InitializeComponent();

            DataContext = new
            {
                Env_list = Data_list.Env_list,
                TmpMemory,
                StarterVersion = Data_list.Starter_Version
            };
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Author_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitter.com/CivilT_T");
        }
    }

    public class MemoryAll: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Data_list.Env_list["Memory_All"]} KB  ({Math.Round(double.Parse(Data_list.Env_list["Memory_All"]) /// (1000 * 1000), 1)} GB)";
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
            return $"{Data_list.Env_list["Memory_Ava"]} KB  ({Math.Round(double.Parse(Data_list.Env_list["Memory_Ava"]) /// (1000 * 1000), 1)} GB)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
