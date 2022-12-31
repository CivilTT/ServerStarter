using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class OpenCommonDialogTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                IsFolderPicker = true
            })
            {
                //cofd.IsFolderPicker = false;
                //cofd.Filters.Add(new CommonFileDialogFilter("プラグインファイル", "*.jar"));

                if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
                    Console.WriteLine(cofd.FileName);
            }
        }
    }
}
