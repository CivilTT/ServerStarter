using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using MW = ModernWpf;

namespace Server_GUI2.Develop.Util
{
    static class ReadContents
    {
        // private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static readonly WebClient wc = new WebClient();

        /// <summary>
        /// インターネット上のJsonを読み込む
        /// </summary>
        /// <returns>The deserialized object from the JSON string.</returns>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static dynamic ReadJson<T>(string url,string errorMessage)
        {
            return ReadJson<T>(url, true, errorMessage);
        }
        public static dynamic ReadJson<T>(string url)
        {
            return ReadJson<T>(url, false, "");
        }
        private  static dynamic ReadJson<T>(string url, bool showErorMessage,  string errorMessage)
        {
            dynamic root = null;
            try
            {
                string jsonStr = wc.DownloadString(url);
                root = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
            }
            catch (Exception ex)
            {
                if (showErorMessage)
                {
                    string message =
                            errorMessage + "\n\n" +
                            $"【エラー要因】\n{ex.Message}";
                    MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            return root;
        }
        
        /// <summary>
        /// インターネット上のJsonを読み込む
        /// </summary>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static dynamic ReadJson(string url, string errorMessage)
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
            }

            return root;
        }

        /// <summary>
        /// ローカルにあるJsonファイルを読み込む
        /// </summary>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static dynamic ReadlocalJson<T>(string path, string errorMessage)
        {
            dynamic root = null;
            try
            {
                string jsonStr = null;

                using (StreamReader sr = new StreamReader(path))
                {
                    jsonStr = sr.ReadToEnd();
                }

                root = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
            }
            catch (Exception ex)
            {
                string message =
                        errorMessage + "\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return root;
        }
        public static dynamic ReadlocalJson(string path, string errorMessage)
        {
            dynamic root = null;
            try
            {
                string jsonStr = null;

                using (StreamReader sr = new StreamReader(path))
                {
                    jsonStr = sr.ReadToEnd();
                }

                root = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr);
            }
            catch (Exception ex)
            {
                string message =
                        errorMessage + "\n\n" +
                        $"【エラー要因】\n{ex.Message}";
                MW.MessageBox.Show(message, "Server Starter", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return root;
        }

        /// <summary>
        /// 以前まで使用していたinfo.txtを読み込む
        /// 戻り値によって取得データを戻す
        /// </summary>
        /// <returns>[UserName(0),,,, GitAccountName(-2), GitAccountEmail(-1)]のリストを返す</returns>
        public static List<string> ReadOldInfo(string path)
        {
            List<string> tmp_Info = new List<string>();
            
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //＞が入っていない行ははじく
                    if (line.IndexOf(">") == -1)
                    {
                        continue;
                    }

                    //-＞の前後をリストとして登録している
                    tmp_Info.Add(line.Substring(line.IndexOf("->") + 2));

                }
            }

            return tmp_Info;
        }

        public static IHtmlDocument ReadHtml(string url, string errorMessage)
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
    }
}
