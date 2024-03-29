﻿using log4net;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using MW = ModernWpf;
using Server_GUI2.Util;
using Server_GUI2.Develop.Util;
using Microsoft.VisualBasic.FileIO;

namespace Server_GUI2.Develop.Server.World
{
    public class WorldException: Exception
    {
        public WorldException(string message) : base(message) { }
    }

    public interface IWorld: IWorldBase
    {
        RemoteWorld RemoteWorld { get; }

        string DisplayName { get; }

        bool CanCahngeRemote { get; }

        CustomMap CustomMap { get; set; }

        bool HasCustomMap { get; }

        bool HasRemote { get; }

        void Unlink();

        void Link(RemoteWorld remote);

        void WrapRun(Version version, bool reGenerate, Action<ServerSettings, string> runFunc);
    }

    /// <summary>
    /// GUIのメインウィンドウの【New World】
    /// </summary>
    public class NewWorld : IWorld
    {
        protected readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool HasRemote => RemoteWorld != null;

        public RemoteWorld RemoteWorld { get; private set; }

        public string DisplayName => "【new World】";

        public bool CanCahngeRemote { get; } = true;

        public CustomMap CustomMap { get; set; }

        public bool HasCustomMap => CustomMap != null;

        public DatapackCollection Datapacks { get; set; } = new DatapackCollection();

        public PluginCollection Plugins { get; set; } = new PluginCollection();

        public ServerSettings Settings { get; set; } = new ServerSettings();

        public ServerType? Type { get; } = null;

        private string _name = Properties.Resources.Main_InputName;

        public string Name
        {
            get => _name;
            set 
            {
                if (IsUseableName(value))
                    _name = value;
                else
                    throw new WorldException($"\"{value}\" is invalid world name.");
            }
        }

        public WorldState ExportWorldState()
        {
            throw new WorldException("\"new world\" must not export world state");
        }

        public bool IsUseableName(string name)
        {
            return Regex.IsMatch(name, @"^[0-9a-zA-Z_-]+$");
        }

        public Version Version { get; } = null;

        /// <summary>
        /// リンクされていないローカルワールド
        /// </summary>
        public NewWorld()
        {
            RemoteWorld = null;
        }

        // リンクを解除
        public void Unlink()
        {
            if (!CanCahngeRemote) throw new WorldException($"Cannot unlink World \"{DisplayName}\"");
            if (!HasRemote) throw new WorldException($"World \"{DisplayName}\" is unlinked");
             
            RemoteWorld = null;
        }

        // リモートをリンク
        public void Link(RemoteWorld remote)
        {
            if (!CanCahngeRemote) throw new WorldException($"Cannot link World \"{DisplayName}\"");
            if (HasRemote) throw new WorldException($"World \"{DisplayName}\" is unlinked");

            RemoteWorld = remote;
        }

        public void WrapRun(Version version, bool reGenerate, Action<ServerSettings, string> runFunc)
        {
            // ローカルワールドを生成
            var custommapPath = version.Path.Worlds.GetWorldDirectory(Name);
            Version custommapVersion;
            if (custommapPath.Nether.Exists)
                custommapVersion = VersionFactory.Instance.OldestSpigotVersion;
            else custommapVersion = VersionFactory.Instance.OldestVanillaVersion;

            var localWorld = new LocalWorld(custommapPath, custommapVersion);

            World world = new World(localWorld);
            if (HasRemote)
                world.Link(RemoteWorld);

            world.Settings = Settings;
            world.Datapacks = Datapacks;
            world.Plugins = Plugins;
            world.CustomMap = CustomMap;


            WorldCollection.Instance.Add(world);

            // 実行
            world.WrapRun(version, reGenerate, runFunc);
        }
    }

    /// <summary>
    /// ローカルとリモートの接続を管理する
    /// GUIのメインウィンドウのWorldにはこれが入る
    /// </summary>
    public class World : IWorld
    {
        public bool Available => Version.Available && RemoteWorld.Available;

        protected readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // 削除イベント
        public event EventHandler DeleteEvent;

        // remoteを削除する際のイベントハンドラ
        private EventHandler deleteRemoteEvent;

        public readonly LocalWorld LocalWorld;

        // localを削除する際のイベントハンドラ
        private EventHandler deleteLocalEvent;

        public RemoteWorld RemoteWorld { get; private set; }

        private IWorldBase world
        {
            get
            {
                if (HasRemote && !isNewRemote)
                    return RemoteWorld;
                else
                    return LocalWorld;
            }
        }

        /// <summary>
        /// サーバーが立っている間true
        /// </summary>
        private bool Using;

        public bool HasRemote => RemoteWorld != null;

        private bool isNewRemote = false;

        //リンク先が変更可能か
        public bool CanCahngeRemote { get; private set; }

        public CustomMap CustomMap { get; set; }

        public bool HasCustomMap => CustomMap != null;

        public DatapackCollection Datapacks {
            get => world.Datapacks;
            set { world.Datapacks = value; }
        }

        public PluginCollection Plugins {
            get => world.Plugins;
            set { world.Plugins = value; }
        }
        
        public ServerSettings Settings
        {
            get => world.Settings;
            set { world.Settings = value; }
        }

        public ServerType? Type => world.Type;

        // ワールドの表示名（内部実装用）（ViewではConverterを通して表示名を決定している）
        public string DisplayName => HasRemote ?
            $"{world.Version.Name}/{world.Name}*" :
            $"{world.Version.Name}/{world.Name}";

        public string Name => world.Name;

        public Version Version => world.Version;

        /// <summary>
        /// リンクされていないローカルワールド
        /// </summary>
        public World(LocalWorld local)
        {
            deleteRemoteEvent = new EventHandler((_, __) => {UnlinkForce(); WorldCollection.Instance.SaveLinkJson();});
            deleteLocalEvent = new EventHandler((_, __) => Delete());

            LocalWorld = local;
            RemoteWorld = null;
            CanCahngeRemote = true;
        }

        /// <summary>
        /// リンクされたローカルワールド
        /// </summary>
        public World(LocalWorld local, RemoteWorld remote)
        {
            deleteRemoteEvent = new EventHandler((_, __) => { UnlinkForce();WorldCollection.Instance.SaveLinkJson(); });
            LocalWorld = local;
            RemoteWorld = remote;
            RemoteWorld.DeleteEvent += deleteRemoteEvent;
            CanCahngeRemote = false;
        }

        // リンクを解除
        public void Unlink()
        {
            //if (!CanCahngeRemote) throw new WorldException($"Cannot unlink World \"{DisplayName}\"");
            UnlinkForce();
        }

        private void UnlinkForce()
        {
            if (!HasRemote) throw new WorldException($"World \"{DisplayName}\" is unlinked");
            RemoteWorld.DeleteEvent -= deleteRemoteEvent;
            CanCahngeRemote = true;
            RemoteWorld = null;
        }

        // リモートをリンク
        public void Link(RemoteWorld remote)
        {
            if (!CanCahngeRemote) throw new WorldException($"Cannot link World \"{DisplayName}\"");
            if (HasRemote) throw new WorldException($"World \"{DisplayName}\" is already linked");

            // TODO: .gitディレクトリを生成して初期化
            CanCahngeRemote = !remote.Exist;

            isNewRemote = true;
            RemoteWorld = remote;
            RemoteWorld.DeleteEvent += deleteRemoteEvent;
        }

        /// <summary>
        /// ワールドを削除(ローカル)
        /// </summary>
        public void Delete()
        {
            // リモートを解除
            if (HasRemote) UnlinkForce();

            // ローカルのイベントを解除
            LocalWorld.DeleteEvent -= deleteLocalEvent;

            // 削除
            LocalWorld.Delete();
        }

        public WorldState ExportWorldState()
        {
            return new WorldState();
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public void WrapRun(Version version, bool reGenerate, Action<ServerSettings, string> runFunc)
        {
            logger.Info("<WrapRun>");
            // リモートがない場合
            if (!HasRemote)
                WrapRun_Unlinked(version, reGenerate, runFunc);
            // 起動前からリンクされていたリモートがある場合
            else if (!CanCahngeRemote)
                WrapRun_Linked(version, reGenerate, runFunc);
            // 新しくローカルをリンクする場合
            else
                WrapRun_NewLink(version, reGenerate, runFunc);
            logger.Info("</WrapRun>");
        }

        /// <summary>
        /// 前回起動時にpush出来なかったworldをpushする
        /// </summary>
        public void UploadWorld()
        {
            if (RemoteWorld.Available)
            {
                try
                {
                    RemoteWorld.FromLocal(LocalWorld);
                }
                catch (GitException)
                {
                    // リモートのワールドデータを更新し、ロック解除
                    RemoteWorld.Using = false;
                    RemoteWorld.UpdateWorldState();
                    // 起動中フラグを回収
                    Using = false;
                }
            }
        }

        /// <summary>
        /// remotes.jsonを保存する際に使う
        /// </summary>
        public RemoteLinkJson ExportLinkJson()
        {
            return new RemoteLinkJson(RemoteWorld, LocalWorld, Using);
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public void WrapRun_Unlinked(Version version, bool reGenerate, Action<ServerSettings, string> runFunc)
        {
            logger.Info("<WrapRun_Unlinked>");

            // カスタムマップの導入＋バージョン変更
            TryImportCustomMapAndChangeVersion(LocalWorld, reGenerate, version);

            // データパックの導入
            logger.Info("Import datapacks");
            Datapacks.Evaluate(LocalWorld.Path.World.Datapccks.FullName);

            // プラグインの導入
            logger.Info("Import plugins");

            Plugins.Evaluate(LocalWorld.Path.World.Plugins);

            version.Path.Plugins.Delete(true);

            LocalWorld.Path.World.Plugins.Directory.CopyTo(version.Path.Plugins.FullName);

            try
            {
                // 実行
                LocalWorld.WrapRun(version, runFunc);
            }
            finally
            {
                // プラグインの設定の反映
                logger.Info("save plugin settings");

                LocalWorld.Path.World.Plugins.Delete(true);
                version.Path.Plugins.Directory.CopyTo(LocalWorld.Path.World.Plugins.FullName);

                logger.Info("</WrapRun_Unlinked>");
            }
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// 新規リモートワールドにPush
        /// </summary>
        private void WrapRun_NewLink(Version version, bool reGenerate, Action<ServerSettings, string> runFunc)
        {
            logger.Info("ready newly linked world data");

            // .gitディレクトリを作る
            var gitLocal = new GitLocal(LocalWorld.Path.FullName);
            gitLocal.Init(RemoteWorld.Storage.AccountName,RemoteWorld.Storage.Email);
            gitLocal.AddAllAndCommit("first commit");

            // ワールドのリンク先を固定する
            CanCahngeRemote = false;

            // 起動中フラグを立てる
            Using = true;

            // リモートワールドを複数人が同時に開かないようにロックし、最終使用者を更新
            RemoteWorld.LastUser = UserSettings.Instance.userSettings.OwnerName ?? "";
            RemoteWorld.Using = true;

            // ローカルのワールドの設定情報を更新
            LocalWorld.Settings = Settings;

            // カスタムマップの導入＋バージョン変更
            TryImportCustomMapAndChangeVersion(LocalWorld, reGenerate, version);


            // データパックの導入
            Datapacks.Evaluate(LocalWorld.Path.World.Datapccks.FullName);

            // プラグインの導入
            Plugins.Evaluate(LocalWorld.Path.World.Plugins);
            version.Path.Plugins.Delete(true);
            LocalWorld.Path.World.Plugins.Directory.CopyTo(version.Path.Plugins.FullName);

            // 実行
            LocalWorld.WrapRun(version, runFunc);

            // プラグインの設定の反映
            logger.Info("save plugin settings");

            LocalWorld.Path.World.Plugins.Delete(true);
            version.Path.Plugins.Directory.CopyTo(LocalWorld.Path.World.Plugins.FullName);

            StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_Push);
            // Push
            try
            {
                RemoteWorld.FromLocal(LocalWorld);

                StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_Unlock);

                // リモートのワールドデータを更新し、ロック解除
                RemoteWorld.Using = false;
                RemoteWorld.EnableWorld(version, version.Type);

                StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_UpdateState, true);

                // リモートの内容をローカルと同期
                RemoteWorld.Settings = LocalWorld.Settings;
                RemoteWorld.Plugins = LocalWorld.Plugins;
                RemoteWorld.Datapacks= LocalWorld.Datapacks;

                RemoteWorld.UpdateWorldState();

                // 起動中フラグを回収
                Using = false;
            }
            catch (GitException)
            {
            }

            // #stateブランチを更新
            RemoteWorld.UpdateWorldState();

            StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_UpdateLink);

            // LinkJsonを更新
            WorldCollection.Instance.SaveLinkJson();
        }

        /// <summary>
        /// 既存リモートワールドからPull
        /// 起動関数を引数に取って起動
        /// 既存リモートワールドにPush
        /// </summary>
        private void WrapRun_Linked(Version version, bool reGenerate , Action<ServerSettings, string> runFunc)
        {
            logger.Info("ready already linked world data");

            if (RemoteWorld.AlreadyUsing) {

                var is_annonimus = RemoteWorld.LastUser == null || RemoteWorld.LastUser == "" || RemoteWorld.LastUser == "Anonymus";

                if ( is_annonimus || RemoteWorld.LastUser != UserSettings.Instance.userSettings.OwnerName )
                {
                    var msg = is_annonimus ? "このワールドは現在匿名ユーザーによって開かれています。" : $"このワールドは現在{RemoteWorld.LastUser}によって開かれています。";
                    // TODO:英訳
                    ServerStarterException.ShowError(msg, new RemoteWorldException("remoteworld is now used by other member"));
                }
            }


            // 起動中フラグを立てる
            Using = true;

            // リモートワールドを複数人が同時に開かないようにロック
            RemoteWorld.LastUser = UserSettings.Instance.userSettings.OwnerName ?? "";
            RemoteWorld.Using = true;
            RemoteWorld.UpdateWorldState();

            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_Fetch, true);

            // Pull
            RemoteWorld.ToLocal(LocalWorld);

            // ローカルのワールドの設定情報を更新
            LocalWorld.Settings = Settings;

            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_CheckVer);
            // カスタムマップの導入＋バージョン変更
            TryImportCustomMapAndChangeVersion(LocalWorld, reGenerate, version);

            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_RemoteDatapack);

            // データパックの導入
            Datapacks.Evaluate(LocalWorld.Path.World.Datapccks.FullName);

            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_RemotePlugin);
            // プラグインの導入
            Plugins.Evaluate(LocalWorld.Path.World.Plugins);
            version.Path.Plugins.Delete(true);
            LocalWorld.Path.World.Plugins.Directory.CopyTo(version.Path.Plugins.FullName);

            // 実行
            LocalWorld.WrapRun(version,runFunc);

            // プラグインの設定の反映
            logger.Info("save plugin settings");

            LocalWorld.Path.World.Plugins.Delete(true);
            version.Path.Plugins.Directory.CopyTo(LocalWorld.Path.World.Plugins.FullName);


            StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_Push);
            // Push
            try
            {
                StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_Unlock);
                RemoteWorld.FromLocal(LocalWorld);

                // リモートのワールドデータを更新し、ロック解除
                RemoteWorld.Using = false;
                RemoteWorld.Version = version;
                StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_UpdateState, true);
                RemoteWorld.UpdateWorldState();

                // 起動中フラグを回収
                Using = false;
            }
            catch (GitException)
            {
            }

            // #stateブランチを更新
            RemoteWorld.UpdateWorldState();

            StartServer.CloseProgressBar.AddMessage(Properties.Resources.CloseBar_UpdateLink);
            // LinkJsonを更新
            WorldCollection.Instance.SaveLinkJson();
        }

        /// <summary>
        /// 必要に応じてCutomMapを導入し、必要に応じてバージョンを変更する
        /// </summary>
        private void TryImportCustomMapAndChangeVersion(LocalWorld local, bool reGenerate, Version version)
        {
            logger.Info("<TryImportCustomMapAndChangeVersion>");

            if (HasCustomMap)
            {
                logger.Info("Import CustomMap");
                CustomMap.Import(local.Path.World.FullName);
                // LocalWorldの中身に変更を反映(VtoSコンバート等も含む)
                local.ReConstruct(local.Path, version, version.Type, local.Settings, local.Datapacks, local.Plugins);
            }
            else if (reGenerate)
            {
                // ワールドデータをリセット
                // TODO: 同一のseed値を使用(level.datの中身見ないとseedが取得できないので先送り)
                FileSystem.DeleteDirectory(local.Path.World.FullName, DeleteDirectoryOption.DeleteAllContents);

            }
            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_CustomMap);

            //versionのダウングレードを確認して警告表示
            if (version < Version)
            {
                logger.Warn($"The World-Data will be recreated by {version} from {Version}");
                int result = CustomMessageBox.Show(
                    $"{Properties.Resources.World_VerDownMsg1}{Version}{Properties.Resources.World_VerDownMsg2}{version}{Properties.Resources.World_VerDownMsg3}\n{Properties.Resources.World_VerDownMsg4}",
                    ButtonType.YesNo,
                    Image.Warning
                    );
                if (result == 1)
                    throw new ServerStarterException(new DowngradeException("User reject downgrading"));
            }

            // version変更
            if (local.Path.Parent.Parent.Name != version.Path.Name)
            {
                // versionのフォルダに移動するとともにVtoS変換
                //Console.WriteLine($"local.path: {local.Path}");
                //Console.WriteLine($"local.path: {version}");
                var newPath = version.Path.Worlds.GetWorldDirectory(local.Path.Name);
                local.Move(newPath, version, version.Type, local.Settings, local.Datapacks, local.Plugins, addSuffixWhenNameCollided: true);
            }
            else if(local.Version != version)
            {
                // 新規ワールドの場合のみ起動
                local.SetVersion(version);
            }


            StartServer.RunProgressBar.AddMessage(Properties.Resources.RunBar_Convert);

            logger.Info("</TryImportCustomMapAndChangeVersion>");
        }
    }
}
