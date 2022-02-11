using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server.World
{
    public interface IWorldBase
    {
        DatapackCollection Datapacks { get;}
        // TODO: pluginの読み込み
        //public ObservableCollection<Datapack> Pligins { get; }
        ServerProperty Property { get; }
        ServerType? Type { get; }
        string Name { get; }
        Version Version { get; }
    }
}
