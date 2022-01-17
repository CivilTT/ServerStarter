using Server_GUI2.Windows.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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


        // 一般
        public string StarterVersion { get { return $"ver {SetUp.StarterVersion}"; } }
        public string PlayerName { get { return UserSettings.userSettings.playerName; } }
        public string OpContents { get { return $"{PlayerName} has op rights in this version's server"; } }


        // Versionの表示に関連
        //private int _verIndex = 0;
        //public int VerIndex
        //{
        //    get
        //    {
        //        return _verIndex;
        //    }
        //    set
        //    {
        //        _verIndex = value;
        //    }
        //}

        private bool _showAll = false;
        public bool ShowAll
        {
            get
            {
                return _showAll;
            }
            set 
            {
                _showAll = value;
                Versions = ShowVersions;
                //VerIndex = 0;
            } 
        }
        private bool _showSpigot = false;
        public bool ShowSpigot
        {
            get 
            {
                return _showSpigot;
            }
            set
            {
                _showSpigot = value;
                Versions = ShowVersions;
                //VerIndex = 0;

                // TODO: Spigotにした際にReleaseのToggleSwitchを使用不可にしたい
                // ToggleSwitchを回すとComboboxの表示が何もなくなってしまう問題を解決したい（これはSelectedIndexを０に設定できれば解決）
            }
        }
        private List<string> ShowVersions
        {
            get
            {
                List<Version> _vers = new List<Version>();
                if (ShowSpigot)
                {
                    _vers = VersionFactory.allVersions.FindAll(x => x.hasSpigot);
                    return _vers.ConvertAll(x => $"Spigot {x.Name}");
                }

                if (ShowAll)
                {
                    _vers = VersionFactory.allVersions;
                }
                else
                {
                    _vers = VersionFactory.allVersions.FindAll(x => x.isRelease);

                    // SnapShotの最新版を表示
                    _vers.Insert(1, VersionFactory.allVersions.Find(x => !x.isRelease && x.isLatest));
                }

                return _vers.ConvertAll(x => VanilaVerConverter(x));
            }
        }
        public List<string> Versions
        {
            get
            {
                return ShowVersions;
            }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Versions"));
                }
            }
        }
        public static string VanilaVerConverter(Version x)
        {
            if (x.isRelease && x.isLatest)
            {
                return $"【Latest Release】 {x.Name}";
            }
            else if (!x.isRelease && x.isLatest)
            {
                return $"【Latest Snapshot】 {x.Name}";
            }
            else if (x.isRelease)
            {
                return $"release {x.Name}";
            }
            else
            {
                return $"snapshot {x.Name}";
            }
        }


        // Worldの表示に関連


        // ボタンなどに呼応した処理
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
