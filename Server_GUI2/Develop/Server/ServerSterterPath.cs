using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;

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

        public BytesFile<T> SubBytesFile<T>(string name) where T : DirectoryPath
        {
            return new BytesFile<T>(SubFile(name), (T)this);
        }

        public TextFile<T> SubTextFile<T>(string name) where T : DirectoryPath
        {
            return new TextFile<T>(SubFile(name),(T)this);
        }

        public JsonFile<T,S> SubJsonFile<T, S>(string name) where T : DirectoryPath
        {
            return new JsonFile<T, S>(SubFile(name),(T)this);
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
        public void Delete(bool deletedOk = false,bool force = false)
        {
            if (deletedOk && !Exists)
                return;
            if (force)
                RemoveReadonlyAttribute(Directory);
            Directory.Delete(true);
        }
        protected void _MoveTo(DirectoryInfo destination)
        {
            Directory.MoveTo(destination.FullName);
        }

        private static void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
                if ((fi.Attributes & FileAttributes.ReadOnly) ==
                    FileAttributes.ReadOnly)
                    fi.Attributes = FileAttributes.Normal;
            //サブフォルダの属性を再帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                RemoveReadonlyAttribute(di);
        }
    }

    public abstract class SubDirectoryPath<T> : DirectoryPath where T : DirectoryPath
    {
        public T Parent;
        protected SubDirectoryPath(DirectoryInfo directory, T parent):base(directory)
        {
            Parent = parent;
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
        protected Either<string, Exception> _ReadAllText()
        {
            try
            {
                var stream = File.OpenRead();
                var result = new StreamReader(stream).ReadToEnd();
                stream.Close();
                return new Success<string, Exception>(result);
            }
            catch (Exception e)
            {
                return new Failure<string, Exception>(e);
            }
        }
        protected Either<EitherVoid, Exception> _WriteAllText(string content)
        {
            try
            {
                System.IO.File.WriteAllText(FullName, content);
                return new Success<EitherVoid, Exception>(EitherVoid.Instance);
            }
            catch (Exception e)
            {
                return new Failure<EitherVoid, Exception>(e);
            }
        }
        protected void _MoveTo(FileInfo destination,bool force = false)
        {
            if (force && destination.Exists)
                destination.Delete();
            File.MoveTo(destination.FullName);
        }
        public void Delete(bool deletedOk = false,bool force = false)
        {
            if (deletedOk && !Exists)
                return;
            if (force)
                RemoveReadonlyAttribute(File);
            File.Delete();
        }

        private static void RemoveReadonlyAttribute(FileInfo fileInfo)
        {
            //属性を変更
            if ((fileInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                fileInfo.Attributes = FileAttributes.Normal;
        }
    }

    public class BytesFile<T> : FilePath where T : DirectoryPath
    {
        public T Parent;
        internal BytesFile(FileInfo file, T parent) : base(file)
        {
            Parent = parent;
        }
        public void MoveTo(FileInfo destination, bool force = false)
        {
            _MoveTo(destination, force);
        }
    }

    public class TextFile<T> : FilePath where T : DirectoryPath
    {
        public T Parent;
        internal TextFile(FileInfo file, T parent) : base(file)
        {
            Parent = parent;
        }
        public void MoveTo(FileInfo destination, bool force = false)
        {
            _MoveTo(destination, force);
        }

        public Either<string, Exception> ReadAllText()
        {
            return _ReadAllText();
        }

        public Either<EitherVoid, Exception> WriteAllText(string content)
        {
            return _WriteAllText(content);
        }
    }

    public class JsonFile<T,S> : FilePath where T : DirectoryPath
    {
        public T Parent;
        internal JsonFile(FileInfo file, T parent) : base(file)
        {
            Parent = parent;
        }
        public void MoveTo(FileInfo destination, bool force = false)
        {
            _MoveTo(destination, force);
        }

        public Either<S, Exception> ReadJson()
        {
            try
            {
                return _ReadAllText().SuccessFunc(x => JsonConvert.DeserializeObject<S>(x));
            }
            catch (Exception e)
            {
                return new Failure<S, Exception>(e);
            }
        }

        public void WriteJson(S content, bool indented = false, bool ignoreDefault = false )
        {
            var format = indented ? Formatting.Indented : Formatting.None;
            var str = ignoreDefault ?
                JsonConvert.SerializeObject(content, format, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }) :
                JsonConvert.SerializeObject(content, format);
            _WriteAllText(str);
        }
    }

    /// <summary>
    /// ServerGuiのカレントディレクトリ
    /// </summary>
    public class ServerGuiPath : DirectoryPath
    {
        public static ServerGuiPath Instance = new ServerGuiPath(new DirectoryInfo(SetUp.CurrentDirectory));

        public WorldDataPath WorldData;
        public LogsPath Logs;
        public GitStatePath GitState;
        public JsonFile<ServerGuiPath, List<RemoteLinkJson>> RemotesJson;
        public JsonFile<ServerGuiPath, VanillaVersonsJson> ManifestJson;
        public JsonFile<ServerGuiPath, List<string>> SpigotVersionJson;
        public JsonFile<ServerGuiPath, StoragesJson> StoragesJson;
        public JsonFile<ServerGuiPath, UserSettingsJson> InfoJson;

        public DirectoryInfo TempDirectory;
        private ServerGuiPath(DirectoryInfo directory) : base(directory)
        {
            WorldData = new WorldDataPath(SubDirectory("World_Data"),this);
            Logs = new LogsPath(SubDirectory("log"),this);
            RemotesJson = new JsonFile<ServerGuiPath, List<RemoteLinkJson>>(SubFile("remotes.json"), this);
            GitState = new GitStatePath(SubDirectory("git_state"), this);
            ManifestJson = new JsonFile<ServerGuiPath, VanillaVersonsJson>(SubFile("version_manifest_v2.json"), this);
            SpigotVersionJson = new JsonFile<ServerGuiPath,List<string>>(SubFile("spigot_versions.json"), this);
            StoragesJson = new JsonFile<ServerGuiPath,StoragesJson>(SubFile("storages.json"), this);
            InfoJson = new JsonFile<ServerGuiPath,UserSettingsJson>(SubFile("info.json"), this);
            TempDirectory = SubDirectory("temp");
        }
    }

    public class LogsPath : SubDirectoryPath<ServerGuiPath>
    {
        public TextFile<LogsPath> ServerStarterLog;
        public TextFile<LogsPath> BuildToolsLog;
        internal LogsPath(DirectoryInfo directory, ServerGuiPath parent) : base(directory, parent)
        {
            ServerStarterLog = new TextFile<LogsPath>(SubFile("Server_Starter.log"), this);
            BuildToolsLog = new TextFile<LogsPath>(SubFile("BuildTools.log.txt"), this);
        }
    }


    public class GitStatePath : SubDirectoryPath<ServerGuiPath>
    {
        public JsonFile<GitStatePath, Dictionary<string, WorldState>> WorldStateJson;
        internal GitStatePath(DirectoryInfo directory, ServerGuiPath parent) : base(directory,parent)
        {
            WorldStateJson = new JsonFile<GitStatePath, Dictionary<string, WorldState>>(SubFile("worldstate.json"), this);
        }
    }

    public class WorldDataPath : SubDirectoryPath<ServerGuiPath>
    {
        internal WorldDataPath(DirectoryInfo directory, ServerGuiPath parent) : base(directory,parent){ }

        public VersionPath[] GetVersionDirectories()
        {
            return Directory.GetDirectories().Select(x => new VersionPath(x, this)).ToArray();
        }
        public VersionPath GetVersionDirectory(string name)
        {
            return new VersionPath(SubDirectory(name), this);
        }
    }

    public class VersionPath : SubDirectoryPath<WorldDataPath>
    {
        public VersionLogsPath Logs;

        public TextFile<VersionPath> ServerProperties;
        public JsonFile<VersionPath,List<OpsRecord>> Ops;
        public JsonFile<VersionPath,List<Player>> WhiteList;
        public JsonFile<VersionPath, List<BannedPlayerRecord>> BannedPlayers;
        public JsonFile<VersionPath, List<BannedIpRecord>> BannedIps;

        public TextFile<VersionPath> Eula;


        internal VersionPath(DirectoryInfo directory, WorldDataPath parent) : base(directory,parent)
        {
            Logs = new VersionLogsPath(SubDirectory("logs"), this);
            ServerProperties = new TextFile<VersionPath>(SubFile("server.properties"), this);
            Ops = new JsonFile<VersionPath, List<OpsRecord>>(SubFile("ops.json"), this);
            WhiteList = new JsonFile<VersionPath, List<Player>>(SubFile("whitelist.json"), this);
            BannedPlayers = new JsonFile<VersionPath, List<BannedPlayerRecord>>(SubFile("banned-players.json"), this);
            BannedIps = new JsonFile<VersionPath, List<BannedIpRecord>>(SubFile("banned-ips.json"), this);
            Eula = new TextFile<VersionPath>(SubFile("eula.txt"), this);
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

    public class VersionLogsPath : SubDirectoryPath<VersionPath>
    {
        internal VersionLogsPath(DirectoryInfo directory, VersionPath parent) : base(directory,parent){ }
    }

    public class WorldPath : SubDirectoryPath<VersionPath>
    {
        public TextFile<WorldPath> ServerProperties;
        public JsonFile<WorldPath, List<OpsRecord>> Ops;
        public JsonFile<WorldPath, List<Player>> WhiteList;
        public JsonFile<WorldPath, List<BannedPlayerRecord>> BannedPlayers;
        public JsonFile<WorldPath, List<BannedIpRecord>> BannedIps;

        public WorldWorldPath World;
        public WorldNetherPath Nether;
        public WorldEndPath End;
        internal WorldPath(DirectoryInfo directory, VersionPath parent) : base(directory,parent)
        {
            Parent = parent;

            ServerProperties = new TextFile<WorldPath>(SubFile("server.properties"), this);
            Ops = new JsonFile<WorldPath, List<OpsRecord>>(SubFile("ops.json"), this);
            WhiteList = new JsonFile<WorldPath, List<Player>>(SubFile("whitelist.json"), this);
            BannedPlayers = new JsonFile<WorldPath, List<BannedPlayerRecord>>(SubFile("banned-players.json"), this);
            BannedIps = new JsonFile<WorldPath, List<BannedIpRecord>>(SubFile("banned-ips.json"), this);

            World = new WorldWorldPath(SubDirectory("world"), this);
            Nether = new WorldNetherPath(SubDirectory("world_nether"), this);
            End = new WorldEndPath(SubDirectory("world_the_end"), this);
        }
        public void MoveTo(WorldPath destination)
        {
            Directory.MoveTo(destination.FullName);
            Create();
        }
    }

    public abstract class WorldSubPath : SubDirectoryPath<WorldPath>
    {
        public TextFile<WorldSubPath> LevelDat;
        public TextFile<WorldSubPath> LevelDatOld;
        public TextFile<WorldSubPath> SessionLock;
        public TextFile<WorldSubPath> UidDat;
        internal WorldSubPath(DirectoryInfo directory, WorldPath parent) : base(directory, parent)
        {
            Parent = parent;
            LevelDat = new TextFile<WorldSubPath>(SubFile("level.dat"), this);
            LevelDatOld = new TextFile<WorldSubPath>(SubFile("level.dat_old"), this);
            SessionLock = new TextFile<WorldSubPath>(SubFile("session.lock"), this);
            UidDat = new TextFile<WorldSubPath>(SubFile("uid.dat"), this);
        }
    }

    public class WorldWorldPath : WorldSubPath
    {
        public WorldDIMPath DIM1;
        public WorldDIMPath DIM_1;
        public DatapacksPath Datapccks;
        public PluginsPath Plugins;

        internal WorldWorldPath(DirectoryInfo directory, WorldPath parent) : base(directory,parent)
        {
            DIM1 = new WorldDIMPath(SubDirectory("DIM1"), this);
            DIM_1 = new WorldDIMPath(SubDirectory("DIM-1"), this);
            Datapccks = new DatapacksPath(SubDirectory("datapacks"), this);
            Plugins = new PluginsPath(SubDirectory("plugins"), this);
        }
    }

    public class WorldNetherPath : WorldSubPath
    {
        public WorldDIMPath DIM_1;
        internal WorldNetherPath(DirectoryInfo directory, WorldPath parent) : base(directory,parent)
        {
            DIM_1 = new WorldDIMPath(SubDirectory("DIM-1"), this);
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

    public class WorldDIMPath : SubDirectoryPath<WorldSubPath>
    {
        internal WorldDIMPath(DirectoryInfo directory, WorldSubPath parent) : base(directory, parent){ }

        public void MoveTo(WorldDIMPath destination)
        {
            _MoveTo(destination.Directory);
        }
    }

    public class DatapacksPath : SubDirectoryPath<WorldWorldPath>
    {
        internal DatapacksPath(DirectoryInfo directory, WorldWorldPath parent) : base(directory, parent){ }
        public DatapackPath[] GetDatapackDirectories()
        {
            return Directory.GetDirectories().Select(x => new DatapackPath(x, this)).ToArray();
        }
        public DatapackPath GetDatapackDirectory(string name)
        {
            return new DatapackPath(SubDirectory(name), this);
        }
    }

    public class DatapackPath : SubDirectoryPath<DatapacksPath>
    {
        public DatapackDataPath Data;
        public TextFile<DatapackPath> Mcmeta;
        internal DatapackPath(DirectoryInfo directory, DatapacksPath parent) : base(directory, parent)
        {
            Parent = parent;
            Data = new DatapackDataPath(SubDirectory("data"), this);
            Mcmeta = new TextFile<DatapackPath>(SubFile("pack.mcmeta"), this);
        }
    }

    public class PluginsPath : SubDirectoryPath<WorldWorldPath>
    {
        internal PluginsPath(DirectoryInfo directory, WorldWorldPath parent) : base(directory, parent) { }
        public BytesFile<PluginsPath>[] GetPluginDirectories()
        {
            return Directory.GetFiles().Select(x => new BytesFile<PluginsPath>(x,this)).ToArray();
        }
        public BytesFile<PluginsPath> GetPluginDirectory(string name)
        {
            return new BytesFile<PluginsPath>(SubFile(name), this);
        }
    }

    public class DatapackDataPath : SubDirectoryPath<DatapackPath>
    {
        internal DatapackDataPath(DirectoryInfo directory, DatapackPath parent) : base(directory, parent){ }
    }
}
