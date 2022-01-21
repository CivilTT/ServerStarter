﻿using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using log4net;
using Server_GUI2.Develop.Util;
using System;
using System.Collections.Generic;
using System.IO;
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

        public List<Version> allVersions = new List<Version>();

        public static Version activeVer = null;


        public static VersionFactory GetInstance()
        {
            return _instance;
        }

        private VersionFactory()
        {
            LoadAllVersions();
            // LoadImported();
        }

        public List<Version> ExistingVersions
        {
            get
            {
                return allVersions.Where(version => version.Exists).ToList();
            }
        }
        public List<Version> SpigptVersions
        {
            get
            {
                return allVersions.Where(version => version is SpigotVersion).ToList();
            }
        }
        public List<Version> VanillaAllVersions
        {
            get
            {
                return allVersions.Where(version => version is VanillaVersion).ToList();
            }
        }
        public List<Version> VanillaReleaseVersions
        {
            get
            {
                return allVersions.Where(version => version is VanillaVersion &&  (version as VanillaVersion).IsRelease ).ToList();
            }
        }

        /// <summary>
        /// マイクラのバージョン一覧を取得
        /// </summary>
        public void LoadAllVersions()
        {
            logger.Info("Import new Version List");

            string url = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
            string errorMessage =
                    "Minecraftのバージョン一覧の取得に失敗しました。\n" +
                    "新しいバージョンのサーバーの導入はできません";
            dynamic root = ReadContents.ReadJson<VanillaVersonsJson>(url, errorMessage);
            if (root == null)
            {
                logger.Info("Missing Versions List");
                // TODO: version_manifest_v2.jsonを保持しておき、インターネットから読み込めないときにはこれを利用する
                // これもないときにはServerStarterは起動できないこととする（そもそもインターネットに接続していない状態でサーバーを立てたいか？）
                return;
            }

            List<string> spigotList = GetSpigotVersions();
            VanillaVersonsJson vanillaversions = root;

            string latestRelease = vanillaversions.latest.release;
            string latestSnapShot = vanillaversions.latest.snapshot;

            string id = "";
            int i = 0;

            // バージョン1.2.5以前はマルチサーバーが存在しない
            while (id != "1.2.5")
            {
                VanillaVersonJson version = vanillaversions.versions[i];
                id = version.id;
                string downloadURL = version.url;
                string type = version.type;
                bool hasSpigot = (spigotList?.Contains(id)) ?? false;
                bool isRelease = type == "release";
                // bool isLatest = id == latestRelease || id == latestSnapShot;

                allVersions.Add(new VanillaVersion(id, downloadURL, isRelease, hasSpigot));

                i++;
            }

            // 最新バージョンがreleaseの際にはsnapshotも同じため、特例としてリストの先頭に挿入する処理を行う
            // TODO: 謎処理 hasSpigotはtrueか？
            if (latestRelease == latestSnapShot)
            {
                VanillaVersonJson version = vanillaversions.versions[0];
                id = version.id;
                string downloadURL = version.url;
                Version _snapshot = new VanillaVersion(id, downloadURL, false, true);
                allVersions.Insert(1, _snapshot);
            }
        }

        private List<string> GetSpigotVersions()
        {
            string url = "https://hub.spigotmc.org/versions/";
            string message =
                "Spigotのバージョン一覧の取得に失敗しました。\n" +
                "新しいバージョンのサーバーの導入はできません";
            IHtmlDocument doc = ReadContents.ReadHtml(url, message);
            if (doc == null)
            {
                return null;
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
        // private List<Version> CheckImported(List<Version> versions)
        // {
        // logger.Info("Getting local Versions");
        // 
        //     foreach (string dirName in Directory.GetDirectories(SetUp.DataPath, "*", SearchOption.TopDirectoryOnly))
        //     {
        // string verName = Path.GetFileName(dirName);
        // 
        // bool isSpigot = verName.Contains("Spigot");
        // verName = isSpigot ? verName.Substring(7) : verName;
        // 
        // newしないと参照渡しになってしまうため
        // Version _ver = versions.Find(x => x.Name == verName);
        // 
        //         if (_ver != null)
        //         {
        // Version ver = new Version(_ver.Name, _ver.downloadURL, _ver.hasSpigot, _ver.isRelease, _ver.isLatest, !isSpigot);
        // installedVersions.Add(ver);
        //     }
        // }
    }


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

    public void Remove(Version version) { }

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
