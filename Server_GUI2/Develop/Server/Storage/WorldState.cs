using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Server_GUI2.Develop.Server.Storage
{
    public class WorldState
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class NewWorldState : WorldState { }

    public class ExistWorldState : WorldState
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("using")]
        [DefaultValue(false)]
        public bool Using { get; set; }
        [JsonProperty("datapacks")]
        public List<string> Datapacks { get; set; } = new List<string>();
        [JsonProperty("properties")]
        [DefaultValue(null)]
        public ServerProperty ServerProperty { get; set; } = null;
    }

    class VanillaWorldState : ExistWorldState { }

    class SpigotWorldState : ExistWorldState { }

    public class JsonWorldStateConverter : CustomCreationConverter<WorldState>
    {
        public override WorldState Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public WorldState Create(Type objectType, JObject jObject)
        {
            var type = (string)jObject.Property("type");
            switch (type)
            {
                case "new":
                    return JsonConvert.DeserializeObject<NewWorldState>(jObject.ToString());
                case "vanilla":
                    return JsonConvert.DeserializeObject<VanillaWorldState>(jObject.ToString());
                case "spigot":
                    return JsonConvert.DeserializeObject<SpigotWorldState>(jObject.ToString());
                default:
                    break;
            }
            throw new ApplicationException($"The given vehicle type {type} is not supported!");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Load JObject from stream
            JObject jObject = JObject.Load(reader);

            //Create target object based on JObject
            var target = Create(objectType, jObject);

            //Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }
    }
}
