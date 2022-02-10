using Server_GUI2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Server_GUI2.Windows.ViewModels;

namespace Server_GUI2.Windows.MainWindow
{
    class MainWindowVM : INotifyPropertyChanged, IOperateWindows
    {
        // あえて直接呼び出さないことで、Load処理の前にallVersionsなどが呼ばれることを防止している
        private static readonly VersionFactory verFactory = VersionFactory.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        // Window操作系
        public Action Close { get; set; }
        public Action Show { get; set; }
        public Action Hide { get; set; }


        // 一般
        public string StarterVersion { get { return $"ver {SetUp.StarterVersion}"; } }
        public string PlayerName { get { return UserSettings.userSettings.playerName; } }
        public string OpContents { get { return $"{PlayerName} has op rights in this version's server"; } }


        // 新規Versionの表示に関連
        /// <summary>
        /// ToggleSwitchのOnOffを保持
        /// </summary>
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
                NewVersions = ShowNewVersions;
                SelectedNewVersion = ShowNewVersions[0];
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
                NewVersions = ShowNewVersions;
                SelectedNewVersion = ShowNewVersions[0];

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowSpigot"));
            }
        }

        /// <summary>
        /// Comboboxに表示するバージョンの一覧を保持
        /// </summary>
        private List<string> ShowNewVersions;
        //{
        //    get
        //    {
        //        //List<Version> _vers = new List<Version>();
        //        //if (ShowSpigot)
        //        //{
        //        //    _vers = verFactory.Versions.FindAll(x => x.hasSpigot);
        //        //    return _vers.ConvertAll(x => $"Spigot {x.Name}");
        //        //}

        //        //if (ShowAll)
        //        //{
        //        //    _vers = verFactory.Versions;
        //        //}
        //        //else
        //        //{
        //        //    _vers = verFactory.Versions.FindAll(x => x.isRelease);

        //        //    // SnapShotの最新版を表示
        //        //    _vers.Insert(1, verFactory.Versions.Find(x => !x.isRelease && x.isLatest));
        //        //}

        //        //return _vers.ConvertAll(x => VanillaVerConverter(x));

        //        // TODO: Live Shaping によるVersionのフィルタリング実装
        //        view = new CollectionViewSource()
        //        {
        //            Source = verFactory.Versions
        //        };
        //        view.Filter += new FilterEventHandler(test);
        //        //view.SortDescriptions.Add(new SortDescription("Salary", ListSortDirection.Descending));
        //        //return verFactory.Versions.Select(x => x is VanillaVersion);
        //    }
        //}

        //private void test(object sender, FilterEventArgs e)
        //{
        //    Version version = e.Item as Version;

        //    e.Accepted = version is VanillaVersion;
        //}
        //public CollectionViewSource view;


        public List<string> NewVersions
        {
            get
            {
                return ShowNewVersions;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewVersions"));
            }
        }

        /// <summary>
        /// 選択されているバージョンを文字列で保持
        /// </summary>
        private string _selectedNewVersion = VanillaVerConverter(verFactory.Versions[0]);
        public string SelectedNewVersion
        {
            get
            {
                return _selectedNewVersion;
            }
            set
            {
                _selectedNewVersion = value;

                // ToggleSwitchが回された後に最初に表示する項目をShow○○から制御するため必要
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedNewVersion"));
            }
        }

        /// <summary>
        /// 型VersionをComboboxで表示する文字列に変換する
        /// </summary>
        private static string VanillaVerConverter(Version x)
        {
            //return null;

            if (x is SpigotVersion)
            {
                return $"Spigot {x.Name}";
            }
            else
            {
                return x.Name;
            }

            //if (x.isRelease && x.isLatest)
            //{
            //    return $"【LatestRelease】 {x.Name}";
            //}
            //else if (!x.isRelease && x.isLatest)
            //{
            //    return $"【LatestSnapshot】 {x.Name}";
            //}
            //else if (x.isRelease)
            //{
            //    return $"release {x.Name}";
            //}
            //else
            //{
            //    return $"snapshot {x.Name}";
            //}
        }



        // 既存Versionの表示に関連
        /// <summary>
        /// Comboboxに表示するバージョンの一覧を保持
        /// </summary>
        private List<string> ShowExistsVersions
        {
            get
            {
                // この部分はバージョンの表示を一つにすれば不要になる？
                List<string> _vers = new List<string>();
                _vers.Add("【new Version】");
                return _vers;
            }
        }
        public List<string> ExistsVersions
        {
            get
            {
                return ShowExistsVersions;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExistsVersions"));
            }
        }
        public static string ExistsVerConverter(Version x)
        {
            if (x is VanillaVersion)
            {
                return x.Name;
            }
            else
            {
                return $"Spigot {x.Name}";
            }
        }

        /// <summary>
        /// 選択されているバージョンを文字列で保持
        /// </summary>
        private string InitSelectedExistsVersion
        {
            get
            {
                LatestRun latestRun = UserSettings.userSettings.latestRun;

                if (latestRun != null && latestRun.VersionName != "" && ExistsVersions.Contains(latestRun.VersionName))
                {
                    return latestRun.VersionName;
                }

                return ExistsVersions[0];
            }
        }
        private string _selectedExistsVersion;
        public string SelectedExistsVersion
        {
            get
            {
                if (_selectedExistsVersion == null)
                    _selectedExistsVersion = InitSelectedExistsVersion;
                return _selectedExistsVersion;
            }
            set
            {
                _selectedExistsVersion = value;

                // この項目は外部からプログラムで制御することがないため、PropertyChangedを実装しない

                // Selectedが変わった段階でnew Versionsの候補を出すか否かを更新する
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNewVers"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReverseShowNewVers"));
            }
        }

        /// <summary>
        /// New VersionsのComboboxなどを表示するか
        /// </summary>
        public bool ShowNewVers
        {
            get
            {
                // 【new Version】が選択されているか
                return SelectedExistsVersion == ExistsVersions[ExistsVersions.Count - 1];
            }
        }
        public bool ReverseShowNewVers
        {
            get
            {
                return !ShowNewVers;
            }
        }


        // Worldの表示に関連
        // TODO: ワールドの表示処理を実装する
        //private List<string> _Worlds
        //{
        //    get
        //    {

        //    }
        //}
        private string _selectedWorld;
        public string SelectedWorld;



        // ボタンなどに呼応した処理
        public RunCommand RunCommand { get; private set; }
        public SettingCommand SettingCommand { get; private set; }
        public DeleteCommand DeleteCommand { get; private set; }
        public CloseCommand CloseCommand { get; private set; }

        public MainWindowVM()
        {
            RunCommand = new RunCommand(this);
            SettingCommand = new SettingCommand(this);
            DeleteCommand = new DeleteCommand(this);
            CloseCommand = new CloseCommand(this);
        }
    }

}
