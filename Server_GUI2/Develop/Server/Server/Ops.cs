﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_GUI2
{
    /// <summary>
    /// ops.jsonのレコード
    /// </summary>
    public class OpsRecord
    {
        [JsonProperty("uuid")]
        public string UUID;
        
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("BypassesPlayerLimit")]
        public bool bypassesPlayerLimit;
    }
}