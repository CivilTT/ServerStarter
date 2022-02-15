using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Server_GUI2.Windows.MoreSettings
{
    class WorldSettingsVM : INotifyPropertyChanged, IOperateWindows
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowProp"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowSW"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowAdd"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowOp"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowWhite"));
            }
        }
        public bool ShowProp => MenuIndex == 0;
        public bool ShowSW => MenuIndex == 1;
        public bool ShowAdd => MenuIndex == 2;
        public bool ShowOp => MenuIndex == 3;
        public bool ShowWhite => MenuIndex == 4;


        //Op
        public ObservableCollection<Player> OpPlayersList = new ObservableCollection<Player>();


    }

    public class Player
    {
        public string Name;
        public string UUID;
        public int OpLevel;
        public bool BypassesPlayerLimit = false;
    }
}
