using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.SystemSettings
{
    class SystemSettingsVM : INotifyPropertyChanged, IOperateWindows
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Action Show { get; set; }
        public Action Hide { get; set; }
        public Action Close { get; set; }


        // 設定項目の表示非表示を操作
        private int _menuIndex = 0;
        public int MenuIndex
        {
            get
            {
                return _menuIndex;
            }
            set
            {
                _menuIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowSW"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowServer"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowPlayers"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNet"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowOthers"));
            }
        }
        public bool ShowSW { get { return MenuIndex == 0; } }
        public bool ShowServer { get { return MenuIndex == 1; } }
        public bool ShowPlayers { get { return MenuIndex == 2; } }
        public bool ShowNet { get { return MenuIndex == 3; } }
        public bool ShowOthers { get { return MenuIndex == 4; } }
    }
}
