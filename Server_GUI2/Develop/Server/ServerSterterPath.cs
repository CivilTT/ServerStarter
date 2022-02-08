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

        public void Create(bool existsOk = false)
        {
            if (existsOk && Exists)
                return;
            Directory.Create();
        }
        public void Delete(bool deletedOk = false)
        {
            if (deletedOk && !Exists)
                return;
            Directory.Delete();
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

        public string ReadAllText()
        {
            var stream = File.OpenRead();
            var result = new StreamReader(stream).ReadToEnd();
            stream.Close();
            return result;
        }
        public void WriteAllText(string content)
        {
            System.IO.File.WriteAllText(FullName,content);
        }
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
            WorldData = new WorldDataPath(SubDirectory("World_Data"),this);
            RemotesJson = new RemotesJsonPath(SubFile("remotes.json"), this);
            GitState = new GitStatePath(SubDirectory("git_state"), this);
        }
    }

    public class RemotesJsonPath : FilePath
    {
        public ServerGuiPath Parent;
        internal RemotesJsonPath(FileInfo file, ServerGuiPath parent) : base(file)
        {
            Parent = parent;
        }
    }

    public class GitStatePath : DirectoryPath
    {
        public ServerGuiPath Parent;
        public WorldstateJsonPath WorldStateJson;
        internal GitStatePath(DirectoryInfo directory, ServerGuiPath parent) : base(directory)
        {
            Parent = parent;
            WorldStateJson = new WorldstateJsonPath(SubFile("worldstate.json"), this);
        }
    }

    public class WorldstateJsonPath : FilePath
    {
        public GitStatePath Parent;
        internal WorldstateJsonPath(FileInfo file, GitStatePath parent) : base(file)
        {
            Parent = parent;
        }
    }


    public class WorldDataPath : DirectoryPath
    {
        public ServerGuiPath Parent;
        internal WorldDataPath(DirectoryInfo directory, ServerGuiPath parent) : base(directory)
        {
            Parent = parent;
        }
        public VersionPath[] GetVersionDirectories()
        {
            return Directory.GetDirectories().Select(x => new VersionPath(x, this)).ToArray();
        }
        public VersionPath GetVersionDirectory(string name)
        {
            return new VersionPath(SubDirectory(name), this);
        }
    }
    public class VersionPath : DirectoryPath
    {
        public WorldDataPath Parent;
        //TODO : Add eura.txt server.jar server.properties ...
        public VersionLogsPath Logs;
        internal VersionPath(DirectoryInfo directory, WorldDataPath parent) : base(directory)
        {
            Parent = parent;
            Logs = new VersionLogsPath(SubDirectory("logs"), this);
        }
        public WorldPath[] GetWorldDirectories()
        {
            return Directory.GetDirectories().Select(x => new WorldPath(x, this)).ToArray();
        }
        public WorldPath GetWorldDirectory(string name)
        {
            return new WorldPath(SubDirectory(name), this);
        }
    }

    public class VersionLogsPath : DirectoryPath
    {
        public VersionPath Parent;
        internal VersionLogsPath(DirectoryInfo directory, VersionPath parent) : base(directory)
        {
            Parent = parent;
        }
    }

    public class WorldPath : DirectoryPath
    {
        public VersionPath Parent;
        public ServerPropertiesPath ServerProperties;
        public WorldWorldPath World;
        public WorldNetherPath Nether;
        public WorldEndPath End;
        internal WorldPath(DirectoryInfo directory, VersionPath parent) : base(directory)
        {
            Parent = parent;
            ServerProperties = new ServerPropertiesPath(SubFile("server.properties"), this);
            World = new WorldWorldPath(SubDirectory("world"), this);
            Nether = new WorldNetherPath(SubDirectory("world_nether"), this);
            End = new WorldEndPath(SubDirectory("world_end"), this);
        }
        public void MoveTo(WorldPath destination)
        {
            Directory.MoveTo(destination.FullName);
            Create();
        }
    }

    public class ServerPropertiesPath : FilePath
    {
        public WorldPath Parent;
        internal ServerPropertiesPath(FileInfo file, WorldPath parent) : base(file)
        {
            Parent = parent;
        }
    }

    public class WorldWorldPath : DirectoryPath
    {
        public WorldPath Parent;
        public DatapacksPath Datapccks;

        internal WorldWorldPath(DirectoryInfo directory, WorldPath parent) : base(directory)
        {
            Parent = parent;
            Datapccks = new DatapacksPath(SubDirectory("datapccks"), this);
        }
    }
    public class WorldNetherPath : DirectoryPath
    {
        public WorldPath Parent;
        internal WorldNetherPath(DirectoryInfo directory, WorldPath parent) : base(directory)
        {
            Parent = parent;
        }
    }
    public class WorldEndPath : DirectoryPath
    {
        public WorldPath Parent;
        internal WorldEndPath(DirectoryInfo directory, WorldPath parent) : base(directory)
        {
            Parent = parent;
        }
    }
    public class DatapacksPath : DirectoryPath
    {
        public WorldWorldPath Parent;
        internal DatapacksPath(DirectoryInfo directory, WorldWorldPath parent) : base(directory)
        {
            Parent = parent;
        }
        public DatapackPath[] GetDatapackDirectories()
        {
            return Directory.GetDirectories().Select(x => new DatapackPath(x, this)).ToArray();
        }
        public DatapackPath GetDatapackDirectory(string name)
        {
            return new DatapackPath(SubDirectory(name), this);
        }
    }
    public class DatapackPath : DirectoryPath
    {
        public DatapacksPath Parent;
        public DatapackDataPath Data;
        public DatapackMcmetaPath Mcmeta;
        internal DatapackPath(DirectoryInfo directory, DatapacksPath parent) : base(directory)
        {
            Parent = parent;
            Data = new DatapackDataPath(SubDirectory("datapacks"), this);
            Mcmeta = new DatapackMcmetaPath(SubFile("pack.mcmeta"), this);
        }
    }
    public class DatapackDataPath : DirectoryPath
    {
        public DatapackPath Parent;
        internal DatapackDataPath(DirectoryInfo directory, DatapackPath parent) : base(directory)
        {
            Parent = parent;
        }
    }
    public class DatapackMcmetaPath : FilePath
    {
        public DatapackPath Parent;
        internal DatapackMcmetaPath(FileInfo file, DatapackPath parent) : base(file)
        {
            Parent = parent;
        }
    }
}
