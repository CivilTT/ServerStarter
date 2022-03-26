using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.WelcomeWindow
{
    class WelcomeWindowVM : GeneralVM
    {
        // General
        public WelcomeWindow OwnerWindow { get; private set; }
        public bool CanStart => (check || NotRegistName.Value) && Agreed.Value;
        public StartCommand StartCommand { get; private set; }

        // 1.
        public BindingValue<string> PlayerName { get; private set; }
        public string checkedName = "";
        private bool check => PlayerName.Value == checkedName;
        public BindingValue<bool> IsActive { get; private set; }
        public BindingValue<string> UUID { get; private set; }
        public CheckValidNameCommand CheckValidNameCommand { get; private set; }
        public BindingValue<bool> NotRegistName { get; private set; }


        // 2.
        public BindingValue<bool> Agreed { get; private set; }



        public WelcomeWindowVM(WelcomeWindow ownerWindow)
        {
            // General
            OwnerWindow = ownerWindow;
            StartCommand = new StartCommand(this);

            // 1.
            PlayerName = new BindingValue<string>("Your Name", () => OnPropertyChanged(new string[2] { "OwnerName", "CanStart" }));
            IsActive = new BindingValue<bool>(false, () => OnPropertyChanged("IsActive"));
            UUID = new BindingValue<string>("", () => OnPropertyChanged("UUID"));
            CheckValidNameCommand = new CheckValidNameCommand(this, ownerWindow);
            NotRegistName = new BindingValue<bool>(false, () => OnPropertyChanged("CanStart"));

            // 2.
            Agreed = new BindingValue<bool>(false, () => OnPropertyChanged("CanStart"));


        }
    }
}
