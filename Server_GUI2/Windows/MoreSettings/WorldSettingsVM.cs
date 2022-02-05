using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //public bool ShowProp { get { return MenuIndex == 0; } }
        //public bool ShowSW { get { return MenuIndex == 1; } }
        //public bool ShowDatapck { get { return MenuIndex == 2; } }
        //public bool ShowPlugin { get { return MenuIndex == 3; } }
        //public bool ShowMap { get { return MenuIndex == 4; } }
        //public bool ShowOp { get { return MenuIndex == 5; } }
        //public bool ShowWhite { get { return MenuIndex == 6; } }

        // GUI作成中用
        // 表示が重複してしまった場合、一度ビルドすれば直る
        public bool ShowProp { get { return false; } }
        public bool ShowSW { get { return false; } }
        public bool ShowDatapck { get { return false; } }
        public bool ShowPlugin { get { return false; } }
        public bool ShowMap { get { return false; } }
        public bool ShowOp { get { return false; } }
        public bool ShowWhite { get { return false; } }


    }
}
