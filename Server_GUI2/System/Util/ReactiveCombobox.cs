using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Server_GUI2
{
    class ReactiveCombobox<Type>
    {
        private readonly ComboBox Combobox;
        private readonly List<Type> Items;
        private Func<Type, bool> Filter;
        public ReactiveCombobox(ComboBox combobox,List<Type> items)
        {
            Combobox = combobox;
            Items = items;
        }

        public void SetFilter(Func<Type,bool> func)
        {
            Filter = func;
        }

        private void Reflesh()
        {
            // フィルタ適用したリストをインデックスとともに保存(インデックスはRemove時に使う)
            Items.Select((item, index) => (item, index)).Where(item => Filter(item.item));
        }
    }
}

