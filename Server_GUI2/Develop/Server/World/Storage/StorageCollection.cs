using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;

namespace Server_GUI2.Develop.Server.World
{
    public class StorageCollection
    {
        public static StorageCollection Instance { get; } = new StorageCollection();

        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();

        private StorageCollection()
        {
            // gitのリポジトリを全取得
            GitStorage.GetStorages().ForEach(x => Storages.Add(x));
        }

        // TODO: リモートリポジトリから消えた場合とリモートリポジトリと通信できない場合エラーを吐く
        public Storage FindStorage(string storage)
        {
            var storageValue = Storages.Where(x => x.Id == storage).First();
            return storageValue;
        }

        public void Add(Storage storage)
        {
            Storages.Add(storage);
        }
    }
}
