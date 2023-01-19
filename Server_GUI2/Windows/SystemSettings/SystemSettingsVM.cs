using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Image = Server_GUI2.Windows.MessageBox.Back.Image;

namespace Server_GUI2.Windows.SystemSettings
{
    class SystemSettingsVM : GeneralVM, IDataErrorInfo
    {
        public Properties.Resources Resources { get; private set; }
        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;
        static readonly StorageCollection Storages = StorageCollection.Instance;

        // 設定項目の表示非表示を操作
        public BindingValue<int> MenuIndex { get; private set; }
        public bool ShowServer => MenuIndex.Value == 0;
        public bool ShowPlayers => MenuIndex.Value == 1;
        public bool ShowSW => MenuIndex.Value == 2;
        public bool ShowNet => MenuIndex.Value == 3;
        public bool ShowOthers => MenuIndex.Value == 4;

        // List操作
        public AddListCommand AddListCommand { get; private set; }
        public EditListCommand EditListCommand { get; private set; }
        public DeleteListCommand DeleteListCommand { get; private set; }

        // ShareWorld
        public BindingValue<string> AccountName { get; private set; }
        public BindingValue<string> AccountEmail { get; private set; }
        public BindingValue<string> RepoName { get; private set; }
        public bool HasSWContents => CheckHasContent(new List<BindingValue<string>> { AccountName, AccountEmail, RepoName }, "");
        public bool GitInstalled => GitStorage.GitInstalled;
        public bool CanAddition_SW => HasSWContents & GitInstalled & !RemoteAdding.Value;
        public BindingValue<bool> RemoteAdding { get; private set; }
        public ObservableCollection<IRemoteWorld> RemoteList { get; private set; }
        public BindingValue<IRemoteWorld> RLIndex { get; private set; }
        public CredentialManagerCommand CredentialManagerCommand { get; private set; }

        // Server
        public bool[] BoolCombo => new bool[2] { true, false };
        public string[] DifficultyCombo => new string[4] { "peaceful", "easy", "normal", "hard" };
        public string[] GamemodeCombo => new string[4] { "survival", "creative", "adventure", "spectator" };
        public string[] TypeCombo => new string[4] { "default", "flat", "largeBiomes", "amplified" };
        /// <summary>
        /// MainSettingsで使用（最終保存データを格納）
        /// </summary>
        public BindingValue<ServerProperty> PropertyIndexs { get; private set; }
        public ObservableCollection<BoolOption> BoolOptions { get; private set; }
        public ObservableCollection<TextOption> TextOptions { get; private set; }
        // Server Memory
        private string _memorySize;
        public string MemorySize
        {
            get => _memorySize;
            set
            {
                _memorySize = value;
                OnPropertyChanged("CanSave");
            }
        }
        public BindingValue<string> MemoryUnitIndex { get; private set; }
        public string[] MemoryUnitList => ServerMemorySize.UnitList.Select(x => $"{x}B").ToArray();
        public bool ValidMemorySize => int.TryParse(MemorySize, out int memory) && memory > 0;

        // Players
        public BindingValue<int> PlayersTabIndex { get; private set; }
        // Player
        public BindingValue<string> PlayerName { get; private set; }
        public bool CanAddition_Pl => CheckHasContent(PlayerName, "");
        public ObservableCollection<Player> PlayerList { get; private set; }
        public Player PLIndex { get; set; } = null;
        // Group
        public BindingValue<string> GroupName { get; private set; }
        public ObservableCollection<Player> PlayerList_Group { get; private set; }
        public BindingValue<Player> PLGIndex { get; private set; }
        public ObservableCollection<Player> MemberList { get; private set; }
        public BindingValue<Player> MLIndex { get; private set; }
        public bool CanAddition_Gr
        {
            get
            {
                bool hasName = CheckHasContent(GroupName, "");
                bool hasMembers = CheckHasContent(MemberList);
                return hasName && hasMembers;
            }
        }
        public ObservableCollection<PlayerGroup> GroupList { get; private set; }
        public PlayerGroup GLIndex { get; set; } = null;


        // Network
        public BindingValue<bool> UsingPortMapping { get; private set; }
        // Error処理をする場合は対象の変数がget, set両方を持っている必要があるため、このような実装にしている
        private string _portNumber;
        public string PortNumber
        {
            get => _portNumber;
            set
            {
                _portNumber = value;
                OnPropertyChanged(new string[2]{ "CanAddition_Po", "CanSave" });
            }
        }
        public bool CanWritePortNumber => UsingPortMapping != null && UsingPortMapping.Value;
        public bool ValidPortNumber => int.TryParse(PortNumber, out int port) && (0 < port && port < 65535);
        public bool AlreadyOpened => (PortStatus.Value?.StatusEnum) != null && PortStatus.Value.StatusEnum == Develop.Util.PortStatus.Status.Open;
        public bool CanAddition_Po => ValidPortNumber && CanWritePortNumber && !AlreadyOpened;
        public AddPortCommand AddPortCommand { get; private set; }
        public string GlobalIP { get { return NetWork.GlobalIP; } }
        public string LocalIP { get { return NetWork.LocalIP; } }
        public ClipbordCommand ClipbordCommand { get; private set; }
        public BindingValue<PortStatus> PortStatus { get; private set; }
        public string StatusColor
        {
            get
            {
                switch (PortStatus.Value.StatusEnum)
                {
                    case Develop.Util.PortStatus.Status.Failed:
                        return "Red";
                    case Develop.Util.PortStatus.Status.Registering:
                        return "Yellow";
                    default:
                        return "#7CBB00";
                }
            }
        }

        // Others
        public string SystemVersion { get { return ManageSystemVersion.StarterVersion; } }
        public BindingValue<string> UserName { get; private set; }
        public HelpCommand HelpCommand { get; private set; }
        public TwitterCommand TwitterCommand { get; private set; }
        public GitCommandVM GitCommandVM { get; private set; }
        public BindingValue<string> LanguageSelected { get; private set; }
        public Dictionary<string, string> Languages => UserSettings.Languages;


        // END Process
        public bool CanSave => !RemoteAdding.Value & ValidPortNumber & ValidMemorySize;
        public SaveCommand SaveCommand { get; private set; }
        public bool Saved = false;


        // ErrorProcess
        public string Error { get { return null; } }
        public string this[string columnName] => CheckInputBox(columnName);




        public SystemSettingsVM()
        {
            // General
            Resources = new Properties.Resources();
            MenuIndex = new BindingValue<int>(0, () => OnPropertyChanged(new string[5] { "ShowSW", "ShowServer", "ShowPlayers", "ShowNet", "ShowOthers" }));
            AddListCommand = new AddListCommand(this);
            EditListCommand = new EditListCommand(this);
            DeleteListCommand = new DeleteListCommand(this);

            // ShareWorld
            AccountName = new BindingValue<string>("", () => OnPropertyChanged(new string[2] { "AccountName", "CanAddition_SW" }));
            AccountEmail = new BindingValue<string>("", () => OnPropertyChanged(new string[2] { "AccountEmail", "CanAddition_SW" }));
            RepoName = new BindingValue<string>("ShareWorld", () => OnPropertyChanged(new string[2] { "RepoName", "CanAddition_SW" }));
            RemoteAdding = new BindingValue<bool>(false, () => OnPropertyChanged(new string[3] { "RemoteAdding", "CanAddition_SW", "CanSave" }));
            RemoteList = new ObservableCollection<IRemoteWorld>(Storages.Storages.SelectMany(storage => storage.RemoteWorlds).OfType<RemoteWorld>());
            RemoteList.AddRange(Storages.Storages.Where(storage => storage.RemoteWorlds.Count == 1).SelectMany(storage => storage.RemoteWorlds));  /*RemoteWorldsがないレポジトリを表示するための処理*/
            RLIndex = new BindingValue<IRemoteWorld>(null, () => OnPropertyChanged("RLIndex"));
            CredentialManagerCommand = new CredentialManagerCommand(this);

            // Server
            // TODO: システム更新ツールの改修（GitHubの読み込みを変更）
            ServerProperty defaultProperties = SaveData.DefaultProperties;
            PropertyIndexs = new BindingValue<ServerProperty>(new ServerProperty(defaultProperties), () => OnPropertyChanged("PropertyIndexs"));
            BoolOptions = BoolOption.GetBoolCollection(PropertyIndexs.Value.BoolOption, new string[2] { "hardcore", "white-list" });
            TextOptions = TextOption.GetTextCollection(PropertyIndexs.Value.StringOption, new string[4] { "difficulty", "gamemode", "level-type", "level-name" });
            MemorySize = UserSettings.Instance.userSettings.ServerMemorySize.Size.ToString();
            MemoryUnitIndex = new BindingValue<string>($"{UserSettings.Instance.userSettings.ServerMemorySize.Unit}B", () => OnPropertyChanged("MemoryUnitIndex"));

            // Players
            PlayersTabIndex = new BindingValue<int>(0, () => UpdateGroupPlayersAndMembers());
            // Player
            PlayerName = new BindingValue<string>("", () => OnPropertyChanged("CanAddition_Pl"));
            PlayerList = new ObservableCollection<Player>(SaveData.Players);
            PlayerList.Sort();
            // Group
            GroupName = new BindingValue<string>("", () => OnPropertyChanged(new string[2] { "GroupName", "CanAddition_Gr" }));
            PlayerList_Group = new ObservableCollection<Player>(SaveData.Players);
            PlayerList_Group.Sort();
            PLGIndex = new BindingValue<Player>(null, () => OnPropertyChanged("CanAddition_Gr"));
            MemberList = new ObservableCollection<Player>();
            MLIndex = new BindingValue<Player>(null, () => OnPropertyChanged("CanAddition_Gr"));
            GroupList = new ObservableCollection<PlayerGroup>(SaveData.PlayerGroups);
            GroupList.Sort();


            // Network
            int portNum = SaveData.PortSettings.PortNumber;
            UsingPortMapping = new BindingValue<bool>(SaveData.PortSettings.UsingPortMapping, () => UpdateUsingPortMapping());
            PortNumber = portNum.ToString();
            AddPortCommand = new AddPortCommand(this);
            ClipbordCommand = new ClipbordCommand(this);
            PortStatus = new BindingValue<PortStatus>(new PortStatus(portNum, Develop.Util.PortStatus.Status.Ready), () => OnPropertyChanged(new string[2] { "PortStatus", "StatusColor" }));

            // Others
            UserName = new BindingValue<string>(UserSettings.Instance.userSettings.OwnerName, () => OnPropertyChanged("UserName"));
            LanguageSelected = new BindingValue<string>(Languages.FirstOrDefault(x => x.Value == UserSettings.Instance.userSettings.Language).Key ?? "English", () => UpdateLanguage());
            HelpCommand = new HelpCommand(this);
            TwitterCommand = new TwitterCommand(this);
            GitCommandVM = new GitCommandVM(this);

            // END Process
            SaveCommand = new SaveCommand(this);

            // 複数スレッドからコレクション操作できるようにする
            BindingOperations.EnableCollectionSynchronization(RemoteList, new object());
        }

        /// <summary>
        /// PlayersタブのGroup内のPlayerとMemberの内容を更新する
        /// </summary>
        public void UpdateGroupPlayersAndMembers()
        {
            if (PlayersTabIndex == null || PlayersTabIndex.Value == 0)
                return;

            // Membersに登録されていないものをPlayersに登録
            PlayerList_Group.ChangeCollection(PlayerList.Where(player => !MemberList.Contains(player)));

            // Membersの中で登録済みのプレイヤーでないものを削除
            MemberList.RemoveAll(player => !PlayerList.Contains(player));

            // GroupListの中で登録済みプレイやー出ないものを削除＆削除した結果、メンバーのいなくなったグループを削除
            foreach (var group in GroupList)
                group.PlayerList.RemoveAll(player => !PlayerList.Contains(player));
            GroupList.RemoveAll(group => group.PlayerList.Count == 0);

            // RegisterボタンのIsEnabledを更新
            OnPropertyChanged("CanAddition_Gr");
        }

        private void UpdateUsingPortMapping()
        {
            OnPropertyChanged(new string[4] { "OtherPropertyIndexs", "UsingPortMapping", "CanWritePortNumber", "CanAddition_Po" });

            if (UsingPortMapping != null && UsingPortMapping.Value)
            {
                CustomMessageBox.Show(
                    Properties.Resources.SystemSettings_PortOn1, 
                    ButtonType.OK, 
                    Image.Infomation,
                    new LinkMessage(Properties.Resources.SystemSettings_PortOn2, "https://civiltt.github.io/ServerStarter/")
                    );
            }
        }

        private string CheckInputBox(string propertyName)
        {
            switch (propertyName)
            {
                case "PortNumber":
                    if (!ValidPortNumber)
                        return Properties.Resources.SystemSettings_CheckPort;
                    break;
                case "MemorySize":
                    if (!ValidMemorySize)
                        return Properties.Resources.MemorySizeError;
                    break;
                default:
                    break;
            }

            return "";
        }

        public void SaveSystemSettings()
        {
            // Auto Port Mappingの設定確認
            WarningPort();

            // 既存のデータを変更する形で処理
            UserSettings.Instance.userSettings.OwnerName = UserName.Value;
            UserSettings.Instance.userSettings.Language = Languages[LanguageSelected.Value];
            PropertyIndexs.Value = BoolOption.SetBoolOption(BoolOptions, PropertyIndexs.Value);
            PropertyIndexs.Value = TextOption.SetStringOption(TextOptions, PropertyIndexs.Value);
            UserSettings.Instance.userSettings.ServerMemorySize = new ServerMemorySize(int.Parse(MemorySize), MemoryUnitIndex.Value[0].ToString());
            UserSettings.Instance.userSettings.DefaultProperties = new ServerProperty(PropertyIndexs.Value);
            UserSettings.Instance.userSettings.Players = PlayerList.ToList();
            UserSettings.Instance.userSettings.PlayerGroups = new List<PlayerGroup>(GroupList);

            UserSettings.Instance.WriteFile();
        }

        private void WarningPort()
        {
            if (!ValidPortNumber)
            {
                CustomMessageBox.Show(Properties.Resources.SystemSettings_Warn, ButtonType.OK, Image.Error);
                UserSettings.Instance.userSettings.PortSettings.UsingPortMapping = false;
                UserSettings.Instance.userSettings.PortSettings.PortNumber = 25565;
            }
            else
            {
                UserSettings.Instance.userSettings.PortSettings.UsingPortMapping = UsingPortMapping.Value;
                UserSettings.Instance.userSettings.PortSettings.PortNumber = int.Parse(PortNumber);
            }
        }

        private void UpdateLanguage()
        {
            if (LanguageSelected?.Value != null)
                Properties.Resources.Culture = CultureInfo.GetCultureInfo(Languages[LanguageSelected.Value]);
            OnPropertyChanged("Resources");
        }
    }

    public class MemberListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Player> memberList)
            {
                return string.Join(",  ", memberList.Select(x => x.Name));
            }

            // 流れてきた値がObservableCollection<Player>ではない場合の処理
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Windowから値が入らないため実装の必要性がない
            throw new NotImplementedException();
        }
    }

    public class BoolOption
    {
        public string Property { get; }
        public string ToolTip { get; } = Properties.Resources.P_Other;
        public bool Value { get; set; }

        private BoolOption(string property, bool value)
        {
            Property = property;
            Value = value;

            bool hasdescription = ServerProperty.Descriptions.TryGetValue(property, out string tooltip);
            if (hasdescription && tooltip != "")
                ToolTip = tooltip;
        }

        public static ObservableCollection<BoolOption> GetBoolCollection(SortedDictionary<string, bool> boolOption, string[] removeProps)
        {
            var resultCollection = new ObservableCollection<BoolOption>();
            foreach (var kvp in boolOption)
            {
                if (removeProps.Contains(kvp.Key))
                    continue;
                resultCollection.Add(new BoolOption(kvp.Key, kvp.Value));
            }

            return resultCollection;
        }

        public static ServerProperty SetBoolOption(ObservableCollection<BoolOption> boolCollection, ServerProperty sourceProp)
        {
            foreach (var boolPart in boolCollection)
                sourceProp.BoolOption[boolPart.Property] = boolPart.Value;

            return sourceProp;
        }
    }

    public class TextOption
    {
        public string Property { get; }
        public string ToolTip { get; } = Properties.Resources.P_Other;
        public string Value { get; set; }

        private TextOption(string property, string value)
        {
            Property = property;
            Value = value;

            bool hasdescription = ServerProperty.Descriptions.TryGetValue(property, out string tooltip);
            if (hasdescription && tooltip != "")
                ToolTip = tooltip;
        }

        public static ObservableCollection<TextOption> GetTextCollection(SortedDictionary<string, string> boolOption, string[] removeProps)
        {
            var resultCollection = new ObservableCollection<TextOption>();
            foreach (var kvp in boolOption)
            {
                if (removeProps.Contains(kvp.Key))
                    continue;
                resultCollection.Add(new TextOption(kvp.Key, kvp.Value));
            }

            return resultCollection;
        }

        public static ServerProperty SetStringOption(ObservableCollection<TextOption> textCollection, ServerProperty sourceProp)
        {
            foreach (var boolPart in textCollection)
                sourceProp.StringOption[boolPart.Property] = boolPart.Value;

            return sourceProp;
        }
    }
}
