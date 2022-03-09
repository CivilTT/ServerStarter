using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    class RemovableItem
    {
        /// <summary>
        /// 所属するRemovableCollectionからこの要素を削除する
        /// </summary>
        public void Remove()
        {

        }
    }

    class RemovableCollection<T>:ObservableCollection<T> where T : RemovableItem
    {
        
    }
}
