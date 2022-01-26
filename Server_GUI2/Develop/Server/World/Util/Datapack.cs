using log4net;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server_GUI2
{
    public class Datapack
    {
        public ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; private set; }

        protected bool IsZip;

        protected Datapack(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 削除の必要性があるかどうか
        /// TODO: Viewを更新するためにEventを作成する
        /// </summary>
        public bool NeedToRemove;

        protected virtual void Remove(string path) { }

        protected virtual void Import(string path) { } 

        /// <summary>
        /// RUN時に実行。
        /// ワールドデータ内にデータパックがあることを保証する
        /// データパック導入に関する一連の処理を行う
        /// </summary>
        public void Ready(string path)
        {
            if (NeedToRemove)
            {
                Remove(path);
            }
            else
            {
                Import(path);
            }
        }
    }

    /// <summary>
    /// すでに導入済のデータパック
    /// </summary>
    public class ExistDatapack : Datapack
    {
        public ExistDatapack(string name) : base(name)
        {
            
        }

        /// <summary>
        /// データパックを新規導入(何もしない)
        /// </summary>
        protected override void Import(string path ) { }


        /// <summary>
        /// TODO: ディレクトリからデータパックを削除
        /// </summary>
        protected override void Remove(string path)
        {
            logger.Info("Remove the datapack");
            if (Directory.Exists(path))
            {
                FileSystem.DeleteDirectory(path, DeleteDirectoryOption.DeleteAllContents);
            }
            else
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// 展開して移動する必要があるデータパック
    /// 移動先に移動するデータパックが存在していないことを前提とする
    /// </summary>
    public class ImportDatapack : Datapack
    {
        private readonly string SourcePath;
        
        /// <summary>
        /// 与えられたパスが有効なデータパックの場合インスタンスを生成する
        /// でなければnullを返す。
        /// </summary>
        public static ImportDatapack TryGenInstance(string sourcePath, bool isZip)
        {
            // フォルダ名を取得する
            string name = System.IO.Path.GetFileNameWithoutExtension(sourcePath);

            if ( isZip ? IsValidZip(sourcePath, name) : IsValidDirectory(sourcePath, name) )
            {
                return new ImportDatapack(name, sourcePath, isZip);
            }
            else
            {
                return null;
            }
        }

        private ImportDatapack(string name, string sourcePath, bool isZip) :base(name)
        {
            SourcePath = sourcePath;
            IsZip = isZip;
        }

        /// <summary>
        /// フォルダがデータパックとして有効かどうかを検証する
        /// </summary>
        /// <returns></returns>
        static private bool IsValidDirectory(string sourcePath, string name)
        {
            // フォルダの直下(or一つ下)に pack.mcmeta & dataフォルダ が存在しているかを確認する
            string dirPath = Directory.Exists($@"{sourcePath}\{name}") ? $@"{sourcePath}\{name}" : sourcePath;

            return Directory.Exists($@"{dirPath}\data") && File.Exists($@"{dirPath}\pack.mcmeta");
        }

        /// <summary>
        /// Zipがデータパックとして有効かどうかを検証する
        /// </summary>
        /// <returns></returns>
        static private bool IsValidZip(string sourcePath, string name)
        {
            bool result;
            using (ZipArchive zipArchive = ZipFile.OpenRead(sourcePath))
            {
                // フォルダの直下(or一つ下)に pack.mcmeta & dataフォルダ が存在しているかを確認する
                string dirPath = (zipArchive.GetEntry(name) == null) ? $@"{name}/" : "";

                ZipArchiveEntry metaEntry = zipArchive.GetEntry($"{dirPath}pack.mcmeta");
                ZipArchiveEntry dataEntry = zipArchive.GetEntry($"{dirPath}data/");

                result = metaEntry != null && dataEntry != null;
            }

            return result;
        }

        protected override void Import(string path)
        {
            if (IsZip)
            {
                string zipPath = $"{path}.zip";

                File.Move(SourcePath, zipPath);

                ZipFile.ExtractToDirectory(zipPath, path);

                File.Delete(zipPath);
            }
            else
            {
                FileSystem.CopyDirectory(SourcePath, path);
            }

            // data, pack.mcmetaが一層深くなっているときは、それを上げる処理をする
            if (Directory.Exists($@"{path}\{Name}"))
            {
                Directory.Move($@"{path}\{Name}\data", $@"{path}\data");
                File.Move($@"{path}\{Name}\pack.mcmeta", $@"{path}\pack.mcmeta");
                Directory.Delete($@"{path}\{Name}");
            }
        }

        /// <summary>
        /// ディレクトリからデータパックを削除(何もしない)
        /// </summary>
        protected override void Remove(string path) { }
    }
}
