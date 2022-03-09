using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.WorldSettings;
using System;
using System.Collections.Generic;
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
            MessageBox.Show(_vm.ShowAll.ToString());
            _vm.Close();
            StartServer.Run(_vm.RunVersion, _vm.RunWorld);
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
            _vm.Hide?.Invoke();
            var systemSettingWindow = new SystemSettingsVM();
            SSwindow.ShowDialog(systemSettingWindow);
            _vm.Show?.Invoke();
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
            _vm.Show?.Invoke();
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
            switch (parameter.ToString())
            {
                // TODO: バージョンとワールドの削除機能
                case "version":
                    Console.WriteLine("VERSION");
                    break;
                case "world":
                    Console.WriteLine("WORLD");
                    break;
                default:
                    break;
            }
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
            SetLatestRun();

            _vm.Close();

            Application.Current.Shutdown();
        }

        private void SetLatestRun()
        {
            //List<string> vers = _vm.ExistsVersions;
            //string existsVer = _vm.SelectedExistsVersion;
            //string newVer = _vm.SelectedNewVersion;
            //string ver = (existsVer == vers[vers.Count - 1]) ? newVer : existsVer;

            //LatestRun latestRun = new LatestRun();

            //UserSettings.userSettings.latestRun = latestRun;
            //UserSettings.WriteFile();
        }
    }
}
