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
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using Server_GUI2.Develop.Server;

namespace Server_GUI2
{
    public class Version : IComparable<Version>, INotifyPropertyChanged
    {
        protected ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static WebClient wc = new WebClient();

        protected VersionFactory VerFactory = VersionFactory.Instance;

        public bool Available;

        public virtual ServerType Type { get; }

        public string Name;

        protected virtual string JarName { get; }
        public virtual string Log4jArgument { get { return ""; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public VersionPath Path { get; }

        private bool _Exists;
        public bool Exists 
        {
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

        protected Version(string name,VersionPath path, bool available)
        {
            Path = path;
            Name = name;
            Exists = Path.Exists;
            Available = available;
        }

        /// <summary>
        /// プロパティの変更をVersionFactoryのObserbableCollectionに通知するためのイベント発火メソッド
        /// </summary>
        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// サーバー起動前に実行
        /// 必要があれば新しいバージョンの導入を行い、versionのパスと.jarの名称を返す
        /// </summary>
        public (VersionPath, string) ReadyVersion()
        {
            if (!Exists)
                SetNewVersion();
            return (Path,JarName);
        }

        /// <summary>
        /// 新しいバージョンの導入を行う
        /// </summary>
        protected virtual void SetNewVersion()
        {
            Exists = true;

            logger.Info("There are already new version, or not");
            if (Exists)
            {
                logger.Info("There are already new version");
                return;
            }

            logger.Info("Download server.jar");
            if (Name == "")
            {
                MessageBoxResult? result = MW.MessageBox.Show("導入するサーバーのバージョンが選択されていません。\r\nサーバーのバージョンを選択をした上で再度「Run」を押してください。", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    // システムを再起動する
                    // TODO: これは単に再表示するだけのほうが良いのでは？
                    System.Windows.Forms.Application.Restart();
                }

                ArgumentException arg = new ArgumentException("Did not select opening version");
                throw new ServerStarterException<ArgumentException>(arg);
            }
        }

        public void Remove()
        {
            Exists = false;

            MessageBoxResult? result = MW.MessageBox.Show($"このバージョンを削除しますか？\r\n「{Data_list.ReadVersion}」とその内部に保管されたワールドデータは完全に削除され、復元ができなくなります。", "Server Starter", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            logger.Warn("Warning the delete Version data");
            if (result == MessageBoxResult.OK)
            {
                Path.Delete();
            }
        }

        // 比較可能にする
        public int CompareTo(Version ver)
        {
            return VersionFactory.GetIndex(ver.Name) - VersionFactory.GetIndex(this.Name);
        }    
        
        // Define the is greater than operator.
        public static bool operator >(Version operand1, Version operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(Version operand1, Version operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(Version operand1, Version operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(Version operand1, Version operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
    }

    public class VanillaVersion: Version
    {
        protected override string JarName { get { return "server.jar"; } }
        public override ServerType Type => ServerType.Vanilla;

        public override string Log4jArgument
        {
            get
            {
                // バージョンごとに引数を変更する
                // TODO: バージョンによってはファイルを新しく用意する必要あり
                int versionIndex = VersionFactory.GetIndex(Name);

                int index18 = VersionFactory.GetIndex("1.18.1");
                int index16 = VersionFactory.GetIndex("1.16.5");
                int index11 = VersionFactory.GetIndex("1.11.2");
                int index6 = VersionFactory.GetIndex("1.6.4");

                if (index16 < versionIndex && versionIndex < index18)
                {
                    return " -Dlog4j2.formatMsgNoLookups=true";
                }
                else if (index11 < versionIndex && versionIndex <= index18)
                {
                    return " -Dlog4j.configurationFile=log4j2_112-116.xml";
                }
                else if (index6 < versionIndex && versionIndex <= index11)
                {
                    return " -Dlog4j.configurationFile=log4j2_17-111.xml";
                }
                else
                {
                    return "";
                }
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

        public VanillaVersion(string name, string downloadURL, bool isRelease, bool hasSpigot ,bool isLatest = false, bool available = true):
            base(name, ServerGuiPath.Instance.WorldData.GetVersionDirectory(name),available)
        {
            IsRelease = isRelease;
            HasSpigot = hasSpigot;
            DownloadURL = downloadURL;
            IsLatest = isLatest;
        }


        protected override void SetNewVersion()
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
                        $"{Name}はマルチサーバーが存在しない可能性があります。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new DownloadException($"Failed to get url to download server.jar (Error Message : {ex.Message})");
            }

            Path.Create();

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
            Server.Start(Path.FullName, JarName, Log4jArgument,new ServerProperty());
        }
    }

    public class SpigotVersion: Version
    {
        protected override string JarName { get { return $"spigot-{Name}.jar"; } }
        public override ServerType Type => ServerType.Spigot;

        public SpigotVersion(string name, bool available) : base(name, ServerGuiPath.Instance.WorldData.GetVersionDirectory(name), available)
        {
            // Initialize
        }

        protected override void SetNewVersion()
        {
            base.SetNewVersion();

            logger.Info("Import Spigot Server");
            Path.Create();

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

            CreateBuildBat();
            Process p = Process.Start($@"{Path}\build.bat");
            p.WaitForExit();

            // 余計なファイルの削除
            Path.Delete();
            MoveLogFile();

            if (p.ExitCode != 0)
            {
                Path.Delete();

                string message;
                switch (p.ExitCode)
                {
                    case 1:
                        message = $"バージョン{Name}はインポート可能なSpigotサーバーとして見つけられませんでした。";
                        break;
                    default:
                        message = 
                            $"Spigotサーバーのビルドに失敗しました\n" +
                            $"（エラーコード：{p.ExitCode}）";
                        break;
                }

                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new ServerException($"Failed to build the spigot server (Error Code : {p.ExitCode})");
            }

            //一度実行し、eula.txtなどの必要ファイルを書き出す
            Server.Start(Path.FullName, JarName, Log4jArgument,new ServerProperty());
        }

        private void CreateBuildBat()
        {
            logger.Info("Generate build.bat");
            try
            {
                using (var writer = new StreamWriter($@"{Path}\build.bat", false))
                {
                    writer.WriteLine("@echo off");
                    writer.WriteLine("cd %~dp0");
                    writer.WriteLine($"java -jar BuildTools.jar --rev {Name}");
                    writer.WriteLine("del /f \"%~dp0%~nx0\"");
                }
            }
            catch (Exception ex)
            {
                string message =
                        "Spigotサーバーをビルドするための必要ファイルの作成に失敗しました。\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new IOException($"Failed to write build.bat (Error Message : {ex.Message})");
            }
        }

        /// <summary>
        /// pathで指定したフォルダ内のディレクトリを全て削除する
        /// </summary>
        private void DeleteInnerDirectory(string path, DeleteDirectoryOption deleteDirectoryOption)
        {
            foreach (string dirPath in Directory.GetDirectories(path))
            {
                FileSystem.DeleteDirectory(dirPath, deleteDirectoryOption);
                logger.Info($"Delete Directory at {dirPath}");
            }
        }

        /// <summary>
        /// buildで出力されるlogファイルをlogフォルダに移動する
        /// </summary>
        private void MoveLogFile()
        {
            Directory.CreateDirectory("log");
            if (File.Exists(@".\log\BuildTools.log.txt"))
            {
                // ファイルがすでに存在すると移動できないため、削除したうえで、再度移動させる
                File.Delete(@".\log\BuildTools.log.txt");
            }

            File.Move($@"{Path}\BuildTools.log.txt", @".\log\BuildTools.log.txt");

        }
    }
}
