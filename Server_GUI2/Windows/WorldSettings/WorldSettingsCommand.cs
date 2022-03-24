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
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
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
            CustomMessageBox.Show("既定のサーバープロパティを適用しました。", ButtonType.OK, Image.Infomation);
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
            CustomMessageBox.Show("既定のサーバープロパティとして保存されました。", ButtonType.OK, Image.Infomation);
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
            // Import中にWindowの操作をされないように隠す
            _vm.Hide();

            switch (parameter)
            {
                case "Datapack":
                    bool isZip = _vm.IsZipDatapack.Value;
                    string path = ShowDialog(isZip, new CommonFileDialogFilter("圧縮ファイル", "*.zip"));

                    if (path == null)
                        break;

                    // datapackとして有効かを確認
                    Datapack datapack = Datapack.TryGenInstance(path, isZip);
                    if (datapack == null)
                    {
                        CustomMessageBox.Show($"この{(isZip ? "ファイル" : "フォルダ")}はデータパックとして無効です。", ButtonType.OK, Image.Error);
                        break;
                    }

                    // FileNameで選択されたフォルダを取得する
                    _vm.Datapacks.Add(datapack);
                    break;
                case "Plugin":
                    path = ShowDialog(true, new CommonFileDialogFilter("プラグインファイル", "*.jar"));

                    if (path == null)
                        break;

                    Plugin plugin = Plugin.TryGenInstance(path, false);
                    if (plugin == null)
                    {
                        CustomMessageBox.Show("Pluginとして無効なファイルです。", ButtonType.OK, Image.Error);
                        break;
                    }

                    _vm.Plugins.Add(plugin);
                    break;
                case "CustomMap":
                    isZip = _vm.IsZipMap;
                    path = ShowDialog(isZip, new CommonFileDialogFilter("圧縮ファイル", "*.zip"));

                    if (path == null)
                        break;

                    CustomMap custom = CustomMap.TryGetInstance(path, isZip);
                    if (custom == null)
                    {
                        CustomMessageBox.Show($"この{(isZip ? "ファイル" : "フォルダ")}は配布ワールドとして無効です。", ButtonType.OK, Image.Error);
                        break;
                    }

                    _vm.RunWorld.CustomMap = custom;
                    break;
                default:
                    throw new ArgumentException("Unknown Import Parameter");
            }

            _vm.Show();
        }

        /// <summary>
        /// ファイル選択のダイアログを表示
        /// ファイルが選択された場合、そのパスを返し、選択されなかった場合はnullを返す
        /// </summary>
        private string ShowDialog(bool isFile, CommonFileDialogFilter filter=null)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                RestoreDirectory = true,
                IsFolderPicker = !isFile,
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
            void DeleteContent(Action DeleteAction, string name, string nullMessage = "削除したい行を選択してください。")
            {
                if (name == null)
                {
                    CustomMessageBox.Show(nullMessage, ButtonType.OK, Image.Warning);
                    return;
                }

                string result = CustomMessageBox.Show($"{name}を削除しますか？", ButtonType.YesNo, Image.Question);
                if (result == "Yes")
                    DeleteAction();
            }

            switch (parameter)
            {
                case "Datapack":
                    ADatapack datapack = _vm.SelectedDatapack.Value;
                    DeleteContent(() => _vm.Datapacks.Remove(datapack), datapack.Name);                    
                    break;
                case "Plugin":
                    APlugin plugin = _vm.SelectedPlugin.Value;
                    DeleteContent(() => _vm.Plugins.Remove(plugin), plugin.Name);
                    break;
                case "CustomMap":
                    _vm.CustomMap = null;
                    break;
                case "Op":
                    OpsRecord ops = _vm.OpPlayersListIndex;
                    DeleteContent(() => _vm.OpPlayersList.Remove(ops), ops.Name);
                    break;
                case "WhiteList":
                    Player player = _vm.WhitePlayersListIndex;
                    DeleteContent(() => _vm.WhitePlayersList.Remove(player), player.Name);
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
            _vm.OpPlayersList.Sort();
            if (!(addedP || addedG))
                CustomMessageBox.Show("選択されたプレイヤーとグループはすでに登録されています。", ButtonType.OK, Image.Warning);
        }

        /// <summary>
        /// PlayerをListに追加した場合にtrueを返し、追加しなかった場合にfalseを返す
        /// </summary>
        private bool AddPlayer(int opLevel)
        {
            // PlayerがNULLになることはない（Addボタンを押せないはずだから）が、一応実装している
            if (_vm.OpPlayerIndex == null)
                return false;

            OpsRecord opPlayer = new OpsRecord(_vm.OpPlayerIndex, opLevel);
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
                OpsRecord opPlayer = new OpsRecord(player, opLevel);
                if (!_vm.OpPlayersList.Contains(opPlayer))
                {
                    _vm.OpPlayersList.Add(opPlayer);
                    added = true;
                }
            }

            return added;
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
            _vm.WhitePlayersList.Sort();
            if (!(addedP || addedG))
                CustomMessageBox.Show("選択されたプレイヤーとグループはすでに登録されています。", ButtonType.OK, Image.Warning);
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

    class SaveCommand : GeneralCommand<WorldSettingsVM>
    {
        public SaveCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.SaveWorldSettings();
            _vm.Saved = true;

            _vm.Close();
        }
    }
}
