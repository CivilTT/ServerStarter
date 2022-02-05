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
    abstract class WorldWrapper
    {
        protected virtual World World { get;}

        /// <summary>
        /// 起動関数を引数に取って起動
        /// </summary>
        public abstract void WrapRun(Action<ServerProperty> runFunc);
    }

    /// <summary>
    /// リンクされていないローカルワールド
    /// </summary>
    class UnLinkedLocalWorldWrapper: WorldWrapper
    {
        private LocalWorld LocalWorld { get; }
        protected override World World => LocalWorld;
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
        public override void WrapRun(Action<ServerProperty> runFunc)
        {
            LocalWorld.WrapRun(runFunc);
        }
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
        private string path;
        private RemoteWorld RemoteWorld { get; }
        protected override World World => RemoteWorld;

        public LinkedRemoteWorldWrapper(RemoteWorld remoteWorld,string path)
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
        public override void WrapRun(Action<ServerProperty> runFunc)
        {
            var local = RemoteWorld.ToLocal(path);
            local.WrapRun(runFunc);
            RemoteWorld.FromLocal(local,false);
        }
    }

    /// <summary>
    /// 新規でリンクされたリモートワールド
    /// </summary>
    class NewLinkedRemoteWorldWrapper : ALinkedRemoteWorldWrapper
    {
        private LocalWorld localWorld;
        private RemoteWorld RemoteWorld { get; }
        protected override World World => RemoteWorld;

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
        public override void WrapRun(Action<ServerProperty> runFunc)
        {
            localWorld.WrapRun(runFunc);
            RemoteWorld.FromLocal(localWorld,true);
        }
    }
}
