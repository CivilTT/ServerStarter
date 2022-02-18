using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Util
{
    public static class LogExt
    {
        public static string ToStr<K, V>(this Dictionary<K, V> dict)
        {
            var items = dict.Select(x => {
                var lines = x.Value.ToString().Split('\n').ToList();
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {x.Key} : {lines[0]}{joined}";
            });
            return "{" + string.Join(",", items) + "\n}";
        }
        public static string ToStr<K,V>(this Dictionary<K,V> dict,Func<V,string> func)
        {
            var items = dict.Select(x => {
                var lines = func(x.Value).Split('\n').ToList();
                var joined = string.Join("",lines.Skip(1).Select( y => "\n  " + y));
                return $"\n  {x.Key} : {lines[0]}{joined}";
                });
            return  "{" + string.Join(",", items) + "\n}";
        }
        public static string ToStr<K, V>(this Dictionary<K, V> dict, Func<K, string> keyfunc, Func<V, string> valuefunc)
        {
            var items = dict.Select(x => {
                var lines = valuefunc(x.Value).Split('\n').ToList();
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {keyfunc(x.Key)} : {lines[0]}{joined}";
            });
            return "{" + string.Join(",", items) + "\n}";
        }
        public static string ToStr<V>(this List<V> list)
        {
            var items = list.Select(x => {
                var lines = x.ToString().Split('\n').ToList();
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {lines[0]}{joined}";
            });
            return "[" + string.Join(",", items) + "\n]";
        }
        public static string ToStr<V>(this List<V> list, Func<V, string> func)
        {
            var items = list.Select(x => {
                var lines = func(x).Split('\n').ToList();
                var joined = string.Join("", lines.Skip(1).Select(y => "\n  " + y));
                return $"\n  {lines[0]}{joined}";
            });
            return "[" + string.Join(",", items) + "\n]";
        }
    }
}
