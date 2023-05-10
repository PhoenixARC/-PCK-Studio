using System;
using System.Drawing;
using System.Numerics;

namespace PckStudio.Extensions
{
    internal static class ColorExtensions
    {
        /// <summary>
        /// Normalizes the Color between 0.0 - 1.0
        /// </summary>
        /// <returns></returns>
        public static Vector4 Normalize(this Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        public static byte BlendValues(float source, float overlay, BlendMode blendType)
        {
            source = Clamp(source, 0.0f, 1.0f);
            overlay = Clamp(overlay, 0.0f, 1.0f);
            float resultValue = blendType switch
            {
                BlendMode.Add => source + overlay,
                BlendMode.Subtract => source - overlay,
                BlendMode.Multiply => source * overlay,
                BlendMode.Average => (source + overlay) / 2.0f,
                BlendMode.AscendingOrder => source > overlay ? overlay : source,
                BlendMode.DescendingOrder => source < overlay ? overlay : source,
                BlendMode.Screen => 1f - (1f - source) * (1f - overlay),
                _ => 0.0f
            };
            return (byte)Clamp(resultValue * 255, 0, 255);
        }

        public static byte Mix(double ratio, byte val1, byte val2)
        {
            return (byte)(ratio * val1 + (1.0 - ratio) * val2);
        }

        public static Color Mix(this Color c1, Color c2, double ratio)
        {
            ratio = Clamp(ratio, 0.0, 1.0);
            return Color.FromArgb(c1.A,
                Mix(ratio, c1.R, c2.R),
                Mix(ratio, c1.G, c2.G),
                Mix(ratio, c1.B, c2.B)
                );
        }
    }
}
