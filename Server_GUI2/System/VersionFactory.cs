using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
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

        public static List<Version> existingVers = new List<Version>();
        public static List<Version> allVanilaVers = new List<Version>();
        public static List<Version> allSpigotVers = new List<Version>();

        public Version activeVer = null;

        
        public static VersionFactory GetInstance()
        {
            return _instance;
        }

        private VersionFactory()
        {
            //TODO: initialization
        }

        public void LoadFromCurrentDirectory() { }

        /// <summary>
        /// バニラサーバーのバージョン一覧を取得
        /// </summary>
        /// <returns>一覧のリストを返す。取得に失敗した場合はnullを返す</returns>
        public List<Version> LoadAllVanillaVersions()
        {
            logger.Info($"Import new Version List");
            
            string url = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
            string errorMessage =
                    "Minecraftのバージョン一覧の取得に失敗しました。\n" +
                    "新しいバージョンのサーバーの導入はできません";
            dynamic root = ReadJson(url, errorMessage);
            if(root == null)
            {
                return null;
            }

            Version latestRelease  = new Version(root.latest.release, root.latest., islatest: true);
            Version latestSnapShot = new Version(root.latest.snapshot, isrelease: false, islatest: true);

            // 次の行の初期登録をする
            allVanilaVers.Add(latestRelease);
            allVanilaVers.Add(latestSnapShot);

            //List<string> list_versions = new List<string>() { $"【latest_release】 {root.latest.release}", $"【latest_snapshot】 {root.latest.snapshot}" };
            
            string id = "";
            int i = 0;

            //ここでrelease、snapshotのみかすべてまとめて取得するのかを決める
            // バージョン1.2.5以前はマルチサーバーが存在しない
            while (id != "1.2.5")
            {
                id = root.versions[i].id;
                string type = root.versions[i].type;
                Version ver;

                if (type == "release")
                {
                    ver = new Version(id);
                }
                else
                {
                    ver = new Version(id, isrelease: false);
                }

                allVanilaVers.Add(ver);

                i++;
            }

            return allVanilaVers;
        }

        /// <summary>
        /// Spigotのバージョン一覧を取得
        /// </summary>
        /// <returns>一覧のリストを返す。取得に失敗した場合はnullを返す</returns>
        public List<Version> LoadAllSpigotVersions()
        {
            string url = "https://hub.spigotmc.org/versions/";
            string message = 
                "Spigotのバージョン一覧の取得に失敗しました。\n" +
                "新しいバージョンのサーバーの導入はできません";
            IHtmlDocument doc = ReadHtml(url, message);
            if(doc == null)
            {
                return null;
            }

            var table = doc.QuerySelectorAll("body > pre > a");

            SortedList<double, string> _vers = new SortedList<double, string>();
            foreach (var htmlDatas in table)
            {
                string ver = htmlDatas.InnerHtml;
                if (ver.Substring(0, 2) != "1.")
                    continue;

                // 1. と .jsonを除いた形
                string ver_tmp = ver.Substring(2).Replace(".json", "");

                // preなどの文字を抽出
                string pat = @"^\d+-(.+)\d$";
                Regex r = new Regex(pat);
                Match m = r.Match(ver_tmp);
                string suffix = m.Groups[1].Value;

                // rcとpreを区別する場合は、-0.1をrcに、-0.2をpreに割り当てれば良い
                double down_num;
                switch (suffix)
                {
                    case "pre":
                        down_num = 0.2;
                        break;
                    case "rc":
                        down_num = 0.1;
                        break;
                    default:
                        down_num = 0.3;
                        break;
                }

                double pre_num = ver.Contains($"-{suffix}") ? double.Parse(ver_tmp.Substring(ver_tmp.Length - 1)) : 0;
                // version名を小数に変換 (-preに関しては小数第２位にその数字を入れ、ひとつ前のバージョンとするために0.1引く)
                double ver_num = ver.Contains($"-{suffix}") ? double.Parse(ver_tmp.Substring(0, ver_tmp.IndexOf($"-{suffix}"))) + pre_num * 0.01 - down_num : double.Parse(ver_tmp);

                _vers.Add(ver_num, "Spigot " + ver.Replace(".json", ""));

                // 1.9.jsonが対応バージョン一覧の最後に記載されているため
                if (ver == "1.9.json")
                    break;
            }

            List<string> vers = new List<string>(_vers.Values);
            // 最新バージョンが一番上にくるようにする
            vers.Reverse();

            return vers;
        }

        /// <summary>
        /// バージョンの新規作成
        /// </summary>
        /// <returns>作成したバージョンの情報</returns>
        public Version Create(bool isVanila=true)
        {
            if(isVanila)
            {
                pass
            }
            else
            {

            }
        }
        
        public void Remove(Version version) { }

        public string[] getVersionNames() { }

        private dynamic ReadJson(string url, string errorMessage)
        {
            dynamic root = null;
            try
            {
                string jsonStr = wc.DownloadString(url);
                root = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr);
            }
            catch (Exception ex)
            {
                string message =
                        errorMessage + "\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainWindow.Set_new_Version = false;
            }

            return root;
        }

        private IHtmlDocument ReadHtml(string url, string errorMessage)
        {
            try
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36");
                Stream st = wc.OpenRead(url);

                string html;
                using (var sr = new StreamReader(st))
                {
                    html = sr.ReadToEnd();
                }

                // HTMLParserのインスタンス生成
                var parser = new HtmlParser();
                IHtmlDocument doc = parser.ParseDocument(html);

                return doc;
            }
            catch (Exception ex)
            {
                string message =
                    errorMessage + "\n\n" +
                    $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);

                return null;
            }
        }

        //Version[] existingVersions
        //Version[] allVanillaVersions
        //Version[] allSpigotVersions
        //Version activeVersion
        //string[] getVersionNames()
    }
}
