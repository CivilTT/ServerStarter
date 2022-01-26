using log4net;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Server_GUI2.Develop.Server.World
{
    public class CustomMap
    {
        protected ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Version Version;

        public string SourcePath { get; private set; }

        public string Path { get { return $@"{Version.Path}\{Name}"; } }

        public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(SourcePath); } }

        protected bool IsZip;

        public static CustomMap TryGetInstance(string sourcePath, bool isZip)
        {
            // フォルダ名を取得する
            string name = System.IO.Path.GetFileNameWithoutExtension(sourcePath);

            if (isZip ? IsValidZip(sourcePath, name) : IsValidDirectory(sourcePath, name))
            {
                return new CustomMap(sourcePath, isZip);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// フォルダが配布マップとして有効かどうかを検証する
        /// </summary>
        /// <returns></returns>
        static private bool IsValidDirectory(string sourcePath, string name)
        {
            // フォルダの直下(or一つ下)に ○○ が存在しているかを確認する
            // TODO: 有効性判定をどのファイルの存在で確認するか決定する
            string dirPath = Directory.Exists($@"{sourcePath}\{name}") ? $@"{sourcePath}\{name}" : sourcePath;

            return Directory.Exists($@"{dirPath}\data") && File.Exists($@"{dirPath}\pack.mcmeta");
        }

        /// <summary>
        /// Zipが配布マップとして有効かどうかを検証する
        /// </summary>
        /// <returns></returns>
        static private bool IsValidZip(string sourcePath, string name)
        {
            bool result;
            using (ZipArchive zipArchive = ZipFile.OpenRead(sourcePath))
            {
                // フォルダの直下(or一つ下)に ○○ が存在しているかを確認する
                // TODO: 有効性判定をどのファイルの存在で確認するか決定する
                string dirPath = (zipArchive.GetEntry(name) == null) ? $@"{name}/" : "";

                ZipArchiveEntry metaEntry = zipArchive.GetEntry($"{dirPath}pack.mcmeta");
                ZipArchiveEntry dataEntry = zipArchive.GetEntry($"{dirPath}data/");

                result = metaEntry != null && dataEntry != null;
            }

            return result;
        }

        private CustomMap(string sourcePath, bool isZip)
        {
            SourcePath = sourcePath;
            IsZip = isZip;
        }


        /// <summary>
        /// TODO: パスのディレクトリにワールドを展開/移動する。ディレクトリは上書きではなくマージする。
        /// </summary>
        public void Import()
        {
            if (IsZip)
            {
                string zipPath = $"{Path}.zip";

                File.Move(SourcePath, zipPath);

                ZipFile.ExtractToDirectory(zipPath, Path);

                File.Delete(zipPath);
            }
            else
            {
                FileSystem.CopyDirectory(SourcePath, Path);
            }

            // 一層深くなっているときは、それを上げる処理をする
            if (Directory.Exists($@"{Path}\{Name}"))
            {
                // TODO: この処理が本当に動くのか要検証
                FileSystem.MoveDirectory($@"{Path}\{Name}", Path);
            }
        }
    }
}
