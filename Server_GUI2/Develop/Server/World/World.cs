using System;
using System.IO;
using System.Collections.ObjectModel;
using Server_GUI2.Develop.Server.Storage;
using Server_GUI2.Develop.Util;

namespace Server_GUI2.Develop.Server.World
{
    public class LocalWorld
    {
        public string Path { get; private set; }
        public string Name { get; set; }
        public string Id => $"{version.Name}/{Name}";
        /// <summary>
        /// 表示名(version/name)
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (IsNewWorld)
                    return "[new world]";
                var suffix = HasRemote ? "(remote)" : "";
                return $"{Version.Name}/{Name}{suffix}";
            }
        }

        public readonly bool IsNewWorld;
        public bool Recreate { get; set; }
        public CustomMap CustomMap { get; set; }
        public ObservableCollection<ADatapack> Datapacks { get; } = new ObservableCollection<ADatapack>();
        public ObservableCollection<ADatapack> Pligins { get; } = new ObservableCollection<ADatapack>();
        private Version version;
        public Version Version => HasRemote ? Remote.Value.Version : version;
        ReadOnlyProperty<RemoteWorld> Remote { get; }
        public ServerProperty property;
        public bool HasRemote => Remote.Value != null;

        public LocalWorld(string name)
        {
            IsNewWorld = true;
            Name = name;
            this.version = null;
            this.Remote = WorldLink.Instance.GetLinkedRemote(this);
        }

        public LocalWorld(string name,Version version)
        {
            IsNewWorld = false;
            Name = name;
            this.version = version;
            this.Remote = WorldLink.Instance.GetLinkedRemote(this);
        }

        //リモートとリンクする
        public void LinkToRemote(RemoteWorld remote)
        {
            WorldLink.Instance.Link(this,remote);
        }

        public void UnLink()
        {
            WorldLink.Instance.UnLink(this);
        }

        // フォルダに即時反映
        public void Rename(string name)
        {

        }

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

                if (CustomMap != null)
                {
                    // カスタムマップの導入
                }
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
                if (Remote.Value == remote)
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
        public void WrapRun(Version version, RemoteWorld remote, Action<ServerProperty> runFunc)
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

    //public class World
    //{
    //    public bool Recreate { get; set; }
    //    public CustomMap CustomMap { get; set; }
    //    public ServerProperty serverProperty { get; }
    //    public Version Version { get; }

    //    public WorldReader WorldReader { get; }
    //    public ObservableCollection<Datapack> Datapacks = new ObservableCollection<Datapack>();
    //    public ObservableCollection<Datapack> Pligins = new ObservableCollection<Datapack>();

    //    public World(WorldReader worldReader, Version version)
    //    {
    //        Version = version;
    //        WorldReader = worldReader;
    //    }

    //    /// <summary>
    //    /// ワールドデータを読み込むー＞与えられた関数を実行ー＞ワールドデータを書き出す
    //    /// RUNするときはサーバー起動関数を引数に与えること
    //    /// </summary>
    //    public void WrapRunAction( Action func, Version version, Storage.Storage stoarge)
    //    {
    //        var writer = Preprocess(version,stoarge);
    //        func();
    //        writer.Postprocess();
    //    }

    //    private WorldWriter Preprocess(Version version, Storage.Storage stoarge)
    //    {
    //        // ワールド書き込み/アップロード用インスタンス
    //        var writer = stoarge.GetWorldWriter(version);
    //        // ワールドデータを指定位置に展開
    //        ConvertWorld(writer.Path, version);
    //        // ワールド書き込みの前処理(Gitに使用中フラグを立てる等)
    //        writer.Preprocess();
    //        return writer;
    //    }

    //    /// <summary>
    //    /// Run後に実行
    //    /// ディレクトリの内容に応じて(Spigot|Vanilla|New)PreWorldインスタンスを返す
    //    /// </summary>
    //    private void ConvertWorld(string worldPath,Version version)
    //    {
    //        if (!Recreate && Version != null && Version > version)
    //        {
    //           // TODO: バージョンが下がる場合は確認画面を表示
    //        }
    //        // ワールドデータを指定パスに展開
    //        WorldReader.ReadTo(worldPath);
    //        // データパックの追加と削除
    //        for (var i = 0; i < Datapacks.Count; i++)
    //        {
    //            Datapacks[i].Ready(worldPath);
    //        }
    //        // 必要に応じてワールドを再生成
    //        if (Recreate)
    //        {
    //            RecreateWorld(worldPath);
    //            // 必要に応じてカスタムワールドを導入する
    //            if (CustomMap != null)
    //            {
    //                CustomMap.Import(worldPath);
    //            }
    //        }
    //        GenWorldConverter(worldPath).ConvertTo(version);
    //    }

    //    /// <summary>
    //    /// TODO: ワールドデータをリセット(データパックはそのまま)
    //    /// </summary>
    //    private static void RecreateWorld(string worldPath)
    //    {
    //    }

    //    /// <summary>
    //    /// TOOD: ワールドデータの形式に応じてPreWorldインスタンスを返却
    //    /// </summary>
    //    private static WorldConverter GenWorldConverter(string worldPath)
    //    {
    //        // world がない -> NewPreWorld
    //        if ( !Directory.Exists(Path.Combine(worldPath, "world")))
    //        {
    //            return new NewWorldConverter(worldPath);
    //        }
    //        // world-nether がない -> VanillaPreWorld
    //        if ( !Directory.Exists(Path.Combine(worldPath, "world-nether")))
    //        {
    //            return new VanillaWorldConverter(worldPath);
    //        }
    //        // その他 -> SpligotPreWorld
    //        return new SpigotWorldConverter(worldPath);
    //    }
    //}
}
