using System.Collections.Generic;
using System.Globalization;

namespace Server_GUI2.Windows.WelcomeWindow
{
    class WelcomeWindowVM : GeneralVM
    {
        // General
        public WelcomeWindow OwnerWindow { get; private set; }
        public bool CanStart => (check || NotRegistName.Value) && Agreed.Value;
        public StartCommand StartCommand { get; private set; }
        public BindingValue<string> LanguageSelected { get; private set; }
        public Dictionary<string, string> Languages => UserSettings.Languages;
        public Properties.Resources Resources => new Properties.Resources();

        // 1.
        public BindingValue<string> PlayerName { get; private set; }
        public string checkedName = null;
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
            LanguageSelected = new BindingValue<string>("English", () => UpdateLanguage());

            // 1.
            PlayerName = new BindingValue<string>("Your Name", () => OnPropertyChanged(new string[3] { "PlayerName", "OwnerName", "CanStart" }));
            IsActive = new BindingValue<bool>(false, () => OnPropertyChanged("IsActive"));
            UUID = new BindingValue<string>("", () => OnPropertyChanged("UUID"));
            CheckValidNameCommand = new CheckValidNameCommand(this, ownerWindow);
            NotRegistName = new BindingValue<bool>(false, () => OnPropertyChanged("CanStart"));

            // 2.
            Agreed = new BindingValue<bool>(false, () => OnPropertyChanged("CanStart"));


        }

        private void UpdateLanguage()
        {
            if (LanguageSelected?.Value != null)
                Properties.Resources.Culture = CultureInfo.GetCultureInfo(Languages[LanguageSelected.Value]);
            OnPropertyChanged("Resources");
        }
    }
}
