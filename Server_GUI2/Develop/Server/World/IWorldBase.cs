using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Server_GUI2.Develop.Server.World
{
    public interface IWorldBase
    {
        // TODO: 一度導入したDatapackやPluginは、どこかに保存しておいてシステムからプルダウンか何かで選択できるようにする
        DatapackCollection Datapacks { get; set; }
        PluginCollection Plugins { get; set; }
        ServerSettings Settings { get; set; }
        ServerType? Type { get; }
        string Name { get; }
        Version Version { get; }
        WorldState ExportWorldState();
    }

    /// <summary>
    /// リモートリポジトリ上にあるブランチを管理するためのjsonデータ
    /// </summary>
    public class WorldState
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("using")]
        [DefaultValue(false)]
        public bool Using { get; set; }

        [JsonProperty("datapacks")]
        public List<string> Datapacks { get; set; } = new List<string>();

        [JsonProperty("plugins")]
        public List<string> Plugins { get; set; } = new List<string>();

        [JsonProperty("properties")]
        [DefaultValue(null)]
        public ServerSettings ServerSetting { get; set; } = null;

        public WorldState(){ }
        public WorldState(string name,string type,string version,bool isUsing,List<string> datapacks, List<string> plugins, ServerSettings serverSettings)
        {
            Name = name;
            Type = type;
            Version = version;
            Using = isUsing;
            Datapacks = datapacks;
            Plugins = plugins;
            ServerSetting = serverSettings;
        }
    }
}
