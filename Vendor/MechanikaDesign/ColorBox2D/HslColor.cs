using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MechanikaDesign.WinForms.UI.ColorPicker
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HslColor
    {
        public static readonly HslColor Empty;
        private double hue;
        private double saturation;
        private double luminance;
        private int alpha;

        public HslColor(int a, double h, double s, double l)
        {
            this.alpha = a;
            this.hue = h;
            this.saturation = s;
            this.luminance = l;
            this.A = a;
            this.H = this.hue;
            this.S = this.saturation;
            this.L = this.luminance;
        }

        public HslColor(double h, double s, double l)
        {
            this.alpha = 0xff;
            this.hue = h;
            this.saturation = s;
            this.luminance = l;
        }

        public HslColor(Color color)
        {
            this.alpha = color.A;
            this.hue = 0.0;
            this.saturation = 0.0;
            this.luminance = 0.0;
            this.RGBtoHSL(color);
        }

        public static HslColor FromArgb(int a, int r, int g, int b)
        {
            return new HslColor(Color.FromArgb(a, r, g, b));
        }

        public static HslColor FromColor(Color color)
        {
            return new HslColor(color);
        }

        public static HslColor FromAhsl(int a)
        {
            return new HslColor(a, 0.0, 0.0, 0.0);
        }

        public static HslColor FromAhsl(int a, HslColor hsl)
        {
            return new HslColor(a, hsl.hue, hsl.saturation, hsl.luminance);
        }

        public static HslColor FromAhsl(double h, double s, double l)
        {
            return new HslColor(0xff, h, s, l);
        }

        public static HslColor FromAhsl(int a, double h, double s, double l)
        {
            return new HslColor(a, h, s, l);
        }

        public static bool operator ==(HslColor left, HslColor right)
        {
            return (((left.A == right.A) && (left.H == right.H)) && ((left.S == right.S) && (left.L == right.L)));
        }

        public static bool operator !=(HslColor left, HslColor right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is HslColor)
            {
                HslColor color = (HslColor)obj;
                if (((this.A == color.A) && (this.H == color.H)) && ((this.S == color.S) && (this.L == color.L)))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((this.alpha.GetHashCode() ^ this.hue.GetHashCode()) ^ this.saturation.GetHashCode()) ^ this.luminance.GetHashCode());
        }

        [DefaultValue((double)0.0), Category("Appearance"), Description("H Channel value")]
        public double H
        {
            get
            {
                return this.hue;
            }
            set
            {
                this.hue = value;
                this.hue = (this.hue > 1.0) ? 1.0 : ((this.hue < 0.0) ? 0.0 : this.hue);
            }
        }
        [Category("Appearance"), Description("S Channel value"), DefaultValue((double)0.0)]
        public double S
        {
            get
            {
                return this.saturation;
            }
            set
            {
                this.saturation = value;
                this.saturation = (this.saturation > 1.0) ? 1.0 : ((this.saturation < 0.0) ? 0.0 : this.saturation);
            }
        }
        [Category("Appearance"), Description("L Channel value"), DefaultValue((double)0.0)]
        public double L
        {
            get
            {
                return this.luminance;
            }
            set
            {
                this.luminance = value;
                this.luminance = (this.luminance > 1.0) ? 1.0 : ((this.luminance < 0.0) ? 0.0 : this.luminance);
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color RgbValue
        {
            get
            {
                return this.HSLtoRGB();
            }
            set
            {
                this.RGBtoHSL(value);
            }
        }
        public int A
        {
            get
            {
                return this.alpha;
            }
            set
            {
                this.alpha = (value > 0xff) ? 0xff : ((value < 0) ? 0 : value);
            }
        }
        public bool IsEmpty
        {
            get
            {
                return ((((this.alpha == 0) && (this.H == 0.0)) && (this.S == 0.0)) && (this.L == 0.0));
            }
        }

        public Color ToRgbColor()
        {
            return this.ToRgbColor(this.A);
        }

        public Color ToRgbColor(int alpha)
        {
            double q;
            if (this.L < 0.5)
            {
                q = this.L * (1 + this.S);
            }
            else
            {
                q = this.L + this.S - (this.L * this.S);
            }
            double p = 2 * this.L - q;
            double hk = this.H / 360;

            // r,g,b colors
            double[] tc = new[]
                    {
                      hk + (1d / 3d), hk, hk - (1d / 3d)
                    };
            double[] colors = new[]
                        {
                          0.0, 0.0, 0.0
                        };

            for (int color = 0; color < colors.Length; color++)
            {
                if (tc[color] < 0)
                {
                    tc[color] += 1;
                }
                if (tc[color] > 1)
                {
                    tc[color] -= 1;
                }

                if (tc[color] < (1d / 6d))
                {
                    colors[color] = p + ((q - p) * 6 * tc[color]);
                }
                else if (tc[color] >= (1d / 6d) && tc[color] < (1d / 2d))
                {
                    colors[color] = q;
                }
                else if (tc[color] >= (1d / 2d) && tc[color] < (2d / 3d))
                {
                    colors[color] = p + ((q - p) * 6 * (2d / 3d - tc[color]));
                }
                else
                {
                    colors[color] = p;
                }

                colors[color] *= 255;
            }

            return Color.FromArgb(alpha, (int)colors[0], (int)colors[1], (int)colors[2]);
        }

        private Color HSLtoRGB()
        {
            int num2;
            int red = this.Round(this.luminance * 255.0);
            int blue = this.Round(((1.0 - this.saturation) * (this.luminance / 1.0)) * 255.0);
            double num4 = ((double)(red - blue)) / 255.0;
            if ((this.hue >= 0.0) && (this.hue <= 0.16666666666666666))
            {
                num2 = this.Round((((this.hue - 0.0) * num4) * 1530.0) + blue);
                return Color.FromArgb(this.alpha, red, num2, blue);
            }
            if (this.hue <= 0.33333333333333331)
            {
                num2 = this.Round((-((this.hue - 0.16666666666666666) * num4) * 1530.0) + red);
                return Color.FromArgb(this.alpha, num2, red, blue);
            }
            if (this.hue <= 0.5)
            {
                num2 = this.Round((((this.hue - 0.33333333333333331) * num4) * 1530.0) + blue);
                return Color.FromArgb(this.alpha, blue, red, num2);
            }
            if (this.hue <= 0.66666666666666663)
            {
                num2 = this.Round((-((this.hue - 0.5) * num4) * 1530.0) + red);
                return Color.FromArgb(this.alpha, blue, num2, red);
            }
            if (this.hue <= 0.83333333333333337)
            {
                num2 = this.Round((((this.hue - 0.66666666666666663) * num4) * 1530.0) + blue);
                return Color.FromArgb(this.alpha, num2, blue, red);
            }
            if (this.hue <= 1.0)
            {
                num2 = this.Round((-((this.hue - 0.83333333333333337) * num4) * 1530.0) + red);
                return Color.FromArgb(this.alpha, red, blue, num2);
            }
            return Color.FromArgb(this.alpha, 0, 0, 0);
        }

        private void RGBtoHSL(Color color)
        {
            int r;
            int g;
            double num4;
            this.alpha = color.A;
            if (color.R > color.G)
            {
                r = color.R;
                g = color.G;
            }
            else
            {
                r = color.G;
                g = color.R;
            }
            if (color.B > r)
            {
                r = color.B;
            }
            else if (color.B < g)
            {
                g = color.B;
            }
            int num3 = r - g;
            this.luminance = ((double)r) / 255.0;
            if (r == 0)
            {
                this.saturation = 0.0;
            }
            else
            {
                this.saturation = ((double)num3) / ((double)r);
            }
            if (num3 == 0)
            {
                num4 = 0.0;
            }
            else
            {
                num4 = 60.0 / ((double)num3);
            }
            if (r == color.R)
            {
                if (color.G < color.B)
                {
                    this.hue = (360.0 + (num4 * (color.G - color.B))) / 360.0;
                }
                else
                {
                    this.hue = (num4 * (color.G - color.B)) / 360.0;
                }
            }
            else if (r == color.G)
            {
                this.hue = (120.0 + (num4 * (color.B - color.R))) / 360.0;
            }
            else if (r == color.B)
            {
                this.hue = (240.0 + (num4 * (color.R - color.G))) / 360.0;
            }
            else
            {
                this.hue = 0.0;
            }
        }

        private int Round(double val)
        {
            return (int)(val + 0.5);
        }

        static HslColor()
        {
            Empty = new HslColor();
        }
    }
}
