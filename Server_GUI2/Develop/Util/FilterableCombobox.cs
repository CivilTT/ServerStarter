using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Server_GUI2
{
    // 任意のクラスを要素に持ち、フィルタ適用できるCombobox
    class FilterableCombobox<Type>
    {
        private readonly ComboBox Combobox;
        private readonly List<Type> Items;
        private Func<Type, bool> Filter;
        public Type SelectedItem
        {
            get { return (Type)Combobox.SelectedItem; }
        }

        public FilterableCombobox(ComboBox combobox,List<Type> items)
        {
            Combobox = combobox;
            Items = items;
            Filter = x => true;
            Reflesh();
        }

        public void SetFilter(Func<Type,bool> func)
        {
            Filter = func;
        }

        // 現在選択中の要素をComboboxから取り除きreturnする
        public Type PopSelected()
        {
            var selected = SelectedItem;
            // remove selected item from Combobox
            Items.Remove(selected);
            Reflesh();
            // set Combobox unselected
            Combobox.SelectedIndex = -1;
            return selected;
        }
        private void Reflesh()
        {
            Combobox.Items.Clear();
            // フィルタ適用したリストをComboboxに追加
            Items.Where(item => Filter(item)).Select(x => Combobox.Items.Add(x));
        }
    }
}

