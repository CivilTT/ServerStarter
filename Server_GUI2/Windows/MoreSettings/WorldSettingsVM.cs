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
        public int MenuIndex { get; set; } = 0;
        public bool ShowProp { get { return MenuIndex == 0; } }
        public bool ShowSW { get { return MenuIndex == 1; } }
        public bool ShowAdd { get { return MenuIndex == 2; } }
        public bool ShowOp { get { return MenuIndex == 3; } }
        public bool ShowWhite { get { return MenuIndex == 4; } }

        // GUI作成中用
        // 表示が重複してしまった場合、一度ビルドすれば直る
        //public bool ShowProp { get { return false; } }
        //public bool ShowSW { get { return false; } }
        //public bool ShowAdd { get { return false; } }
        //public bool ShowOp { get { return false; } }
        //public bool ShowWhite { get { return false; } }


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
