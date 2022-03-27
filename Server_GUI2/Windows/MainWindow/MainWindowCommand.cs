using Server_GUI2.Develop.Server.World;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.WorldSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Server_GUI2.Windows.MainWindow
{
    class RunCommand : GeneralCommand<MainWindowVM>
    {
        public RunCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.Hide();
            StartServer.Run(_vm.RunVersion, _vm.RunWorld);

            // TODO: 必要に応じて再表示する

            _vm.Close();
        }
    }

    class SettingCommand : GeneralCommand<MainWindowVM>
    {
        private readonly IShowWindowService<SystemSettingsVM> SSwindow;

        public SettingCommand(MainWindowVM vm, IShowWindowService<SystemSettingsVM> ssWindow)
        {
            _vm = vm;
            SSwindow = ssWindow;
        }

        public override void Execute(object parameter)
        {
            _vm.Hide();
            var systemSettingWindow = new SystemSettingsVM();
            SSwindow.ShowDialog(systemSettingWindow);
            _vm.Show();
        }
    }

    class WorldSettingCommand : GeneralCommand<MainWindowVM>
    {
        private readonly IShowWindowService<WorldSettingsVM> WSwindow;

        public WorldSettingCommand(MainWindowVM vm, IShowWindowService<WorldSettingsVM> wsWindow)
        {
            _vm = vm;
            WSwindow = wsWindow;
        }

        public override void Execute(object parameter)
        {
            _vm.Hide?.Invoke();
            var worldSettingWindow = new WorldSettingsVM(_vm.RunVersion, _vm.RunWorld);
            WSwindow.ShowDialog(worldSettingWindow);

            // OnClosingにコメントアウトの理由あり
            //_vm.Show?.Invoke();
        }
    }

    class DeleteCommand : GeneralCommand<MainWindowVM>
    {
        public DeleteCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            void Delete<T>(ObservableCollection<T> collection, T removeItemIndex, string removeItemName, Action deleteAction)
            {
                string removeType = "";
                if (removeItemIndex is Version version)
                    removeType = "バージョン";
                else if (removeItemIndex is IWorld world)
                    removeType = "ワールド";

                string message =
                    $"この{removeType}を削除しますか？\n" +
                    $"「{removeItemName}」は完全に削除され、復元ができなくなります。";
                string result = CustomMessageBox.Show(message, MessageBox.Back.ButtonType.YesNo, MessageBox.Back.Image.Warning);
                if (result != "Yes")
                    return;

                deleteAction();
                collection.Remove(removeItemIndex);
                removeItemIndex = collection[0];
            }

            switch (parameter.ToString())
            {
                case "version":
                    Version versionIndex = _vm.ExistsVersionIndex.Value;
                    string name = $"{versionIndex.Type} {versionIndex.Name}";
                    Delete(_vm.ExistsVersions, versionIndex, name, () => versionIndex.Remove());
                    break;

                case "world":
                    IWorld worldIndex = _vm.WorldIndex.Value;
                    name = worldIndex.DisplayName;
                    Delete(_vm.Worlds, worldIndex, name, () => ((World)worldIndex).Delete());
                    break;

                default:
                    break;
            }
        }
    }

    class SetShutdown : GeneralCommand<MainWindowVM>
    {
        public SetShutdown(MainWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            UserSettings.Instance.userSettings.ShutdownPC = _vm.ShutdownPC;
        }
    }

    class CloseCommand : GeneralCommand<MainWindowVM>
    {
        public CloseCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            // userSettingsに何か変更が加えられていた場合は保存してから終了する
            UserSettings.Instance.WriteFile();

            _vm.Close();
        }
    }
}
