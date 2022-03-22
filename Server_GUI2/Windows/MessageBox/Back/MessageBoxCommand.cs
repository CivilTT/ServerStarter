using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.MessageBox.Back
{
    class ButtonCommand : GeneralCommand<MessageBoxVM>
    {
        public ButtonCommand(MessageBoxVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.SelectedIndex = int.Parse(parameter.ToString());
            _vm.UserSelected = true;
            _vm.Close();
        }
    }
}
