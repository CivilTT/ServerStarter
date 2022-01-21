using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using log4net;
using System.Windows;
using MW = ModernWpf;
using System.Net;

namespace Server_GUI2
{
    public class Version:IComparable<Version>, INotifyPropertyChanged
    {
        public ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static WebClient wc = new WebClient();

        public string Name;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string Path { get; }

        // TODO: Existsは毎回取得ではなくフィールドとして持ちたい。Existsに変更があった際に NotifyPropertyChanged() を実行する。
        public bool Exists
        {
            get
            {
                return Directory.Exists(Path);
            }
        }


        //public string downloadURL;

        // このバージョンはVanilaか
        // public bool isVanila = true;


        // 最新バージョンか否か
        // public bool isLatest;


        public ServerProperty ServerProperty { get; set; }

        protected Version(string name)
        {
            Name = name;
        }

        // プロパティの変更をVersionFactoryのObserbableCollectionに通知するためのイベント発火メソッド
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void SetNewVersion()
        {
            logger.Info("There are already new version, or not");
            if (Directory.Exists(Path))
            {
                logger.Info("There are already new version");
                return;
            }

            logger.Info("Download server.jar");
            if (Data_list.Version == "")
            {
                MessageBoxResult? result = MW.MessageBox.Show("導入するサーバーのバージョンが選択されていません。\r\nサーバーのバージョンを選択をした上で再度「Run」を押してください。", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    System.Windows.Forms.Application.Restart();
                }
                throw new ArgumentException("Did not select opening version");
            }
        }
        public virtual void Remove() { }

        // 比較可能にする
        public virtual int CompareTo(Version obj)
        {
            // TODO: Versionの比較について実装する
            throw new NotImplementedException();
        }
    }

    public class VanillaVersion: Version
    {
        public override string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\{Name}\";
            }
        }



        // このバージョンがリリース版かスナップショットか
        public bool IsRelease;

        // Spigotとしてこのバージョンはありうるのか（ローカルにあるか否かは関係ない）
        public bool HasSpigot;

        // server.jarのダウンロードurl
        private string DownloadURL;

        public VanillaVersion(string name, string downloadURL, bool isRelease, bool hasSpigot): base(name)
        {
            IsRelease = isRelease;
            HasSpigot = hasSpigot;
            DownloadURL = downloadURL;
        }

        public override void SetNewVersion()
        {
            base.SetNewVersion();

            logger.Info("Import vanila Server");

            string url2;
            try
            {
                //サーバーダウンロードのurlが記されたjsonをダウンロード
                string new_jsonStr = wc.DownloadString(DownloadURL);

                dynamic root2 = Newtonsoft.Json.JsonConvert.DeserializeObject(new_jsonStr);
                url2 = root2.downloads.server.url;
            }
            catch (Exception ex)
            {
                string message =
                        "Vanila サーバーのダウンロードに失敗しました。\n" +
                        $"{Data_list.Version}はマルチサーバーが存在しない可能性があります。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new DownloadException($"Failed to get url to download server.jar (Error Message : {ex.Message})");
            }

            Directory.CreateDirectory(Path);

            try
            {
                wc.DownloadFile(url2, $@"{Path}\server.jar");
                wc.Dispose();
            }
            catch (Exception ex)
            {
                string message =
                        "Vanila サーバーのダウンロードに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new DownloadException($"Failed to download server.jar (Error Message : {ex.Message})");
            }

            //一度実行し、eula.txtなどの必要ファイルを書き出す
            Start_server(true);
            MainWindow.Pd.Value = 15;
            MainWindow.Pd.Message = "Output the server.jar, eula.txt and so on";

            //eulaの書き換え
            Change_eula();

        }
    }

    public class SpigotVersion: Version
    {
        public override string Path
        {
            get
            {
                return $@"{MainWindow.Data_Path}\Spigot_{Name}\";
            }
        }

        // builderのダウンロードurl
        private string DownloadURL;

        public SpigotVersion(string name, string downloadURL) : base(name)
        {
            DownloadURL = downloadURL;
        }
    }
}
