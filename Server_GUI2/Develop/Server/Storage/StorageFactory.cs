using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.Storage
{
    class StorageFactory
    {
        public StorageFactory Instance { get; } = new StorageFactory();

        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();
        private StorageFactory()
        {
            Storages.Add(new LocalStorage());
            GitStorage.GetStorages().ForEach(x => Storages.Add(x));
        }
    }
}
