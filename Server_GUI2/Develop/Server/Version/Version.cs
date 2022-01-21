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

        private bool _Exists;
        public bool Exists {
            get
            {
                return _Exists;
            }
            set
            {
                if (_Exists != value )
                {
                    _Exists = value;
                    NotifyPropertyChanged();
                }
            }
        
        }

        public ServerProperty ServerProperty { get; set; }

        protected Version(string name)
        {
            Name = name;
            Exists = Directory.Exists(Path);
        }

        public virtual void Start() { }

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
            Exists = true;

            logger.Info("There are already new version, or not");
            if (Exists)
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

        public void Remove()
        {
            Exists = false;

            MessageBoxResult? result = MW.MessageBox.Show($"このバージョンを削除しますか？\r\n「{Data_list.ReadVersion}」とその内部に保管されたワールドデータは完全に削除され、復元ができなくなります。", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            logger.Warn("Warning the delete Version data");
            if (result == MessageBoxResult.OK)
            {
                Directory.Delete(Path, true);
            }
        }

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

        // このバージョンが最新版か
        public bool IsLatest;

        // Spigotとしてこのバージョンはありうるのか（ローカルにあるか否かは関係ない）
        public bool HasSpigot;

        // server.jarのダウンロードurl
        private string DownloadURL;

        public VanillaVersion(string name, string downloadURL, bool isRelease, bool hasSpigot ,bool isLatest = false): base(name)
        {
            IsRelease = isRelease;
            HasSpigot = hasSpigot;
            DownloadURL = downloadURL;
            IsLatest = isLatest;
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
            Start();
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

        public override void SetNewVersion()
        {
            base.SetNewVersion();

            logger.Info("Import Spigot Server");
            Directory.CreateDirectory(Path);

            try
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                wc.DownloadFile("https://hub.spigotmc.org/jenkins/job/BuildTools/lastSuccessfulBuild/artifact/target/BuildTools.jar", $@"{Path}\BuildTools.jar");
                wc.Dispose();
            }
            catch (Exception ex)
            {
                string message =
                        "Spigot サーバーのビルドファイルのダウンロードに失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new DownloadException($"Failed to download BuildTools.jar (Error Message : {ex.Message})");
            }

            Create_bat(ver_folder);
            Process p = Process.Start($@"{Path}\build.bat");
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                spi_func.Delete_dir(ver_folder);
                Move_log(ver_folder);
                Directory.Delete($@"{MainWindow.Data_Path}\Spigot_{Data_list.Version}\", true);

                string message;
                switch (p.ExitCode)
                {
                    case 1:
                        message = $"バージョン{Data_list.Version}はインポート可能なSpigotサーバーとして見つけられませんでした。";
                        break;
                    default:
                        message = $"Spigotサーバーのビルドに失敗しました（エラーコード：{p.ExitCode}）";
                        break;
                }

                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new ServerException($"Failed to build the spigot server (Error Code : {p.ExitCode})");
            }

            // 余計なファイルの削除
            spi_func.Delete_dir(ver_folder);
            Move_log(ver_folder);

            //一度実行し、eula.txtなどの必要ファイルを書き出す
            Start();
            MainWindow.Pd.Value = 15;
            MainWindow.Pd.Message = "Output the server.jar, eula.txt and so on";

            //eulaの書き換え
            Change_eula();
        }
    }
}
