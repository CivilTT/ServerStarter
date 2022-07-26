using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.MessageBox
{
    public static class CustomMessageBox
    {
        public static int Show(string message, ButtonType type, Image icon, int timeout=-1)
        {
            string[] buttons = SetButtonList(type);
            return Show(message, buttons, icon, timeout);
        }
        public static int Show(string message, ButtonType type, Image icon, LinkMessage link, int timeout=-1)
        {
            string[] buttons = SetButtonList(type);
            return Show(message, buttons, icon, link, timeout);
        }
        public static int Show(string message, string[] buttons, Image icon, int timeout=-1)
        {
            return Show(message, buttons, icon, new LinkMessage("", ""), timeout);
        }

        public static int Show(string message, string[] buttons, Image icon, LinkMessage link, int timeout=-1)
        {
            return Show(message, "Server Starter", buttons, icon, link, timeout);
        }
        /// <summary>
        /// カスタムメッセージボックスを表示する<br/>
        /// 選択したボタンの内容を文字列として返すが、ユーザーが選択しなかった場合は -2 を返す<br/>
        /// timeoutはミリ秒で指定し、Timeoutによって終了した場合は -1 を返す
        /// </summary>
        public static int Show(string message, string title, string[] buttons, Image icon, LinkMessage link, int timeout)
        {
            var window = new ShowNewWindow<Back.MessageBox, MessageBoxVM>();
            var vm = new MessageBoxVM(message, title, buttons, icon, link, timeout);
            window.ShowDialog(vm);
            if (vm.isTimeUp)
                return -1;
            if (!vm.UserSelected)
                return -2;

            return vm.SelectedIndex;
        }

        private static string[] SetButtonList(ButtonType type)
        {
            string[] buttons;
            switch (type)
            {
                case ButtonType.OK:
                    buttons = new string[1] { "OK" };
                    break;
                case ButtonType.OKCancel:
                    buttons = new string[2] { "OK", Properties.Resources.Cancel };
                    break;
                case ButtonType.YesNo:
                    buttons = new string[2] { Properties.Resources.Yes, Properties.Resources.No };
                    break;
                default:
                    throw new ArgumentException("This is unknown type");
            }

            return buttons;
        }
    }
}
