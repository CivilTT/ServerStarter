using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Util;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Windows.SystemSettings
{
    class AddListCommand : GeneralCommand<SystemSettingsVM>
    {
        public AddListCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            void AddContent<T>(ObservableCollection<T> list, T content, string alreadyContainMessage, string nullMessage= "追加したいプレイヤーを選択してください。")
            {
                if (content == null)
                {
                    MW.MessageBox.Show(nullMessage, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Containsを作動させるためには該当のクラス（型）でIEquatable<T>を実装している必要性あり
                if (list.Contains(content))
                {
                    MW.MessageBox.Show(alreadyContainMessage, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                list.Add(content);
                list.Sort();
            }

            switch (parameter)
            {
                case "Remote":
                    GitStorage.AddStorage(_vm.AccountName.Value, _vm.RepoName.Value, _vm.AccountEmail.Value);
                    //var gitList = _vm.RemoteList;
                    //var repo = new Repository(_vm.RepoName.Value)
                    //var gitContent= new AccountInfo(
                    //    _vm.AccountName.Value,
                    //    _vm.AccountEmail.Value,
                    //    repo);
                    //// TODO: 追加の前にそのレポジトリが有効か否かを確認する必要あり？
                    //AddContent(gitList, gitContent, "このレポジトリはすでに登録されています。");
                    break;

                case "Player":
                    var playerList = _vm.PlayerList;
                    var playerContent = new Player(_vm.PlayerName.Value);
                    playerContent.GetUuid();
                    if (playerContent.UUID == "")
                    {
                        MW.MessageBox.Show("このプレイヤー名は存在しません。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    AddContent(playerList, playerContent, "このプレイヤーはすでに登録されています。");
                    break;

                case "GroupPlayer":
                    var playerListGroup = _vm.PlayerList_Group;
                    var memberList = _vm.MemberList;
                    var selectedPlayer = _vm.PLGIndex?.Value ?? null;
                    AddContent(memberList, selectedPlayer, "このプレイヤーはすでにグループメンバーに登録されています。");
                    playerListGroup.Remove(selectedPlayer);
                    break;

                case "Group":
                    var memberList2 = _vm.MemberList;
                    var groupList = _vm.GroupList;
                    string groupName = _vm.GroupName.Value;
                    var groupContent = new PlayerGroup(groupName, memberList2);
                    AddContent(groupList, groupContent, "このグループはすでに登録されています。");
                    break;

                default:
                    throw new ArgumentException("This Parameter is not registed (self Exception)");
            }            
        }
    }

    class EditListCommand : GeneralCommand<SystemSettingsVM>
    {
        public EditListCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "Remote":
                    var remoteList = _vm.RemoteList;
                    var remoteDeleteItem = _vm.RLIndex.Value;
                    _vm.AccountName.Value = remoteDeleteItem.AccountName;
                    _vm.AccountEmail.Value = remoteDeleteItem.Email;
                    _vm.RepoName.Value = remoteDeleteItem.RepositoryName;
                    remoteList.Remove(remoteDeleteItem);
                    break;
                case "Group":
                    var groupList = _vm.GroupList;
                    var groupIndex = _vm.GLIndex;
                    if (groupIndex == null)
                    {
                        MW.MessageBox.Show("編集したい行を選択してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    // TODO: 編集ボタンを押してもMembersに追記されない
                    _vm.MemberList.ChangeCollection(groupIndex.PlayerList);
                    _vm.UpdateGroupPlayersAndMembers();
                    groupList.Remove(groupIndex);
                    break;
                default:
                    break;
            }
        }
    }

    class DeleteListCommand : GeneralCommand<SystemSettingsVM>
    {
        public DeleteListCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            void DeleteContent<T>(ObservableCollection<T> list, T deleteItem, string name, string nullMessage= "削除したい行を選択してください。")
            {
                if (name == null)
                {
                    MW.MessageBox.Show(nullMessage, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult? result = MW.MessageBox.Show($"{name}を削除しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                list.Remove(deleteItem);
            }

            switch (parameter)
            {
                case "Remote":
                    //var remoteList = _vm.RemoteList;
                    //var remoteDeleteItem = _vm.RLIndex.Value ?? null;
                    //var remoteName = _vm.RLIndex.Value?. ?? null;
                    MessageBoxResult? result = MW.MessageBox.Show($"{_vm.RLIndex.Value.RepositoryName}を削除しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                        _vm.RLIndex.Value.Delete();
                    //DeleteContent(remoteList, remoteDeleteItem, remoteName);
                    break;

                case "Player":
                    var playerList = _vm.PlayerList;
                    var playerDeleteItem = _vm.PLIndex ?? null;
                    var playerName = _vm.PLIndex?.Name ?? null;
                    DeleteContent(playerList, playerDeleteItem, playerName);
                    break;

                case "GroupMember":
                    var playerList_Group = _vm.PlayerList_Group;
                    var memberList = _vm.MemberList;
                    var memberIndex = _vm.MLIndex?.Value ?? null;
                    if (memberIndex != null)
                    {
                        memberList.Remove(memberIndex);
                        playerList_Group.Add(memberIndex);
                        playerList_Group.Sort();
                    }
                    else
                    {
                        MW.MessageBox.Show("削除したいメンバーを選択してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;

                case "Group":
                    var groupList = _vm.GroupList;
                    var groupIndex = _vm.GLIndex;
                    var groupName = _vm.GLIndex?.GroupName ?? null;
                    DeleteContent(groupList, groupIndex, groupName);
                    break;

                default:
                    throw new ArgumentException("This Parameter is not registed (self Exception)");
            }
        }
    }

    class CredentialManagerCommand : GeneralCommand<SystemSettingsVM>
    {
        public CredentialManagerCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            Process.Start("control", "/name Microsoft.CredentialManager");
        }
    }

    class AddPortCommand : GeneralCommand<SystemSettingsVM>
    {
        public AddPortCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            PortSetting portSetting = new PortSetting(_vm);
            _ = portSetting.AddPort();
        }
    }

    class PortSetting
    {
        readonly SystemSettingsVM _vm;

        public PortSetting(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public async Task AddPort()
        {
            int portNum = int.Parse(_vm.PortNumber.Value);
            _vm.PortStatus.Value = new PortStatus(portNum, PortStatus.Status.Registering);

            bool isSuccess = await PortMapping.AddPort(portNum);

            if (isSuccess)
                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Open;
            else
                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Failed;
        }

        public async Task DeletePort()
        {
            int portNum = int.Parse(_vm.PortNumber.Value);
            PortStatus status = _vm.PortStatus.Value;

            // そもそもポート開放していない場合は何もしない
            if (status == null || (status.StatusEnum.Value != PortStatus.Status.Open && status.StatusEnum.Value != PortStatus.Status.Registering))
                return;

            var result = MW.MessageBox.Show(
                $"{portNum}番のポートを閉鎖します。よろしいですか？\n" +
                $"違う番号のポートを閉鎖する場合は「いいえ」を選択し、Port Numberを変更してください。",
                "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
            {
                _vm.UsingPortMapping.Value = true;
                return;
            }

            bool isSuccess = await PortMapping.DeletePort(portNum);

            if (isSuccess)
                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Close;
            else
            {
                MW.MessageBox.Show(
                        "ポートの閉鎖に失敗しました。\n" +
                        "ルーターなどの設定を変更した場合は、Auto Port Mappingを始めた際の設定に戻し、Auto Port MappingをOffにしてください。",
                        "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);

                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Open;
                _vm.UsingPortMapping.Value = true;
            }
        }
    }

    class ClipbordCommand : GeneralCommand<SystemSettingsVM>
    {
        public ClipbordCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            Clipboard.SetText(_vm.IP);
        }
    }

    class TwitterCommand : GeneralCommand<SystemSettingsVM>
    {
        public TwitterCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            string url = $"https://twitter.com/{parameter}";
            Process.Start(url);
        }
    }

    class GitCommandVM : GeneralCommand<SystemSettingsVM>
    {
        public GitCommandVM(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            string url = $"https://github.com/{parameter}";
            Process.Start(url);
        }
    }

    class SaveCommand : GeneralCommand<SystemSettingsVM>
    {
        public SaveCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            // 既存のデータを変更する形で処理
            UserSettings.Instance.userSettings.PlayerName = _vm.UserName.Value;
            UserSettings.Instance.userSettings.Language = "English;";
            UserSettings.Instance.userSettings.DefaultProperties = _vm.PropertyIndexs.Value;
            UserSettings.Instance.userSettings.Players = _vm.PlayerList.ToList();
            UserSettings.Instance.userSettings.PlayerGroups = _vm.GroupList.ToList();
            UserSettings.Instance.userSettings.PortStatus = _vm.PortStatus.Value.StatusEnum.Value == PortStatus.Status.Open ? _vm.PortStatus.Value : null;

            UserSettings.Instance.WriteFile();
            _vm.Saved = true;

            _vm.Close();
        }
    }
}
