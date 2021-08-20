using log4net;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Server_GUI2
{
    /// <summary>
    /// info_builder.xaml の相互作用ロジック
    /// </summary>
    public partial class info_builder : Window
    {

        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public List<string> list_info_index = new List<string>();

        public info_builder()
        {
            InitializeComponent();

            this_version.Text = Data_list.Starter_Version;
        }

        private void regist_info()
        {
            string[] index = new string[] {
                $"{Data_list.Info_index[0]}->{name.Text}",
                $"{Data_list.Info_index[1]}->{this_version.Text}",
                $"{Data_list.Info_index[2]}->",
                $"{Data_list.Info_index[3]}->",
                $"{Data_list.Info_index[4]}->False",
                $"{Data_list.Info_index[5]}->{git_name.Text}",
                $"{Data_list.Info_index[6]}->{git_address.Text}"
            };

            list_info_index.AddRange(index);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Build new info.txt");
            
            regist_info();
            
            using (var writer = new StreamWriter($@"{MainWindow.Data_Path}\info.txt", false))
            {
                foreach(string index in list_info_index)
                {
                    writer.WriteLine(index);
                }
            }
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
