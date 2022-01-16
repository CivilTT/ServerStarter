using Server_GUI2.Windows.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.ViewModels
{
    class MainWindowVM : INotifyPropertyChanged, IOperateWindows
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public Action Close { get; set; }
        public Action Show { get; set; }
        public Action Hide { get; set; }

        public string StrVersion;
        public string StrWorld;
        private Version SelectedVersion;
        public World SelectedWorld;

        public string StarterVersion { get { return $"ver {SetUp.StarterVersion}"; } }
        public string PlayerName { get { return UserSettings.userSettings.playerName; } }
        public string OpConstnts { get { return $"{PlayerName} has op rights in this version's server"; } }

        public RunCommand RunCommand { get; private set; }
        public SettingCommand SettingCommand { get; private set; }
        public DeleteCommand DeleteCommand { get; private set; }

        public MainWindowVM()
        {
            RunCommand = new RunCommand(this);
            SettingCommand = new SettingCommand(this);
            DeleteCommand = new DeleteCommand(this);
        }

    }

}
