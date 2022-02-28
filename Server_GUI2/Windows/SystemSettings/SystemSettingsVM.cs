using Newtonsoft.Json;
using Server_GUI2.Windows.ViewModels;
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

namespace Server_GUI2.Windows.SystemSettings
{
    class SystemSettingsVM : GeneralVM
    {
        readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;

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
        public ObservableCollection<AccountInfo> RemoteList { get; private set; }
        public BindingValue<AccountInfo> RLIndex { get; private set; }
        public bool HasBranch => (RLIndex.Value?.Branch ?? "null") != "";
        public CredentialManagerCommand CredentialManagerCommand { get; private set; }

        // Server
        public bool[] BoolCombo => new bool[2] { true, false };
        public string[] DifficultyCombo => new string[4] { "peaceful", "easy", "normal", "hard" };
        public string[] GamemodeCombo => new string[4] { "survival", "creative", "adventure", "spectator" };
        public string[] TypeCombo => new string[4] { "default", "flat", "largeBiomes", "amplified" };
        public BindingValue<ServerProperty> PropertyIndexs { get; private set; }



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
        public BindingValue<string> PortNumber { get; private set; }
        public bool CanWritePortNumber => UsingPortMapping != null && UsingPortMapping.Value;
        public bool ValidPortNumber => int.TryParse(PortNumber.Value, out int port) && (0 < port && port < 65535);
        public bool AlreadyOpened => (PortStatus.Value?.StatusEnum.Value) != null && PortStatus.Value.StatusEnum.Value == Develop.Util.PortStatus.Status.Open;
        public bool CanAddition_Po => ValidPortNumber && CanWritePortNumber && !AlreadyOpened;
        public AddPortCommand AddPortCommand { get; private set; }
        readonly WebClient wc = new WebClient();
        public string IP { get { return wc.DownloadString("https://ipv4.icanhazip.com/").Replace("\\r\\n", "").Replace("\\n", "").Trim(); } }
        public ClipbordCommand ClipbordCommand { get; private set; }
        public BindingValue<PortStatus> PortStatus { get; private set; }

        // Others
        public string SystemVersion { get { return SetUp.StarterVersion; } }
        public BindingValue<string> UserName { get; private set; }
        public TwitterCommand TwitterCommand { get; private set; }
        public GitCommandVM GitCommandVM { get; private set; }


        // END Process
        public SaveCommand SaveCommand { get; private set; }


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
            RemoteList = new ObservableCollection<AccountInfo>(SaveData.RemoteContents);
            RLIndex = new BindingValue<AccountInfo>(null, () => OnPropertyChanged(new string[2] { "RLIndex", "HasBranch" }));
            CredentialManagerCommand = new CredentialManagerCommand(this);

            // Server
            ServerProperty defaultProperties = SaveData.DefaultProperties;
            PropertyIndexs = new BindingValue<ServerProperty>(defaultProperties, () => OnPropertyChanged("PropertyIndexs"));

            // Players
            PlayersTabIndex = new BindingValue<int>(0, () => UpdateGroupPlayersAndMembers());
            // Player
            PlayerName = new BindingValue<string>("", () => OnPropertyChanged("CanAddition_Pl"));
            PlayerList = new ObservableCollection<Player>(SaveData.Players);
            // Group
            GroupName = new BindingValue<string>("", () => OnPropertyChanged("CanAddition_Gr"));
            PlayerList_Group = new ObservableCollection<Player>(SaveData.Players);
            PLGIndex = new BindingValue<Player>(null, () => OnPropertyChanged("CanAddition_Gr"));
            MemberList = new ObservableCollection<Player>();
            MLIndex = new BindingValue<Player>(null, () => OnPropertyChanged("CanAddition_Gr"));
            Console.WriteLine(SaveData.PlayerGroups.Count);
            GroupList = new ObservableCollection<PlayerGroup>(SaveData.PlayerGroups);


            // Network
            UsingPortMapping = new BindingValue<bool>(SaveData.PortStatus != null, () => UpdateUsingPortMapping());
            PortNumber = new BindingValue<string>(SaveData.PortStatus?.PortNumber.ToString() ?? "25565", () => OnPropertyChanged("CanAddition_Po"));
            AddPortCommand = new AddPortCommand(this);
            ClipbordCommand = new ClipbordCommand(this);
            PortStatus status = SaveData.PortStatus;
            PortStatus defaultStatus = new PortStatus(25565, Develop.Util.PortStatus.Status.Ready);
            PortStatus = new BindingValue<PortStatus>(UsingPortMapping.Value && status.StatusEnum.Value == Develop.Util.PortStatus.Status.Open ? status : defaultStatus, () => OnPropertyChanged("PortStatus"));

            // Others
            UserName = new BindingValue<string>(UserSettings.Instance.userSettings.PlayerName, () => OnPropertyChanged("UserName"));
            TwitterCommand = new TwitterCommand(this);
            GitCommandVM = new GitCommandVM(this);

            // END Process
            SaveCommand = new SaveCommand(this);
            // TODO: ×ボタンによってWindowを終了する場合、info.jsonへの保存は行わない旨の警告を出す。（ただし、PortMappingに関しては、ポートを開放している場合、保存しない場合であっても閉鎖する）
        }

        /// <summary>
        /// PlayersタブのGroup内のPlayerとMemberの内容を更新する
        /// </summary>
        private void UpdateGroupPlayersAndMembers()
        {
            if (PlayersTabIndex == null || PlayersTabIndex.Value == 0)
                return;

            PlayerList_Group.Clear();
            List<Player> tmpMember = MemberList.ToList();

            // Membersに登録されていないものをPlayersに登録
            PlayerList.Where(player => !MemberList.Contains(player)).ToList().ForEach(player => PlayerList_Group.Add(player));
            // これをやっていきtmpMemberに残ったものが、「Membersの中で登録済みのプレイヤーでないもの」となる
            tmpMember.AddRange(PlayerList.Where(player => MemberList.Contains(player)));

            // Membersの中で登録済みのプレイヤーでないものを削除
            tmpMember.ForEach(member => MemberList.Remove(member));
        }

        private void UpdateUsingPortMapping()
        {
            OnPropertyChanged(new string[3] { "UsingPortMapping", "CanWritePortNumber", "CanAddition_Po" });

            if (UsingPortMapping == null || UsingPortMapping.Value)
                return;

            PortSetting portSetting = new PortSetting(this);
            _ = portSetting.DeletePort();
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

    public class AccountInfo : IEquatable<AccountInfo>, IComparable<AccountInfo>
    {
        [JsonProperty("Name")]
        public string Name { get; private set; }
        [JsonProperty("Email")]
        public string Email { get; private set; }
        [JsonProperty("Repository")]
        public string Repository { get; private set; }
        [JsonProperty("Branch")]
        public string Branch { get; private set; }
        [JsonProperty("IsShow")]
        public bool IsShow { get; set; }

        public AccountInfo(
            string name,
            string email,
            string repo,
            string branch,
            bool isshow = true)
        {
            Name = name;
            Email = email;
            Repository = repo;
            Branch = branch;
            IsShow = isshow;
        }

        public bool Equals(AccountInfo other)
        {
            return other.Name == Name;
        }

        public int CompareTo(AccountInfo other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public class PlayerGroup : IEquatable<PlayerGroup>, IComparable<PlayerGroup>
    {
        [JsonProperty("GroupName")]
        public string GroupName { get; private set; }

        [JsonProperty("PlayerList")]
        public ObservableCollection<Player> PlayerList { get; private set; }

        public PlayerGroup(string name, ObservableCollection<Player> list)
        {
            GroupName = name;
            PlayerList = list;
        }

        public bool Equals(PlayerGroup other)
        {
            return other.GroupName == GroupName;
        }

        public int CompareTo(PlayerGroup other)
        {
            GroupName.WriteLine();
            other.GroupName.WriteLine();
            return GroupName.CompareTo(other.GroupName);
        }
    }

    public class Player : IEquatable<Player>, IComparable<Player>
    {
        readonly WebClient wc;
        public string Name { get; private set; }
        public string UUID { get; private set; }

        public Player(string name)
        {
            wc = new WebClient();
            Name = name;
            UUID = "";
            UUID = GetUuid(name);
        }

        private string GetUuid(string name)
        {
            string url = $@"https://api.mojang.com/users/profiles/minecraft/{name}";
            string jsonStr = wc.DownloadString(url);

            dynamic root = JsonConvert.DeserializeObject(jsonStr);
            if (root == null)
                return "";

            string uuid = root.id;
            Name = root.name;

            string uuid_1 = uuid.Substring(0, 8);
            string uuid_2 = uuid.Substring(8, 4);
            string uuid_3 = uuid.Substring(12, 4);
            string uuid_4 = uuid.Substring(16, 4);
            string uuid_5 = uuid.Substring(20);
            uuid = uuid_1 + "-" + uuid_2 + "-" + uuid_3 + "-" + uuid_4 + "-" + uuid_5;

            return uuid;
        }

        public bool Equals(Player other)
        {
            return other.UUID == UUID;
        }

        public int CompareTo(Player other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
