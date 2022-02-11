using Server_GUI2.Develop.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
    public class RemoteWorldException : Exception
    {
        public RemoteWorldException(string message) : base(message) { }
    }

    /// <summary>
    /// リモートにある/(作る予定の)ワールドの情報
    /// </summary>
    public abstract class RemoteWorld : IWorldBase
    {
        public event EventHandler DeleteEvent;
        public bool Exist;
        public bool Using;

        public DatapackCollection Datapacks  { get; private set; }

        public ServerProperty Property  { get; private set; }

        public ServerType? Type  { get; private set; }

        private string _name;
        /// <summary>
        /// ワールドの名称
        /// </summary>
        public string Name  {
            get => _name;
            set
            {
                if (Storage.IsUsableName(value))
                    _name = value;
                else
                    throw new RemoteWorldException($"\"{value}\" is unavailable remote world name.");
            }
        }

        /// <summary>
        /// ブランチのuuid
        /// </summary>
        public string Id  { get; private set; }

        public Version Version  { get; private set; }

        public Storage Storage;

        /// <summary>
        /// WorldStateからRemotoworldを構成する
        /// </summary>
        public RemoteWorld(string id, WorldState state, Storage storage)
        {
            Exist = true;
            Storage = storage;
            Name = state.Name;
            Id = id;
            Version = VersionFactory.Instance.GetVersionFromName(state.Version);
            Type = ServerTypeExt.FromStr(state.Type);
            Property = state.ServerProperty;
            Datapacks = new DatapackCollection(state.Datapacks);
            Using = false;
        }

        public RemoteWorld(
            Storage storage,
            string id,
            string name,
            bool exist,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            )
        {
            Exist = exist;
            Storage = storage;
            Name = name;
            Id = id;
            Version = version;
            Type = type;
            Property = property;
            Datapacks = datapacks;
            Using = false;
        }

        /// <summary>
        /// WorldStateを更新
        /// </summary>
        /// <param name="worldState"></param>
        public void UpdateWorldState()
        {
            Storage.SaveWorldStates();
        }

        /// <summary>
        /// ワールドデータを指定パスにPullする
        /// </summary>
        public abstract void ToLocal(LocalWorld local);

        /// <summary>
        /// ローカルワールドデータをPushする
        /// </summary>
        public abstract void FromLocal(LocalWorld local);

        /// <summary>
        /// リモートワールドデータを削除する
        /// </summary>
        public virtual void Delete()
        {
            if (DeleteEvent != null) DeleteEvent(this,null);
        }

        public WorldState ExportWorldState()
        {
            if (!Exist) throw new WorldException("non-exist world must not export worldstate");
            return new WorldState(Name,Type.ToString(),Version.Name,Using,Datapacks.ExportList(),Property);
        }
    }

    public class GitRemoteWorld : RemoteWorld
    {
        private GitRemote remote;

        public GitRemoteWorld( GitRemote remote, string id, WorldState state, Storage storage ):
            base(id, state, storage)
        {
            this.remote = remote;
        }

        public GitRemoteWorld(
            GitRemote remote,
            Storage storage,
            string id,
            string name,
            bool exist,
            Version version,
            ServerType? type,
            ServerProperty property,
            DatapackCollection datapacks
            ):base( storage,id,name,exist,version,type,property,datapacks )
        {
            this.remote = remote;
        }


        /// <summary>
        /// TODO: ワールドデータを指定パスにPull/Cloneする
        /// </summary>
        public override void ToLocal(LocalWorld local)
        {
            var gitlocal = new GitLocal(local.Path.FullName);
            // gitリポジトリではない or 別のブランチを追跡している場合
            bool ready = false;
            if (gitlocal.IsGitLocal())
            {
                ready = gitlocal.GetBranchs().ContainsKey(Name);
                if (!ready)
                {
                    // .gitディレクトリごと削除
                    var gitPath = Path.Combine(local.Path.FullName, ".git");
                    Directory.Delete(gitPath, true);
                }
            }

            if (!ready)
            {
                // #main ローカルブランチを作る
                gitlocal.Init("#main");
                gitlocal.Commit("#main");
                // add remote "origin"
                var named = gitlocal.AddRemote(remote, "origin");
                named.CreateBranch(Name);
            }
            var remoteBranch = new GitNamedRemote(gitlocal, remote, "origin");
            // fetch
            gitlocal.Fetch(remoteBranch.CreateBranch(Name));
            // checkout
            gitlocal.GetBranch(Name).Checkout();
            // merge
            gitlocal.Merge();

            local.ReConstruct(local.Path, Version);
        }

        /// <summary>
        /// ワールドデータを指定パスにPushする
        /// TODO: Git処理ラインの最適化
        /// </summary>
        public override void FromLocal(LocalWorld localWorld)
        {
            var local = new GitLocal(localWorld.Path.FullName);
            // gitリポジトリではない or 別のブランチを追跡している場合
            bool ready = false;
            if (local.IsGitLocal())
            {
                ready = local.GetBranchs().ContainsKey(Name);
                if (!ready)
                {
                    // .gitディレクトリごと削除
                    var gitPath = System.IO.Path.Combine(localWorld.Path.FullName, ".git");
                    Directory.Delete(gitPath, true);
                }
            }
            // gitリポジトリでないとき
            if (!ready)
            {
                // #main ローカルブランチを作る
                local.Init("#main");
                local.Commit("#main");
                // add remote "origin"
                var named = local.AddRemote(remote, "origin");
                var remoteBranch = named.CreateBranch(Name);
                var localBranch = local.GetBranch(Name);
                localBranch.Checkout();
                local.AddAll();
                local.Commit("test commit");
                localBranch.PushTrack(remoteBranch);
            }
            else
            {
                // checkout
                local.GetBranch(Name).Checkout();
                local.AddAll();
                local.Commit("test commit");
                // push
                local.Push();
            }
        }

        // TODO: ブランチの削除
        public override void Delete()
        {
            base.Delete();
        }
    }
}
