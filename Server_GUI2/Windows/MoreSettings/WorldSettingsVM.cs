﻿using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Develop.Server.World;

namespace Server_GUI2.Windows.WorldSettings
{
    class WorldSettingsVM : GeneralVM
    {
        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;
        static readonly StorageCollection Storages = StorageCollection.Instance;

        public Version RunVersion { get; private set; }
        public IWorld RunWorld { get; private set; }

        // 設定項目の表示非表示を操作
        public BindingValue<int> MenuIndex { get; private set; }
        public bool ShowProp => MenuIndex.Value == 0;
        public bool ShowSW => MenuIndex.Value == 1;
        public bool ShowAdd => MenuIndex.Value == 2;
        public bool ShowOp => MenuIndex.Value == 3;   
        public bool ShowWhite => MenuIndex.Value == 4;

        // General
        public string RunInfo => $"{RunVersion.Name} / {RunWorld.Name}";
        public SaveCommand SaveCommand { get; private set; }

        // ServerProperty
        public SetDefaultProperties SetDefaultProperties { get; private set; }
        public SetAsDefaultProperties SetAsDefaultProperties { get; private set; }
        public bool[] BoolCombo => new bool[2] { true, false };
        public string[] DifficultyCombo => new string[4] { "peaceful", "easy", "normal", "hard" };
        public string[] GamemodeCombo => new string[4] { "survival", "creative", "adventure", "spectator" };
        public string[] TypeCombo => new string[4] { "default", "flat", "largeBiomes", "amplified" };
        /// <summary>
        /// MainSettingsで使用（最終保存データを格納）
        /// </summary>
        public BindingValue<ServerProperty> PropertyIndexs { get; private set; }
        /// <summary>
        /// TrueFalseの左側の項目一覧
        /// </summary>
        public string[] OtherTFPropertyIndexs
        {
            get
            {
                ServerProperty properties = RunWorld.Property;
                string[] removeIndex = new string[2] { "hardcore", "white-list" };
                List<string> allindex = properties.BoolOption.Keys.ToList();
                allindex.RemoveAll(index => removeIndex.Contains(index));
                return allindex.ToArray();
            }
        }
        /// <summary>
        /// TrueFalseの左側で選択している項目
        /// </summary>
        public BindingValue<string> SelectedTFIndex { get; private set; }
        /// <summary>
        /// Stringの左側の項目一覧
        /// </summary>
        public string[] OtherPropertyIndexs
        {
            get
            {
                ServerProperty properties = RunWorld.Property;
                string[] removeIndex = new string[4] { "difficulty", "gamemode", "level-type", "level-name" };
                List<string> allindex = properties.StringOption.Keys.ToList();
                allindex.RemoveAll(index => removeIndex.Contains(index));
                return allindex.ToArray();
            }
        }
        /// <summary>
        /// Stringの左側で選択している項目
        /// </summary>
        public BindingValue<string> SelectedPropIndex { get; private set; }
        /// <summary>
        /// TrueFalseの右側で選択している項目
        /// </summary>
        public bool SelectedTFProperty
        {
            get => PropertyIndexs.Value.BoolOption[SelectedTFIndex.Value];
            set => PropertyIndexs.Value.BoolOption[SelectedTFIndex.Value] = value;
        }
        /// <summary>
        /// Stringの右側の記載事項
        /// </summary>
        public string OtherStringProperty
        {
            get => PropertyIndexs.Value.StringOption[SelectedPropIndex.Value];
            set => PropertyIndexs.Value.StringOption[SelectedPropIndex.Value] = value;
        }


        // ShareWorld
        public BindingValue<bool> UseSW { get; private set; }
        public ObservableCollection<Storage> Accounts { get; private set; }
        public BindingValue<Storage> AccountIndex { get; private set; }
        public ObservableCollection<RemoteWorld> RemoteDataList { get; private set; }
        public BindingValue<RemoteWorld> RemoteIndex { get; private set; }
        public string RemoteName { get; set; }
        public bool ShowNewRemoteData => RemoteIndex?.Value == /*//TODO: 何とイコールにすればよい？//*/null;

        // Additionals
        public ImportAdditionalsCommand ImportAdditionalsCommand { get; private set; }
        public DeleteAdditionalsCommand DeleteAdditionalsCommand { get; private set; }
        // DataPack
        public BindingValue<bool> IsZipDatapack { get; private set; }
        public DatapackCollection Datapacks { get; private set; }
        public BindingValue<ADatapack> SelectedDatapack { get; private set; }
        // Plugin
        public bool ShowPluginTab => RunVersion is SpigotVersion;
        public PluginCollection Plugins { get; private set; }
        public BindingValue<APlugin> SelectedPlugin { get; private set; }
        public BindingValue<bool> IsCrossPlay { get; private set; }
        // Custom Map
        public bool IsZipMap { get; set; } = true;
        public CustomMap CustomMap { get; set; }
        public string ServerResourcePack
        {
            get => PropertyIndexs.Value.StringOption["resource-pack"];
            set => PropertyIndexs.Value.StringOption["resource-pack"] = value;
        }

        //Op
        public List<Player> Players { get; private set; }
        public Player OpPlayerIndex { get; set; }
        public List<PlayerGroup> Groups { get; private set; }
        public PlayerGroup OpGroupIndex { get; set; }
        public int[] OpLevels => new int[4] { 1, 2, 3, 4 };
        public int OpLevelIndex { get; set; } = 4;
        public bool CanAddOpPlayer => OpPlayerIndex != null;
        public AddOpPlayerCommand AddOpPlayerCommand { get; private set; }
        public DeleteOpPlayerCommand DeleteOpPlayerCommand { get; private set; }
        public ObservableCollection<OpPlayer> OpPlayersList { get; private set; }
        public OpPlayer OpPlayersListIndex { get; set; }

        // WhiteList
        public Player WhitePlayerIndex { get; set; }
        public PlayerGroup WhiteGroupIndex { get; set; }
        public bool CanAddWhitePlayer => WhitePlayerIndex != null;
        public AddWhiteCommand AddWhiteCommand { get; private set; }
        public DeleteWhiteCommand DeleteWhiteCommand { get; private set; }
        public ObservableCollection<Player> WhitePlayersList { get; private set; }
        public Player WhitePlayersListIndex { get; set; }


        public WorldSettingsVM(Version runVer, IWorld runWor)
        {
            RunVersion = runVer;
            RunWorld = runWor;

            // General
            MenuIndex = new BindingValue<int>(0, () => OnPropertyChanged(new string[5] { "ShowProp", "ShowSW", "ShowAdd", "ShowOp", "ShowWhite" }));
            SaveCommand = new SaveCommand(this);

            // ServerProperty
            SetDefaultProperties = new SetDefaultProperties(this);
            SetAsDefaultProperties = new SetAsDefaultProperties(this);
            PropertyIndexs = new BindingValue<ServerProperty>(RunWorld.Property, () => OnPropertyChanged("PropertyIndexs"));
            SelectedTFIndex = new BindingValue<string>(OtherTFPropertyIndexs[0], () => OnPropertyChanged("SelectedTFProperty"));
            SelectedPropIndex = new BindingValue<string>(OtherPropertyIndexs[0], () => OnPropertyChanged("OtherStringProperty"));

            // ShareWorld
            // TODO: 既存ワールドをリモート化するときには【new Remote Data】しか選べないようにする
            // TODO: (@txkodo) Worldと同じく【new Remote Data】をStoragesに持たせておけないか
            UseSW = new BindingValue<bool>(RunWorld.HasRemote, () => OnPropertyChanged(""));
            Accounts = Storages.Storages;
            AccountIndex = new BindingValue<Storage>(Accounts.FirstOrDefault(), () => OnPropertyChanged("RemoteDataList"));
            RemoteDataList = AccountIndex.Value?.RemoteWorlds ?? new ObservableCollection<RemoteWorld>();

            // Additionals
            ImportAdditionalsCommand = new ImportAdditionalsCommand(this);
            DeleteAdditionalsCommand = new DeleteAdditionalsCommand(this);
            // Datapack
            IsZipDatapack = new BindingValue<bool>(true, () => OnPropertyChanged("IsZipDatapack"));
            Datapacks = RunWorld.Datapacks;
            SelectedDatapack = new BindingValue<ADatapack>(Datapacks.Datapacks.FirstOrDefault(), () => OnPropertyChanged(""));
            // Plugin
            if (RunVersion is SpigotVersion)
            {
                Plugins = RunWorld.Plugins;
                SelectedPlugin = new BindingValue<APlugin>(Plugins.Plugins.FirstOrDefault(), () => OnPropertyChanged(""));

                // TODO: 所定のpluginを使用する設定になっていれば初期値をtrueにする
                IsCrossPlay = new BindingValue<bool>(false, () => CrossPlay());
            }
            // Custom Map


            // Op
            Players = SaveData.Players;
            OpPlayerIndex = Players.FirstOrDefault();
            Groups = SaveData.PlayerGroups;
            Groups.Add(new PlayerGroup("(No Group)", null));
            OpGroupIndex = Groups.FirstOrDefault();
            AddOpPlayerCommand = new AddOpPlayerCommand(this);
            DeleteOpPlayerCommand = new DeleteOpPlayerCommand(this);
            // TODO: （@txkodo）WorldインスタンスにOpとWhiteListの情報を持ってほしい
            OpPlayersList = new ObservableCollection<OpPlayer>();

            // WhiteList
            WhitePlayerIndex = Players.FirstOrDefault();
            WhiteGroupIndex = Groups.FirstOrDefault();
            AddWhiteCommand = new AddWhiteCommand(this);
            DeleteWhiteCommand = new DeleteWhiteCommand(this);
            WhitePlayersList = new ObservableCollection<Player>();
            //WhitePlayersList = RunWorld.WhiteList; -> みたいな感じにしたい


        }

        private void CrossPlay()
        {
            if (IsCrossPlay.Value)
            {
                // TODO: 導入するプラグインを一覧に追加する
                // ダウンロード処理については実行時に行う？
                // World/Pluginにフラグを持たせておいて、処理を実行時にさせる？
                // クロスプレイには19132番(UDP)のポート開放を25565と合わせて行う必要性あり
                // 注意事項（導入するプラグインの一覧とそれらの利用規約に同意したとする・19132番を開放させる必要性がある（AutoPortMappingを利用する場合は自動で開放する））に同意させる
            }
            else
            {
                // TODO: クロスプレイに必要なプラグインを削除する
            }
        }

    }

    public class OpPlayer : Player, IEquatable<OpPlayer>
    {
        public int OpLevel { get; set; }
        public bool BypassesPlayerLimit { get; set; }

        public OpPlayer(Player player, int opLevel, bool bypassesPlayerLimit=false) : base(player.Name)
        {
            UUID = player.UUID;
            OpLevel = opLevel;
            BypassesPlayerLimit = bypassesPlayerLimit;
        }

        public bool Equals(OpPlayer other)
        {
            return other.UUID == UUID;
        }
    }
}
