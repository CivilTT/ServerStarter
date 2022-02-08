using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
    /// <summary>
    /// ローカルとリモートのワールドのリンクを保持するクラス
    /// </summary>
    public abstract class WorldWrapper
    {
        public abstract World World { get;}

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public abstract void WrapRun(Version version, Action<ServerProperty> runFunc);

        public abstract string DisplayName { get; }
        public Version Version => World.Version;
    }

    /// <summary>
    /// リンクされていないローカルワールド
    /// </summary>
    class UnLinkedLocalWorldWrapper: WorldWrapper
    {
        private LocalWorld LocalWorld { get; }
        public override World World => LocalWorld;
        public UnLinkedLocalWorldWrapper(LocalWorld world)
        {
            LocalWorld = world;
        }

        /// <summary>
        /// リモートワールドとリンクする
        /// </summary>
        public NewLinkedRemoteWorldWrapper Link(RemoteWorld remoteWorld)
        {
            return new NewLinkedRemoteWorldWrapper(remoteWorld, LocalWorld);
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public override void WrapRun(Version version, Action<ServerProperty> runFunc)
        {
            LocalWorld.WrapRun(runFunc);
        }

        public override string DisplayName => $"{LocalWorld.Version.Name}/{LocalWorld.Name}";
    }

    abstract class ALinkedRemoteWorldWrapper : WorldWrapper
    {
        public abstract UnLinkedLocalWorldWrapper UnLink();
    }

    /// <summary>
    /// リンクされているリモートワールド
    /// </summary>
    class LinkedRemoteWorldWrapper : ALinkedRemoteWorldWrapper
    {
        private WorldPath path;
        private RemoteWorld RemoteWorld { get; }
        public override World World => RemoteWorld;

        public LinkedRemoteWorldWrapper(RemoteWorld remoteWorld, WorldPath path)
        {
            this.path = path;
            RemoteWorld = remoteWorld;
        }

        public override UnLinkedLocalWorldWrapper UnLink()
        {
            // リモートの情報をpullしてから接続を解除
            return new UnLinkedLocalWorldWrapper(RemoteWorld.ToLocal(path));
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public override void WrapRun(Version version, Action<ServerProperty> runFunc)
        {
            var local = RemoteWorld.ToLocal(path);
            // TODO: バージョンに従ってフォルダ移動
            //local.Move();
            local.WrapRun(runFunc);
            RemoteWorld.FromLocal(local,false);
        }
        public override string DisplayName => $"{RemoteWorld.Version.Name}/{RemoteWorld.Name}";
    }

    /// <summary>
    /// 新規でリンクされたリモートワールド
    /// </summary>
    class NewLinkedRemoteWorldWrapper : ALinkedRemoteWorldWrapper
    {
        private LocalWorld localWorld;
        private RemoteWorld RemoteWorld { get; }
        public override World World => RemoteWorld;

        public NewLinkedRemoteWorldWrapper(RemoteWorld remoteWorld, LocalWorld localWorld)
        {
            this.localWorld = localWorld;
            RemoteWorld = remoteWorld;
        }

        public override UnLinkedLocalWorldWrapper UnLink()
        {
            // リモートの情報をpullする必要はないのでそのまま返す
            return new UnLinkedLocalWorldWrapper(localWorld);
        }

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public override void WrapRun(Version version, Action<ServerProperty> runFunc)
        {
            localWorld.WrapRun(runFunc);
            RemoteWorld.FromLocal(localWorld,true);
        }
        public override string DisplayName => $"{RemoteWorld.Version.Name}/{RemoteWorld.Name}";
    }
}
