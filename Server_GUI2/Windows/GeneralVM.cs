using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Windows.ViewModels
{
    class GeneralVM
    {
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

        private Action action;

        public BindingValue(T defaultValue, Action action)
        {
            this.action = action;
            Value = defaultValue;
        }
    }

}
