using Server_GUI2.Util;
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
        public RemoteWorldException( string message) : base(message) { }
    }

    public interface IRemoteWorld : IWorldBase
    {
        Storage Storage { get; }
        /// <summary>
        /// GUI上の表示名
        /// </summary>
        string DisplayName { get; }
    }

    public class NewRemoteWorld : IRemoteWorld
    {
        public DatapackCollection Datapacks { get; set; } = new DatapackCollection();

        public PluginCollection Plugins { get; set; } = new PluginCollection();

        public ServerSettings Settings { get; set; } = new ServerSettings();

        public ServerType? Type { get; } = null;

        // TODO: 使用できない名前だったらはじく
        public string Name { get; set; }

        public Version Version { get; } = null;

        public Storage Storage { get; private set; }

        public string DisplayName { get; } = "【new remote world】";

        bool Available => Storage.Available;

        public WorldState ExportWorldState()
        {
            throw new NotImplementedException();
        }

        public NewRemoteWorld(Storage storage)
        {
            Storage = storage;
        }
    }

    /// <summary>
    /// リモートにある/(作る予定の)ワールドの情報
    /// </summary>
    public abstract class RemoteWorld : IRemoteWorld
    {
        public event EventHandler DeleteEvent;
        public bool Exist;
        public bool Using;
        public string LastUser;
        public bool Available;

        public bool IsVisible { get; set; } = true;

        public DatapackCollection Datapacks  { get; set; }

        public ServerSettings Settings { get; set; }

        public PluginCollection Plugins { get; set; }

        public ServerType? Type  { get; private set; }

        private string _name;

        /// <summary>
        /// ワールドの名称
        /// </summary>
        public string Name {
            get => _name;
            set
            {
                if (_name == value) return;
                if (Storage.IsUsableName(value))
                {
                    _name = value;
                    Storage.AddUsedName(value);
                }
                else
                    throw new RemoteWorldException($"\"{value}\" is unavailable remote world name.");
            }
        }

        /// <summary>
        /// ブランチのuuid
        /// </summary>
        public string Id  { get; private set; }

        public Version Version  { get; private set; }

        public Storage Storage { get; private set; }

        public string DisplayName => Name;

        public bool AlreadyUsing;

        /// <summary>
        /// WorldStateからRemotoworldを構成する
        /// </summary>
        public RemoteWorld(string id, WorldState state, Storage storage, bool available)
        {
            Exist = true;
            Storage = storage;
            Name = state.Name;
            LastUser = state.LastUser;
            Id = id;
            Version = VersionFactory.Instance.GetVersionFromName(state.Version);
            Type = ServerTypeExt.FromStr(state.Type);
            AlreadyUsing = state.Using;
            Settings = state.ServerSetting;
            Datapacks = new DatapackCollection(state.Datapacks);
            Plugins = new PluginCollection(state.Plugins);
            Using = false;
            Available = available;
        }

        public RemoteWorld(
            Storage storage,
            string id,
            string name,
            bool exist,
            Version version,
            ServerType? type,
            ServerSettings settings,
            DatapackCollection datapacks,
            PluginCollection plugins,
            bool available
            )
        {
            Exist = exist;
            Storage = storage;
            Name = name;
            Id = id;
            Version = version;
            Type = type;
            Settings = settings;
            Datapacks = datapacks;
            Plugins = plugins;
            Using = false;
            Available = available;
        }

        /// <summary>
        /// WorldStateを更新
        /// </summary>
        /// <param name="worldState"></param>
        public void UpdateWorldState()
        {
            Storage.UpdateWorldStates(Id,ExportWorldState());
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
            DeleteEvent?.Invoke(this,null);
        }

        public WorldState ExportWorldState()
        {
            if (!Exist) throw new WorldException("non-exist world must not export worldstate");
            return new WorldState(Name, Type.ToString(), Version.Name, Using, UserSettings.Instance.userSettings.OwnerName, Datapacks.ExportList(), Plugins.ExportList(), Settings);
        }

        public void EnableWorld(Version version, ServerType type)
        {
            Type = type;
            Exist = true;
            Version = version;
        }
    }

    public class GitRemoteWorld : RemoteWorld
    {
        private GitRemote remote;

        public GitRemoteWorld( GitRemote remote, string id, WorldState state, Storage storage, bool available ):
            base(id, state, storage, available)
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
            ServerSettings settings,
            DatapackCollection datapacks,
            PluginCollection plugins,
            bool available
            ) :base( storage,id,name,exist,version,type,settings,datapacks, plugins, available)
        {
            this.remote = remote;
        }


        /// <summary>
        /// ワールドデータを指定パスにPull/Cloneする
        /// </summary>
        public override void ToLocal(LocalWorld local)
        {
            if (!Available) throw new RemoteWorldException($"remoteworld {Id} is not available");

            var gitlocal = new GitLocal(local.Path.FullName);
            bool isRepository = gitlocal.IsGitRepository();

            if (isRepository)
            {
                var branches = gitlocal.GetBranchs();
                if (
                    // {Name}ブランチがある
                    branches.ContainsKey(Id) &&
                    // {Name}ブランチがリンクされている
                    branches[Id] is GitLinkedLocalBranch branch
                    )
                {
                    // pull
                    branch.Pull();

                    // データを更新
                    local.ReConstruct(local.Path, Version);

                    return;
                }
            }

            // .gitディレクトリごと削除
            var gitPath = Path.Combine(local.Path.FullName, ".git");
            if (Directory.Exists(gitPath))
            {
                DirectoryPath.RemoveReadonlyAttribute(new DirectoryInfo(gitPath));
                Directory.Delete(gitPath, true);
            }
            
            gitlocal.Init(remote.Account, remote.Email);
            gitlocal.AddAllAndCommit("First Commit");

            // リモートの追加
            var named = gitlocal.AddRemote(remote, "origin");

            // pull
            named.GetBranchs()[Id].CreateLinkedBranch(Id).Pull();

            // データを更新
            local.ReConstruct(local.Path, Version);
        }

        /// <summary>
        /// ワールドデータを指定パスにPushする
        /// </summary>
        public override void FromLocal(LocalWorld local)
        {
            if (!Available) throw new RemoteWorldException($"remoteworld {Name} is not available");

            var gitlocal = new GitLocal(local.Path.FullName);
            bool isRepository = gitlocal.IsGitRepository();

            if (isRepository)
            {
                var branches = gitlocal.GetBranchs();
                if (
                    // {Name}ブランチがある
                    branches.ContainsKey(Id) &&
                    // {Name}ブランチがリンクされている
                    branches[Id] is GitLinkedLocalBranch branch
                    )
                {
                    var owner = UserSettings.Instance.userSettings.OwnerName;
                    // push
                    branch.CommitPush( owner == "" ? "Anonymus" : owner);
                    return;
                }
            }

            // .gitディレクトリごと削除
            var gitPath = Path.Combine(local.Path.FullName, ".git");
            if (Directory.Exists(gitPath))
            {
                DirectoryPath.RemoveReadonlyAttribute(new DirectoryInfo(gitPath));
                Directory.Delete(gitPath, true);
            }
            gitlocal.Init(remote.Account,remote.Email);
            gitlocal.AddAllAndCommit("First Commit");

            // リモートの追加
            var named = gitlocal.AddRemote(remote, "origin");

            // push
            gitlocal.CreateBranch(Id).CreateLinkedBranch(named, Id).CommitPush("created branch");
        }

        /// <summary>
        /// リモートにあるデータを削除する
        /// </summary>
        public override void Delete()
        {
            if (!Available) throw new RemoteWorldException($"remoteworld {Name} is not available");
            base.Delete();
            var local = new GitLocal(ServerGuiPath.Instance.GitState.FullName);
            var named = local.AddRemote(remote, "#temp");
            named.GetBranchs()[Id].Delete();
            named.Remove();
        }
    }
}
