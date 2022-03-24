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
        public static string Show(string message, ButtonType type, Image icon)
        {
            string[] buttons = SetButtonList(type);
            return Show(message, buttons, icon);
        }
        public static string Show(string message, ButtonType type, Image icon, LinkMessage link)
        {
            string[] buttons = SetButtonList(type);
            return Show(message, buttons, icon, link);
        }
        public static string Show(string message, string[] buttons, Image icon)
        {
            return Show(message, buttons, icon, new LinkMessage("", ""));
        }

        public static string Show(string message, string[] buttons, Image icon, LinkMessage link)
        {
            return Show(message, "Server Starter", buttons, icon, link);
        }
        /// <summary>
        /// カスタムメッセージボックスを表示する
        /// 選択したボタンの内容を文字列として返すが、ユーザーが選択しなかった場合はstring.Emptyを返す
        /// </summary>
        public static string Show(string message, string title, string[] buttons, Image icon, LinkMessage link)
        {
            var window = new ShowNewWindow<Back.MessageBox, MessageBoxVM>();
            var vm = new MessageBoxVM(message, title, buttons, icon, link);
            window.ShowDialog(vm);
            if (!vm.UserSelected)
                return string.Empty;

            return buttons[vm.SelectedIndex];
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
                    buttons = new string[2] { "OK", "Cancel" };
                    break;
                case ButtonType.YesNo:
                    buttons = new string[2] { "Yes", "No" };
                    break;
                default:
                    throw new ArgumentException("This is unknown type");
            }

            return buttons;
        }
    }
}
