using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Server_GUI2.Windows.SystemSettings;

namespace Server_GUI2.Windows.MoreSettings
{
    class WorldSettingsVM : GeneralVM
    {
        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;

        // 設定項目の表示非表示を操作
        public BindingValue<int> MenuIndex { get; private set; }
        public bool ShowProp => MenuIndex.Value == 0;
        public bool ShowSW => MenuIndex.Value == 1;
        public bool ShowAdd => MenuIndex.Value == 2;
        public bool ShowOp => MenuIndex.Value == 3;   
        public bool ShowWhite => MenuIndex.Value == 4;



        // ShareWorld
        public bool UseSW { get; set; } = true;

        //Op
        public ObservableCollection<OpPlayer> OpPlayersList = new ObservableCollection<OpPlayer>();



        public WorldSettingsVM()
        {
            // General
            MenuIndex = new BindingValue<int>(0, () => OnPropertyChanged(new string[5] { "ShowProp", "ShowSW", "ShowAdd", "ShowOp", "ShowWhite" }));
            

        }

    }

    public class OpPlayer : Player
    {
        public int OpLevel;
        public bool BypassesPlayerLimit;

        public OpPlayer(string name, int opLevel, bool bypassesPlayerLimit=false) : base(name)
        {
            OpLevel = opLevel;
            BypassesPlayerLimit = bypassesPlayerLimit;
        }
    }
}
