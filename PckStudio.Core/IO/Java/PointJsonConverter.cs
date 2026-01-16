using System;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PckStudio.Core.IO.Java
{
    public class PointJsonConverter : JsonConverter<Point>
    {
        public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo.Add("x", value.X);
            jo.Add("y", value.Y);
            jo.WriteTo(writer);
        }

        public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new Point((int)(jo["x"] ?? 0), (int)(jo["y"] ?? 0));
        }
    }
}