using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MW = ModernWpf;

namespace Server_GUI2.Windows.SystemSettings
{
    class AddListCommand : ICommand
    {
        private readonly SystemSettingsVM _vm;

        public event EventHandler CanExecuteChanged;

        public AddListCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            void AddContent<T>(ObservableCollection<T> list, T content, string alreadyContainMessage, string nullMessage="")
            {
                if (content == null)
                {
                    MW.MessageBox.Show(nullMessage, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Containsを作動させるためには該当のクラス（型）でIEquatable<>を実装している必要性あり
                if (list.Contains(content))
                {
                    MW.MessageBox.Show(alreadyContainMessage, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                list.Add(content);
            }

            switch (parameter)
            {
                case "Remote":
                    var gitList = _vm.RemoteList;
                    var gitContent= new AccountInfo(
                        _vm.AccountName.Value,
                        _vm.AccountEmail.Value,
                        _vm.RepoName.Value,
                        "");
                    AddContent(gitList, gitContent, "このレポジトリはすでに登録されています。");
                    break;

                case "Player":
                    var playerList = _vm.PlayerList;
                    var playerContent = new Player(_vm.PlayerName.Value);
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
                    AddContent(memberList, selectedPlayer, "このプレイヤーはすでにグループメンバーに登録されています。", "追加したいプレイヤーを選択してください。");
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

    class DeleteListCommand : ICommand
    {
        private readonly SystemSettingsVM _vm;

        public event EventHandler CanExecuteChanged;

        public DeleteListCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            void DeleteContent<T>(ObservableCollection<T> list, T deleteItem, string name, string nullMessage)
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
                    var remoteList = _vm.RemoteList;
                    var remoteDeleteItem = _vm.RLIndex.Value ?? null;
                    var remoteName = _vm.RLIndex.Value?.Name ?? null;
                    DeleteContent(remoteList, remoteDeleteItem, remoteName, "削除したい行を選択してください。");
                    break;

                case "Player":
                    var playerList = _vm.PlayerList;
                    var playerDeleteItem = _vm.PLIndex.Value ?? null;
                    var playerName = _vm.PLIndex.Value?.Name ?? null;
                    DeleteContent(playerList, playerDeleteItem, playerName, "削除したい行を選択してください。");
                    break;

                case "GroupMember":
                    var playerList_Group = _vm.PlayerList_Group;
                    var memberList = _vm.MemberList;
                    var memberIndex = _vm.MLIndex?.Value ?? null;
                    if (memberIndex != null)
                    {
                        memberList.Remove(memberIndex);
                        playerList_Group.Add(memberIndex);
                    }
                    else
                    {
                        MW.MessageBox.Show("削除したいメンバーを選択してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;

                case "Group":
                    var groupList = _vm.GroupList;
                    var groupIndex = _vm.MLIndex.Value;
                    break;

                default:
                    throw new ArgumentException("This Parameter is not registed (self Exception)");
            }
        }
    }
}
