using Server_GUI2.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.ViewModels
{
    abstract class GeneralVM : INotifyPropertyChanged, IOperateWindows
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Action Show { get; set; }
        public Action Hide { get; set; }
        public Action Close { get; set; }

        /// <summary>
        /// PropertyChanged?.Invokeの記述を省略する
        /// </summary>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void OnPropertyChanged(string[] propertyList)
        {
            foreach (string property in propertyList)
                OnPropertyChanged(property);
        }

        /// <summary>
        /// GUI上で情報が入力されているか否かを確認する
        /// 引数を複数取る場合、全ての変数に対してAND検証した結果を返す
        /// </summary>
        /// <returns>情報がある場合はtrue, ない場合やそもそも検査対象の変数がnullの時はfalse</returns>
        protected bool CheckHasContent<T>(BindingValue<T> bindingValue, T checkValue)
        {
            return bindingValue != null && !EqualityComparer<T>.Default.Equals(bindingValue.Value, checkValue) && bindingValue.Value != null;
        }
        protected bool CheckHasContent<T>(List<BindingValue<T>> bindingValues, T checkValue)
        {
            foreach (var binding in bindingValues)
                if (!CheckHasContent(binding, checkValue)) return false;

            return true;
        }
        protected bool CheckHasContent<T>(ObservableCollection<T> collection)
        {
            return collection != null && collection.Count != 0;
        }
    }

    public static class AdditionalCollectionFuncs
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

        public static void AddRange<T>(this ObservableCollection<T> sourceCollection, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                sourceCollection.Add(item);
            }
        }

        public static void RemoveAll<T>(this ObservableCollection<T> collection, Predicate<T> match)
        {
            foreach (var item in collection)
            {
                if (match(item))
                    collection.Remove(item);
            }
        }

        /// <summary>
        /// コレクションの中身のデータを全て入れ替える
        /// </summary>
        public static void ChangeCollection<T>(this ObservableCollection<T> sourceCollection, IEnumerable<T> collection)
        {
            sourceCollection.Clear();
            sourceCollection.AddRange(collection);
        }
    }

    interface IOperateWindows
    {
        Action Show { get; set; }
        Action Hide { get; set; }
        Action Close { get; set; }
    }

    public class BindingValue<T>
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
