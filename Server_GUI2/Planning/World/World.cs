using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Planning.World
{
    abstract class LocalWorld
    {
        public string Path { get; private set; }
        public readonly bool IsNewWorld;
        public bool Recreate { get; set; }
        //public CustomMap CustomMap { get; set; }
        public ObservableCollection<Datapack> Datapacks { get;} = new ObservableCollection<Datapack>();
        public ObservableCollection<Datapack> Pligins { get; } = new ObservableCollection<Datapack>();

        public Version Version => HasRemote ? Remote.Version : Version;
        RemoteWorld Remote
        { get; }
        public ServerProperty property;
        public bool HasRemote => Remote != null;

        //リモートとリンクする
        public abstract void LinkToRemote();

        // フォルダに即時反映
        public abstract void Rename(string name);

        // server.property を保存 
        public void SaveProperty()
        {
            // 自身のフォルダ内にserver.propertyを保存
            // LevelNameは動的に変更するため指定なし
            property.LevelName = "";
        }

        /// <summary>
        /// サーバー起動前処理
        /// </summary>
        private void Preprocess(Version version)
        {
            // 起動前
            if (IsNewWorld || Recreate)
            {
                if (IsNewWorld)
                {
                    // 新規ワールドの場合
                    // 指定バージョンに同名のワールドが存在するかどうかを確認し(1)とかつけて対応
                    // フォルダを作成
                }
                else
                {
                    // 既存ワールドを新規作成する場合
                }

                //if (CustomMap != null)
                //{
                //    // カスタムマップの導入
                //} 
            }
            else
            {
                // 既存ワールドの場合
                if (Remote != null)
                {
                    // もともとリモートのワールドが紐づいていたらpull
                }
                if (version != Version)
                {
                    // バージョンが変わる場合
                    // versionが違ったらバージョンダウンの確認後フォルダを移動
                    // 同名のワールドが存在する場合は(1)とかつける
                }
            }

            // フォルダ構成を確認し変換
            // VtoS StoV
        }

        /// <summary>
        /// サーバー起動後処理
        /// </summary>
        private void Postprocess(RemoteWorld remote)
        {
            if (remote != null)
            {
                if (Remote == remote )
                {
                    if (Recreate)
                    {
                        // pullせずにワールドを再生成した場合push -u -f
                    }
                    else
                    {
                        // もともとの接続先と同じだったらpush
                    }
                }
                else
                {
                    // 新規のリモート接続だったらpush -u -f
                }
            }
        }

        // ワールドデータを準備して起動
        public void WrapRun(Version version,RemoteWorld remote,Action<ServerProperty> runFunc)
        {
            //起動前処理
            Preprocess(version);

            // levelnameを変更
            property.LevelName = "world?";
            // 起動
            runFunc(property);

            // 起動後
            Postprocess(remote);
        }
    }


    /// <summary>
    /// リモート保存先の一覧
    /// </summary>
    abstract class StorageFactory
    {
        ObservableCollection<Storage> Storages { get; }
    }

    /// <summary>
    /// リモート保存先
    /// </summary>
    abstract class Storage
    {
        ObservableCollection<RemoteWorld> Worlds { get; }

        /// <summary>
        /// ワールド名が使用可能か
        /// </summary>
        public abstract bool IsUsableName(string name);

        /// <summary>
        /// ワールドとのリンクを解除しリモート保存先一覧から除去
        /// </summary>
        public abstract void Delete();
    }
    
    /// <summary>
    /// git account
    /// </summary>
    abstract class GitStorage : Storage
    {
        public string Account { get; }
        public string Repository { get; }
    }

    abstract class RemoteWorld
    {
        public LocalWorld Link { get; }
        public abstract bool IsLinked();
        public Version Version { get; }
        public Storage Storage { get; }

        /// <summary>
        /// 即時反映
        /// リンクされたローカルワールドにプルし、リモートブランチを削除
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// データをプル、関数を実行、データをプッシュ
        /// </summary>
        public abstract void WrapRun(Action func,string path);
    }

    /// <summary>
    /// Gitリモートワールド
    /// </summary>
    abstract class GitRemoteWorld : RemoteWorld
    {

    }

    /// <summary>
    /// 新規のGitリモートワールド、サーバー終了後にローカルワールドをプッシュ
    /// </summary>
    abstract class NewGitRemoteWorld
    {

    }
}
