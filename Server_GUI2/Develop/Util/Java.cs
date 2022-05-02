using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    public class JaveVersionException : Exception
    {
        public JaveVersionException(string message):base(message){ }
    }

    public static class Java
    {
        /// <summary>
        /// 導入されているjavaの中でversionNum以上で一番近いバージョンの実行ファイルパスを返す
        /// 該当なしの場合エラー
        /// TODO: エラーの場合にJavaの導入を促すウィンドウを表示する
        /// </summary>
        public static string GetBestJavaPath(int versionNum)
        {
            var versionMap = new Dictionary<int, string>();
            var maxVersion = 0;
            foreach ( var dir in new DirectoryInfo(@"C:\Program Files\Java").GetDirectories())
            {
                var match = Regex.Match(dir.Name, @"jdk-(?<ver>[0-9]+)\.[0-9]+\.[0-9]+");
                // jdk-??.?.?
                if (match.Success)
                {
                    var verint = int.Parse(match.Groups["ver"].Value);
                    if (maxVersion < verint)
                        maxVersion = verint;
                    versionMap[verint] = dir.FullName;
                }
            }

            var bestVersion = versionNum;
            while (!versionMap.ContainsKey(bestVersion))
            {
                bestVersion += 1;
                if (bestVersion > maxVersion)
                {
                    CustomMessageBox.Show(
                        $"このバージョンのサーバーを起動するためには Java{versionNum} 以上が必要です\n" +
                        $"以下のリンクよりJavaをインストールしたうえで，再度起動してください",
                        ButtonType.OK,
                        Image.Error,
                        new LinkMessage("Javaのインストール", "https://www.oracle.com/java/technologies/downloads/")
                        );
                    throw new JaveVersionException($"java version later than {versionNum} is needed");
                }
            }
            return $@"{versionMap[bestVersion]}\bin\java.exe";
        }
    }
}
