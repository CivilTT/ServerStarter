using Server_GUI2.Windows.MoreSettings;
using Server_GUI2.Windows.ViewModels;
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
            //Server.Run(_vm.RunWorld);
        }
    }

    class SettingCommand : GeneralCommand<MainWindowVM>
    {
        public SettingCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.Hide?.Invoke();
            Server_GUI2.SystemSettings _info = new Server_GUI2.SystemSettings();
            _info.ShowDialog();
            _vm.Show?.Invoke();
        }
    }

    class WorldSettingCommand : GeneralCommand<MainWindowVM>
    {
        public WorldSettingCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.Hide?.Invoke();
            WorldSettings window = new WorldSettings();
            window.ShowDialog();
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
