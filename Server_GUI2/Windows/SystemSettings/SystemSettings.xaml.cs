using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System.ComponentModel;
using System.Globalization;
using Image = Server_GUI2.Windows.MessageBox.Back.Image;

namespace Server_GUI2.Windows.SystemSettings
{
    /// <summary>
    /// SystemSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSettings : GeneralCB
    {
        public SystemSettings()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            SystemSettingsVM vm = (SystemSettingsVM)DataContext;

            // Windowを閉じるときには一度ポートを必ず閉鎖する（MainWindowに戻ってRunするとは限らないから）
            if (vm.PortStatus.Value.StatusEnum.Value == PortStatus.Status.Open)
            {
                PortSetting portSetting = new PortSetting(vm);
                _ = portSetting.DeletePort();
            }

            if (vm.RemoteAdding.Value)
            {
                CustomMessageBox.Show(Properties.Resources.SystemSettings_Registering, ButtonType.OK, Image.Infomation);
                e.Cancel = true;
                return;
            }

            if (!vm.Saved)
            {
                string[] buttons = new string[3] { Properties.Resources.Yes, Properties.Resources.No, Properties.Resources.Cancel };
                int result = CustomMessageBox.Show(Properties.Resources.Window_CheckSave, buttons, Image.Warning);
                switch (result)
                {
                    case 0:
                        vm.SaveSystemSettings();
                        break;
                    case 1:
                        Properties.Resources.Culture = CultureInfo.GetCultureInfo(UserSettings.Instance.userSettings.Language);
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
