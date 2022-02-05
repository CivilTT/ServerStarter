using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;

namespace Server_GUI2.Develop.Server.Storage
{
    public class StorageFactory
    {
        public static StorageFactory Instance { get; } = new StorageFactory();

        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();

        private StorageFactory()
        {
            GitStorage.GetStorages().ForEach(x => Storages.Add(x));
        }

        public void AddStorage(Storage storage)
        {
            Storages.Add(storage);
        }

        /// <summary>
        /// リモートワールドをjsonをもとに返す関数を返す
        /// </summary>
        public ReadOnlyProperty<RemoteWorld> RemoteWorldFromId(string id)
        {
            var segments = id.Split('/').ToList();
            var storageId = String.Join("/",segments.GetRange(0, segments.Count - 1));
            return new ReadOnlyProperty<RemoteWorld>(() => Storages.Where
                (
                    x => x.Id == storageId
                ).FirstOrDefault()?.Worlds.Where
                (
                    x => x.Id == id
                ).FirstOrDefault());
        }
    }
}
