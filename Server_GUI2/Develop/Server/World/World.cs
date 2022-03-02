using log4net;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MW = ModernWpf;

namespace Server_GUI2.Develop.Server.World
{
    public class WorldException: Exception
    {
        public WorldException(string message) : base(message) { }
    }

    public interface IWorld: IWorldBase
    {
        bool Available { get; }

        RemoteWorld RemoteWorld { get; }

        string DisplayName { get; }

        bool CanCahngeRemote { get; }

        CustomMap CustomMap { get; set; }

        bool HasCustomMap { get; }

        bool HasRemote { get; }

        void Unlink();

        void Link(RemoteWorld remote);

        void WrapRun(Version version, Action<ServerProperty> runFunc);
    }

    /// <summary>
    /// GUIのメインウィンドウの【New World】
    /// </summary>
    public class NewWorld : IWorld
    {
        public bool Available => Version.Available && RemoteWorld.Available;

        protected readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool HasRemote => RemoteWorld != null;

        public RemoteWorld RemoteWorld { get; private set; }

        public string DisplayName => "【new World】";

        public bool CanCahngeRemote { get; } = true;

        public CustomMap CustomMap { get; set; }

        public bool HasCustomMap => CustomMap != null;

        public DatapackCollection Datapacks { get; } = new DatapackCollection(new List<string>());

        public PluginCollection Plugins { get; } = new PluginCollection(new List<string>());

        public ServerProperty Property { get; } = new ServerProperty();

        public ServerType? Type { get; } = null;

        private string _name = "input_name";

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
            return Regex.IsMatch(name, @"^[0-9a-zA-Z_-]$");
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
            if (!CanCahngeRemote) throw new WorldException($"Cannot unlink World \"{DisplayName}\"");
            if (HasRemote) throw new WorldException($"World \"{DisplayName}\" is unlinked");

            RemoteWorld = remote;
        }

        public void WrapRun(Version version, Action<ServerProperty> runFunc)
        {
            // ローカルワールドを生成
            var localWorld = new LocalWorld( version.Path.GetWorldDirectory(Name), version );

            World world;
            if (HasRemote)
                world = new World(localWorld,RemoteWorld);
            else
                world = new World(localWorld);

            WorldCollection.Instance.Add(world);

            // 実行
            world.WrapRun(version,runFunc);
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
                if (HasRemote)
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

        //リンク先が変更可能か
        public bool CanCahngeRemote { get; private set; }

        public CustomMap CustomMap { get; set; }

        public bool HasCustomMap => CustomMap != null;

        public DatapackCollection Datapacks => world.Datapacks;
        public PluginCollection Plugins => world.Plugins;

        public ServerProperty Property => world.Property;

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
            deleteRemoteEvent = new EventHandler((_, __) => UnlinkForce());
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
            deleteRemoteEvent = new EventHandler((_, __) => UnlinkForce());
            LocalWorld = local;
            RemoteWorld = remote;
            RemoteWorld.DeleteEvent += deleteRemoteEvent;
            CanCahngeRemote = false;
        }

        // リンクを解除
        public void Unlink()
        {
            if (!CanCahngeRemote) throw new WorldException($"Cannot unlink World \"{DisplayName}\"");
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
            if (!CanCahngeRemote) throw new WorldException($"Cannot unlink World \"{DisplayName}\"");
            if (HasRemote) throw new WorldException($"World \"{DisplayName}\" is unlinked");

            RemoteWorld.DeleteEvent += deleteRemoteEvent;
            RemoteWorld = remote;
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
            return new WorldState() ;
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public void WrapRun(Version version, Action<ServerProperty> runFunc)
        {
            // リモートがない場合
            if (!HasRemote)
                WrapRun_Unlinked(version, runFunc);
            // 起動前からリンクされていたリモートがある場合
            else if (!CanCahngeRemote)
                WrapRun_Linked(version, runFunc);
            // 新しくローカルをリンクする場合
            else
                WrapRun_NewLink(version, runFunc);
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
        public void WrapRun_Unlinked(Version version, Action<ServerProperty> runFunc)
        {
            // カスタムマップの導入＋バージョン変更
            TryImportCustomMapAndChangeVersion(LocalWorld, version);

            // データパックの導入
            Datapacks.Evaluate(LocalWorld.Path.FullName);

            // プラグインの導入
            Plugins.Evaluate(LocalWorld.Path.FullName);

            // 実行
            LocalWorld.WrapRun(runFunc);
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// 新規リモートワールドにPush
        /// </summary>
        private void WrapRun_NewLink(Version version, Action<ServerProperty> runFunc)
        {
            // ワールドのリンク先を固定する
            CanCahngeRemote = false;

            // 起動中フラグを立てる
            Using = true;

            // リモートワールドを複数人が同時に開かないようにロック
            RemoteWorld.Using = true;
            RemoteWorld.UpdateWorldState();

            // カスタムマップの導入＋バージョン変更
            TryImportCustomMapAndChangeVersion(LocalWorld, version);

            // データパックの導入
            Datapacks.Evaluate(LocalWorld.Path.FullName);

            // プラグインの導入
            Plugins.Evaluate(LocalWorld.Path.FullName);

            // 実行
            LocalWorld.WrapRun(runFunc);

            // Push
            RemoteWorld.FromLocal(LocalWorld);

            // リモートのワールドデータを更新し、ロック解除
            RemoteWorld.Using = false;
            RemoteWorld.UpdateWorldState();

            // 起動中フラグを回収
            // TODO: Pushが成功しなかった場合はフラグを立てたままにする
            Using = false;
        }

        /// <summary>
        /// 既存リモートワールドからPull
        /// 起動関数を引数に取って起動
        /// 既存リモートワールドにPush
        /// </summary>
        private void WrapRun_Linked(Version version, Action<ServerProperty> runFunc)
        {
            // 起動中フラグを立てる
            Using = true;

            // リモートワールドを複数人が同時に開かないようにロック
            RemoteWorld.Using = true;
            RemoteWorld.UpdateWorldState();

            // Pull
            RemoteWorld.ToLocal(LocalWorld);

            // カスタムマップの導入＋バージョン変更
            TryImportCustomMapAndChangeVersion(LocalWorld, version);

            // データパックの導入
            Datapacks.Evaluate(LocalWorld.Path.FullName);

            // プラグインの導入
            Plugins.Evaluate(LocalWorld.Path.FullName);

            // 実行
            LocalWorld.WrapRun(runFunc);

            // Push
            RemoteWorld.FromLocal(LocalWorld);

            // リモートのワールドデータを更新し、ロック解除
            RemoteWorld.Using = false;
            RemoteWorld.UpdateWorldState();

            // 起動中フラグを回収
            // TODO: Pushが成功しなかった場合はフラグを立てたままにする
            Using = false;

            // LinkJsonを更新
            WorldCollection.Instance.SaveLinkJson();
        }

        /// <summary>
        /// 必要に応じてCutomMapを導入し、必要に応じてバージョンを変更する
        /// </summary>
        private void TryImportCustomMapAndChangeVersion(LocalWorld local, Version version)
        {
            if (HasCustomMap)
            {
                CustomMap.Import(local.Path.World.FullName);
                // LocalWorldの中身に変更を反映(VtoSコンバート等も含む)
                local.ReConstruct(local.Path, version, version.Type, local.Property, local.Datapacks);
            }

            //versionのダウングレードを確認して警告表示
            if (version < Version)
            {
                logger.Warn($"The World-Data will be recreated by {version} from {Version}");
                var result = MW.MessageBox.Show(
                    $"ワールドデータを{Version}から{version}へバージョンダウンしようとしています。\n" +
                    $"データが破損する可能性が極めて高い操作ですが、危険性を理解したうえで実行しますか？", "Server Starter", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    throw new DowngradeException("User reject downgrading");
                }
            }

            // version変更
            if (local.Path.Parent != version.Path)
            {
                // versionのフォルダに移動するとともにVtoS変換
                Console.WriteLine($"local.path: {local.Path}");
                Console.WriteLine($"local.path: {version}");
                var newPath = version.Path.GetWorldDirectory(local.Path.Name);
                local.Move(newPath, version, version.Type, local.Property, local.Datapacks, addSuffixWhenNameCollided: true);
            }
        }
    }
}
