using System.Drawing;
using System.Numerics;

namespace PckStudio.Core.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Normalizes the Color between 0.0 - 1.0
        /// </summary>
        /// <returns></returns>
        public static Vector4 Normalize(this Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static Color Inversed(this Color color)
        {
            return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }

        public static Color GreyScaled(this Color color)
        {
            int greyScaleValue = (color.R + color.G + color.B) / 3;
            return Color.FromArgb(color.A, greyScaleValue, greyScaleValue, greyScaleValue);
        }

        public static int ToBGR(this Color color)
        {
            return color.B << 16 | color.G << 8 | color.R;
        }

        public static byte BlendValues(byte source, byte overlay, BlendMode blendType)
        {
            return (byte)MathExtensions.Clamp(BlendValues(source / 255f, overlay / 255f, blendType) * 255, 0, 255);
        }

        public static float BlendValues(float source, float overlay, BlendMode blendType)
        {
            source = MathExtensions.Clamp(source, 0.0f, 1.0f);
            overlay = MathExtensions.Clamp(overlay, 0.0f, 1.0f);
            float resultValue = blendType switch
            {
                BlendMode.Add => source + overlay,
                BlendMode.Subtract => source - overlay,
                BlendMode.Multiply => source * overlay,
                BlendMode.Average => (source + overlay) / 2.0f,
                BlendMode.AscendingOrder => source > overlay ? overlay : source,
                BlendMode.DescendingOrder => source < overlay ? overlay : source,
                BlendMode.Screen => 1f - (1f - source) * (1f - overlay),
                BlendMode.Overlay => source < 0.5f ? 2f * source * overlay : 1f - 2f * (1f - source) * (1f - overlay),
                _ => 0.0f
            };
            return MathExtensions.Clamp(resultValue, 0.0f, 1.0f);
        }

        public static byte Mix(double ratio, byte val1, byte val2)
        {
            ratio = MathExtensions.Clamp(ratio, 0.0, 1.0);
            return (byte)(ratio * val1 + (1.0 - ratio) * val2);
        }

        public static Color Mix(this Color c1, Color c2, double ratio)
        {
            ratio = MathExtensions.Clamp(ratio, 0.0, 1.0);
            return Color.FromArgb(c1.A,
                Mix(ratio, c1.R, c2.R),
                Mix(ratio, c1.G, c2.G),
                Mix(ratio, c1.B, c2.B)
                );
        }
    }
}
