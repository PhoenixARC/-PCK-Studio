using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PckStudio.Core.IO.Java
{
    public class McMeta : Dictionary<string, JObject>
    {
        private McMeta(Dictionary<string, JObject> type) : base(type)
        {
        }

        public bool Contains(string name) => ContainsKey(name);

        public static McMeta LoadMcMeta(string mcMetaJson)
        {
            JObject a = JObject.Parse(mcMetaJson);
            Dictionary<string, JObject> types = new Dictionary<string, JObject>();
            foreach (KeyValuePair<string, JToken> item in a)
            {
                types.Add(item.Key, item.Value.ToObject<JObject>());
            }
            return new McMeta(types);
        }
    }
}
