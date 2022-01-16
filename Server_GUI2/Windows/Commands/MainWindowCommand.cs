using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            System.Windows.MessageBox.Show("TEST");
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

}
