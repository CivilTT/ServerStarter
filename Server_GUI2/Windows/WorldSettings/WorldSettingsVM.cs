﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using Server_GUI2.Util;

namespace Server_GUI2.Windows.WorldSettings
{
    class WorldSettingsVM : GeneralVM, IDataErrorInfo
    {
        public Properties.Resources Resources => new Properties.Resources();
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
        public bool CanSave => !UseSW.Value || (!ShowNewRemoteData || ValidRemoteName);
        public SaveCommand SaveCommand { get; private set; }
        public bool Saved = false;
        public HelpCommand HelpCommand { get; private set; }

        // ServerProperty
        public bool[] BoolCombo => new bool[2] { true, false };
        public string[] DifficultyCombo => new string[4] { "peaceful", "easy", "normal", "hard" };
        public string[] GamemodeCombo => new string[4] { "survival", "creative", "adventure", "spectator" };
        public string[] TypeCombo => new string[4] { "default", "flat", "largeBiomes", "amplified" };
        public SetDefaultProperties SetPropCommand { get; private set; }
        public SaveDefaultProperties SavePropCommand { get; private set; }
        /// <summary>
        /// MainSettingsで使用（最終保存データを格納）
        /// </summary>
        public BindingValue<ServerProperty> PropertyIndexs { get; private set; }
        public ObservableCollection<BoolOption> BoolOptions { get; private set; }
        public ObservableCollection<TextOption> TextOptions { get; private set; }


        // ShareWorld
        public BindingValue<bool> UseSW { get; private set; }
        public bool CanEdit => UseSW.Value && ! RunWorld.HasRemote;
        public ObservableCollection<Storage> Accounts { get; private set; }
        public bool HasStorages => Storages.Storages.Count != 0;
        public BindingValue<Storage> AccountIndex { get; private set; }
        public ObservableCollection<IRemoteWorld> RemoteDataList { get; private set; }
        public BindingValue<IRemoteWorld> RemoteIndex { get; private set; }
        public bool CanSelectRemoteIndex => RunWorld is NewWorld && CanEdit;
        private string remoteName = "";
        public string RemoteName
        {
            get => remoteName;
            set
            {
                remoteName = value;
                OnPropertyChanged("CanSave");
            }
        }
        public bool ValidRemoteName => AccountIndex?.Value.IsUsableName(RemoteName) ?? false;
        public bool ShowNewRemoteData => RemoteIndex?.Value is NewRemoteWorld;

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
        public bool ShowMapTab => RunWorld is NewWorld;
        public bool IsZipMap { get; set; } = true;
        public BindingValue<CustomMap> CustomMap { get; private set; }
        public string ServerResourcePack
        {
            get => PropertyIndexs.Value.StringOption["resource-pack"];
            set => PropertyIndexs.Value.StringOption["resource-pack"] = value;
        }

        //Op
        public List<Player> Players { get; private set; }
        public bool ValidPlayers => Players.Count != 0;
        public Player OpPlayerIndex { get; set; }
        public List<PlayerGroup> Groups { get; private set; }
        public PlayerGroup OpGroupIndex { get; set; }
        public int[] OpLevels => new int[4] { 1, 2, 3, 4 };
        public int OpLevelIndex { get; set; } = 4;
        public bool CanAddOpPlayer => OpPlayerIndex != null;
        public AddOpPlayerCommand AddOpPlayerCommand { get; private set; }
        public ObservableCollection<OpsRecord> OpPlayersList { get; private set; }
        public OpsRecord OpPlayersListIndex { get; set; }

        // WhiteList
        public Player WhitePlayerIndex { get; set; }
        public PlayerGroup WhiteGroupIndex { get; set; }
        public bool CanAddWhitePlayer => WhitePlayerIndex != null;
        public AddWhiteCommand AddWhiteCommand { get; private set; }
        public ObservableCollection<Player> WhitePlayersList { get; private set; }
        public Player WhitePlayersListIndex { get; set; }

        // Error Process
        public string Error { get { return null; } }
        public string this[string columnName] => CheckInputBox(columnName);

        public WorldSettingsVM(Version runVer, IWorld runWor)
        {
            RunVersion = runVer;
            RunWorld = runWor;

            // General
            MenuIndex = new BindingValue<int>(0, () => OnPropertyChanged(new string[5] { "ShowProp", "ShowSW", "ShowAdd", "ShowOp", "ShowWhite" }));
            SaveCommand = new SaveCommand(this);
            HelpCommand = new HelpCommand(this);

            // ServerProperty
            PropertyIndexs = new BindingValue<ServerProperty>(new ServerProperty(RunWorld.Settings.ServerProperties), () => OnPropertyChanged("PropertyIndexs"));
            SetPropCommand = new SetDefaultProperties(this);
            SavePropCommand = new SaveDefaultProperties(this);
            BoolOptions = BoolOption.GetBoolCollection(PropertyIndexs.Value.BoolOption, new string[2] { "hardcore", "white-list" });
            TextOptions = TextOption.GetTextCollection(PropertyIndexs.Value.StringOption, new string[4] { "difficulty", "gamemode", "level-type", "level-name" });

            // ShareWorld
            UseSW = new BindingValue<bool>(RunWorld.HasRemote, () => SetUseSW());
            if (HasStorages)
            {
                Accounts = new ObservableCollection<Storage>(Storages.Storages);
                AccountIndex = new BindingValue<Storage>(Accounts.FirstOrDefault(), () => UpdateRemoteList());
                RemoteDataList = new ObservableCollection<IRemoteWorld>(AccountIndex.Value?.RemoteWorlds ?? new ObservableCollection<IRemoteWorld>());
                RemoteDataList.RemoveAll(remote => (remote is RemoteWorld world) && !world.IsVisible);
                RemoteIndex = new BindingValue<IRemoteWorld>(
                    RunWorld.HasRemote ? RunWorld.RemoteWorld : AccountIndex.Value.RemoteWorlds.Last(), 
                    () => OnPropertyChanged(new string[2] { "RemoteIndex", "ShowNewRemoteData" }));
                RemoteName = RunWorld.RemoteWorld?.Name ?? RunWorld.Name;
            }

            // Additionals
            ImportAdditionalsCommand = new ImportAdditionalsCommand(this);
            DeleteAdditionalsCommand = new DeleteAdditionalsCommand(this);
            // Datapack
            IsZipDatapack = new BindingValue<bool>(true, () => OnPropertyChanged("IsZipDatapack"));
            Datapacks = new DatapackCollection(RunWorld.Datapacks);
            SelectedDatapack = new BindingValue<ADatapack>(Datapacks.Datapacks.FirstOrDefault(), () => OnPropertyChanged(""));
            // Plugin
            if (RunVersion is SpigotVersion)
            {
                Plugins = new PluginCollection(RunWorld.Plugins);
                SelectedPlugin = new BindingValue<APlugin>(Plugins.Plugins.FirstOrDefault(), () => OnPropertyChanged(""));

                // TODO: 所定のpluginを使用する設定になっていれば初期値をtrueにする
                //IsCrossPlay = new BindingValue<bool>(false, () => CrossPlay());
            }
            // Custom Map
            CustomMap = new BindingValue<CustomMap>(RunWorld.CustomMap, () => OnPropertyChanged("CustomMap"));

            CheckPlayer();
            // Op (new することで参照渡しにならないようにする)
            Players = new List<Player>(SaveData.Players);
            OpPlayerIndex = Players.FirstOrDefault();
            Groups = new List<PlayerGroup>(SaveData.PlayerGroups)
            {
                new PlayerGroup("(No Group)", null)
            };
            OpGroupIndex = Groups.FirstOrDefault();
            AddOpPlayerCommand = new AddOpPlayerCommand(this);
            OpPlayersList = new ObservableCollection<OpsRecord>(RunWorld.Settings.Ops);

            // WhiteList
            WhitePlayerIndex = Players.FirstOrDefault();
            WhiteGroupIndex = Groups.FirstOrDefault();
            AddWhiteCommand = new AddWhiteCommand(this);
            WhitePlayersList = new ObservableCollection<Player>(RunWorld.Settings.WhiteList);
        }

        private void CheckPlayer()
        {
            List<Player> notExistPlayers = new List<Player>();
            notExistPlayers.AddRange(RunWorld.Settings.Ops.Where(opPlayer => !SaveData.Players.Contains(opPlayer.Player)).Select(opPlayer => opPlayer.Player));
            notExistPlayers.AddRange(RunWorld.Settings.WhiteList.Where(player => !SaveData.Players.Contains(player)));
            
            if (notExistPlayers.Count != 0)
            {
                string message = Properties.Resources.WorldSettings_Player;
                message += string.Join("\n", notExistPlayers.Select(x => $"\n{Properties.Resources.UserName} : {x.Name}\nUUID : {x.UUID}"));
                int result = CustomMessageBox.Show(message, ButtonType.YesNo, Image.Question);
                if (result == 0)
                    SaveData.Players.AddRange(notExistPlayers);
            }
        }

        private string CheckInputBox(string propertyName)
        {
            switch (propertyName)
            {
                case "RemoteName":
                    if (!ValidRemoteName)
                        return "This name is not available";
                    break;
                default:
                    break;
            }

            return "";
        }

        /// <summary>
        /// UseSWが回された際にStoragesの有無と設定画面の表示非表示を管理
        /// </summary>
        private void SetUseSW()
        {
            OnPropertyChanged(new string[3] { "UseSW", "CanEdit", "CanSelectRemoteIndex" });

            if (UseSW != null && UseSW.Value && !HasStorages)
            {
                int result = CustomMessageBox.Show(Properties.Resources.WorldSettings_SW, ButtonType.YesNo, Image.Question);
                if (result == 0)
                {
                    Hide();
                    var window = new ShowNewWindow<SystemSettings.SystemSettings, SystemSettingsVM>();
                    var vm = new SystemSettingsVM();
                    window.ShowDialog(vm);
                    Show();
                    if (HasStorages)
                    {
                        OnPropertyChanged(new string[2] { "Accounts", "RemoteDataList" });
                        AccountIndex.Value = Accounts.FirstOrDefault();
                        RemoteIndex.Value = AccountIndex.Value.RemoteWorlds.Last();
                    }
                }

                UseSW.Value = HasStorages;
            }
        }

        private void UpdateRemoteList()
        {
            if (RemoteDataList != null && AccountIndex.Value != null)
            {
                RemoteDataList.ChangeCollection(AccountIndex.Value.RemoteWorlds);
                RemoteDataList.RemoveAll(remote => (remote is RemoteWorld world) && !world.IsVisible);
                RemoteIndex.Value = RemoteDataList.Last();
            }
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

                // クロスプレイについては一旦保留にし、実装する場合は、ImportにURLを渡せる形にし、チェックボックスを入れたら自動でプラグインが導入されるようにする
            }
            else
            {
                // TODO: クロスプレイに必要なプラグインを削除する
            }
        }

        public void SaveWorldSettings()
        {
            PropertyIndexs.Value = BoolOption.SetBoolOption(BoolOptions, PropertyIndexs.Value);
            PropertyIndexs.Value = TextOption.SetStringOption(TextOptions, PropertyIndexs.Value);
            RunWorld.Settings.ServerProperties = new ServerProperty(PropertyIndexs.Value);

            if (UseSW.Value && !RunWorld.HasRemote)
            {
                // TODO: CreateRemoteWorldするのはRunの後にしないと，再度WorldSettingsを開いた際にすでにSWになったワールドのように表示されてしまう（アカウントやブランチ名の変更ができない）
                if (RemoteIndex.Value is RemoteWorld world)
                    RunWorld.Link(world);
                else
                    RunWorld.Link(AccountIndex.Value.CreateRemoteWorld(RemoteName));
            }
            else if (!UseSW.Value && RunWorld.HasRemote)
            {
                RunWorld.Unlink();
                // AccountIndex.Value.RemoveRemoteWorld()は無いのか
            }

            RunWorld.Datapacks = new DatapackCollection(Datapacks);
            if (RunVersion is SpigotVersion)
                RunWorld.Plugins = new PluginCollection(Plugins);
            RunWorld.CustomMap = CustomMap.Value;

            RunWorld.Settings.Ops = new List<OpsRecord>(OpPlayersList);
            RunWorld.Settings.WhiteList = new List<Player>(WhitePlayersList);
        }

    }
}
