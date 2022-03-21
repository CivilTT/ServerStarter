using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Server_GUI2.Windows.MessageBox
{
    /// <summary>
    /// MessageBox.xaml の相互作用ロジック
    /// </summary>
    public partial class MessageBox : GeneralCB
    {
        public MessageBox()
        {
            InitializeComponent();
        }
    }

    public static class CustomMessageBox
    {
        public static string Show(string message, ButtonType type, Image icon)
        {
            string[] buttons;
            switch (type)
            {
                case ButtonType.OK:
                    buttons = new string[1] { "OK" };
                    break;
                case ButtonType.OKCancel:
                    buttons = new string[2] { "OK", "Cancel" };
                    break;
                case ButtonType.YesNo:
                    buttons = new string[2] { "Yes", "No" };
                    break;
                default:
                    throw new ArgumentException("This is unknown type");
            }

            return Show(message, buttons, icon);
        }
        public static string Show(string message, string[] buttons, Image icon)
        {
            return Show(message, buttons, icon, new LinkMessage("", ""));
        }

        public static string Show(string message, string[] buttons, Image icon, LinkMessage link)
        {
            return Show(message, "Server Starter", buttons, icon, link);
        }

        public static string Show(string message, string title, string[] buttons, Image icon, LinkMessage link)
        {
            var window = new ShowNewWindow<MessageBox, MessageBoxVM>();
            var vm = new MessageBoxVM(message, title, buttons, icon, link);
            window.ShowDialog(vm);

            return buttons[vm.SelectedIndex];
        }
    }
}
