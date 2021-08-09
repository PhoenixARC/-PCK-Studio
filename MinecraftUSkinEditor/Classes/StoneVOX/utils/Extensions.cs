using OpenTK;
using OpenTK.Graphics;
using System;
using System.Drawing;

namespace stonevox
{
    public static class Extensions
    {
        public static int SafeToInt32(this string text)
        {
            try
            {
                return Convert.ToInt32(text);
            }
            catch
            {
                return -1;
            }
        }

        public static float SafeToFloat(this string text)
        {
            try
            {
                return Convert.ToSingle(text);
            }
            catch
            {
                return -1;
            }
        }

        public static Vector3 lerp (this Vector3 a, Vector3 to, float time)
        {
            return a + ((to - a) * time);
        }

        public static Vector3 negated(this Vector3 a)
        {
            return a * -1f;
        }

        public static float dot(this Vector3 a, Vector3 other)
        {
            return a.X * other.X + a.Y * other.Y + a.Z * other.Z;
        }

        public static Vector3 unit(this Vector3 a)
        {
            return a / a.Length;
        }

        public static Vector3 cross(this Vector3 me, Vector3 a)
        {
            return new Vector3(
               me.Y * a.Z - me.Z * a.Y,
                me.Z * a.X - me.X * a.Z,
                 me.X * a.Y - me.Y * a.X
                );
        }

        public static Color4 Add(this Color4 color, ref Color4 other)
        {
            return new Color4()
            {
                A = color.A + other.A,
                R = color.R + other.R,
                G = color.G + other.G,
                B = color.B + other.B,
            };
        }

        public static Color4 Add(this Color4 color, Color4 other)
        {
            return new Color4()
            {
                A = color.A + other.A,
                R = color.R + other.R,
                G = color.G + other.G,
                B = color.B + other.B,
        };
        }

        public static Color ToSystemDrawingColor(this Color4 color)
        {
            return Color.FromArgb((int)(color.A * 255f), (int)(color.R * 255f), (int)(color.G * 255f), (int)(color.B * 255f));
        }

        public static Color4 ToColor4(this Color color)
        {
            return new Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static float ScaleHorizontal(this float value)
        {
            return Scale.hPosScale(value);
        }

        public static float ScaleHorizontalSize(this float value)
        {
            return Scale.hSizeScale(value);
        }
        public static float ScaleVertical(this float value)
        {
            return Scale.vPosScale(value);
        }

        public static float ScaleVerticlSize(this float value)
        {
            return Scale.vSizeScale(value);
        }
        public static float UnScaleHorizontal(this float value)
        {
            return Scale.hUnPosScale(value);
        }

        public static float UnScaleHorizontalSize(this float value)
        {
            return Scale.hUnSizeScale(value);
        }
        public static float UnScaleVertical(this float value)
        {
            return Scale.vUnPosScale(value);
        }

        public static float UnScaleVerticlSize(this float value)
        {
            return Scale.vUnSizeScale(value);
        }

        public static Bitmap ResizeImage(this Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }

        public static double Clamp(this double value, double min, double max)
        {
            if (value >= max)
                return max;
            else if (value <= min)
                return min;
            return value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            if (value >= max)
                return max;
            else if (value <= min)
                return min;
            return value;
        }
    }
}
