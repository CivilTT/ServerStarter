using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    public class Datapack
    {
        protected World World;
        public string Name { get; private set; }

        public string Path
        {
            get
            {
                return $@"{World.Path}\datapacks\{Name}";
            } 
        }

        protected Datapack(World world)
        {
            World = world;
        }

        public virtual void remove(){ }

        /// <summary>
        /// RUN時に実行。ワールドデータ内にデータパックがあることを保証する
        /// </summary>
        public virtual void ready() { }
    }

    /// <summary>
    /// すでに導入済のデータパック
    /// </summary>
    public class ExistDatapack : Datapack
    {
        /// <summary>
        /// TODO: ディレクトリからデータパックを削除
        /// </summary>
        public override void remove()
        {
        }
    }

    /// <summary>
    /// 展開して移動する必要があるデータパック
    /// </summary>
    public class ImportDatapack : Datapack
    {
        private string SourcePath;

        private bool IsZip;

        /// <summary>
        /// 与えられたパスが有効なデータパックの場合インスタンスを生成する
        /// でなければnullを返す。
        /// </summary>
        public ImportDatapack TryGenInstance(World world, string sourcePath, bool isZip)
        {
            if ( isZip ? IsValidZip(sourcePath) : IsValidDirectory(sourcePath) )
            {
                return new ImportDatapack(world,sourcePath,isZip);
            }
            else
            {
                return null;
            }
        }

        private ImportDatapack(World world, string sourcePath, bool isZip) :base(world)
        {
            SourcePath = sourcePath;
            IsZip = isZip;
        }

        /// <summary>
        /// TODO: フォルダがデータパックとして有効かどうかを検証する
        /// </summary>
        /// <returns></returns>
        static private bool IsValidDirectory(string path)
        {
            return false;
        }

        /// <summary>
        /// TODO: Zipがデータパックとして有効かどうかを検証する
        /// </summary>
        /// <returns></returns>
        static private bool IsValidZip(string path)
        {
            return false;
        }

        /// <summary>
        /// ディレクトリからデータパックを削除(何もしない)
        /// </summary>
        public override void remove() { }

        /// <summary>
        /// TODO: ワールドデータ内にデータパックを移動して展開
        /// </summary>
        public override void ready() { }

    }
}
