using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    public class Player : IEquatable<Player>, IComparable<Player>
    {
        [JsonProperty("Name")]
        public string Name { get; private set; }
        [JsonProperty("UUID")]
        public string UUID { get; protected set; }

        public Player(string name, string uuid = "")
        {
            Name = name;
            UUID = uuid;
        }

        public void GetUuid()
        {
            string url = $@"https://api.mojang.com/users/profiles/minecraft/{Name}";
            WebClient wc = new WebClient();
            string jsonStr = wc.DownloadString(url);

            dynamic root = JsonConvert.DeserializeObject(jsonStr);
            if (root == null)
                return;

            string uuid = root.id;
            Name = root.name;

            string uuid_1 = uuid.Substring(0, 8);
            string uuid_2 = uuid.Substring(8, 4);
            string uuid_3 = uuid.Substring(12, 4);
            string uuid_4 = uuid.Substring(16, 4);
            string uuid_5 = uuid.Substring(20);
            uuid = uuid_1 + "-" + uuid_2 + "-" + uuid_3 + "-" + uuid_4 + "-" + uuid_5;

            UUID = uuid;
        }

        public bool Equals(Player other)
        {
            return other.UUID == UUID;
        }

        public int CompareTo(Player other)
        {
            return Name.CompareTo(other.Name);
        }
    }

}
