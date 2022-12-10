using Server_GUI2.Develop.Server.World;
using Server_GUI2.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.WorldSettings;
using System;
using System.Collections.ObjectModel;

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
            StartServer.Run(_vm.RunVersion, false, _vm.RunWorld);

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
            _vm.OnPropertyChanged("Resources");
            _vm.OnPropertyChanged("OpContents");
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

            _vm.OnPropertyChanged("OwnerHasOp");
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
            bool Delete<T>(ObservableCollection<T> collection, T removeItemIndex, string removeItemName, Action deleteAction)
            {
                string removeType = "";
                if (removeItemIndex is Version version)
                    removeType = Properties.Resources.Version;
                else if (removeItemIndex is IWorld world)
                    removeType = Properties.Resources.World;

                string message =
                    $"{Properties.Resources.Main_DeleteMsg1}{removeType}{Properties.Resources.Main_DeleteMsg2}\n" +
                    $"{Properties.Resources.Main_DeleteMsg3}{removeItemName}{Properties.Resources.Main_DeleteMsg4}";
                int result = CustomMessageBox.Show(message, MessageBox.Back.ButtonType.YesNo, MessageBox.Back.Image.Warning);
                if (result != 0)
                    return false;

                deleteAction();
                collection.Remove(removeItemIndex);
                return true;
            }

            Version versionIndex = _vm.ExistsVersionIndex.Value;
            IWorld worldIndex = _vm.WorldIndex.Value;
            switch (parameter.ToString())
            {
                case "version":
                    bool deleted = Delete(_vm.ExistsVersions, versionIndex, versionIndex.Name,
                        () => {
                            versionIndex.Remove();
                            _vm.Worlds.WriteLine(world => world.DisplayName);
                            _vm.Worlds.RemoveAll(world => world.Version == versionIndex);
                        });
                    if (deleted)
                    {
                        _vm.ExistsVersionIndex.Value = _vm.ExistsVersions[0];
                        _vm.Worlds.WriteLine(world => world.DisplayName);
                        _vm.WorldIndex.Value = _vm.Worlds.Contains(worldIndex) ? worldIndex : _vm.Worlds[0];
                        Console.WriteLine(_vm.WorldIndex.Value.DisplayName);
                    }
                    break;

                case "world":
                    deleted = Delete(_vm.Worlds, worldIndex, worldIndex.DisplayName, () => ((World)worldIndex).Delete());
                    if (deleted)
                        _vm.WorldIndex.Value = _vm.Worlds[0];
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
