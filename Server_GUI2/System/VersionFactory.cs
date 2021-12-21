using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    public sealed class VersionFactory
    {
        public static VersionFactory _instance = new VersionFactory();
        
        public static VersionFactory GetInstance()
        {
            return _instance;
        }

        private VersionFactory()
        {
            //TODO: initialization
        }

        public void LoadAllVanillaVersions() { }

        public void LoadAllSpigotVersions() { }

        public Version Create() { }
        
        public void Remove(Version version) { }

        //Version[] existingVersions
        //Version[] allVanillaVersions
        //Version[] allSpigotVersions
        //Version activeVersion
        //string[] getVersionNames()
    }
}
