using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Server_GUI2.Windows.Commands
{
    class RunCommand : ICommand
    {
        private MainWindowVM _vm;

        public event EventHandler CanExecuteChanged;

        public RunCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            // Bindingが作動しないようにしたいときはfalseにする
            return true;
        }

        public void Execute(object parameter)
        {
            System.Windows.MessageBox.Show(_vm.ShowAll.ToString());
            //Server.Run(_vm.RunWorld);
        }
    }

    class SettingCommand : ICommand
    {
        private MainWindowVM _vm;

        public event EventHandler CanExecuteChanged;

        public SettingCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _vm.Hide?.Invoke();
            Infomation _info = new Infomation();
            _info.ShowDialog();
            _vm.Show?.Invoke();
        }
    }

    class DeleteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private MainWindowVM _vm;

        public DeleteCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
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

    class CloseCommand : ICommand
    {
        private MainWindowVM _vm;

        public event EventHandler CanExecuteChanged;

        public CloseCommand(MainWindowVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            SetLatestRun();

            _vm.Close();

            Application.Current.Shutdown();
        }

        private void SetLatestRun()
        {
            List<string> vers = _vm.ExistsVersions;
            string existsVer = _vm.SelectedExistsVersion;
            string newVer = _vm.SelectedNewVersion;
            string ver = (existsVer == vers[vers.Count - 1]) ? newVer : existsVer;

            LatestRun latestRun = new LatestRun();

            UserSettings.userSettings.latestRun = latestRun;
            UserSettings.WriteFile();
        }
    }
}
