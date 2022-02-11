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
        protected void _MoveTo(DirectoryInfo destination)
        {
            Directory.MoveTo(destination.FullName);
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
        protected void _MoveTo(FileInfo destination)
        {
            File.MoveTo(destination.FullName);
        }
        public void Delete(bool deletedOk = false)
        {
            if (deletedOk && !Exists)
                return;
            File.Delete();
        }
    }

    public class AnyFile : FilePath
    {
        public DirectoryPath Parent;
        internal AnyFile(FileInfo file, DirectoryPath parent) : base(file)
        {
            Parent = parent;
        }
        public void MoveTo(FileInfo destination)
        {
            _MoveTo(destination);
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

    public abstract class WorldSubPath : DirectoryPath
    {
        public AnyFile LevelDat;
        public AnyFile LevelDatOld;
        public AnyFile SessionLock;
        public AnyFile UidDat;
        public WorldPath Parent;
        internal WorldSubPath(DirectoryInfo directory, WorldPath parent) : base(directory)
        {
            Parent = parent;
            LevelDat = new AnyFile(SubFile("level.dat"), this);
            LevelDatOld = new AnyFile(SubFile("level.dat_old"), this);
            SessionLock = new AnyFile(SubFile("session.lock"), this);
            UidDat = new AnyFile(SubFile("uid.dat"), this);
        }
    }

    public class WorldWorldPath : WorldSubPath
    {
        public WorldDIMPath DIM1;
        public WorldDIMPath DIM_1;
        public DatapacksPath Datapccks;

        internal WorldWorldPath(DirectoryInfo directory, WorldPath parent) : base(directory,parent)
        {
            DIM1 = new WorldDIMPath(SubDirectory("DIM1"), this);
            DIM_1 = new WorldDIMPath(SubDirectory("DIM-1"), this);
            Datapccks = new DatapacksPath(SubDirectory("datapacks"), this);
        }
    }

    public class WorldNetherPath : WorldSubPath
    {
        public WorldDIMPath DIM_1;
        internal WorldNetherPath(DirectoryInfo directory, WorldPath parent) : base(directory,parent)
        {
            DIM_1 = new WorldDIMPath(SubDirectory("DIM_1"), this);
        }
    }

    public class WorldEndPath : WorldSubPath
    {
        public WorldDIMPath DIM1;
        internal WorldEndPath(DirectoryInfo directory, WorldPath parent) : base(directory, parent)
        {
            DIM1 = new WorldDIMPath(SubDirectory("DIM1"), this);
        }
    }

    public class WorldDIMPath : DirectoryPath
    {
        public WorldSubPath Parent;

        internal WorldDIMPath(DirectoryInfo directory, WorldSubPath parent) : base(directory)
        {
            Parent = parent;
        }

        public void MoveTo(WorldDIMPath destination)
        {
            _MoveTo(destination.Directory);
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
            Data = new DatapackDataPath(SubDirectory("data"), this);
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
