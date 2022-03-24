using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using MW = ModernWpf;

namespace Server_GUI2.Windows.WorldSettings
{
    /// <summary>
    /// WorldSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class WorldSettings : GeneralCB
    {
        public WorldSettings()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            WorldSettingsVM vm = (WorldSettingsVM)DataContext;

            if (!vm.Saved)
            {
                string message =
                    "Worldに関する設定内容を保存しますか？";
                string[] buttons = new string[3] { "Save", "Not Save", "Cancel" };
                string result = CustomMessageBox.Show(message, buttons, MessageBox.Back.Image.Warning);
                switch (result)
                {
                    case "Save":
                        vm.SaveWorldSettings();
                        break;
                    case "Not Save":
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }

            // ShowDialog中にさらにShowDialogすると、最後のDialogが終了した際に元の画面とその子画面が両方表示されるため、これを避けるためにここでShowし、Main側に再表示の機能を持たせない
            Owner.Show();
        }
    }
}
