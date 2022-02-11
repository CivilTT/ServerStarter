﻿using log4net;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MW = ModernWpf;

namespace Server_GUI2.Develop.Server.World
{
    /// <summary>
    /// World一覧を管理する
    /// </summary>
    public class WorldCollection
    {
        public static WorldCollection Instance { get; } = new WorldCollection();

        private RemotesJsonPath jsonPath = ServerGuiPath.Instance.RemotesJson;

        /// <summary>
        /// データ整合性のためリスト変換はしない。
        /// </summary>
        public ObservableCollection<IWorld> Worlds { get; } = new ObservableCollection<IWorld>();

        private WorldCollection()
        {
            var linkJson = LoadLinkJson();

            // new World を追加
            Worlds.Add(new NewWorld());

            //　ローカルワールド一覧とリンク情報を組み合わせてWorldWrapperを構成
            foreach (var local in LocalWorldCollection.Instance.LocalWorlds)
            {
                var linkData = linkJson.Where(x =>
                   x.LocalVersion == local.Version.Name &&
                   x.LocalWorld == local.Name
                    ).FirstOrDefault();

                // TODO: リモートとの通信ができなかった場合のフォールバック
                // 通信できないワールドは一覧に追加しないorグレーアウトして選択できないように
                if (linkData != null)
                {
                    var remote = StorageCollection.Instance.FindStorage(linkData.RemoteStorage).FindRemoteWorld(linkData.RemoteWorld);
                    // TODO: usingフラグが立ちっぱなしだったらpushする
                    // サーバー起動後にネットワークが切断された場合に起こりうる
                    var world = new World(local,remote);
                    Add(world);
                }
                else
                {
                    var world = new World(local);
                    Add(world);
                }
            }
        }

        /// <summary>
        /// WorldWrapperを追加(自動呼び出し)
        /// </summary>
        public void Add(World world)
        {
            Worlds.Add(world);
            world.DeleteEvent += new EventHandler((_, __) => Worlds.Remove(world));
        }

        /// <summary>
        /// ローカルとリモートのリンク情報を取得
        /// </summary>
        private List<RemoteLinkJson> LoadLinkJson()
        {
            var json = jsonPath.ReadAllText();
            return JsonConvert.DeserializeObject<List<RemoteLinkJson>>(json);
        }

        /// <summary>
        /// ローカルとリモートの現在のリンク情報を保存
        /// </summary>
        public void SaveLinkJson()
        {
            // リモートを確実に持つワールドを抜き出して変換
            var obj = Worlds.OfType<World>().Where(x => x.HasRemote && (! x.CanCahngeRemote)).Select(x => x.ExportLinkJson()).ToList();
            JsonConvert.SerializeObject(obj);
        }
    }
    public class RemoteLinkJson
    {
        [JsonProperty("remote_storage")]
        public string RemoteStorage;

        [JsonProperty("remote_world")]
        public string RemoteWorld;

        [JsonProperty("local_version")]
        public string LocalVersion;

        [JsonProperty("local_world")]
        public string LocalWorld;

        [JsonProperty("using")]
        public bool Using;

        /// <summary>
        /// JsonSerialiser用コンストラクタ
        /// </summary>
        public RemoteLinkJson()
        { }

        public RemoteLinkJson(RemoteWorld remote, LocalWorld local, bool Using)
        {
            RemoteStorage = remote.Storage.Id;
            RemoteWorld = remote.Id;
            LocalVersion = local.Version.Name;
            LocalWorld = local.Name;
            this.Using = Using;
        }
    }
}
