using Microsoft.WindowsAPICodePack.Dialogs;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using Server_GUI2.Windows.SystemSettings;
using System;
using System.Collections.ObjectModel;

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
            _vm.BoolOptions.ChangeCollection(BoolOption.GetBoolCollection(_vm.PropertyIndexs.Value.BoolOption, new string[2] { "hardcore", "white-list" }));
            _vm.TextOptions.ChangeCollection(TextOption.GetTextCollection(_vm.PropertyIndexs.Value.StringOption, new string[4] { "difficulty", "gamemode", "level-type", "level-name" }));
            CustomMessageBox.Show(Properties.Resources.WorldSettings_SetProp, ButtonType.OK, Image.Infomation);
        }
    }

    class SaveDefaultProperties : GeneralCommand<WorldSettingsVM>
    {
        public SaveDefaultProperties(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.PropertyIndexs.Value = BoolOption.SetBoolOption(_vm.BoolOptions, _vm.PropertyIndexs.Value);
            _vm.PropertyIndexs.Value = TextOption.SetStringOption(_vm.TextOptions, _vm.PropertyIndexs.Value);
            UserSettings.Instance.userSettings.DefaultProperties = new ServerProperty(_vm.PropertyIndexs.Value);
            CustomMessageBox.Show(Properties.Resources.WorldSettings_SaveProp, ButtonType.OK, Image.Infomation);
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
                    string path = ShowDialog(isZip, new CommonFileDialogFilter(Properties.Resources.ZipFile, "*.zip"));

                    if (path == null)
                        break;

                    // datapackとして有効かを確認
                    Datapack datapack = Datapack.TryGenInstance(path, isZip);
                    if (datapack == null)
                    {
                        CustomMessageBox.Show(Properties.Resources.WorldSettings_Datapack, ButtonType.OK, Image.Error);
                        break;
                    }

                    // FileNameで選択されたフォルダを取得する
                    _vm.Datapacks.Add(datapack);
                    break;
                case "Plugin":
                    path = ShowDialog(true, new CommonFileDialogFilter(Properties.Resources.Plugin, "*.jar"));

                    if (path == null)
                        break;

                    Plugin plugin = Plugin.TryGenInstance(path, false);
                    if (plugin == null)
                    {
                        CustomMessageBox.Show(Properties.Resources.WorldSettings_Plugin, ButtonType.OK, Image.Error);
                        break;
                    }

                    _vm.Plugins.Add(plugin);
                    break;
                case "CustomMap":
                    isZip = _vm.IsZipMap;
                    path = ShowDialog(isZip, new CommonFileDialogFilter(Properties.Resources.ZipFile, "*.zip"));

                    if (path == null)
                        break;

                    CustomMap custom = CustomMap.TryGetInstance(path, isZip);
                    if (custom == null)
                    {
                        CustomMessageBox.Show(Properties.Resources.WorldSettings_Map, ButtonType.OK, Image.Error);
                        break;
                    }

                    _vm.CustomMap.Value = custom;
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
            string selectType = isFile ? Properties.Resources.File : Properties.Resources.Folder;
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = Properties.Resources.WorldSettings_Select1 + selectType + Properties.Resources.WorldSettings_Select2,
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
            void DeleteContent(Action DeleteAction, string name)
            {
                if (name == null)
                {
                    CustomMessageBox.Show(Properties.Resources.WorldSettings_Remove, ButtonType.OK, Image.Warning);
                    return;
                }

                int result = CustomMessageBox.Show($"{Properties.Resources.WorldSettings_Delete1}{name}{Properties.Resources.WorldSettings_Delete2}", ButtonType.YesNo, Image.Question);
                if (result == 0)
                    DeleteAction();
            }

            switch (parameter)
            {
                case "Datapack":
                    ADatapack datapack = _vm.SelectedDatapack.Value;
                    string name = datapack?.Name ?? null;
                    DeleteContent(() => _vm.Datapacks.Remove(datapack), name);                    
                    break;
                case "Plugin":
                    APlugin plugin = _vm.SelectedPlugin.Value;
                    name = plugin?.Name ?? null;
                    DeleteContent(() => _vm.Plugins.Remove(plugin), name);
                    break;
                case "CustomMap":
                    _vm.CustomMap.Value = null;
                    break;
                case "Op":
                    OpsRecord ops = _vm.OpPlayersListIndex;
                    name = ops?.Name ?? null;
                    DeleteContent(() => _vm.OpPlayersList.Remove(ops), name);
                    break;
                case "WhiteList":
                    Player player = _vm.WhitePlayersListIndex;
                    name = player?.Name ?? null;
                    DeleteContent(() => _vm.WhitePlayersList.Remove(player), name);
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
                CustomMessageBox.Show(Properties.Resources.WorldSettings_Register, ButtonType.OK, Image.Warning);
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
                CustomMessageBox.Show(Properties.Resources.WorldSettings_Register, ButtonType.OK, Image.Warning);
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
