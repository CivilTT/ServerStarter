using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System.ComponentModel;
using System.Windows.Controls;
using Image = Server_GUI2.Windows.MessageBox.Back.Image;

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
                string[] buttons = new string[3] { Properties.Resources.Yes, Properties.Resources.No, Properties.Resources.Cancel };
                int result = CustomMessageBox.Show(Properties.Resources.Window_CheckSave, buttons, Image.Warning);
                switch (result)
                {
                    case 0:
                        vm.SaveWorldSettings();
                        break;
                    case 1:
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }

            // ShowDialog中にさらにShowDialogすると、最後のDialogが終了した際に元の画面とその子画面が両方表示されるため、これを避けるためにここでShowし、Main側に再表示の機能を持たせない
            Owner.Show();
        }
    }
}
