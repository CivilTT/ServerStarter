using Microsoft.WindowsAPICodePack.Dialogs;
using Server_GUI2.Develop.Server.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Windows.MoreSettings
{
    class SetDefaultProperties : GeneralCommand<WorldSettingsVM>
    {
        public SetDefaultProperties(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            _vm.PropertyIndexs.Value = UserSettings.Instance.userSettings.DefaultProperties;
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
            UserSettings.Instance.userSettings.DefaultProperties = _vm.PropertyIndexs.Value;
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
                    bool isZip = _vm.IsZipDatapack;
                    string path = ShowDialog(isZip);

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
                    path = ShowDialog(false);

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
                    path = ShowDialog(isZip);

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
        /// </summary>
        private string ShowDialog(bool isZip)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                IsFolderPicker = isZip
            })
            {
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

    class SaveCommand : GeneralCommand<WorldSettingsVM>
    {
        public SaveCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            // TODO: 必要に応じてSave処理を記述する
            _vm.Close();
        }
    }
}
