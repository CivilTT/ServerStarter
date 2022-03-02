using Server_GUI2.Develop.Server.World;
using Server_GUI2.Util;
using Server_GUI2.Windows.MoreSettings;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Server_GUI2.Windows.MainWindow
{
    class MainWindowVM : GeneralVM
    {
        // あえて直接呼び出さないことで、Load処理の前にallVersionsなどが呼ばれることを防止している
        private static readonly VersionFactory verFactory = VersionFactory.Instance;

        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;

        // 一般
        public string StarterVersion { get { return $"ver {SetUp.StarterVersion}"; } }
        public string PlayerName { get { return UserSettings.Instance.userSettings.PlayerName; } }
        public string OpContents { get { return $"{PlayerName} has op rights in this version's server"; } }
        readonly ObservableCollection<Version> AllVers = VersionFactory.Instance.Versions;
        readonly ObservableCollection<IWorld> AllWorlds = WorldCollection.Instance.Worlds;


        // General
        public bool CanRun => Regex.IsMatch(NewWorldName.Value, @"^[0-9a-zA-Z_-]+$");


        // 新規Versionの表示に関連
        /// <summary>
        /// ToggleSwitchのOnOffを保持
        /// </summary>
        public BindingValue<bool> ShowAll { get; private set; }
        //private bool _showAll = false;
        //public bool ShowAll
        //{
        //    get
        //    {
        //        return _showAll;
        //    }
        //    set 
        //    {
        //        _showAll = value;
        //        NewVersions = ShowNewVersions;
        //        SelectedNewVersion = ShowNewVersions[0];
        //    } 
        //}
        public BindingValue<bool> ShowSpigot { get; private set; }
        //private bool _showSpigot = false;
        //public bool ShowSpigot
        //{
        //    get 
        //    {
        //        return _showSpigot;
        //    }
        //    set
        //    {
        //        _showSpigot = value;
        //        NewVersions = ShowNewVersions;
        //        SelectedNewVersion = ShowNewVersions[0];

        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowSpigot"));
        //    }
        //}

        //Version
        /// <summary>
        /// Comboboxに表示するバージョンの一覧を保持
        /// </summary>
        public ObservableCollection<Version> ExistsVersions { get; private set; }
        public BindingValue<Version> ExistsVersionIndex { get; private set; }
        public bool ShowNewVersions => (ExistsVersionIndex.Value?.Name ?? "") == "【new Version】";
        public ObservableCollection<Version> NewVersions { get; private set; }
        public BindingValue<Version> NewVersionIndex { get; private set; }
        public Version RunVersion => ShowNewVersions ? NewVersionIndex.Value : ExistsVersionIndex.Value;
        
        // World
        public ObservableCollection<IWorld> Worlds { get; private set; }
        public BindingValue<IWorld> WorldIndex { get; private set; }
        public bool ShowNewWorld => (WorldIndex.Value?.Name ?? "") == "【new World】";
        public BindingValue<string> NewWorldName { get; private set; }
        // TODO: 仮で実行するワールドを既存のものから選択したものにしている
        // 本実装では新規に対応したものに差し替え
        //public World RunWorld => ShowNewWorld ? new NewWorld() : WorldIndex.Value;
        public IWorld RunWorld => WorldIndex.Value;


        // Setting
        public BindingValue<bool> ResetWorld { get; private set; }
        public bool SaveWorld { get; set; }
        public bool ShowSaveWorld => ResetWorld != null && ResetWorld.Value;
        public bool OwnerHasOp { get; set; }
        public bool ShutdownPC { get; set; }

        //private List<string> ShowNewVersions;
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


        //public List<string> NewVersions
        //{
        //    get
        //    {
        //        return ShowNewVersions;
        //    }
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewVersions"));
        //    }
        //}

        /// <summary>
        /// 選択されているバージョンを文字列で保持
        /// </summary>
        //private string _selectedNewVersion = VanillaVerConverter(verFactory.Versions[0]);
        //public string SelectedNewVersion
        //{
        //    get
        //    {
        //        return _selectedNewVersion;
        //    }
        //    set
        //    {
        //        _selectedNewVersion = value;

        //        // ToggleSwitchが回された後に最初に表示する項目をShow○○から制御するため必要
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedNewVersion"));
        //    }
        //}

        /// <summary>
        /// 型VersionをComboboxで表示する文字列に変換する
        /// </summary>
        //private static string VanillaVerConverter(Version x)
        //{
        //    //return null;

        //    if (x is SpigotVersion)
        //    {
        //        return $"Spigot {x.Name}";
        //    }
        //    else
        //    {
        //        return x.Name;
        //    }

        //    //if (x.isRelease && x.isLatest)
        //    //{
        //    //    return $"【LatestRelease】 {x.Name}";
        //    //}
        //    //else if (!x.isRelease && x.isLatest)
        //    //{
        //    //    return $"【LatestSnapshot】 {x.Name}";
        //    //}
        //    //else if (x.isRelease)
        //    //{
        //    //    return $"release {x.Name}";
        //    //}
        //    //else
        //    //{
        //    //    return $"snapshot {x.Name}";
        //    //}
        //}



        // 既存Versionの表示に関連
        /// <summary>
        /// Comboboxに表示するバージョンの一覧を保持
        /// </summary>
        //private List<string> ShowExistsVersions
        //{
        //    get
        //    {
        //        // この部分はバージョンの表示を一つにすれば不要になる？
        //        List<string> _vers = new List<string>();
        //        _vers.Add("【new Version】");
        //        return _vers;
        //    }
        //}
        //public List<string> ExistsVersions
        //{
        //    get
        //    {
        //        return ShowExistsVersions;
        //    }
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExistsVersions"));
        //    }
        //}
        //public static string ExistsVerConverter(Version x)
        //{
        //    if (x is VanillaVersion)
        //    {
        //        return x.Name;
        //    }
        //    else
        //    {
        //        return $"Spigot {x.Name}";
        //    }
        //}

        /// <summary>
        /// 選択されているバージョンを文字列で保持
        /// </summary>
        //private string InitSelectedExistsVersion
        //{
        //    get
        //    {
        //        LatestRun latestRun = UserSettings.Instance.userSettings.LatestRun;

        //        if (latestRun != null && latestRun.VersionName != "" && ExistsVersions.Contains(latestRun.VersionName))
        //        {
        //            return latestRun.VersionName;
        //        }

        //        return ExistsVersions[0];
        //    }
        //}
        //private string _selectedExistsVersion;
        //public string SelectedExistsVersion
        //{
        //    get
        //    {
        //        if (_selectedExistsVersion == null)
        //            _selectedExistsVersion = InitSelectedExistsVersion;
        //        return _selectedExistsVersion;
        //    }
        //    set
        //    {
        //        _selectedExistsVersion = value;

        //        // この項目は外部からプログラムで制御することがないため、PropertyChangedを実装しない

        //        // Selectedが変わった段階でnew Versionsの候補を出すか否かを更新する
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNewVers"));
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReverseShowNewVers"));
        //    }
        //}

        /// <summary>
        /// New VersionsのComboboxなどを表示するか
        /// </summary>
        //public bool ShowNewVers
        //{
        //    get
        //    {
        //        // 【new Version】が選択されているか
        //        return SelectedExistsVersion == ExistsVersions[ExistsVersions.Count - 1];
        //    }
        //}
        //public bool ReverseShowNewVers
        //{
        //    get
        //    {
        //        return !ShowNewVers;
        //    }
        //}


        // Worldの表示に関連
        // TODO: ワールドの表示処理を実装する
        //private List<string> _Worlds
        //{
        //    get
        //    {

        //    }
        //}
        //private string _selectedWorld;
        //public string SelectedWorld;



        // ボタンなどに呼応した処理
        public RunCommand RunCommand { get; private set; }
        public DeleteCommand DeleteCommand { get; private set; }
        public CloseCommand CloseCommand { get; private set; }


        // Window
        public SettingCommand SettingCommand { get; private set; }
        public WorldSettingCommand WorldSettingCommand { get; private set; }


        //public MainWindowVM(IShowWindowService<SystemSettingsVM> ssWindow, IShowWindowService<WorldSettingsVM> wsWindow)
        public MainWindowVM()
        {
            // General
            RunCommand = new RunCommand(this);
            DeleteCommand = new DeleteCommand(this);
            CloseCommand = new CloseCommand(this);


            // Version
            ExistsVersions = new ObservableCollection<Version>(AllVers.Where(ver => ver.Exists))
            {
                new VanillaVersion("【new Version】", "", true, false)
            };
            // TODO: LatestRunのデータをstringではなく、VersionやIWorldで持つべき？
            // Defaultについては以前の開設バージョンがなければ、リストの一番上の要素を選択する実装にする
            Version firstSelectVer = SaveData.LatestRun == null ? ExistsVersions[0] : VersionFactory.Instance.GetVersionFromName(SaveData.LatestRun.VersionName);
            ExistsVersionIndex = new BindingValue<Version>(firstSelectVer, () => OnPropertyChanged("ShowNewVersions"));
            NewVersions = new ObservableCollection<Version>(AllVers.OfType<VanillaVersion>());
            ShowAll = new BindingValue<bool>(false, () => UpdateNewVersions());
            ShowSpigot = new BindingValue<bool>(false, () => UpdateNewVersions());


            // World
            Worlds = new ObservableCollection<IWorld>(AllWorlds);
            IWorld firstSelectWor = Worlds[0];
            if (SaveData.LatestRun != null)
            {
                LocalWorld targetWorld = LocalWorldCollection.Instance.FindLocalWorld(SaveData.LatestRun.VersionName, SaveData.LatestRun.WorldName);
                firstSelectWor = AllWorlds.OfType<World>().Where(world => world.LocalWorld == targetWorld).FirstOrDefault();
            }
            WorldIndex = new BindingValue<IWorld>(firstSelectWor, () => OnPropertyChanged("ShowNewWorld"));
            NewWorldName = new BindingValue<string>("Input World Name", () => OnPropertyChanged("CanRun"));

            // Setting
            ResetWorld = new BindingValue<bool>(false, () => OnPropertyChanged("ShowSaveWorld"));

            // Window
            //SettingCommand = new SettingCommand(this, ssWindow);
            //WorldSettingCommand = new WorldSettingCommand(this, wsWindow);
        }

        private void UpdateNewVersions()
        {
            if (ShowSpigot != null && ShowSpigot.Value)
            {
                NewVersions.ChangeCollection(AllVers.OfType<SpigotVersion>());
            }
            else
            {
                if (ShowAll != null && ShowAll.Value)
                    NewVersions.ChangeCollection(AllVers.OfType<VanillaVersion>());
                else
                    NewVersions.ChangeCollection(AllVers.OfType<VanillaVersion>().Where(ver => ver.IsRelease));
            }

            OnPropertyChanged("NewVersions");
        }
    }

    public class ExistsVersionConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Version version)
            {
                return version.Name;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new System.NotImplementedException();
        }
    }

    public class NewVersionConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VanillaVersion vanila)
            {
                if (vanila.IsLatest)
                {
                    if (vanila.IsRelease)
                        return $"【Latest Release】{vanila.Name}";
                    else
                        return $"【Latest SnapShot】{vanila.Name}";
                }
                else
                {
                    if (vanila.IsRelease)
                        return $"release {vanila.Name}";
                    else
                        return $"snapshot {vanila.Name}";
                }
            }

            if (value is SpigotVersion spigot)
            {
                return $"Spigot {spigot.Name}";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new System.NotImplementedException();
        }
    }

    public class WorldConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IWorld world)
            {
                return world.DisplayName;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new System.NotImplementedException();
        }
    }

}
