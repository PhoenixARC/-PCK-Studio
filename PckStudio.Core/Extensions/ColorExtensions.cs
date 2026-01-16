using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        public static byte Mix(float ratio, byte val1, byte val2)
        {
            ratio = MathExtensions.Clamp(ratio, 0.0f, 1.0f);
            return (byte)(ratio * val1 + (1.0 - ratio) * val2);
        }

        public static Image ToGreyScale(this Image source, out Color avgColor)
        {
            Bitmap bm = new Bitmap(source);
            BitmapData srcData = bm.LockBits(
            new Rectangle(0, 0, bm.Width, bm.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;

            IntPtr scan0 = srcData.Scan0;

            long[] totals = new long[] { 0, 0, 0 };

            int width = bm.Width;
            int height = bm.Height;
            int pixelCount = width * height;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * 4;

                        byte r = p[idx + 0];
                        byte g = p[idx + 1];
                        byte b = p[idx + 2];

                        totals[idx + 0] += r;
                        totals[idx + 1] += g;
                        totals[idx + 2] += b;

                        byte gs = (byte)((r + g + b) / 3f);
                        p[idx + 0] = gs;
                        p[idx + 1] = gs;
                        p[idx + 2] = gs;
                    }
                }
            }
            bm.UnlockBits(srcData);

            int avgB = (int)(totals[0] / pixelCount);
            int avgG = (int)(totals[1] / pixelCount);
            int avgR = (int)(totals[2] / pixelCount);
            avgColor = Color.FromArgb(avgR, avgG, avgB);
            return bm;
        }

        public static string ToHTMLColor(this Color color) => $"#{color.ToArgb().ToString("X").Substring(2)}";

        public static Color Mix(this Color c1, Color c2, float ratio)
        {
            ratio = MathExtensions.Clamp(ratio, 0.0f, 1.0f);
            return Color.FromArgb(c1.A,
                Mix(ratio, c1.R, c2.R),
                Mix(ratio, c1.G, c2.G),
                Mix(ratio, c1.B, c2.B)
                );
        }
    }
}
