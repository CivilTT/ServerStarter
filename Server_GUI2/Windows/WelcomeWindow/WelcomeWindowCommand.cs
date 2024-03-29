﻿using Server_GUI2.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System.ComponentModel;
using System.Windows;

namespace Server_GUI2.Windows.WelcomeWindow
{
    class CheckValidNameCommand : GeneralCommand<WelcomeWindowVM>
    {
        private Window Owner;
        private Player Player;

        public CheckValidNameCommand(WelcomeWindowVM vm, Window owner)
        {
            _vm = vm;
            Owner = owner;
        }

        public override void Execute(object parameter)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Search;
            worker.RunWorkerCompleted += SearchCompleted;
            worker.RunWorkerAsync();

            _vm.IsActive.Value = true;            
        }

        private void Search(object sender, DoWorkEventArgs e)
        {
            Player = new Player(_vm.PlayerName.Value);
        }

        private void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Player?.UUID == "" || Player?.UUID == null)
            {
                CustomMessageBox.Show(Properties.Resources.SystemSettings_Exist, ButtonType.OK, Image.Error);
                //MW.MessageBox.Show(Owner, "このプレイヤー名は存在しません。""このプレイヤー名は存在しません。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                _vm.checkedName = "";
                _vm.UUID.Value = "";
            }
            else
            {
                // checkedNameを先に代入しないと、checkが狙った値を返さない
                _vm.checkedName = Player.Name;
                _vm.PlayerName.Value = Player.Name;
                _vm.UUID.Value = Player.UUID;
            }
            _vm.IsActive.Value = false;
        }
    }

    class StartCommand : GeneralCommand<WelcomeWindowVM>
    {
        public StartCommand(WelcomeWindowVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {

            if (!_vm.NotRegistName.Value)
            {
                string playerName = _vm.PlayerName.Value;
                string playerUUID = _vm.UUID.Value;
                Player player = new Player(playerName, playerUUID);
                UserSettings.Instance.userSettings.OwnerName = playerName;
                UserSettings.Instance.userSettings.Players.Add(player);
            }
            UserSettings.Instance.userSettings.Agreement.SystemTerms = _vm.Agreed.Value;
            UserSettings.Instance.userSettings.Language = _vm.Languages[_vm.LanguageSelected.Value];

            UserSettings.Instance.WriteFile();

            _vm.OwnerWindow.DialogResult = true;
            _vm.Close();
        }
    }
}
