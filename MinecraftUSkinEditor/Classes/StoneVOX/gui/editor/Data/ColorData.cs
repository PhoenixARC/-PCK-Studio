using OpenTK.Graphics;
using System.ComponentModel;
using System.Drawing;

namespace stonevox
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorData
    {
        private float r;
        private float g;
        private float b;
        private float a;

        public float R
        {
            get
            {
                return r;
            }

            set
            {
                r = value;
            }
        }

        public float G
        {
            get
            {
                return g;
            }

            set
            {
                g = value;
            }
        }

        public float B
        {
            get
            {
                return b;
            }

            set
            {
                b = value;
            }
        }

        public float A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
            }
        }

        public ColorData() { r = 1; g = 1; b = 1; a = 1; }

        public ColorData(Color color)
        {
            r = color.R / 256f;
            g = color.G / 256f;
            b = color.B / 256f;
            a = color.A / 256f;
        }

        public ColorData(Color4 color)
        {
            r = color.R;
            g = color.G;
            b = color.B;
            a = color.A;
        }

        public Color4 ToColor4()
        {
            return new Color4(R,G,B,A);
        }

        public static implicit operator Color4(ColorData color)
        {
            return new Color4(color.R, color.G, color.B, color.A);
        }
        public static implicit operator ColorData(Color4 color)
        {
            return new ColorData(color);
        }

        public static implicit operator Color(ColorData color)
        {
            return Color.FromArgb((int)(color.a * 256f), (int)(color.r * 256f), (int)(color.g * 256f), (int)(color.b * 256));
        }
        public static implicit operator ColorData(Color color)
        {
            return new ColorData(color);
        }
    }

    public static class Color4Extension
    {
        public static ColorData ToColorData(this Color4 color)
        {
            return new ColorData(color);
        }
    }
}
