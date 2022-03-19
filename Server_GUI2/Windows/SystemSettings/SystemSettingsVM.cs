using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Server_GUI2.Util;
using Server_GUI2.Develop.Util;
using MW = ModernWpf;
using System.Text.RegularExpressions;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Server;

namespace Server_GUI2.Windows.SystemSettings
{
    class SystemSettingsVM : GeneralVM, IDataErrorInfo
    {
        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;
        static readonly StorageCollection Storages = StorageCollection.Instance;

        // 設定項目の表示非表示を操作
        public BindingValue<int> MenuIndex { get; private set; }
        public bool ShowSW => MenuIndex.Value == 0;
        public bool ShowServer => MenuIndex.Value == 1;
        public bool ShowPlayers => MenuIndex.Value == 2;
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
        public bool CanAddition_SW => CheckHasContent(new List<BindingValue<string>> { AccountName, AccountEmail, RepoName }, "");
        public ObservableCollection<Storage> RemoteList { get; private set; }
        public BindingValue<Storage> RLIndex { get; private set; }
        public bool HasBranch => RLIndex.Value?.RemoteWorlds.Count != 0;
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
        /// <summary>
        /// TrueFalseの左側の項目一覧
        /// </summary>
        public string[] OtherTFPropertyIndexs
        {
            get
            {
                ServerProperty defaultProperties = SaveData.DefaultProperties;
                string[] removeIndex = new string[2] { "hardcore", "white-list" };
                List<string> allindex = defaultProperties.BoolOption.Keys.ToList();
                allindex.RemoveAll(index => removeIndex.Contains(index));
                return allindex.ToArray();
            }
        }
        /// <summary>
        /// TrueFalseの左側で選択している項目
        /// </summary>
        public BindingValue<string> SelectedTFIndex { get; private set; }
        /// <summary>
        /// Stringの左側の項目一覧
        /// </summary>
        public string[] OtherPropertyIndexs
        {
            get
            {
                ServerProperty defaultProperties = SaveData.DefaultProperties;
                string[] removeIndex = new string[4] { "difficulty", "gamemode", "level-type", "level-name" };
                List<string> allindex = defaultProperties.StringOption.Keys.ToList();
                allindex.RemoveAll(index => removeIndex.Contains(index));
                return allindex.ToArray();
            }
        }
        /// <summary>
        /// Stringの左側で選択している項目
        /// </summary>
        public BindingValue<string> SelectedPropIndex { get; private set; }
        /// <summary>
        /// TrueFalseの右側で選択している項目
        /// </summary>
        public bool SelectedTFProperty
        {
            get => PropertyIndexs.Value.BoolOption[SelectedTFIndex.Value];
            set => PropertyIndexs.Value.BoolOption[SelectedTFIndex.Value] = value;
        }
        /// <summary>
        /// Stringの右側の記載事項
        /// </summary>
        public string OtherStringProperty
        {
            get => PropertyIndexs.Value.StringOption[SelectedPropIndex.Value];
            set => PropertyIndexs.Value.StringOption[SelectedPropIndex.Value] = value;
        }


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
                OnPropertyChanged("CanAddition_Po");
            }
        }
        public bool CanWritePortNumber => UsingPortMapping != null && UsingPortMapping.Value;
        public bool ValidPortNumber => int.TryParse(PortNumber, out int port) && (0 < port && port < 65535);
        public bool AlreadyOpened => (PortStatus.Value?.StatusEnum.Value) != null && PortStatus.Value.StatusEnum.Value == Develop.Util.PortStatus.Status.Open;
        public bool CanAddition_Po => ValidPortNumber && CanWritePortNumber && !AlreadyOpened;
        public AddPortCommand AddPortCommand { get; private set; }
        public string GlobalIP { get { return NetWork.GlobalIP; } }
        public string LocalIP { get { return NetWork.LocalIP; } }
        public ClipbordCommand ClipbordCommand { get; private set; }
        public BindingValue<PortStatus> PortStatus { get; private set; }

        // Others
        public string SystemVersion { get { return ManageSystemVersion.StarterVersion; } }
        public BindingValue<string> UserName { get; private set; }
        public TwitterCommand TwitterCommand { get; private set; }
        public GitCommandVM GitCommandVM { get; private set; }


        // END Process
        public SaveCommand SaveCommand { get; private set; }
        public bool Saved = false;


        // ErrorProcess
        public string Error { get { return null; } }
        public string this[string columnName] => CheckInputBox(columnName);


        public SystemSettingsVM()
        {
            // General
            MenuIndex = new BindingValue<int>(0, () => OnPropertyChanged(new string[5] { "ShowSW", "ShowServer", "ShowPlayers", "ShowNet", "ShowOthers" }));
            AddListCommand = new AddListCommand(this);
            EditListCommand = new EditListCommand(this);
            DeleteListCommand = new DeleteListCommand(this);

            // ShareWorld
            AccountName = new BindingValue<string>("", () => OnPropertyChanged(new string[2] { "AccountName", "CanAddition_SW" }));
            AccountEmail = new BindingValue<string>("", () => OnPropertyChanged(new string[2] { "AccountEmail", "CanAddition_SW" }));
            RepoName = new BindingValue<string>("ShareWorld", () => OnPropertyChanged(new string[2] { "RepoName", "CanAddition_SW" }));
            RemoteList = Storages.Storages;
            RLIndex = new BindingValue<Storage>(null, () => OnPropertyChanged(new string[2] { "RLIndex", "HasBranch" }));
            CredentialManagerCommand = new CredentialManagerCommand(this);

            // Server
            ServerProperty defaultProperties = SaveData.DefaultProperties;
            PropertyIndexs = new BindingValue<ServerProperty>(new ServerProperty(defaultProperties), () => OnPropertyChanged("PropertyIndexs"));
            SelectedTFIndex = new BindingValue<string>(OtherTFPropertyIndexs[0], () => OnPropertyChanged("SelectedTFProperty"));
            SelectedPropIndex = new BindingValue<string>(OtherPropertyIndexs[0], () => OnPropertyChanged("OtherStringProperty"));

            // Players
            PlayersTabIndex = new BindingValue<int>(0, () => UpdateGroupPlayersAndMembers());
            // Player
            PlayerName = new BindingValue<string>("", () => OnPropertyChanged("CanAddition_Pl"));
            PlayerList = new ObservableCollection<Player>(SaveData.Players);
            // Group
            GroupName = new BindingValue<string>("", () => OnPropertyChanged(new string[2] { "GroupName", "CanAddition_Gr" }));
            PlayerList_Group = new ObservableCollection<Player>(SaveData.Players);
            PLGIndex = new BindingValue<Player>(null, () => OnPropertyChanged("CanAddition_Gr"));
            MemberList = new ObservableCollection<Player>();
            MLIndex = new BindingValue<Player>(null, () => OnPropertyChanged("CanAddition_Gr"));
            GroupList = new ObservableCollection<PlayerGroup>(SaveData.PlayerGroups);


            // Network
            int portNum = SaveData.PortSettings.PortNumber;
            UsingPortMapping = new BindingValue<bool>(SaveData.PortSettings.UsingPortMapping, () => );
            PortNumber = portNum.ToString();
            AddPortCommand = new AddPortCommand(this);
            ClipbordCommand = new ClipbordCommand(this);
            PortStatus = new BindingValue<PortStatus>(new PortStatus(portNum, Develop.Util.PortStatus.Status.Ready), () => OnPropertyChanged("PortStatus"));

            // Others
            UserName = new BindingValue<string>(UserSettings.Instance.userSettings.PlayerName, () => OnPropertyChanged("UserName"));
            TwitterCommand = new TwitterCommand(this);
            GitCommandVM = new GitCommandVM(this);

            // END Process
            SaveCommand = new SaveCommand(this);
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
        }

        private void UpdateUsingPortMapping()
        {
            OnPropertyChanged(new string[4] { "OtherPropertyIndexs", "UsingPortMapping", "CanWritePortNumber", "CanAddition_Po" });

            if (UsingPortMapping.Value)
            {
                string message =
                    "この機能はルータに対してポート開放を行います。" +
                    "セキュリティソフトに対してはポート開放されないため、必要に応じてご自身で設定してください。" +
                    "詳細は設定方法をご参照ください。";
                MW.MessageBox.Show(message, "Server Starter", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private string CheckInputBox(string propertyName)
        {
            switch (propertyName)
            {
                case "PortNumber":
                    if (!ValidPortNumber)
                        return "This Port Number is not valid";
                    break;
                default:
                    break;
            }

            return "";
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
}
