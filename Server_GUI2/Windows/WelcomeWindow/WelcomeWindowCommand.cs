using Server_GUI2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Windows.WelcomeWindow
{
    class CheckValidNameCommand : GeneralCommand<WelcomeWindowVM>
    {
        private Window Owner;

        public CheckValidNameCommand(WelcomeWindowVM vm, Window owner)
        {
            _vm = vm;
            Owner = owner;
        }

        public override void Execute(object parameter)
        {
            Player player = new Player(_vm.PlayerName.Value);
            player.GetUuid();
            if (player.UUID == "")
            {
                MW.MessageBox.Show(Owner, "このプレイヤー名は存在しません。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                _vm.checkedName = "";
                _vm.UUID.Value = "";
            }
            else
            {
                // checkedNameを先に代入しないと、checkが狙った値を返さない
                _vm.checkedName = player.Name;
                _vm.PlayerName.Value = player.Name;
                _vm.UUID.Value = player.UUID;
            }
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
            string playerName = _vm.PlayerName.Value;
            string playerUUID = _vm.UUID.Value;
            Player player = new Player(playerName, playerUUID);

            UserSettings.Instance.userSettings.PlayerName = playerName;
            UserSettings.Instance.userSettings.Players.Add(player);

            UserSettings.Instance.WriteFile();

            _vm.Close();
        }
    }
}
