using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            void AddContent<T>(ObservableCollection<T> list, T content, string errorMessage)
            {
                if (list.Contains(content))
                {
                    MW.MessageBox.Show(errorMessage, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                list.Add(content);
            }

            switch (parameter)
            {
                case "Git":
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
            void DeleteContent<T>(ObservableCollection<T> list, T deleteItem, string name)
            {
                if (name == null)
                {
                    MW.MessageBox.Show("削除したい行を選択してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult? result = MW.MessageBox.Show($"{name}を削除しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                list.Remove(list[list.IndexOf(deleteItem)]);
            }

            switch (parameter)
            {
                case "Remote":
                    var remoteList = _vm.RemoteList;
                    var remoteDeleteItem = _vm.RLIndex.Value ?? null;
                    var remoteName = _vm.RLIndex.Value?.Name ?? null;
                    DeleteContent(remoteList, remoteDeleteItem, remoteName);
                    break;
                case "Player":
                    var PlayerList = _vm.PlayerList;
                    var PlayerDeleteItem = _vm.PLIndex.Value ?? null;
                    var PlayerName = _vm.PLIndex.Value?.Name ?? null;
                    DeleteContent(PlayerList, PlayerDeleteItem, PlayerName);
                    break;
                default:
                    throw new ArgumentException("This Parameter is not registed (self Exception)");
            }
        }
    }
}
