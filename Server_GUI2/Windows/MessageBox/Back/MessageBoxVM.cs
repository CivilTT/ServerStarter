using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW = ModernWpf;

namespace Server_GUI2.Windows.MessageBox.Back
{
    public enum Image
    {
        Infomation,
        Question,
        Warning,
        Error
    }

    public enum ButtonType
    {
        OK,
        OKCancel,
        YesNo
    }

    class MessageBoxVM : GeneralVM
    {
        // Width
        public int WindowWidth { get; set; }
        public int ButtonWidth { get; private set; }

        // Contents
        public string Title { get; private set; }
        public string Message { get; private set; }
        public LinkMessage LinkMessage { get; private set; }
        public bool VisibleLink => LinkMessage.Message != "";
        public Image ImageType { get; private set; } = Image.Infomation;
        public string ImagePath
        {
            get
            {
                if (MW.ThemeManager.Current.ApplicationTheme == MW.ApplicationTheme.Dark)
                    return $"pack://application:,,,/Resources/MessageBox/{ImageType}(B).png";
                else
                    return $"pack://application:,,,/Resources/MessageBox/{ImageType}(W).png";
            }
        }

        // Buttons
        public string[] Buttons { get; private set; }
        public int SelectedIndex { get; set; }
        // ボタンを増やす場合は変数を増やしたのちにxamlのStackPanelに追記すればよい
        public List<ButtonManager> ButtonManagers { get; private set; } = new List<ButtonManager>();
        public ButtonCommand ButtonCommand { get; private set; }
        public bool UserSelected = false;


        public MessageBoxVM(string message, string title, string[] buttons, Image icon, LinkMessage link)
        {
            Message = message;
            Title = title;
            Buttons = buttons;
            ImageType = icon;
            LinkMessage = link;

            ButtonCommand = new ButtonCommand(this);

            SetButtonContent();
            SetButtonWidth();
            SetWindowWidth();
        }

        private void SetButtonContent()
        {
            int buttonCount = 4;
            if (Buttons.Length > buttonCount)
                throw new ArgumentException($"You can't set {Buttons.Length} buttons");

            // 表示するボタンの登録
            foreach (string content in Buttons)
                ButtonManagers.Add(new ButtonManager(content));

            // 余ったボタンに空文字列を登録しておくことでXaml側でのOutOfIndexErrorを回避している
            for (int i = 0; i < buttonCount - Buttons.Length; i++)
                ButtonManagers.Add(new ButtonManager(""));
        }

        private void SetButtonWidth()
        {
            int maxLength = 0;
            foreach (string buttonContent in Buttons)
            {
                int length = Encoding.GetEncoding("Shift_JIS").GetByteCount(buttonContent);
                if (maxLength < length)
                    maxLength = length;
            }

            ButtonWidth = 95;
            if (7 < maxLength)
                ButtonWidth = 130;
        }

        private void SetWindowWidth()
        {
            string[] del = { "\n" };
            string[] splitMessages = Message.Split(del, StringSplitOptions.RemoveEmptyEntries);

            int maxLength = 0;
            foreach (string splitMessage in splitMessages)
            {
                int length = Encoding.GetEncoding("Shift_JIS").GetByteCount(splitMessage);
                if (maxLength < length)
                    maxLength = length;
            }

            WindowWidth = 350;
            int buttonsWidth = (ButtonWidth + 10) * Buttons.Length;
            if (45 < maxLength && maxLength < 65 && buttonsWidth < 450)
                WindowWidth = 450;
            else if (65 <= maxLength && buttonsWidth < 550)
                WindowWidth = 550;
            else
                WindowWidth = buttonsWidth + 10;
        }
    }

    public class LinkMessage
    {
        public string Message { get; private set; }
        public string URL { get; private set; }
        public LinkMessage(string message, string url)
        {
            Message = message;
            URL = url;
        }
    }

    class ButtonManager
    {
        public string Content { get; private set; } = "";
        public bool Visibility => Content != "";
        public ButtonManager(string content)
        {
            Content = content;
        }
    }
}
