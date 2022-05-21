using log4net;
using Server_GUI2.Develop.Server;
using Server_GUI2.Develop.Util;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using Server_GUI2.Windows.ProgressBar;
using Server_GUI2.Windows.WelcomeWindow;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Server_GUI2
{
    public static class SetUp
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static string DataPath => ServerGuiPath.Instance.WorldData.FullName;

        // Initialize()より前には呼ばない前提
        public static ProgressBar InitProgressBar;


        public static void Initialize()
        {
            var lastVersion = GetLastVersion();

            // 利用規約に同意しているか
            logger.Info("Check to already agree to the system terms");
            if (!UserSettings.Instance.userSettings.Agreement.SystemTerms)
            {
                logger.Info("Started Welcome Window");
                WelcomeWindow window = new WelcomeWindow();
                bool? result = window.ShowDialog();
                if (result != true)
                    Environment.Exit(0);
            }

            CheckConnetNet();

            // ProgressBarを表示する
            InitProgressBar = new ProgressBar("Ready for Server Starter", 9);
            InitProgressBar.AddMessage("Checked first User Settings");

            // 仕様変更が必要な場合に実装
            ChangeSpecification(lastVersion);
            InitProgressBar.AddMessage("Checked ChangeSpecification");

            // SystemVersionの確認＆バージョンアップ
            ManageSystemVersion.CheckVersion();
            InitProgressBar.AddMessage("Checked the versionUP about this system, Server Starter");
        }

        /// <summary>
        /// 前回起動時のバージョン情報を取得
        /// 2.0.0.0以前のバージョンの場合空文字列となる
        /// </summary>
        /// <returns></returns>
        private static string GetLastVersion()
        {
            return ServerGuiPath.Instance.InfoJson.ReadJson().SuccessFunc(
                x => x.StarterVersion
                ).SuccessOrDefault("");
        }

        /// <summary>
        /// ネットワークに接続されているか調べる
        /// これを実装しなくてもネットがないことを想定して処理されてるところ多め
        /// </summary>
        private static void CheckConnetNet()
        {
            if (!NetWork.Accessible)
            {
                CustomMessageBox.Show(Properties.Resources.Setup_Net, ButtonType.OK, Image.Error);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// TODO: システムが使用するディレクトリが変更された場合、ここに書いていく
        /// （将来的に変更が増えてきたら別の持ち方を検討すべき）
        /// </summary>
        private static void ChangeSpecification(string lastVersion)
        {
            // Init
            ServerGuiPath.Instance.WorldData.Create();

            // 0.X -> 1.0.0.0
            FileInfo starterJson = new FileInfo(Path.Combine(DataPath, "Starter_Version.json"));
            starterJson.Delete();

            // 1.X -> 2.0.0.0
            FileInfo infoTxt = new FileInfo(Path.Combine(DataPath, "info.txt"));
            infoTxt.Delete();

            // 2.0.0.0未満の場合のみ実行
            if (lastVersion == "")
                ToVersion2_0_0_0();
        }

        private static void ToVersion2_0_0_0()
        {
            void MoveTo(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, bool delete)
            {
                targetDirectory.Create();
                foreach (var dir in sourceDirectory.GetDirectories())
                {
                    var name = dir.Name;
                    if (name != "world" && name != "world_nether" && name != "world_the_end")
                    {
                        dir.MoveTo(Path.Combine(targetDirectory.FullName, name));
                    }
                }
                foreach (var file in sourceDirectory.GetFiles())
                {
                    var name = file.Name;
                    file.MoveTo(Path.Combine(targetDirectory.FullName, name));
                }
                if (delete)
                {
                    sourceDirectory.Delete();
                }
            }

            void convert(VersionPath version, DirectoryInfo world)
            {
                DirectoryInfo sourceDirectory = world;
                DirectoryInfo targetDirectory;
                var name = world.Name;
                var delete = false;
                if(name == "plugins" || name == "bundler" || name == "logs" || name == "libraries")
                {
                    return;
                }
                else if (Regex.IsMatch(name, "_nether$"))
                {
                    name = Regex.Match(name, "(^[0-9A-Za-z_-]+)_nether$").Groups[1].Value;
                    var w = version.Worlds.GetWorldDirectory(name);
                    w.Create(true);
                    targetDirectory = w.Nether.Directory;
                    delete = true;
                    MoveTo(sourceDirectory, targetDirectory, delete);
                }
                else if (Regex.IsMatch(name, "_the_end$"))
                {
                    name = Regex.Match(name, "(^[0-9A-Za-z_-]+)_the_end$").Groups[1].Value;
                    var w = version.Worlds.GetWorldDirectory(name);
                    w.Create(true);
                    targetDirectory = w.End.Directory;
                    delete = true;
                    MoveTo(sourceDirectory, targetDirectory, delete);
                }
                else
                {
                    var w = version.Worlds.GetWorldDirectory(name);
                    w.Directory.Create();
                    targetDirectory = w.World.Directory;
                    delete = name != "worlds";
                    MoveTo(sourceDirectory, targetDirectory, delete);
                    version.ServerProperties.File.CopyTo(w.ServerProperties.FullName);
                    version.Ops.File.CopyTo(w.Ops.FullName);
                    version.WhiteList.File.CopyTo(w.WhiteList.FullName);
                    version.BannedIps.File.CopyTo(w.BannedIps.FullName);
                    version.BannedPlayers.File.CopyTo(w.BannedPlayers.FullName);
                    // プラグインをコピー
                    version.Plugins.Directory.CopyTo(w.World.Plugins.FullName);
                }
            }

            foreach (var version in ServerGuiPath.Instance.WorldData.GetVersionDirectories())
            {
                var worlds = new DirectoryInfo(Path.Combine(version.Directory.FullName, "worlds"));

                if (worlds.Exists)
                    convert(version, worlds);

                foreach (var world in version.Directory.GetDirectories())
                    if (world.Name != "worlds")
                        convert(version, world);
            }
        }
    }
    //private static async Task MoveToAsync(DirectoryInfo from, DirectoryInfo to)
    //{
    //    //Creates all of the directories and sub-directories
    //    foreach (DirectoryInfo dirInfo in from.GetDirectories("*", SearchOption.AllDirectories))
    //    {
    //        string dirPath = dirInfo.FullName;
    //        string outputPath = dirPath.Replace(from.FullName, to.FullName);
    //        Directory.CreateDirectory(outputPath);

    //        foreach (FileInfo file in dirInfo.EnumerateFiles())
    //        {
    //            using (FileStream SourceStream = file.OpenRead())
    //            {
    //                using (FileStream DestinationStream = File.Create(outputPath + file.Name))
    //                {
    //                    await SourceStream.CopyToAsync(DestinationStream);
    //                }
    //            }
    //        }
    //    }
    //}
}
