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

        public static List<Version> allVersions = new List<Version>();

        public Version activeVer = null;


        public static VersionFactory GetInstance()
        {
            return _instance;
        }

        public VersionFactory()
        {
            //TODO: initialization
        }

        //public void LoadFromCurrentDirectory() { }

        /// <summary>
        /// マイクラのバージョン一覧を取得
        /// </summary>
        /// <returns>一覧のリストを返す。取得に失敗した場合はnullを返す</returns>
        public void LoadAllVersions()
        {
            logger.Info($"Import new Version List");

            string url = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
            string errorMessage =
                    "Minecraftのバージョン一覧の取得に失敗しました。\n" +
                    "新しいバージョンのサーバーの導入はできません";
            dynamic root = ReadContents.ReadJson(url, errorMessage);
            if (root == null)
            {
                // TODO: version_manifest_v2.jsonを保持しておき、インターネットから読み込めないときにはこれを利用する
                // これもないときにはServerStarterは起動できないこととする（そもそもインターネットに接続していない状態でサーバーを立てたいか？）
                return;
            }

            List<string> spigotList = GetSpigotVersions();

            string latestRelease = root.latest.release;
            string latestSnapShot = root.latest.snapshot;

            //List<string> list_versions = new List<string>() { $"【latest_release】 {root.latest.release}", $"【latest_snapshot】 {root.latest.snapshot}" };

            string id = "";
            int i = 0;

            //ここでrelease、snapshotのみかすべてまとめて取得するのかを決める
            // バージョン1.2.5以前はマルチサーバーが存在しない
            while (id != "1.2.5")
            {
                id = root.versions[i].id;
                string downloadURL = root.versions[i].url;
                string type = root.versions[i].type;
                bool hasSpigot = (spigotList != null) && spigotList.Contains(id);

                allVersions.Add(new Version(id, downloadURL, hasSpigot, type == "release"));

                i++;
            }

            if (latestRelease == latestSnapShot)
            {
                // 最新バージョンがreleaseの際にはsnapshotも同じため、特例としてリストの先頭に挿入する処理を行う
                Version _snapshot = allVersions[0];
                _snapshot.isRelease = false;
                allVersions.Insert(0, _snapshot);
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
            }

            return vers;
        }

        ///// <summary>
        ///// Spigotのバージョン一覧を取得
        ///// </summary>
        ///// <returns>一覧のリストを返す。取得に失敗した場合はnullを返す</returns>
        //public List<Version> LoadAllSpigotVersions()
        //{
        //    string url = "https://hub.spigotmc.org/versions/";
        //    string message =
        //        "Spigotのバージョン一覧の取得に失敗しました。\n" +
        //        "新しいバージョンのサーバーの導入はできません";
        //    IHtmlDocument doc = ReadContents.ReadHtml(url, message);
        //    if (doc == null)
        //    {
        //        return null;
        //    }

        //    var table = doc.QuerySelectorAll("body > pre > a");

        //    SortedList<double, string> _vers = new SortedList<double, string>();
        //    foreach (var htmlDatas in table)
        //    {
        //        string ver = htmlDatas.InnerHtml;
        //        if (ver.Substring(0, 2) != "1.")
        //            continue;

        //        // 1. と .jsonを除いた形
        //        string ver_tmp = ver.Substring(2).Replace(".json", "");

        //        // preなどの文字を抽出
        //        string pat = @"^\d+-(.+)\d$";
        //        Regex r = new Regex(pat);
        //        Match m = r.Match(ver_tmp);
        //        string suffix = m.Groups[1].Value;

        //        // rcとpreを区別する場合は、-0.1をrcに、-0.2をpreに割り当てれば良い
        //        double down_num;
        //        switch (suffix)
        //        {
        //            case "pre":
        //                down_num = 0.2;
        //                break;
        //            case "rc":
        //                down_num = 0.1;
        //                break;
        //            default:
        //                down_num = 0.3;
        //                break;
        //        }

        //        double pre_num = ver.Contains($"-{suffix}") ? double.Parse(ver_tmp.Substring(ver_tmp.Length - 1)) : 0;
        //        // version名を小数に変換 (-preに関しては小数第２位にその数字を入れ、ひとつ前のバージョンとするために0.1引く)
        //        double ver_num = ver.Contains($"-{suffix}") ? double.Parse(ver_tmp.Substring(0, ver_tmp.IndexOf($"-{suffix}"))) + pre_num * 0.01 - down_num : double.Parse(ver_tmp);

        //        _vers.Add(ver_num, "Spigot " + ver.Replace(".json", ""));

        //        // 1.9.jsonが対応バージョン一覧の最後に記載されているため
        //        if (ver == "1.9.json")
        //            break;
        //    }

        //    List<string> vers = new List<string>(_vers.Values);
        //    // 最新バージョンが一番上にくるようにする
        //    vers.Reverse();

        //    return null;
        //    //return vers;
        //}

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
