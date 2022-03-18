using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    public class ReadOnlyProperty<T>
    {
        public T Value => getter();
        private Func<T> getter;
        public ReadOnlyProperty(Func<T> getter)
        {
            this.getter = getter;
        }

        public ReadOnlyProperty<U> Fmap<U>(Func<T,U> func)
        {
            return new ReadOnlyProperty<U>(() => func(getter()));
        }
    }
}
