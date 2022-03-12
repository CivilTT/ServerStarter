using Microsoft.WindowsAPICodePack.Dialogs;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Windows.WorldSettings
{
    class SetDefaultProperties : GeneralCommand<WorldSettingsVM>
    {
        public SetDefaultProperties(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.PropertyIndexs.Value = new ServerProperty(UserSettings.Instance.userSettings.DefaultProperties);
            MW.MessageBox.Show("既定のサーバープロパティを適用しました。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    class SetAsDefaultProperties : GeneralCommand<WorldSettingsVM>
    {
        public SetAsDefaultProperties(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            UserSettings.Instance.userSettings.DefaultProperties = new ServerProperty(_vm.PropertyIndexs.Value);
            MW.MessageBox.Show("既定のサーバープロパティとして保存されました。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }


    class ImportAdditionalsCommand : GeneralCommand<WorldSettingsVM>
    {
        public ImportAdditionalsCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "Datapack":
                    bool isZip = _vm.IsZipDatapack.Value;
                    string path = ShowDialog(isZip, new CommonFileDialogFilter("圧縮ファイル", "*.zip"));

                    if (path == null)
                        return;

                    // datapackとして有効かを確認
                    Datapack datapack = Datapack.TryGenInstance(path, isZip);
                    if (datapack == null)
                    {
                        MW.MessageBox.Show($"この{(isZip ? "ファイル" : "フォルダ")}はデータパックとして無効です。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // FileNameで選択されたフォルダを取得する
                    _vm.Datapacks.Add(datapack);
                    break;
                case "Plugin":
                    path = ShowDialog(true, new CommonFileDialogFilter("プラグインファイル", "*.jar"));

                    if (path == null)
                        return;

                    Plugin plugin = Plugin.TryGenInstance(path, false);
                    if (plugin == null)
                    {
                        MW.MessageBox.Show("Pluginとして無効なファイルです。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _vm.Plugins.Add(plugin);
                    break;
                case "CustomMap":
                    isZip = _vm.IsZipMap;
                    path = ShowDialog(isZip, new CommonFileDialogFilter("圧縮ファイル", "*.zip"));

                    if (path == null)
                        return;

                    CustomMap custom = CustomMap.TryGetInstance(path, isZip);
                    if (custom == null)
                    {
                        MW.MessageBox.Show($"この{(isZip ? "ファイル" : "フォルダ")}は配布ワールドとして無効です。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _vm.RunWorld.CustomMap = custom;
                    break;
                default:
                    throw new ArgumentException("Unknown Import Parameter");
            }
        }

        /// <summary>
        /// ファイル選択のダイアログを表示
        /// ファイルが選択された場合、そのパスを返し、選択されなかった場合はnullを返す
        /// TODO: GUIでisZipを変更しても、それが反映されない
        /// </summary>
        private string ShowDialog(bool isFile, CommonFileDialogFilter filter=null)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                RestoreDirectory = true,
                IsFolderPicker = !isFile
            })
            {
                if (isFile && filter != null)
                    cofd.Filters.Add(filter);

                if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
                    return cofd.FileName;

                return null;
            }
        }
    }

    class DeleteAdditionalsCommand : GeneralCommand<WorldSettingsVM>
    {
        public DeleteAdditionalsCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "Datapack":
                    _vm.Datapacks.Remove(_vm.SelectedDatapack.Value);
                    break;
                case "Plugin":
                    _vm.Plugins.Remove(_vm.SelectedPlugin.Value);
                    break;
                case "CustomMap":
                    _vm.CustomMap = null;
                    break;
                default:
                    break;
            }
        }
    }

    class AddOpPlayerCommand : GeneralCommand<WorldSettingsVM>
    {
        public AddOpPlayerCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            int opLevel = _vm.OpLevelIndex;
            bool addedP = AddPlayer(opLevel);
            bool addedG = AddGroup(opLevel);
            if (!(addedP || addedG))
                MW.MessageBox.Show("選択されたプレイヤーとグループはすでに登録されています。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// PlayerをListに追加した場合にtrueを返し、追加しなかった場合にfalseを返す
        /// </summary>
        private bool AddPlayer(int opLevel)
        {
            // PlayerがNULLになることはない（Addボタンを押せないはずだから）が、一応実装している
            if (_vm.OpPlayerIndex == null)
                return false;

            OpPlayer opPlayer = new OpPlayer(_vm.OpPlayerIndex, opLevel);
            if (!_vm.OpPlayersList.Contains(opPlayer))
            {
                _vm.OpPlayersList.Add(opPlayer);
                return true;
            }
            return false;
        }

        private bool AddGroup(int opLevel)
        {
            if (_vm.OpGroupIndex == null || _vm.OpGroupIndex.GroupName == "(No Group)")
                return false;

            bool added = false;
            ObservableCollection<Player> players = _vm.OpGroupIndex.PlayerList;
            foreach (Player player in players)
            {
                OpPlayer opPlayer = new OpPlayer(player, opLevel);
                if (!_vm.OpPlayersList.Contains(opPlayer))
                {
                    _vm.OpPlayersList.Add(opPlayer);
                    added = true;
                }
            }

            return added;
        }
    }

    class DeleteOpPlayerCommand : GeneralCommand<WorldSettingsVM>
    {
        public DeleteOpPlayerCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.OpPlayersList.Remove(_vm.OpPlayersListIndex);
        }
    }

    class AddWhiteCommand : GeneralCommand<WorldSettingsVM>
    {
        public AddWhiteCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            bool addedP = AddPlayer();
            bool addedG = AddGroup();
            if (!(addedP || addedG))
                MW.MessageBox.Show("選択されたプレイヤーとグループはすでに登録されています。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool AddPlayer()
        {
            // PlayerがNULLになることはない（Addボタンを押せないはずだから）が、一応実装している
            if (_vm.WhitePlayerIndex == null)
                return false;

            Player player = _vm.WhitePlayerIndex;
            if (!_vm.WhitePlayersList.Contains(player))
            {
                _vm.WhitePlayersList.Add(player);
                return true;
            }
            return false;
        }

        private bool AddGroup()
        {
            if (_vm.WhiteGroupIndex == null || _vm.WhiteGroupIndex.GroupName == "(No Group)")
                return false;

            bool added = false;
            ObservableCollection<Player> players = _vm.WhiteGroupIndex.PlayerList;
            foreach (Player player in players)
            {
                if (!_vm.WhitePlayersList.Contains(player))
                {
                    _vm.WhitePlayersList.Add(player);
                    added = true;
                }
            }

            return added;
        }
    }

    class DeleteWhiteCommand : GeneralCommand<WorldSettingsVM>
    {
        public DeleteWhiteCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {

        }
    }

    class SaveCommand : GeneralCommand<WorldSettingsVM>
    {
        public SaveCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            // TODO: 必要に応じてSave処理を記述する
            //_vm.RunWorld.Property = new ServerProperty(_vm.PropertyIndexs.Value);


            _vm.Close();
        }
    }
}
