using log4net;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

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
            void AddContent<T>(ObservableCollection<T> list, T content, string alreadyContainMessage)
            {
                if (content == null)
                {
                    CustomMessageBox.Show(Properties.Resources.SystemSettings_Remove, ButtonType.OK, Image.Warning);
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
                        result
                            .SuccessAction(storage => _vm.RemoteList.AddRange(storage.RemoteWorlds.OfType<IRemoteWorld>()))
                            .FailureAction(exception => Application.Current.Dispatcher.Invoke(() => CustomMessageBox.Show(_vm.RepoName.Value + Properties.Resources.SystemSettings_RemoteFail, ButtonType.OK, Image.Warning)));
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
                        CustomMessageBox.Show(Properties.Resources.SystemSettings_Exist, ButtonType.OK, Image.Error);
                        return;
                    }
                    AddContent(playerList, playerContent, Properties.Resources.SystemSettings_RegisterP);
                    break;

                case "GroupPlayer":
                    var playerListGroup = _vm.PlayerList_Group;
                    var memberList = _vm.MemberList;
                    var selectedPlayer = _vm.PLGIndex?.Value ?? null;
                    AddContent(memberList, selectedPlayer, Properties.Resources.SystemSettings_RegisterP);
                    playerListGroup.Remove(selectedPlayer);
                    break;

                case "Group":
                    var memberList2 = new ObservableCollection<Player>(_vm.MemberList);
                    var groupList = _vm.GroupList;
                    string groupName = _vm.GroupName.Value;
                    var groupContent = new PlayerGroup(groupName, memberList2);
                    AddContent(groupList, groupContent, Properties.Resources.SystemSettings_RegisterG);
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
                        CustomMessageBox.Show(Properties.Resources.SystemSettings_Edit, ButtonType.OK, Image.Warning);
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
        protected static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public DeleteListCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            logger.Info($"Execute {parameter}");
            int DeleteContent<T>(ObservableCollection<T> list, T deleteItem, string name, string notSelected= null)
            {
                if (name == notSelected)
                {
                    CustomMessageBox.Show(Properties.Resources.WorldSettings_Remove, ButtonType.OK, Image.Warning);
                    return -2;
                }

                int result = CustomMessageBox.Show($"{Properties.Resources.WorldSettings_Delete1}{name}{Properties.Resources.WorldSettings_Delete2}", ButtonType.YesNo, Image.Question);
                if (result != 0)
                    return result;

                list.Remove(deleteItem);
                return 0;
            }

            switch (parameter)
            {
                case "Remote":
                    var remoteItem = _vm.RLIndex.Value;
                    var storageAccount = remoteItem?.Storage.AccountName ?? null;
                    var storageRepo = remoteItem?.Storage.RepositoryName ?? null;
                    var worldName = $"/{remoteItem?.Name ?? null}";
                    int result = DeleteContent(_vm.RemoteList, remoteItem, $"{storageAccount}/{storageRepo}{worldName}", "//");
                    logger.Info($"Remote {result}");
                    if (result == 0)
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
                        CustomMessageBox.Show(Properties.Resources.SystemSettings_Remove, ButtonType.OK, Image.Warning);
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
                _vm.PortStatus.Value = new PortStatus(portNum, PortStatus.Status.Open);
            else
                _vm.PortStatus.Value = new PortStatus(portNum, PortStatus.Status.Failed);
        }

        public async Task DeletePort()
        {
            PortStatus status = _vm.PortStatus.Value;

            // そもそもポート開放していない場合は何もしない
            if (status == null || (status.StatusEnum != PortStatus.Status.Open && status.StatusEnum != PortStatus.Status.Registering))
                return;

            bool isSuccess = await PortMapping.DeletePort(status.PortNumber);

            if (isSuccess)
                _vm.PortStatus.Value = new PortStatus(status.PortNumber, PortStatus.Status.Close);
            else
            {
                CustomMessageBox.Show(Properties.Resources.SystemSettings_Port, ButtonType.OK, Image.Error);

                _vm.PortStatus.Value = new PortStatus(status.PortNumber, PortStatus.Status.Open);
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

    class HelpCommand : GeneralCommand<SystemSettingsVM>
    {
        public HelpCommand(SystemSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            string url = $"https://civiltt.github.io/ServerStarter/{parameter}";
            Process.Start(url);
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
