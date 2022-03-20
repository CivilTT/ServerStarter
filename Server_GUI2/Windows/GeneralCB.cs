using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;

namespace Server_GUI2.Windows
{
    public partial class GeneralCB : Window
    {
        public GeneralCB()
        {
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IOperateWindows vm)
            {
                vm.Close += () =>
                {
                    //Close();
                    // ProgressBarが別スレッドで動くため、それに対応した実装
                    Dispatcher.Invoke(new MethodInvoker(Close));
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
