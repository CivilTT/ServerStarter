using log4net;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Windows.WorldSettings;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Server_GUI2.Windows.MainWindow
{
    class MainWindowVM : GeneralVM, IDataErrorInfo
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;

        // 一般
        public string StarterVersion { get { return $"ver {ManageSystemVersion.StarterVersion}"; } }
        public string PlayerName { get { return UserSettings.Instance.userSettings.OwnerName; } }
        public string OpContents { get { return $"{PlayerName} {Properties.Resources.Main_Op}"; } }
        public Properties.Resources Resources => new Properties.Resources();
        readonly ObservableCollection<Version> AllVers = VersionFactory.Instance.Versions;
        readonly ObservableCollection<IWorld> AllWorlds = WorldCollection.Instance.Worlds;


        // General
        public bool CanRun => !ShowNewWorld || ValidWorldName;


        // 新規Versionの表示に関連
        /// <summary>
        /// ToggleSwitchのOnOffを保持
        /// </summary>
        public BindingValue<bool> ShowAll { get; private set; }
        public BindingValue<bool> ShowSpigot { get; private set; }

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
        public bool ShowNewWorld => (WorldIndex.Value?.DisplayName ?? "") == "【new World】";
        private string _newWorldName;
        public string NewWorldName
        {
            get => _newWorldName;
            set
            {
                _newWorldName = value;
                UpdateNewWorld();
            }
        }
        private bool ValidWorldName
        {
            get
            {
                if (WorldIndex.Value is NewWorld world)
                    return world.IsUseableName(NewWorldName);
                return true;
            }
        }
        public IWorld RunWorld => WorldIndex.Value;

        // Setting
        public BindingValue<bool> ResetWorld { get; private set; }
        public bool SaveWorld { get; set; }
        public bool ShowSaveWorld => ResetWorld != null && ResetWorld.Value;
        public bool HasOwner => UserSettings.Instance.userSettings.OwnerName != "";
        public bool OwnerHasOp
        {
            get
            {
                if (!HasOwner)
                    return false;

                Player owner = SaveData.Players.Where(player => player.Name == PlayerName).FirstOrDefault();
                if (WorldIndex.Value == null)
                    return false;
                OpsRecord ownerOp = new OpsRecord(owner, 4);
                return WorldIndex.Value.Settings.Ops.Contains(ownerOp);
            }
            set
            {
                Player owner = SaveData.Players.Where(player => player.Name == PlayerName).FirstOrDefault();
                if (owner == null)
                    return;

                if (value)
                {
                    OpsRecord ownerOp = new OpsRecord(owner, 4);
                    WorldIndex.Value.Settings.Ops.Add(ownerOp);
                }
                else
                {
                    OpsRecord ownerOp = WorldIndex.Value.Settings.Ops.Where(player => player.Name == PlayerName).FirstOrDefault();
                    WorldIndex.Value.Settings.Ops.Remove(ownerOp);
                }
            }
        }
        public bool ShutdownPC { get; set; } = SaveData.ShutdownPC;
        public SetShutdown SetShutdown { get; private set; }

        // ボタンなどに呼応した処理
        public RunCommand RunCommand { get; private set; }
        public DeleteCommand DeleteCommand { get; private set; }
        public CloseCommand CloseCommand { get; private set; }


        // Window
        public SettingCommand SettingCommand { get; private set; }
        public WorldSettingCommand WorldSettingCommand { get; private set; }


        // Error Process
        public string Error { get { return null; } }
        public string this[string columnName] => CheckInputBox(columnName);

        public MainWindowVM(IShowWindowService<SystemSettingsVM> ssWindow, IShowWindowService<WorldSettingsVM> wsWindow)
        {
            // General
            RunCommand = new RunCommand(this);
            DeleteCommand = new DeleteCommand(this);
            CloseCommand = new CloseCommand(this);
            SetUp.InitProgressBar.AddMessage(Properties.Resources.InitBar_SetCommands);


            // Version
            ExistsVersions = new ObservableCollection<Version>(AllVers.Where(ver => ver.Exists))
            {
                new VanillaVersion("【new Version】", "", true, false)
            };
            ExistsVersionIndex = new BindingValue<Version>(SetFirstVer(), () => OnPropertyChanged(new string[2] { "ExistsVersionIndex", "ShowNewVersions" }));
            NewVersions = new ObservableCollection<Version>(AllVers.OfType<VanillaVersion>());
            NewVersionIndex = new BindingValue<Version>(NewVersions.FirstOrDefault(), () => OnPropertyChanged("NewVersionIndex"));
            ShowAll = new BindingValue<bool>(false, () => UpdateNewVersions());
            ShowSpigot = new BindingValue<bool>(false, () => UpdateNewVersions());
            SetUp.InitProgressBar.AddMessage(Properties.Resources.InitBar_MCVer);


            // World
            Worlds = new ObservableCollection<IWorld>(AllWorlds);
            WorldIndex = new BindingValue<IWorld>(SetFirstWor(), () => OnPropertyChanged(new string[4] { "WorldIndex", "ShowNewWorld", "CanRun", "OwnerHasOp" }));
            NewWorldName = Properties.Resources.Main_InputName;
            SetUp.InitProgressBar.AddMessage(Properties.Resources.InitBar_MCWor);


            // Setting
            ResetWorld = new BindingValue<bool>(false, () => OnPropertyChanged("ShowSaveWorld"));
            SetShutdown = new SetShutdown(this);


            // Window
            SettingCommand = new SettingCommand(this, ssWindow);
            WorldSettingCommand = new WorldSettingCommand(this, wsWindow);
            SetUp.InitProgressBar.AddMessage(Properties.Resources.InitBar_Close);
            SetUp.InitProgressBar.Close();
        }

        private Version SetFirstVer()
        {
            if (SaveData.LatestRun == null || SaveData.LatestRun.VersionName == null || SaveData.LatestRun.VersionName == "")
                return ExistsVersions.FirstOrDefault();
            else
            {
                var result = VersionFactory.Instance.GetVersionFromName(SaveData.LatestRun.VersionName);
                return ExistsVersions.Contains(result) ? result : ExistsVersions.FirstOrDefault();
            }
        }

        private IWorld SetFirstWor()
        {
            if (SaveData.LatestRun == null || SaveData.LatestRun.WorldName == null || SaveData.LatestRun.WorldName == "")
            {
                return Worlds.FirstOrDefault();
            }
            else
            {
                LocalWorld targetLocalWorld = LocalWorldCollection.Instance.FindLocalWorld(RunVersion.Name, SaveData.LatestRun.WorldName);
                var result = Worlds.OfType<World>().Where(x => x.LocalWorld == targetLocalWorld).FirstOrDefault();
                return result ?? Worlds.FirstOrDefault();
            }
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
            NewVersionIndex.Value = NewVersions.FirstOrDefault();
        }

        private void UpdateNewWorld()
        {
            OnPropertyChanged("CanRun");

            if (WorldIndex.Value is NewWorld world)
            {
                try
                {
                    world.Name = NewWorldName;
                }
                catch (WorldException ex)
                {
                    logger.Warn(ex.Message);
                }
            }
        }

        private string CheckInputBox(string propertyName)
        {
            switch (propertyName)
            {
                case "NewWorldName":
                    if (!ValidWorldName)
                        return Properties.Resources.Main_InvalidName;
                    break;
                default:
                    break;
            }

            return "";
        }
    }

    public class ExistsVersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Version version)
            {
                return version.Name;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new NotImplementedException();
        }
    }

    public class NewVersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                return $"{spigot.Name}";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new NotImplementedException();
        }
    }

    public class WorldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IWorld world)
            {
                return world.DisplayName;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new NotImplementedException();
        }
    }

}
