using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.ViewModels
{
    public static class GeneralVM
    {
        public static void Sort<T>(this ObservableCollection<T> collection)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort();

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
    }

    interface IOperateWindows
    {
        Action Show { get; set; }
        Action Hide { get; set; }
        Action Close { get; set; }
    }

    class BindingValue<T>
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                action();
            }
        }

        private readonly Action action;

        public BindingValue(T defaultValue, Action action)
        {
            this.action = action;
            Value = defaultValue;
        }
    }

}
