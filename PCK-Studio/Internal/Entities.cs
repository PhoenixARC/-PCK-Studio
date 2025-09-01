using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PckStudio.Properties;
using PckStudio.Core.Json;

namespace PckStudio.Json
{
    internal class JsonEntities
    {
        [JsonProperty("entries")]
        public List<EntityInfo> Entries { get; set; }
    }

    internal static class Entities
    {
        private static JsonEntities _jsonModelData, _jsonMaterialData, _jsonBehaviourData;
        internal static JsonEntities JsonModelData => _jsonModelData ??= JsonConvert.DeserializeObject<JsonEntities>(Resources.entityModelsData);
        internal static JsonEntities JsonMaterialData => _jsonMaterialData ??= JsonConvert.DeserializeObject<JsonEntities>(Resources.entityMaterialsData);
        internal static JsonEntities JsonBehaviourData => _jsonBehaviourData ??= JsonConvert.DeserializeObject<JsonEntities>(Resources.entityBehavioursData);
        internal static List<EntityInfo> ModelInfos => JsonModelData.Entries;
        internal static List<EntityInfo> MaterialInfos => JsonMaterialData.Entries;
        internal static List<EntityInfo> BehaviourInfos => JsonBehaviourData.Entries;
    }
}
