using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Server_GUI2.Windows.SystemSettings
{
    class AddGitAccountCommand : ICommand
    {
        private readonly SystemSettingsVM _vm;

        public event EventHandler CanExecuteChanged;

        public AddGitAccountCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine($"Name : {_vm.AccountName}");
            Console.WriteLine($"E-mail : {_vm.AccountEmail}");
            Console.WriteLine($"Repo : {_vm.RepoName}");
            Console.WriteLine($"Bool : {_vm.IsPrivate}");
        }
    }
}
