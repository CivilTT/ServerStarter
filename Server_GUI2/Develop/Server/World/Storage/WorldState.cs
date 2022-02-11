using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Server_GUI2.Develop.Server.World
{
    /// <summary>
    /// リモートリポジトリ上にあるブランチを管理するためのjsonデータ
    /// </summary>
    public class WorldState
    {
        [JsonIgnore]
        public bool Exist { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("using")]
        [DefaultValue(false)]
        public bool Using { get; set; }
        [JsonProperty("datapacks")]
        public List<string> Datapacks { get; set; } = new List<string>();
        [JsonProperty("properties")]
        [DefaultValue(null)]
        public ServerProperty ServerProperty { get; set; } = null;
    }
}
