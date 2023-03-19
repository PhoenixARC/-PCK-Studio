using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MechanikaDesign.WinForms.UI.ColorPicker
{
    [DefaultEvent("ColorChanged")]
    public partial class ColorBox2D: UserControl
    {

        #region Events
        public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs args);
        public event ColorChangedEventHandler ColorChanged;

        #endregion

        #region Fields

        private HslColor colorHSL;
        private ColorModes colorMode;
        private Color colorRGB = Color.Empty;
        private Point markerPoint = Point.Empty;
        private bool mouseMoving;

        #endregion

        #region Properties
        public ColorModes ColorMode
        {
            get { return this.colorMode; }
            set
            {
                this.colorMode = value;
                this.ResetMarker();
                this.Refresh();
            }
        }

        public HslColor ColorHSL
        {
            get { return this.colorHSL; }
            set
            {
                this.colorHSL = value;
                this.colorRGB = this.colorHSL.RgbValue;
                this.ResetMarker();
                this.Refresh();
            }
        }

        public Color ColorRGB
        {
            get { return this.colorRGB; }
            set
            {
                this.colorRGB = value;
                this.colorHSL = HslColor.FromColor(this.colorRGB);
                this.ResetMarker();
                this.Refresh();
            }
        }

        #endregion

        #region Constructors

        public ColorBox2D()
        {
            InitializeComponent();
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            this.colorHSL = HslColor.FromAhsl(1.0, 1.0, 1.0);
            this.colorRGB = this.colorHSL.RgbValue;
            this.colorMode = ColorModes.Hue;
        }

        #endregion

        #region Overriden Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.mouseMoving = true;
            this.SetMarker(e.X, e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.mouseMoving)
            {
                this.SetMarker(e.X, e.Y);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.mouseMoving = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            HslColor color = HslColor.FromAhsl(0xff);
            HslColor color2 = HslColor.FromAhsl(0xff);
            switch (this.ColorMode)
            {
                case ColorModes.Hue:
                    color.H = this.ColorHSL.H;
                    color2.H = this.ColorHSL.H;
                    color.S = 0.0;
                    color2.S = 1.0;
                    break;

                case ColorModes.Saturation:
                    color.S = this.ColorHSL.S;
                    color2.S = this.ColorHSL.S;
                    color.L = 1.0;
                    color2.L = 0.0;
                    break;

                case ColorModes.Luminance:
                    color.L = this.ColorHSL.L;
                    color2.L = this.ColorHSL.L;
                    color.S = 1.0;
                    color2.S = 0.0;
                    break;
            }
            for (int i = 0; i < (base.Height - 4); i++)
            {
                int green = MathExtensions.Round(255.0 - ((255.0 * i) / ((double)(base.Height - 4))));
                Color empty = Color.Empty;
                Color rgbValue = Color.Empty;
                switch (this.ColorMode)
                {
                    case ColorModes.Red:
                        empty = Color.FromArgb(this.ColorRGB.R, green, 0);
                        rgbValue = Color.FromArgb(this.ColorRGB.R, green, 0xff);
                        break;

                    case ColorModes.Green:
                        empty = Color.FromArgb(green, this.ColorRGB.G, 0);
                        rgbValue = Color.FromArgb(green, this.ColorRGB.G, 0xff);
                        break;

                    case ColorModes.Blue:
                        empty = Color.FromArgb(0, green, this.ColorRGB.B);
                        rgbValue = Color.FromArgb(0xff, green, this.ColorRGB.B);
                        break;

                    case ColorModes.Hue:
                        color2.L = color.L = 1.0 - (((double)i) / ((double)(base.Height - 4)));
                        empty = color.RgbValue;
                        rgbValue = color2.RgbValue;
                        break;

                    case ColorModes.Saturation:
                    case ColorModes.Luminance:
                        color2.H = color.H = ((double)i) / ((double)(base.Width - 4));
                        empty = color.RgbValue;
                        rgbValue = color2.RgbValue;
                        break;
                }

                Rectangle rect = new Rectangle(2, 2, base.Width - 4, 1);
                Rectangle rectangle2 = new Rectangle(2, i + 2, base.Width - 4, 1);
                if ((this.ColorMode == ColorModes.Saturation) || (this.ColorMode == ColorModes.Luminance))
                {
                    rect = new Rectangle(2, 2, 1, base.Height - 4);
                    rectangle2 = new Rectangle(i + 2, 2, 1, base.Height - 4);
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, empty, rgbValue, 90f, false))
                    {
                        e.Graphics.FillRectangle(brush, rectangle2);
                        continue;
                    }
                }
                using (LinearGradientBrush brush2 = new LinearGradientBrush(rect, empty, rgbValue, 0f, false))
                {
                    e.Graphics.FillRectangle(brush2, rectangle2);
                }
            }
            Pen white = Pens.White;
            if (this.colorHSL.L >= 0.78431372549019607)
            {
                if ((this.colorHSL.H < 0.072222222222222215) || (this.colorHSL.H > 0.55555555555555558))
                {
                    if (this.colorHSL.S <= 0.27450980392156865)
                    {
                        white = Pens.Black;
                    }
                }
                else
                {
                    white = Pens.Black;
                }
            }
            e.Graphics.DrawEllipse(white, this.markerPoint.X - 5, this.markerPoint.Y - 5, 10, 10);
        }


        #endregion

        #region Private Methods

        private HslColor GetColor(int x, int y)
        {
            int num;
            int num2;
            int num3;
            HslColor color = HslColor.FromAhsl(0xff);
            switch (this.ColorMode)
            {
                case ColorModes.Red:
                    num2 = MathExtensions.Round(255.0 * (1.0 - (((double)y) / ((double)(base.Height - 4)))));
                    num3 = MathExtensions.Round((255.0 * x) / ((double)(base.Width - 4)));
                    return HslColor.FromColor(Color.FromArgb(this.colorRGB.R, num2, num3));

                case ColorModes.Green:
                    num = MathExtensions.Round(255.0 * (1.0 - (((double)y) / ((double)(base.Height - 4)))));
                    num3 = MathExtensions.Round((255.0 * x) / ((double)(base.Width - 4)));
                    return HslColor.FromColor(Color.FromArgb(num, this.colorRGB.G, num3));

                case ColorModes.Blue:
                    num = MathExtensions.Round((255.0 * x) / ((double)(base.Width - 4)));
                    num2 = MathExtensions.Round(255.0 * (1.0 - (((double)y) / ((double)(base.Height - 4)))));
                    return HslColor.FromColor(Color.FromArgb(num, num2, this.colorRGB.B));

                case ColorModes.Hue:
                    color.H = this.colorHSL.H;
                    color.S = ((double)x) / ((double)(base.Width - 4));
                    color.L = 1.0 - (((double)y) / ((double)(base.Height - 4)));
                    return color;

                case ColorModes.Saturation:
                    color.S = this.colorHSL.S;
                    color.H = ((double)x) / ((double)(base.Width - 4));
                    color.L = 1.0 - (((double)y) / ((double)(base.Height - 4)));
                    return color;

                case ColorModes.Luminance:
                    color.L = this.colorHSL.L;
                    color.H = ((double)x) / ((double)(base.Width - 4));
                    color.S = 1.0 - (((double)y) / ((double)(base.Height - 4)));
                    return color;
            }
            return color;
        }

        private void ResetMarker()
        {
            switch (this.colorMode)
            {
                case ColorModes.Red:
                    this.markerPoint.X = MathExtensions.Round(((base.Width - 4) * this.colorRGB.B) / 255.0);
                    this.markerPoint.Y = MathExtensions.Round((base.Height - 4) * (1.0 - (((double)this.colorRGB.G) / 255.0)));
                    return;

                case ColorModes.Green:
                    this.markerPoint.X = MathExtensions.Round(((base.Width - 4) * this.colorRGB.B) / 255.0);
                    this.markerPoint.Y = MathExtensions.Round((base.Height - 4) * (1.0 - (((double)this.colorRGB.R) / 255.0)));
                    return;

                case ColorModes.Blue:
                    this.markerPoint.X = MathExtensions.Round(((base.Width - 4) * this.colorRGB.R) / 255.0);
                    this.markerPoint.Y = MathExtensions.Round((base.Height - 4) * (1.0 - (((double)this.colorRGB.G) / 255.0)));
                    return;

                case ColorModes.Hue:
                    this.markerPoint.X = MathExtensions.Round((base.Width - 4) * this.colorHSL.S);
                    this.markerPoint.Y = MathExtensions.Round((base.Height - 4) * (1.0 - this.colorHSL.L));
                    return;

                case ColorModes.Saturation:
                    this.markerPoint.X = MathExtensions.Round((base.Width - 4) * this.colorHSL.H);
                    this.markerPoint.Y = MathExtensions.Round((base.Height - 4) * (1.0 - this.colorHSL.L));
                    return;

                case ColorModes.Luminance:
                    this.markerPoint.X = MathExtensions.Round((base.Width - 4) * this.colorHSL.H);
                    this.markerPoint.Y = MathExtensions.Round((base.Height - 4) * (1.0 - this.colorHSL.S));
                    return;
            }
        }

        private void SetMarker(int x, int y)
        {
            x = MathExtensions.LimitToRange(x, 0, base.Width - 4);
            y = MathExtensions.LimitToRange(y, 0, base.Height - 4);
            if ((this.markerPoint.X != x) || (this.markerPoint.Y != y))
            {
                this.markerPoint = new Point(x, y);
                this.colorHSL = this.GetColor(x, y);
                this.colorRGB = this.colorHSL.RgbValue;
                this.Refresh();
                if (this.ColorChanged != null)
                {
                    this.ColorChanged(this, new ColorChangedEventArgs(this.colorRGB));
                }
            }
        }

        #endregion

    }
}
