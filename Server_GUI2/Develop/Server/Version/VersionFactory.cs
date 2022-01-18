using AngleSharp.Html.Dom;
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

namespace Server_GUI2
{
    public class VersionFactory
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        readonly WebClient wc = new WebClient();

        public static VersionFactory _instance = new VersionFactory();

        public List<Version> allVersions = new List<Version>();

        public List<Version> installedVersions = new List<Version>();

        public static Version activeVer = null;


        public static VersionFactory GetInstance()
        {
            return _instance;
        }

        public VersionFactory()
        {
            LoadAllVersions();
            LoadImported();
        }

        /// <summary>
        /// マイクラのバージョン一覧を取得
        /// </summary>
        /// <returns>一覧のリストを返す。取得に失敗した場合はnullを返す</returns>
        public void LoadAllVersions()
        {
            logger.Info("Import new Version List");

            string url = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
            string errorMessage =
                    "Minecraftのバージョン一覧の取得に失敗しました。\n" +
                    "新しいバージョンのサーバーの導入はできません";
            dynamic root = ReadContents.ReadJson(url, errorMessage);
            if (root == null)
            {
                logger.Info("Missing Versions List");

                // TODO: version_manifest_v2.jsonを保持しておき、インターネットから読み込めないときにはこれを利用する
                // これもないときにはServerStarterは起動できないこととする（そもそもインターネットに接続していない状態でサーバーを立てたいか？）
                return;
            }

            List<string> spigotList = GetSpigotVersions();

            string latestRelease = root.latest.release;
            string latestSnapShot = root.latest.snapshot;

            string id = "";
            int i = 0;

            // バージョン1.2.5以前はマルチサーバーが存在しない
            while (id != "1.2.5")
            {
                id = root.versions[i].id;
                string downloadURL = root.versions[i].url;
                string type = root.versions[i].type;
                bool hasSpigot = (spigotList != null) && spigotList.Contains(id);
                bool isRelease = type == "release";
                bool isLatest = id == latestRelease || id == latestSnapShot;

                allVersions.Add(new Version(id, downloadURL, hasSpigot, isRelease, isLatest));

                i++;
            }

            // 最新バージョンがreleaseの際にはsnapshotも同じため、特例としてリストの先頭に挿入する処理を行う
            if (latestRelease == latestSnapShot)
            {
                id = root.versions[0].id;
                string downloadURL = root.versions[0].url;
                Version _snapshot = new Version(id, downloadURL, false, false, true);
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
        /// すでにインストールされているバージョンの一覧を取得
        /// </summary>
        public void LoadImported()
        {
            logger.Info("Getting local Versions");

            foreach (string dirName in Directory.GetDirectories(SetUp.DataPath, "*", SearchOption.TopDirectoryOnly))
            {
                string verName = Path.GetFileName(dirName);

                bool isSpigot = verName.Contains("Spigot_");
                verName = isSpigot ? verName.Substring(7) : verName;

                // newしないと参照渡しになってしまうため
                Version _ver = allVersions.Find(x => x.Name == verName);
                Version ver = new Version(_ver.Name, _ver.downloadURL, _ver.hasSpigot, _ver.isRelease, _ver.isLatest);

                if (ver != null)
                {
                    ver.isVanila = !isSpigot;
                    installedVersions.Add(ver);
                }
            }
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
}
