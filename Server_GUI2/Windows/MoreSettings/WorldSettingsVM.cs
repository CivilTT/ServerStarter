using Server_GUI2.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Server_GUI2.Windows.SystemSettings;
using Server_GUI2.Develop.Server.World;

namespace Server_GUI2.Windows.MoreSettings
{
    class WorldSettingsVM : GeneralVM
    {
        static readonly UserSettingsJson SaveData = UserSettings.Instance.userSettings;
        static readonly StorageCollection Storages = StorageCollection.Instance;

        public Version RunVersion;
        public World RunWorld;

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
        public bool ShowNewRemoteData => RemoteIndex.Value == /*//TODO: 何とイコールにすればよい？//*/null;


        // DataPack
        public bool IsZipDatapack { get; set; } = true;
        public ObservableCollection<ADatapack> Datapacks { get; private set; }




        //Op
        public ObservableCollection<OpPlayer> OpPlayersList = new ObservableCollection<OpPlayer>();



        public WorldSettingsVM(Version runVer, World runWor)
        {
            RunVersion = runVer;
            RunWorld = runWor;

            // General
            MenuIndex = new BindingValue<int>(0, () => OnPropertyChanged(new string[5] { "ShowProp", "ShowSW", "ShowAdd", "ShowOp", "ShowWhite" }));
            SaveCommand = new SaveCommand(this);

            // ServerProperty
            ServerProperty properties = RunWorld.Property;
            PropertyIndexs = new BindingValue<ServerProperty>(properties, () => OnPropertyChanged("PropertyIndexs"));
            SelectedTFIndex = new BindingValue<string>(OtherTFPropertyIndexs[0], () => OnPropertyChanged("SelectedTFProperty"));
            SelectedPropIndex = new BindingValue<string>(OtherPropertyIndexs[0], () => OnPropertyChanged("OtherStringProperty"));

            // ShareWorld
            // TODO: 既存ワールドをリモート化するときには【new Remote Data】しか選べないようにする
            UseSW = new BindingValue<bool>(RunWorld.HasRemote, () => OnPropertyChanged(""));
            Accounts = Storages.Storages;
            AccountIndex = new BindingValue<Storage>(Accounts[0], () => OnPropertyChanged("RemoteDataList"));
            RemoteDataList = AccountIndex.Value.RemoteWorlds;


            // Datapack
            Datapacks = RunWorld.Datapacks.Datapacks;



        }

    }

    public class OpPlayer : Player
    {
        public int OpLevel;
        public bool BypassesPlayerLimit;

        public OpPlayer(string name, int opLevel, bool bypassesPlayerLimit=false) : base(name)
        {
            OpLevel = opLevel;
            BypassesPlayerLimit = bypassesPlayerLimit;
        }
    }
}
