using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Internal.Json
{
    internal class JsonModelMetaData
    {
        [JsonProperty("textureLocations", Required = Required.Always)]
        public string[] TextureLocations { get; set; }

        //[JsonProperty("parents", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Populate)]
        //public Dictionary<string, string> ParentBones { get; set; }
    }
}
