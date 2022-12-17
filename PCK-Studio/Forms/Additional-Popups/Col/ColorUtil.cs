using System;

namespace ColorPicker
{
    /// <summary>Utilitats per les conversions de color</summary>
    public static class ColorUtil
    {
        #region Public Static Methods

        /// <summary>
        /// Converts from CMYK to RGB
        /// CMYK [0; 255]
        /// RGB [0; 255]
        /// --------------------------------------------
        /// Red   = 1-minimum(1,Cyan*(1-Black)+Black)   
        /// Green = 1-minimum(1,Magenta*(1-Black)+Black)
        /// Blue  = 1-minimum(1,Yellow*(1-Black)+Black) 
        /// </summary>
        /// <param name="c">Cyan</param>
        /// <param name="m">Magenta</param>
        /// <param name="y">Yellow</param>
        /// <param name="k">Black</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public static void CMYK2RGB(int c, int m, int y, int k, out int r, out int g, out int b)
        {
            double dk;

            dk = (double)k / 255.0;
            r = (int)Math.Round((1.0 - Math.Min(1.0, (((double)c / 255.0) * (1.0 - dk)) + dk)) * 255.0);
            g = (int)Math.Round((1.0 - Math.Min(1.0, (((double)m / 255.0) * (1.0 - dk)) + dk)) * 255.0);
            b = (int)Math.Round((1.0 - Math.Min(1.0, (((double)y / 255.0) * (1.0 - dk)) + dk)) * 255.0);
        }

        /// <summary>
        /// Converts from HSV to RGB
        /// H [0.0; 360.0]
        /// SV [0.0; 1.0]
        /// RGB [0; 255]
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public static void HSV2RGB(double h, double s, double v, out int r, out int g, out int b)
        {
            double dr, dg, db;

            if (v <= 0)
            {
                dr = 0.0;
                dg = 0.0;
                db = 0.0;
            }
            else if (s <= 0)
            {
                dr = v;
                dg = v;
                db = v;
            }
            else
            {
                double hf = h / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = v * (1.0 - s);
                double qv = v * (1.0 - s * f);
                double tv = v * (1.0 - s * (1.0 - f));
                switch (i)
                {
                    case 0: // Red is the dominant color
                        dr = v;
                        dg = tv;
                        db = pv;
                        break;
                    case 1: // Green is the dominant color
                        dr = qv;
                        dg = v;
                        db = pv;
                        break;
                    case 2:
                        dr = pv;
                        dg = v;
                        db = tv;
                        break;
                    case 3: // Blue is the dominant color
                        dr = pv;
                        dg = qv;
                        db = v;
                        break;
                    case 4:
                        dr = tv;
                        dg = pv;
                        db = v;
                        break;
                    case 5: // Red is the dominant color
                        dr = v;
                        dg = pv;
                        db = qv;
                        break;
                    case 6: // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                        dr = v;
                        dg = tv;
                        db = pv;
                        break;
                    case -1:
                        dr = v;
                        dg = pv;
                        db = qv;
                        break;
                    default: // The color is not defined, we should throw an error.
                        dr = dg = db = v; // Just pretend its black/white
                        break;
                }
            }
            r = Math.Min(255, Math.Max(0, (int)(dr * 255.0)));
            g = Math.Min(255, Math.Max(0, (int)(dg * 255.0)));
            b = Math.Min(255, Math.Max(0, (int)(db * 255.0)));
        }

        /// <summary>
        /// Convers from RGB to CMYK
        /// CMYK [0; 255]
        /// RGB [0; 255]
        /// RGB --> CMYK                           
        /// ---------------------------------------
        /// Black   = minimum(1-Red,1-Green,1-Blue)
        /// Cyan    = (1-Red-Black)/(1-Black)      
        /// Magenta = (1-Green-Black)/(1-Black)    
        /// Yellow  = (1-Blue-Black)/(1-Black)     
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <param name="c">Cyan</param>
        /// <param name="m">Magenta</param>
        /// <param name="y">Yellow</param>
        /// <param name="k">Black</param>
        public static void RGB2CMYK(int r, int g, int b, out int c, out int m, out int y, out int k)
        {
            double dr, dg, db;
            double dk, idk;

            dr = 1.0 - (r / 255.0);
            dg = 1.0 - (g / 255.0);
            db = 1.0 - (b / 255.0);

            dk = Math.Min(dr, Math.Min(dg, db));
            idk = 1.0 - dk;
            if (-1e-12 < idk && idk < 1e-12) // check if idk is zero (or very close to zero)
            {
                c = 0;
                m = 0;
                y = 0;
                k = 255;
            }
            else
            {
                c = (int)Math.Round(255.0 * (dr - dk) / idk);
                m = (int)Math.Round(255.0 * (dg - dk) / idk);
                y = (int)Math.Round(255.0 * (db - dk) / idk);
                k = (int)Math.Round(255.0 * dk);
            }
        }

        /// <summary>
        /// Converts from RGB to HSV
        /// RGB [0; 255]
        /// H [0.0; 360.0]
        /// SV [0.0; 1.0]
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        public static void RGB2HSV(int r, int g, int b, out double h, out double s, out double v)
        {
            double dr = r / 255.0;
            double dg = g / 255.0;
            double db = b / 255.0;

            double max = Math.Max(dr, Math.Max(dg, db));
            double min = Math.Min(dr, Math.Min(dg, db));

            v = max;

            if (max == min)
            {
                h = 0.0;
                s = 0.0;
            }
            else
            {
                double dc = max - min;

                if (max == dr)
                    h = (dg - db) / dc;
                else if (max == dg)
                    h = (db - dr) / dc + 2.0;
                else
                    h = (dr - dg) / dc + 4.0;
                h *= 60.0;
                if (h < 0.0)
                    h += 360.0;
                s = dc / max;
            }
        }

        #endregion Public Static Methods

    }
}
