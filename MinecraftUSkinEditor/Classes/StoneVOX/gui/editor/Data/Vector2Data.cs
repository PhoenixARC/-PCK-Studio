using OpenTK;
using System.ComponentModel;

namespace stonevox
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Vector2Data
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2Data() { }

        public static implicit operator Vector2 (Vector2Data vecData)
        {
            return new Vector2(vecData.X, vecData.Y);
        }

        public static implicit operator Vector2Data (Vector2 vec2)
        {
            return new Vector2Data()
            {
                X = vec2.X,
                Y = vec2.Y
            };
        }
    }
}
