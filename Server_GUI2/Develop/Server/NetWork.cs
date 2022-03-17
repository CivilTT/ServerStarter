using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server
{
    class NetWorkException : Exception
    {
        public NetWorkException(string message = "network not accessible") : base(message) { }
    }

    static class NetWork
    {
        public static bool Accessible { get; }
    }
}
