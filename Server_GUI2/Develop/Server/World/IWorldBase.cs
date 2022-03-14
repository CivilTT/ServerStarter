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
        DatapackCollection Datapacks { get; set; }
        PluginCollection Plugins { get; set; }
        // TODO: Propertyにgetしか持たせない理由とは？
        // WorldSettingでプロパティを設定し、設定の終わったインスタンスをワールドのこれに直接割り当てられない
        // こんな場合にどのようにPropertyを反映させることを想定している？
        // 参照渡しをそのまま使ってGUIに反映させるとユーザーが変更をキャンセルした場合の処理が面倒。
        // だから、GUIを表示するときに新しくインスタンスを生成 --> それらを編集 --> 保存するときに生成・編集したインスタンス群を元のWorldのインスタンス群に割り当てる
        // ような形に現状の実装はなってる
        // DatapackやPluginなんかも同じようにどうすれば良いか困ってる
        ServerProperty Property { get; set; }
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
        public ServerProperty ServerProperty { get; set; } = null;

        public WorldState(){ }
        public WorldState(string name,string type,string version,bool isUsing,List<string> datapacks, List<string> plugins, ServerProperty serverProperty)
        {
            Name = name;
            Type = type;
            Version = version;
            Using = isUsing;
            Datapacks = datapacks;
            Plugins = plugins;
            ServerProperty = serverProperty;
        }
    }
}
