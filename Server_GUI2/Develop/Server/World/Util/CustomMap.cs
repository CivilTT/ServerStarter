﻿using log4net;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Server_GUI2.Develop.Server.World
{
    public class CustomMap
    {
        protected ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Version Version;

        public string SourcePath { get; private set; }

        public string Name { get { return Path.GetFileNameWithoutExtension(SourcePath); } }

        protected bool IsZip;

        public static CustomMap TryGetInstance(string sourcePath, bool isZip)
        {
            // フォルダ名を取得する
            string name = Path.GetFileNameWithoutExtension(sourcePath);

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
            string dirPath = Directory.Exists($@"{sourcePath}\{name}") ? $@"{sourcePath}\{name}" : sourcePath;

            return Directory.Exists($@"{dirPath}\data");
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
                result = zipArchive.Entries.Where(entry => entry.FullName.Contains("data/")).Count() != 0;
            }

            return result;
        }

        private CustomMap(string sourcePath, bool isZip)
        {
            SourcePath = sourcePath;
            IsZip = isZip;
        }


        /// <summary>
        /// パスのディレクトリにワールドを展開/移動する。
        /// </summary>
        public void Import(string path)
        {
            if (IsZip)
            {
                logger.Info("Import Zip CustomMap");
                string zipPath = $"{path}.zip";

                File.Move(SourcePath, zipPath);

                ZipFile.ExtractToDirectory(zipPath, path);

                File.Delete(zipPath);
            }
            else
            {
                logger.Info("Import non-Zip CustomMap");
                FileSystem.MoveDirectory(SourcePath, path);
            }

            // 一層深くなっているときは、それを上げる処理をする
            if (!Directory.Exists($@"{path}\data"))
            {
                foreach (string folderPath in Directory.GetDirectories(path))
                {
                    if (!Directory.Exists($@"{folderPath}\data")) { continue; }
                    
                    var dir = new DirectoryInfo(folderPath);
                    dir.MoveTo("temp");
                    FileSystem.DeleteDirectory(path, DeleteDirectoryOption.DeleteAllContents);
                    var tmp = new DirectoryInfo("temp");
                    tmp.MoveTo(path);
                }
            }
        }
    }
}
