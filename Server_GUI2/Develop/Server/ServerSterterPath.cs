using System;
using System.IO;
using System.Linq;

namespace Server_GUI2.Develop.Server
{
    public abstract class DirectoryPath
    {
        protected DirectoryPath(DirectoryInfo directory)
        {
            Directory = directory;
        }
        public DirectoryInfo Directory;
        public bool Exists => Directory.Exists;
        public string Name => Directory.Name;
        public string FullName => Directory.FullName;

        protected FileInfo SubFile(string name)
        {
            return new FileInfo(Path.Combine(FullName, name));
        }

        protected DirectoryInfo SubDirectory(string name)
        {
            return new DirectoryInfo(Path.Combine(FullName, name));
        }
    }
    public abstract class FilePath
    {
        protected FilePath(FileInfo file)
        {
            File = file;
        }
        public FileInfo File;
        public bool Exists => File.Exists;
        public string Name => File.Name;
        public string FullName => File.FullName;
    }

    /// <summary>
    /// ServerGuiのカレントディレクトリ
    /// </summary>
    public class ServerGuiPath : DirectoryPath
    {
        // TODO: 元のカレントディレクトリに戻す
        public static ServerGuiPath Instance = new ServerGuiPath(new DirectoryInfo(Environment.GetEnvironmentVariable("SERVER_STERTER_TEST")));
        public WorldDataPath WorldData;
        public RemotesJsonPath RemotesJson;
        public GitStatePath GitState;
        private ServerGuiPath(DirectoryInfo directory) : base(directory)
        {
            WorldData = new WorldDataPath(SubDirectory("World_Data"));
            RemotesJson = new RemotesJsonPath(SubFile("remotes.json"));
            GitState = new GitStatePath(SubDirectory("git_state"));
        }
    }

    public class RemotesJsonPath : FilePath
    {
        internal RemotesJsonPath(FileInfo file) : base(file)
        {

        }
    }

    public class GitStatePath : DirectoryPath
    {
        public WorldstateJsonPath WorldStateJson;
        internal GitStatePath(DirectoryInfo directory) : base(directory)
        {
            WorldStateJson = new WorldstateJsonPath(SubFile("worldstate.json"));
        }
    }

    public class WorldstateJsonPath : FilePath
    {
        internal WorldstateJsonPath(FileInfo file) : base(file)
        {

        }
    }


    public class WorldDataPath : DirectoryPath
    {
        internal WorldDataPath(DirectoryInfo directory) : base(directory)
        {

        }
        public VersionPath[] GetVersionDirectories()
        {
            return Directory.GetDirectories().Select(x => new VersionPath(x)).ToArray();
        }
        public VersionPath GetVersionDirectory(string name)
        {
            return new VersionPath(SubDirectory(name));
        }
    }

    public class VersionPath : DirectoryPath
    {
        //TODO : eura.txt server.jar server.properties ...
        public VersionLogsPath Logs;
        internal VersionPath(DirectoryInfo directory) : base(directory)
        {
            Logs = new VersionLogsPath(SubDirectory("logs"));
        }
        public WorldPath[] GetWorldDirectories()
        {
            return Directory.GetDirectories().Select(x => new WorldPath(x)).ToArray();
        }
        public WorldPath GetWorldDirectory(string name)
        {
            return new WorldPath(SubDirectory(name));
        }
    }

    public class VersionLogsPath : DirectoryPath
    {
        internal VersionLogsPath(DirectoryInfo directory) : base(directory)
        {

        }
    }

    public class WorldPath : DirectoryPath
    {
        public WorldWorldPath World;
        public WorldNetherPath Nether;
        public WorldEndPath End;
        internal WorldPath(DirectoryInfo directory) : base(directory)
        {
            World = new WorldWorldPath(SubDirectory("world"));
            Nether = new WorldNetherPath(SubDirectory("world_nether"));
            End = new WorldEndPath(SubDirectory("world_end"));
        }
    }
    public class WorldWorldPath : DirectoryPath
    {
        public DatapacksPath Datapccks;

        internal WorldWorldPath(DirectoryInfo directory) : base(directory)
        {
            Datapccks = new DatapacksPath(SubDirectory("datapccks"));
        }
    }
    public class WorldNetherPath : DirectoryPath
    {
        internal WorldNetherPath(DirectoryInfo directory) : base(directory)
        {

        }
    }
    public class WorldEndPath : DirectoryPath
    {
        internal WorldEndPath(DirectoryInfo directory) : base(directory)
        {

        }
    }
    public class DatapacksPath : DirectoryPath
    {
        internal DatapacksPath(DirectoryInfo directory) : base(directory)
        {

        }
        public DatapackPath[] GetDatapackDirectories()
        {
            return Directory.GetDirectories().Select(x => new DatapackPath(x)).ToArray();
        }
        public DatapackPath GetDatapackDirectory(string name)
        {
            return new DatapackPath(SubDirectory(name));
        }
    }
    public class DatapackPath : DirectoryPath
    {
        internal DatapackPath(DirectoryInfo directory) : base(directory)
        {

        }
    }
}
