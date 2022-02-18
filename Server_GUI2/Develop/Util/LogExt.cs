using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Util
{
    public static class LogExt
    {
        public static string ToStr(this object obj)
        {
            if (obj == null) return $"null";
            return obj.ToString();
        }

        public static string ToStr(this string str)
        {
            if (str == null) return $"null";
            return str;
        }

        public static string ToStr<K, V>(this Dictionary<K, V> dict)
        {
            if (dict == null) return $"null";

            var items = dict.Select(x => {
                dynamic xx = x.Value;
                string s = ToStr(xx);
                var lines = Enumerable.ToList(s.Split('\n'));
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {x.Key} : {lines[0]}{joined}";
            });
            return "{" + string.Join(",", items) + "\n}";
        }
        public static string ToStr<K,V>(this Dictionary<K,V> dict,Func<V,string> func)
        {
            if (dict == null) return $"null";

            var items = dict.Select(x => {
                var lines = func(x.Value).Split('\n').ToList();
                var joined = string.Join("",lines.Skip(1).Select( y => "\n  " + y));
                return $"\n  {x.Key} : {lines[0]}{joined}";
                });
            return  "{" + string.Join(",", items) + "\n}";
        }
        public static string ToStr<K, V>(this Dictionary<K, V> dict, Func<K, string> keyfunc, Func<V, string> valuefunc)
        {
            if (dict == null) return $"null";

            var items = dict.Select(x => {
                var lines = valuefunc(x.Value).Split('\n').ToList();
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {keyfunc(x.Key)} : {lines[0]}{joined}";
            });
            return "{" + string.Join(",", items) + "\n}";
        }
        public static string ToStr<V>(this IEnumerable<V> list)
        {
            if (list == null) return $"null";

            var items = list.Select(x => {
                dynamic xx = x;
                string s = ToStr(xx);
                var lines = Enumerable.ToList(s.Split('\n'));
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {lines[0]}{joined}";
            });
            return "[" + string.Join(",", items) + "\n]";
        }
        public static string ToStr<V>(this IEnumerable<V> list, Func<V, string> func)
        {
            if (list == null) return $"null";

            var items = list.Select(x => {
                var lines = func(x).Split('\n').ToList();
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {lines[0]}{joined}";
            });
            return "[" + string.Join(",", items) + "\n]";
        }

        public static void WriteLine(this object obj)
        {
            dynamic xx = obj;
            string s = ToStr(xx);
            Console.WriteLine(s);
        }

        public static void WriteLine<V>(this IEnumerable<V> list, Func<V,string> func)
        {
            string s = ToStr(list, func);
            Console.WriteLine(s);
        }

        public static void WriteLine<K,V>(this Dictionary<K,V> list, Func<V, string> func)
        {
            string s = ToStr(list, func);
            Console.WriteLine(s);
        }

        public static void WriteLine<K,V>(this Dictionary<K,V> list, Func<K, string> keyfunc, Func<V, string> valuefunc)
        {
            string s = ToStr(list, keyfunc,valuefunc);
            Console.WriteLine(s);
        }
    }
}
