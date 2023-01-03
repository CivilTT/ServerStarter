using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Server_GUI2.Util;
using System;
using System.Collections.Generic;
using System.Net;

namespace UnitTestProject1
{
    [TestClass]
    public class ReadJsonTest
    {
        static readonly WebClient wc = new WebClient();

        [TestMethod]
        public void TestMethod1()
        {
            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
            string jsonStr = wc.DownloadString("https://api.github.com/repos/CivilTT/ServerStarter/releases");
            List<GitReleaseJson> root = JsonConvert.DeserializeObject<List<GitReleaseJson>>(jsonStr);
            root.WriteLine(content => content.VersionName);
        }
    }

    public class GitReleaseJson
    {
        [JsonProperty("name")]
        public string Name;
        [JsonIgnore]
        public string VersionName => Name.Replace("version ", "");
        [JsonProperty("assets")]
        public List<Asset> Assets;
    }

    public class Asset
    {
        [JsonProperty("name")]
        public string FileName;
        [JsonProperty("browser_download_url")]
        public string DownloadUrl;
    }
}
