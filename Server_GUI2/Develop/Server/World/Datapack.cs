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

        protected World World;
        public string Name { get; private set; }

        protected bool IsZip;

        public string Path { get { return $@"{World.Path}\datapacks\{Name}"; } }


        protected Datapack(World world, string name)
        {
            World = world;
            Name = name;
        }

        /// <summary>
        /// 削除の必要性があるかどうか
        /// TODO: Viewを更新するためにEventを作成する
        /// </summary>
        public bool IsRemove;

        protected virtual void Remove() { }

        protected virtual void Import() { } 

        /// <summary>
        /// RUN時に実行。
        /// ワールドデータ内にデータパックがあることを保証する
        /// データパック導入に関する一連の処理を行う
        /// </summary>
        public void Ready()
        {
            if (IsRemove)
                Remove();

            Import();
        }
    }

    /// <summary>
    /// すでに導入済のデータパック
    /// </summary>
    public class ExistDatapack : Datapack
    {
        public ExistDatapack(World world, string name) : base(world, name)
        {
            
        }

        /// <summary>
        /// データパックを新規導入(何もしない)
        /// </summary>
        protected override void Import() { }


        /// <summary>
        /// TODO: ディレクトリからデータパックを削除
        /// </summary>
        protected override void Remove()
        {
            logger.Info("Remove the datapack");
            if (Directory.Exists(Path))
            {
                FileSystem.DeleteDirectory(Path, DeleteDirectoryOption.DeleteAllContents);
            }
            else
            {
                File.Delete(Path);
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
        public static ImportDatapack TryGenInstance(World world, string sourcePath, bool isZip)
        {
            // フォルダ名を取得する
            string name = System.IO.Path.GetFileNameWithoutExtension(sourcePath);

            if ( isZip ? IsValidZip(sourcePath, name) : IsValidDirectory(sourcePath, name) )
            {
                return new ImportDatapack(world, name, sourcePath, isZip);
            }
            else
            {
                return null;
            }
        }

        private ImportDatapack(World world, string name, string sourcePath, bool isZip) :base(world, name)
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

        protected override void Import()
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

            // data, pack.mcmetaが一層深くなっているときは、それを上げる処理をする
            if (Directory.Exists($@"{Path}\{Name}"))
            {
                Directory.Move($@"{Path}\{Name}\data", $@"{Path}\data");
                File.Move($@"{Path}\{Name}\pack.mcmeta", $@"{Path}\pack.mcmeta");
                Directory.Delete($@"{Path}\{Name}");
            }
        }

        /// <summary>
        /// ディレクトリからデータパックを削除(何もしない)
        /// </summary>
        protected override void Remove() { }
    }
}
