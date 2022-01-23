using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using log4net;
using Server_GUI2.Develop.Util;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using MW = ModernWpf;
using Newtonsoft.Json;

namespace Server_GUI2
{
    public class VersionFactory
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        readonly WebClient wc = new WebClient();

        private static VersionFactory _instance = new VersionFactory();

        // TODO: vanilla only/ release only / spigot only はViewModelのほうでリアルタイムフィルタ使って実装 https://blog.okazuki.jp/entry/2013/12/07/000341
        public ObservableCollection<Version> Versions { get; private set; }

        public Version SelectedVersion { get; set; }

        private readonly Dictionary<string, Version> VersionMap = new Dictionary<string, Version>();

        public static VersionFactory GetInstance()
        {
            return _instance;
        }

        private Dictionary<string, int> versionIndexMap = new Dictionary<string, int>();

        private VersionFactory()
        {
            var versions = new List<Version>();

            // spigotのサーバーインスタンスを追加
            List<string> spigotList = GetSpigotVersionList(ref versions);

            // vanillaのサーバーインスタンスを追加
            LoadVanillaVersions(ref versions, spigotList);

            // サーバーをソート
            versions.Sort();
            Versions = new ObservableCollection<Version>(versions);
        }

        public Version GetVersionFromName(string name)
        {
            return VersionMap[name];
        }

        public int GetVersionIndex(Version version)
        {
            return versionIndexMap[version.Name];
        }

        /// <summary>
        /// マイクラのバージョン一覧を取得
        /// </summary>
        public void LoadVanillaVersions(ref List<Version> versions, List<string> spigotList)
        {
            logger.Info("Import new Vanilla Version List");

            // jsonを取得
            VanillaVersonsJson vanillaVersions = GetVanillaVersionJson();

            if (vanillaVersions == null)
            {
                logger.Info("Missing Versions List");
                return;
            }

            string latestRelease = vanillaVersions.latest.release;
            string latestSnapShot = vanillaVersions.latest.snapshot;

            string id = "";
            int i = 0;

            // バージョン1.2.5以前はマルチサーバーが存在しない
            while (id != "1.2.5")
            {
                VanillaVersonJson version = vanillaVersions.versions[i];
                id = version.id;
                string downloadURL = version.url;
                string type = version.type;
                bool hasSpigot = (spigotList?.Contains(id)) ?? false;
                bool isRelease = type == "release";
                bool isLatest = id == latestRelease || id == latestSnapShot;
                var versionInstance = new VanillaVersion(id, downloadURL, isRelease, hasSpigot, isLatest);
                versions.Add(versionInstance);
                versionIndexMap[id] = i;
                i++;
            }

            // 最新バージョンがreleaseの際にはsnapshotも同じため、特例としてリストの先頭に挿入する処理を行う
            //if (latestRelease == latestSnapShot)
            //{
            //VanillaVersonJson version = vanillaversions.versions[0];
            //id = version.id;
            //string downloadURL = version.url;
            //Version _snapshot = new VanillaVersion(id, downloadURL, false, true);
            //allVersions.Insert(1, _snapshot);
            //}
        }

        /// <summary>
        /// version_manifest_v2.jsonを取得
        /// </summary>
        // version_manifest_v2.jsonを保持し、インターネットから読み込めないときにはこれを利用する
        private VanillaVersonsJson GetVanillaVersionJson()
        {
            string url = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
            VanillaVersonsJson versions = ReadContents.ReadJson<VanillaVersonsJson>(url);

            if (versions != null)
            {
                using (var sw = new StreamWriter($@"{Directory.GetCurrentDirectory()}\version_manifest_v2.json", false, Encoding.UTF8))
                {
                    sw.Write(JsonConvert.SerializeObject(versions));
                }
            } else
            {
                using (var reader = new StreamReader($@"{Directory.GetCurrentDirectory()}\version_manifest_v2.json"))
                {
                    // TODO: 保存されたversion_manifest_v2.jsonがないときServerStarterは起動できないこととする（そもそもインターネットに接続していない状態でサーバーを立てたいか？）
                    string errorMessage =
                       "Minecraftのバージョン一覧の取得に失敗しました。\n" +
                       "新しいバージョンのサーバーの導入はできません";
                    versions = ReadContents.ReadlocalJson<VanillaVersonsJson>(reader.ReadToEnd(), errorMessage);
                }
            }
            return versions;
        }

        private List<string> GetSpigotVersionList(ref List<Version> versions)
        {
            logger.Info("Import new Vanilla Version List");

            string url = "https://hub.spigotmc.org/versions/";
            string message =
                "Spigotのバージョン一覧の取得に失敗しました。\n" +
                "新しいバージョンのサーバーの導入はできません";
            IHtmlDocument doc = ReadContents.ReadHtml(url, message);
            if (doc == null)
            {
                return new List<string>();
            }

            var table = doc.QuerySelectorAll("body > pre > a");
            List<string> vers = new List<string>();

            foreach (var htmlDatas in table)
            {
                string ver = htmlDatas.InnerHtml;
                if (ver.Substring(0, 2) != "1.")
                    continue;

                // .jsonを除いた形
                ver = ver.Replace(".json", "");

                vers.Add(ver);

                // versionを生成してリストに追加
                // TODO: SpigotVersionの引数downloadURLの削除
                versions.Add(new SpigotVersion(ver));

                // 1.9.jsonが対応バージョン一覧の最後に記載されているため
                if (ver == "1.9")
                    break;
            }

            return vers;
        }

        /// <summary>
        /// バージョンのリストを引数に取る
        /// バージョンのリストに含まれないが、すでにインストールされているバージョンがあった場合にそのリストに追加(オフライン時等)
        /// manifest.jsonがない場合はそもそも実行しないので必要なし。
        /// </summary>
        //private List<Version> CheckImported(List<Version> versions)
        //{
        //    logger.Info("Getting local Versions");

        //    foreach (string dirName in Directory.GetDirectories(SetUp.DataPath, "*", SearchOption.TopDirectoryOnly))
        //    {
        //        string verName = Path.GetFileName(dirName);

        //        bool isSpigot = verName.Contains("Spigot");
        //        verName = isSpigot ? verName.Substring(7) : verName;

        //        newしないと参照渡しになってしまうため
        //        Version _ver = versions.Find(x => x.Name == verName);

        //        if (_ver != null)
        //        {
        //            Version ver = new Version(_ver.Name, _ver.downloadURL, _ver.hasSpigot, _ver.isRelease, _ver.isLatest, !isSpigot);
        //            installedVersions.Add(ver);
        //        }
        //    }

        /// <summary>
        /// バージョンの新規作成
        /// </summary>
        /// <returns>作成したバージョンの情報</returns>
        //public Version Create(bool isVanila=true)
        //{
        //    if(isVanila)
        //    {
        //        //pass
        //    }
        //    else
        //    {

        //    }
        //}

        /// <summary>
        /// バージョンの削除(紐づけられたディレクトリを削除し、インスタンスは削除しない)
        /// </summary>
        public void Remove(Version version) {
            version.Remove();
        }

        //public string[] getVersionNames() { }
        //Version[] existingVersions
        //Version[] allVanillaVersions
        //Version[] allSpigotVersions
        //Version activeVersion
        //string[] getVersionNames()
    }


    public class VanillaVersonsJson
    {
        public static int LatestJsonVer = 1;

        [JsonProperty("latest")]
        //"latest": {
        //    "release": "1.18.1",
        //    "snapshot": "22w03a"
        //}
        public VanillaLatestVersonJson latest;

        [JsonProperty("versions")]
        public List<VanillaVersonJson> versions;

    }
    public class VanillaVersonJson
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("url")]
        public string url;
        
        [JsonProperty("type")]
        public string type;
    }

    public class VanillaLatestVersonJson
    {
        [JsonProperty("release")]
        public string release;

        [JsonProperty("snapshot")]
        public string snapshot;
    }
}
