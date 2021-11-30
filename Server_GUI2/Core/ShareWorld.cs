using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2
{
    class ShareWorld
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Git git = new Git();
        private readonly Data_list data = new Data_list();


        public void Check_ShareWorld()
        {
            Download_ShareWorld();

            //異なるバージョンが指定された場合、初めに確認
            Alert_version();


            //ShareWorldの処理に必要なbatが存在するか否かを確認
            Check_file_directory_SW();

            //起動済みサーバーがあるか否かの確認
            //Server_bat-files\info.txtの中身にてserver_openの項目がTrueであれば起動を中止し、FalseならばTrueに書き換えたうえで起動前にpushを行う
            Check_server_open();

            if(Data_list.Import_spigot)
                VtoS();
        }

        public void Upload_ShareWorld()
        {
            MainWindow.Pd = new ProgressDialog
            {
                Title = "push ShareWorld"
            };
            MainWindow.Pd.Show();

            //info.txtのなかのserver_openをFalseに戻す
            MainWindow.Pd.Value = 50;
            Change_info(4, Opening_Server: false);

            //push
            git.Push();
            MainWindow.Pd.Close();
            // MainWindow.pd.Dispose();
        }

        private void Download_ShareWorld()
        {
            if (!Directory.Exists($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\ShareWorld\"))
            {
                git.Clone();
            }
            else
            {
                git.Pull();
            }
            MainWindow.Pd.Value = 50;


            //Server_bat-files内のinfo.txtの中身を読み取る(ShareWorld起動時のみ使用するためここに記載している)
            logger.Info("Read the ShareWorld info");

            try
            {
                if (File.Exists($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\ShareWorld\info.txt"))
                {
                    using (StreamReader sr = new StreamReader($@"{MainWindow.Data_Path}\{Data_list.ReadVersion}\ShareWorld\info.txt", Encoding.GetEncoding("Shift_JIS")))
                    {
                        info2 = data.Set_info(sr);
                    }
                }
                else
                {
                    info2 = Data_list.Info;
                }
            }
            catch (Exception ex)
            {
                string message =
                        "ShareWorld内のinfo.txtの読み込みに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to read info.txt in ShareWorld (Error Message : {ex.Message})");
            }
        }

        private void Alert_version()
        {
            //起動バージョンが前回と違う場合は警告を出す
            if (info2[2] != Data_list.ReadVersion)
            {
                logger.Warn("The Version is Different of latest open by ShareWorld.");
                MessageBoxResult? result = MW.MessageBox.Show($"前回のShareWorldでのサーバー起動バージョンは{info2[2]}です。\r\nバージョン{Data_list.ReadVersion}で起動を続けますか？\r\n（「いいえ(N)」を選択した場合はもう一度起動をやり直してください。）", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    throw new UserSelectException("User chose NO");
                }
            }
        }

        private void Check_file_directory_SW()
        {
            //batファイルの変更を反映できるよう毎度作成する
            logger.Info("Create bat files (pull & push)");
            git.Create_bat_pull();
            git.Create_bat_push();
        }

        private void Check_server_open()
        {
            logger.Info("Check the ShareWorld's info (There are already started Server or not)");
            if (info2[4] == "True")
            {
                if (info2[0] != Data_list.Info[0])
                {
                    MW.MessageBox.Show(
                        $"ShareWorldのサーバーはすでに{info2[0]}によって起動されています。\r\n" +
                        $"{info2[0]}のサーバーが閉じたことを確認したうえでサーバーを再起動してください。", "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);

                    throw new ServerException("There are already opened server so system is over");
                }

                // ShareWorldが前回、異常終了しているため、処理する
                FixShareWorld();
            }

            Change_info(4, Opening_Server: true);

            //変更内容をpush
            git.Push();
        }

        private void FixShareWorld()
        {
            // TODO ShareWorldのフラグを追加
            // 　　　　　Must Clone 　　　　　 : クローンの必要があるかどうかを管理。強制pushを行った際には他の共有者はcloneしなければ競合する
            // Clash is before Starting server : ShareWorldのクラッシュはサーバー起動の前後どちらかであったかを判別。クラッシュしていない場合は0を記録
            // 今後の管理事項追加にも対処できるようにJson形式に改める必要あり？

            var result = MW.MessageBox.Show(
                "前回のShareWorldは異常終了しています。\n" +
                "原因を解決できていない場合はサーバーの起動に再び失敗する可能性があります。\n" +
                "サーバーの起動を取りやめますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Change_info(4, Opening_Server: false);

                git.Push();
            }

            // TODO ShareWorldの状態に応じてデータ状態を復旧する（ifの中身は上下ともに未完成）

            // サーバー起動前にクラッシュした場合
            // クラッシュ後直ちに再起動される（と予想される）ため、
            if (Data_list.Import_spigot)
            {
                StoV();
            }
        }
    }

    class ShareJson
    {
        public void GetSWstatus()
        {

        }

        public void SetSWstatus()
        {

        }
    }
}
