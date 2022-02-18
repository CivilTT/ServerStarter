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

namespace Server_GUI2.Windows.SystemSettings
{
    class SystemSettingsVM : INotifyPropertyChanged, IOperateWindows
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Action Show { get; set; }
        public Action Hide { get; set; }
        public Action Close { get; set; }


        // 設定項目の表示非表示を操作
        private int _menuIndex = 0;
        public int MenuIndex
        {
            get => _menuIndex;
            set
            {
                _menuIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowSW"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowServer"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowPlayers"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNet"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowOthers"));
            }
        }
        public bool ShowSW => MenuIndex == 0;
        public bool ShowServer => MenuIndex == 1;
        public bool ShowPlayers => MenuIndex == 2;
        public bool ShowNet => MenuIndex == 3;
        public bool ShowOthers => MenuIndex == 4;

        // ShareWorld
        public BindingValue<string> AccountName { get; private set; }
        public BindingValue<string> AccountEmail { get; private set; }
        public BindingValue<string> RepoName { get; private set; }
        public bool CanAddition_SW
        {
            get
            {
                bool hasName = AccountName.Value != "" && AccountName.Value != null;
                bool hasEmail = AccountEmail.Value != "" && AccountEmail.Value != null;
                bool hasRepo = RepoName.Value != "" && RepoName.Value != null;
                return hasName && hasEmail && hasRepo;
            }
        }
        public AddListCommand AddListCommand { get; private set; }
        public DeleteListCommand DeleteListCommand { get; private set; }
        public ObservableCollection<AccountInfo> RemoteList { get; private set; }
        public BindingValue<AccountInfo> RLIndex { get; private set; }



        // Players
        public BindingValue<int> PlayersTabIndex { get; private set; }
        // Player
        public BindingValue<string> PlayerName { get; private set; }
        public bool CanAddition_Pl
        {
            get
            {
                bool hasName = PlayerName.Value != "" && PlayerName.Value != null;
                return hasName;
            }
        }
        public ObservableCollection<Player> PlayerList { get; private set; }
        public BindingValue<Player> PLIndex { get; private set; }
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
                bool hasName = GroupName.Value != "" && GroupName.Value != null;
                bool hasMembers = MemberList != null && MemberList.Count != 0;
                return hasName && hasMembers;
            }
        }
        public ObservableCollection<PlayerGroup> GroupList { get; private set; }
        public BindingValue<PlayerGroup> GLIndex { get; private set; }


        public SystemSettingsVM()
        {
            // General
            AddListCommand = new AddListCommand(this);
            DeleteListCommand = new DeleteListCommand(this);

            // ShareWorld
            AccountName = new BindingValue<string>("", () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AccountName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanAddition_SW"));
            });
            AccountEmail = new BindingValue<string>("", () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AccountEmail"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanAddition_SW"));
            });
            RepoName = new BindingValue<string>("ShareWorld", () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RepoName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanAddition_SW"));
            });
            RemoteList = new ObservableCollection<AccountInfo>();
            RLIndex = new BindingValue<AccountInfo>(null, () => new PropertyChangedEventArgs("RLIndex"));



            // Players
            PlayersTabIndex = new BindingValue<int>(0, () =>
            {
                if (PlayersTabIndex != null && PlayersTabIndex.Value == 1)
                    UpdateGroupPlayersAndMembers();
            });
            // Player
            PlayerName = new BindingValue<string>("", () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanAddition_Pl"));
            });
            PlayerList = new ObservableCollection<Player>();
            PLIndex = new BindingValue<Player>(null, () => new PropertyChangedEventArgs("PLIndex"));
            // Group
            GroupName = new BindingValue<string>("", () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GroupName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanAddition_Gr"));
            });
            PlayerList_Group = new ObservableCollection<Player>();
            PLGIndex = new BindingValue<Player>(null, () => new PropertyChangedEventArgs("PLGIndex"));
            MemberList = new ObservableCollection<Player>();
            MLIndex = new BindingValue<Player>(null, () => new PropertyChangedEventArgs("MLIndex"));
            GroupList = new ObservableCollection<PlayerGroup>();
            GLIndex = new BindingValue<PlayerGroup>(null, () => new PropertyChangedEventArgs("GLIndex"));
        }

        /// <summary>
        /// PlayersタブのGroup内のPlayerとMemberの内容を更新する
        /// </summary>
        private void UpdateGroupPlayersAndMembers()
        {
            PlayerList_Group.Clear();
            List<Player> tmpMember = MemberList.ToList();

            // 登録済みのプレイヤー名をループ
            foreach (var player in PlayerList)
            {
                // Membersに登録されていないものをPlayersに登録
                if (!MemberList.Contains(player))
                {
                    PlayerList_Group.Add(player);
                }
                else
                {
                    // これをやっていきtmpMemberに残ったものが、「Membersの中で登録済みのプレイヤーでないもの」となる
                    tmpMember.Remove(player);
                }
            }

            // Membersの中で登録済みのプレイヤーでないものを削除
            foreach (var removedPlayer in tmpMember)
            {
                MemberList.Remove(removedPlayer);
            }
        }
    }

    public class MemberListConverter : IValueConverter
    {
        // TODO: Listを表示するためのConverterを作成する
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ObservableCollection<Player> memberList))
            {
                // 流れてきた値がObservableCollection<Player>ではない場合の処理
            }

            // Kusa, Gomi, CivilTTみたいな感じ
            //return 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class AccountInfo : IEquatable<AccountInfo>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Repository { get; private set; }
        public string Branch { get; private set; }
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
    }

    class PlayerGroup : IEquatable<PlayerGroup>
    {
        public string GroupName { get; private set; }
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
    }

    class Player : IEquatable<Player>
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
    }
}
