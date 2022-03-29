using log4net;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server_GUI2.Develop.Server.World
{
    public class DatapackCollection
    {
        public ObservableCollection<ADatapack> Datapacks { get;}
        private readonly List<Action<string>> operations = new List<Action<string>>();

        public DatapackCollection()
        {
            Datapacks = new ObservableCollection<ADatapack>();
        }

        public DatapackCollection(List<string> datapackNames)
        {
            Datapacks = new ObservableCollection<ADatapack>(datapackNames.Select(x => new ExistDatapack(x)));
        }

        public DatapackCollection(DatapackCollection datapacks)
        {
            Datapacks = new ObservableCollection<ADatapack>(datapacks.Datapacks);
            operations = new List<Action<string>>(datapacks.operations);
        }

        public List<string> ExportList()
        {
            return Datapacks.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// データパックを追加する
        /// (ディレクトリ操作は行わない)
        /// </summary>
        public void Add(Datapack datapack)
        {
            Datapacks.Add(datapack);
            operations.Add(datapack.Import);
        }

        /// <summary>
        /// データパックを削除する
        /// (ディレクトリ操作は行わない)
        /// </summary>
        public void Remove(ADatapack datapack)
        {
            Datapacks.Remove(datapack);
            operations.Add(datapack.Remove);
        }

        /// <summary>
        /// データパックの削除と追加をディレクトリ上で実際に行う
        /// </summary>
        public void Evaluate(string path)
        {
            foreach ( var operation in operations)
                operation(path);
            StartServer.RunProgressBar.AddMessage("Imported Datapacks.");
        }

        public List<string> GetNames()
        {
            return Datapacks.Select(x => x.Name).ToList();
        }
    }

    public abstract class ADatapack
    {
        public ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; private set; }

        protected bool IsZip;

        protected ADatapack(string name)
        {
            Name = name;
        }

        public virtual void Remove(string path) { }
    }

    /// <summary>
    /// すでに導入済のデータパック
    /// </summary>
    class ExistDatapack : ADatapack
    {
        public ExistDatapack(string name) : base(name)
        {
            
        }

        public override void Remove(string datapacksPath)
        {
            var path = Path.Combine(datapacksPath, Name);
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
    public class Datapack : ADatapack
    {
        private readonly string SourcePath;
        
        /// <summary>
        /// 与えられたパスが有効なデータパックの場合インスタンスを生成する
        /// でなければnullを返す。
        /// </summary>
        public static Datapack TryGenInstance(string sourcePath, bool isZip)
        {
            // フォルダ名を取得する
            string name = Path.GetFileNameWithoutExtension(sourcePath);

            if ( isZip ? IsValidZip(sourcePath, name) : IsValidDirectory(sourcePath, name) )
            {
                return new Datapack(name, sourcePath, isZip);
            }
            else
            {
                return null;
            }
        }

        private Datapack(string name, string sourcePath, bool isZip) :base(name)
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

        /// <summary>
        /// データパックを新規導入
        /// </summary>
        public void Import(string datapacksPath)
        {
            var path = Path.Combine(datapacksPath, Name);
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
        public override void Remove(string datapacksPath) { }
    }
}
