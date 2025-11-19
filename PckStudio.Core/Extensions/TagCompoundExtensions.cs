using System.Numerics;
using Cyotek.Data.Nbt;

namespace PckStudio.Core.Extensions
{
    static class TagCompoundExtensions
    {
        public static Vector3 GetVector3(this TagCompound tag, string name) => tag.TryGetVector3(name, out Vector3 vec3) ? vec3 : default;

        public static bool TryGetVector2(this TagCompound tag, string name, out Vector2 value)
        {
            (string x, string y) = (name + "X", name + "Y");
            bool hasValues = tag.Contains(x) && tag.Contains(y);
            value = hasValues ? new Vector2((float)tag[x].GetValue(), (float)tag[y].GetValue()) : default;
            return hasValues;
        }

        public static bool TryGetVector3(this TagCompound tag, string name, out Vector3 value)
        {
            string z = name + "Z";
            bool hasValues = tag.TryGetVector2(name, out Vector2 vec2) && tag.Contains(z);
            value = hasValues ? new Vector3(vec2, (float)tag[z].GetValue()) : default;
            return hasValues;
        }
    }
}