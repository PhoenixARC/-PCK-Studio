using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MechanikaDesign.WinForms.UI.ColorPicker
{
    public static class ColorHelper
    {
        // r,g,b values are from 0 to 1
        // h = [0,360], s = [0,1], v = [0,1]
        //              if s == 0, then h = -1 (undefined)
        /// <summary>
        /// Generates the Hue, Saturation, and Value for a given color
        /// </summary>
        /// <param name="color">The Color to generate the values for</param>
        /// <param name="hue">Out value for Hue</param>
        /// <param name="saturation">Out value for Saturation</param>
        /// <param name="value">Out value for Value</param>
        public static void HSVFromRGB(Color color, out double hue, out double saturation, out double value)
        {
            double min, max, delta, r, g, b;
            r = (double)color.R / 255d;
            g = (double)color.G / 255d;
            b = (double)color.B / 255d;

            min = Math.Min(r, Math.Min(g, b));
            max = Math.Max(r, Math.Max(g, b));
            value = max;                               // v
            delta = max - min;
            if (max != 0)
                saturation = delta / max;               // s
            else
            {
                // r = g = b = 0                // s = 0, v is undefined
                saturation = 0;
                hue = -1;
                return;
            }
            if (r == max)
                hue = (g - b) / delta;         // between yellow & magenta
            else if (g == max)
                hue = 2 + (b - r) / delta;     // between cyan & yellow
            else
                hue = 4 + (r - g) / delta;     // between magenta & cyan
            hue *= 60;                               // degrees
            if (hue < 0)
                hue += 360;
        }


        /// <summary>
        /// Generates a Color from a Hue, Saturation, and Value combination
        /// </summary>
        /// <param name="hue">Hue to use for the Color. (Max 360)</param>
        /// <param name="saturation">Saturation to use for the Color. (Max 1.0)</param>
        /// <param name="value">Value to use for the Color. (Max 1.0)</param>
        /// <returns>Generated Color</returns>
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            try
            {
                int i;
                double f, p, q, t, r, g, b;

                if (saturation == 0)
                {
                    // achromatic (grey)
                    r = g = b = value;
                    return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
                }
                hue /= 60;                        // sector 0 to 5
                i = (int)Math.Floor(hue);
                f = hue - i;                      // factorial part of h
                p = value * (1 - saturation);
                q = value * (1 - saturation * f);
                t = value * (1 - saturation * (1 - f));
                switch (i)
                {
                    case 0:
                        r = value;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = value;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = value;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = value;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = value;
                        break;
                    default:                // case 5:
                        r = value;
                        g = p;
                        b = q;
                        break;
                }
                return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
            }
            catch
            {

            }
            return Color.Empty;
        }


    }
}
