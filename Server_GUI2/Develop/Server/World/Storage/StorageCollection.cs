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
    public class StorageFactory
    {
        public static StorageFactory Instance { get; } = new StorageFactory();

        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();

        private StorageFactory()
        {
            // gitのリポジトリを全取得
            GitStorage.GetStorages().ForEach(x => Storages.Add(x));
        }

        public void Add(Storage storage)
        {
            Storages.Add(storage);
        }
    }
}
