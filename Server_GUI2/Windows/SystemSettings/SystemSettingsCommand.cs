using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
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
                    CustomMessageBox.Show(nullMessage, ButtonType.OK, Image.Warning);
                    return;
                }

                // Containsを作動させるためには該当のクラス（型）でIEquatable<T>を実装している必要性あり
                if (list.Contains(content))
                {
                    CustomMessageBox.Show(alreadyContainMessage, ButtonType.OK, Image.Warning);
                    return;
                }

                list.Add(content);
                list.Sort();
            }

            switch (parameter)
            {
                case "Remote":
                    void Adding(object sender, DoWorkEventArgs e)
                    {
                        Either<GitStorage, Exception> result = GitStorage.AddStorage(_vm.AccountName.Value, _vm.RepoName.Value, _vm.AccountEmail.Value);
                        result.SuccessAction(storage => _vm.RemoteList.AddRange(storage.RemoteWorlds.OfType<IRemoteWorld>()));
                    }
                    void Finished(object sender, RunWorkerCompletedEventArgs e)
                    {
                        _vm.RemoteAdding.Value = false;
                    }
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += Adding;
                    worker.RunWorkerCompleted += Finished;
                    worker.RunWorkerAsync();

                    _vm.RemoteAdding.Value = true;
                    break;

                case "Player":
                    var playerList = _vm.PlayerList;
                    var playerContent = new Player(_vm.PlayerName.Value);
                    if (playerContent.UUID == null)
                    {
                        CustomMessageBox.Show("このプレイヤー名は存在しません。", ButtonType.OK, Image.Error);
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
                    var memberList2 = new ObservableCollection<Player>(_vm.MemberList);
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
                //case "Remote":
                //    var remoteList = _vm.RemoteList;
                //    var remoteDeleteItem = _vm.RLIndex.Value;
                //    _vm.AccountName.Value = remoteDeleteItem.AccountName;
                //    _vm.AccountEmail.Value = remoteDeleteItem.Email;
                //    _vm.RepoName.Value = remoteDeleteItem.RepositoryName;
                //    remoteList.Remove(remoteDeleteItem);
                //    break;
                case "Group":
                    var groupList = _vm.GroupList;
                    var groupIndex = _vm.GLIndex;
                    if (groupIndex == null)
                    {
                        CustomMessageBox.Show("編集したい行を選択してください。", ButtonType.OK, Image.Warning);
                        return;
                    }
                    _vm.MemberList.ChangeCollection(groupIndex.PlayerList);
                    _vm.GroupName.Value = groupIndex.GroupName;
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
            string DeleteContent<T>(ObservableCollection<T> list, T deleteItem, string name, string notSelected= null, string nullMessage= "削除したい行を選択してください。")
            {
                if (name == notSelected)
                {
                    CustomMessageBox.Show(nullMessage, ButtonType.OK, Image.Warning);
                    return string.Empty;
                }

                string result = CustomMessageBox.Show($"{name}を削除しますか？", ButtonType.YesNo, Image.Question);
                if (result != "Yes")
                    return result;

                list.Remove(deleteItem);
                return "Yes";
            }

            switch (parameter)
            {
                case "Remote":
                    var remoteItem = _vm.RLIndex.Value;
                    var storageAccount = remoteItem?.Storage.AccountName ?? null;
                    var storageRepo = remoteItem?.Storage.RepositoryName ?? null;
                    var worldName = $"/{remoteItem?.Name ?? null}";
                    string result = DeleteContent(_vm.RemoteList, remoteItem, $"{storageAccount}/{storageRepo}{worldName}", "//", "削除したいリモートを選択してください。");
                    if (result == "Yes")
                    {
                        if (remoteItem is RemoteWorld world)
                            world.Delete();
                        else if (remoteItem is NewRemoteWorld world1)
                            world1.Storage.Delete();
                    }
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
                        CustomMessageBox.Show("削除したいメンバーを選択してください。", ButtonType.OK, Image.Warning);
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
            int portNum = int.Parse(_vm.PortNumber);
            _vm.PortStatus.Value = new PortStatus(portNum, PortStatus.Status.Registering);

            bool isSuccess = await PortMapping.AddPort(portNum);

            if (isSuccess)
                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Open;
            else
                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Failed;
        }

        public async Task DeletePort()
        {
            PortStatus status = _vm.PortStatus.Value;

            // そもそもポート開放していない場合は何もしない
            if (status == null || (status.StatusEnum.Value != PortStatus.Status.Open && status.StatusEnum.Value != PortStatus.Status.Registering))
                return;

            bool isSuccess = await PortMapping.DeletePort(status.PortNumber);

            if (isSuccess)
                _vm.PortStatus.Value.StatusEnum.Value = PortStatus.Status.Close;
            else
            {
                string message =
                    "ポートの閉鎖に失敗しました。\n" +
                    "ポートを開放したまま留置します。";
                CustomMessageBox.Show(message, ButtonType.OK, Image.Error);

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
            switch (parameter)
            {
                case "global":
                    Clipboard.SetText(_vm.GlobalIP);
                    break;
                case "local":
                    Clipboard.SetText(_vm.LocalIP);
                    break;
                default:
                    break;
            }
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
            _vm.SaveSystemSettings();
            _vm.Saved = true;

            _vm.Close();
        }
    }
}
