using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Server
{
    public enum ServerType
    {
        Vanilla,
        Spigot
    }

    public static class ServerTypeExt
    {
        public static string ToStr(this ServerType state)
        {
            return state == ServerType.Vanilla ? "Vanilla" : "Spigot";
        }

        public static ServerType FromStr(string str)
        {
            switch (str)
            {
                case "Vanilla":
                    return ServerType.Vanilla;
                case "Spigot":
                    return ServerType.Spigot;
                default:
                    throw new ArgumentException($"\"{str}\" is invalid string for ServerType");
            }
        }
    }
}
