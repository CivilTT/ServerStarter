using Server_GUI2.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Server_GUI2.Windows
{
    abstract class GeneralVM : INotifyPropertyChanged, IOperateWindows
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Action Show { get; set; }
        public Action ShowDialog { get; set; }
        public Action Hide { get; set; }
        public Action Close { get; set; }

        /// <summary>
        /// PropertyChanged?.Invokeの記述を省略する
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void OnPropertyChanged(string[] propertyList)
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
            var _collection = new ObservableCollection<T>(collection.Where(x => !match(x)));
            collection.ChangeCollection(_collection);
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
        Action ShowDialog { get; set; }
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

    public class InverseBoolConverter : IValueConverter
    {
        // 2.Convertメソッドを実装
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 流れてきた値がboolじゃない時は不正値として変換
            // お好みで例外を投げても良い
            if (!(value is bool b)) { return DependencyProperty.UnsetValue; }

            // 流れてきたbool値を変換してreturnする
            return !b;
        }

        // 3.ConvertBackメソッドを実装
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ただの反転なのでBinding元に書き戻すときも全く同様の処理で良い
            if (!(value is bool b)) { return DependencyProperty.UnsetValue; }
            return !b;
        }
    }

    /// <summary>
    /// 複数のConverterを取れるようにする
    /// </summary>
    [ContentProperty(nameof(Converters))]
    public class ValueConverterGroup : IValueConverter
    {
        public Collection<IValueConverter> Converters { get; } = new Collection<IValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (Converters == null) return result;

            foreach (var conv in Converters)
            {
                result = conv.Convert(result, targetType, parameter, culture);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (Converters == null) return result;

            foreach (var conv in Converters.Reverse())
            {
                result = conv.ConvertBack(result, targetType, parameter, culture);
            }

            return result;
        }
    }

}
