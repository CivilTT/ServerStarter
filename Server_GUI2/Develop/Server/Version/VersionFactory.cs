using AngleSharp.Html.Dom;
using log4net;
using Server_GUI2.Develop.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System;
using Newtonsoft.Json;
using Server_GUI2.Develop.Server;
using Server_GUI2.Util;

namespace Server_GUI2
{
    public class VersionFactory
    {
        private static Dictionary<string, int> VersionIndex = new Dictionary<string, int>();

        /// <summary>
        /// Vanillaの新しいサーバーがダウンロード可能
        /// </summary>
        public bool VanillaImportable { get; private set; }

        /// <summary>
        /// Spigotの新しいサーバーがダウンロード可能
        /// </summary>
        public bool SpigotImportable { get; private set; }

        public static int GetIndex(string name)
        {
            return VersionIndex[name];
        }

        public readonly static VersionFactory Instance  = new VersionFactory();

        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // vanilla only / release only / spigot only はViewModelのほうでリアルタイムフィルタ使って実装 https://blog.okazuki.jp/entry/2013/12/07/000341
        public ObservableCollection<Version> Versions { get; private set; }

        public Version SelectedVersion { get; set; }

        private readonly Dictionary<string, Version> VersionMap = new Dictionary<string, Version>();

        private VersionFactory()
        {
            var versions = new List<Version>();

            // spigotのサーバーインスタンスを追加
            List<string> spigotList = GetSpigotVersionList(ref versions);

            // vanillaのサーバーインスタンスを追加
            LoadVanillaVersions(ref versions, spigotList);

            // spigotのバージョンのインデックスを互換なvanillaのバージョンのものと同じにする
            AddSpigotOnlyVersionToVersionIndex(spigotList);

            // サーバーをソート
            versions.Sort();
            versions.Reverse();
            Versions = new ObservableCollection<Version>(versions);
            Versions.WriteLine();
        }

        public Version GetVersionFromName(string name)
        {
            return VersionMap[name];
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

                // リモートにアクセス可能orすでに存在する
                bool available = VanillaImportable || ServerGuiPath.Instance.WorldData.GetVersionDirectory(id).Exists;

                var versionInstance = new VanillaVersion(id,downloadURL, isRelease, hasSpigot, isLatest, available);
                versions.Add(versionInstance);
                VersionMap[id] = versionInstance;
                VersionIndex[id] = i;
                i++;
            }
        }

        /// <summary>
        /// SpigotとVanilaでバージョンの表記が違う場合に書き足していく
        /// </summary>
        private void AddSpigotOnlyVersionToVersionIndex(List<string> spigotList)
        {
            foreach (var i in spigotList)
            {
                // Spigot_{withoutPrefix}
                var withoutPrefix = i.Substring(7);
                var name = withoutPrefix == "1.14-pre5" ? "1.14 Pre-Release 5" : withoutPrefix;
                VersionIndex["Spigot_"+ withoutPrefix] = VersionIndex[name];
            }
        }

        /// <summary>
        /// version_manifest_v2.jsonを取得<br>
        /// isOnlineはリンク先のversion_manifest_v2.jsonを取得できたときtrueになる
        /// </summary>
        // version_manifest_v2.jsonを保持し、インターネットから読み込めないときにはこれを利用する
        private VanillaVersonsJson GetVanillaVersionJson()
        {
            string url = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";

            VanillaVersonsJson versions = ReadContents.ReadJson<VanillaVersonsJson>(url);

            VanillaImportable = versions != null;

            if (VanillaImportable)
            {
                // jsonをローカルに保存
                ServerGuiPath.Instance.ManifestJson.WriteJson(versions);
            }
            else
            {
                // jsonをローカルから読み込み
                versions = ServerGuiPath.Instance.ManifestJson.ReadJson().SuccessOrDefault(new VanillaVersonsJson());
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

            SpigotImportable = doc != null;

            var vers = new List<string>();

            if (SpigotImportable)
            {
                var table = doc.QuerySelectorAll("body > pre > a");
                foreach (var htmlDatas in table)
                {
                    string verName = htmlDatas.InnerHtml;

                    if (verName.Substring(0, 2) != "1.")
                        continue;

                    // Spigot_1.x.x
                    verName = "Spigot_" + verName.Replace(".json", "");

                    vers.Add(verName);

                    // 1.9.jsonが対応バージョン一覧の最後に記載されているため
                    if (verName == "1.9")
                        break;
                }
                // ローカルに保存
                ServerGuiPath.Instance.SpigotVersionJson.WriteJson(vers);
            }
            else
            {
                // ローカルから読み込み
                vers = ServerGuiPath.Instance.SpigotVersionJson.ReadJson().SuccessOrDefault(new List<string>());
            }

            foreach (var ver in vers)
            {
                // バージョンが利用可能か
                var available = SpigotImportable || ServerGuiPath.Instance.WorldData.GetVersionDirectory(ver).Exists;

                // versionを生成してリストに追加
                var versionInstance = new SpigotVersion(ver, available);
                versions.Add(versionInstance);

                VersionMap[ver] = versionInstance;
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
