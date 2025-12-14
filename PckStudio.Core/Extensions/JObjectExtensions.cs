using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PckStudio.Core.Extensions
{
    internal static class JObjectExtensions
    {
        public static bool TryGetValue<T>(this JObject jObject, string propertyName, out T value)
        {
            if (!jObject.TryGetValue(propertyName, out JToken token))
            {
                value = default;
                return false;
            }
            value = token.ToObject<T>() ?? default;
            return value is T;
        }
    }
}
