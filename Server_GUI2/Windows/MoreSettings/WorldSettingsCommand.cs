using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Windows.MoreSettings
{

    class ImportCommand : GeneralCommand<WorldSettingsVM>
    {
        public ImportCommand(WorldSettingsVM vm)
        {
            _vm = vm;
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "Datapack":
                    bool isZip = _vm.IsZipDatapack;
                    using (var cofd = new CommonOpenFileDialog()
                    {
                        Title = "フォルダを選択してください",
                        InitialDirectory = $@"{_vm.RunWorld.LocalWorld.Path}\datapacks",
                        // フォルダ選択モードにする
                        IsFolderPicker = isZip,
                    })
                    {
                        if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                            return;

                        // datapackとして有効かを確認
                        Datapack datapack = Datapack.TryGenInstance(cofd.FileName, isZip);
                        if (datapack == null)
                        {
                            MW.MessageBox.Show($"この{(isZip ? "ファイル" : "フォルダ")}はデータパックとして無効です。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // FileNameで選択されたフォルダを取得する
                        string datapack_name = Path.GetFileName(cofd.FileName);
                        Imported.Items.Remove("(None)");
                        Imported.Items.Add("【new】" + datapack_name);
                        // MessageBox.Show($"{cofd.FileName}を選択しました");
                    }
                    break;
                default:
                    throw new ArgumentException("Unknown Import Parameter");
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
            _vm.Close();
        }
    }
}
