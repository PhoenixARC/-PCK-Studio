using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MechanikaDesign.WinForms.UI.ColorPicker
{
    [DefaultEvent("ColorChanged")]
    public partial class ColorSliderVertical : UserControl
    {
        #region Events
        
        public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs args);
        public event ColorChangedEventHandler ColorChanged;

        #endregion

        #region Fields

        private HslColor colorHSL = HslColor.FromAhsl(0xff);
        private ColorModes colorMode;
        private Color colorRGB = Color.Empty;
        private bool mouseMoving;
        private int position;
        private bool setHueSilently;
        private Color nubColor;

        #endregion

        #region Properties

        public Color ColorRGB
        {
            get { return this.colorRGB; }
            set
            {
                this.colorRGB = value;
                if (!this.setHueSilently)
                {
                    this.colorHSL = HslColor.FromColor(this.ColorRGB);
                }
                this.ResetSlider();
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
                this.ResetSlider();
                this.Refresh();
            }
        }

        public ColorModes ColorMode
        {
            get { return this.colorMode; }
            set
            {
                this.colorMode = value;
                this.ResetSlider();
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the color of the selection nub.
        /// </summary>
        /// <value>
        /// The color of the nub.
        /// </value>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Black")]
        public Color NubColor
        {
            get { return this.nubColor; }
            set { this.nubColor = value; }
        }

        /// <summary>
        /// Gets or sets the position of the selection nub.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position
        {
            get { return this.position; }
            set
            {
                int num = value;
                num = MathExtensions.LimitToRange(num, 0, base.Height - 9);
                if (num != this.position)
                {
                    this.position = num;
                    this.ResetHSLRGB();
                    this.Refresh();
                    if (this.ColorChanged != null)
                    {
                        this.ColorChanged(this, new ColorChangedEventArgs(this.colorRGB));
                    }
                }
            }
        }

        #endregion

        #region Constructors

        public ColorSliderVertical()
        {
            InitializeComponent();
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            this.colorHSL = HslColor.FromAhsl(1.0, 1.0, 1.0);
            this.colorRGB = this.colorHSL.RgbValue;
            this.colorMode = ColorModes.Hue;
        }

        #endregion

        #region Overridden Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.mouseMoving = true;
            this.Position = e.Y - 4;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.mouseMoving = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.mouseMoving)
            {
                this.Position = e.Y - 4;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            HslColor color = HslColor.FromAhsl(0xff);
            switch (this.ColorMode)
            {
                case ColorModes.Hue:
                    color.L = color.S = 1.0;
                    break;

                case ColorModes.Saturation:
                    color.H = this.ColorHSL.H;
                    color.L = this.ColorHSL.L;
                    break;

                case ColorModes.Luminance:
                    color.H = this.ColorHSL.H;
                    color.S = this.ColorHSL.S;
                    break;
            }
            for (int i = 0; i < (base.Height - 8); i++)
            {
                double num2 = 0.0;
                if (this.ColorMode < ColorModes.Hue)
                {
                    num2 = 255.0 - MathExtensions.Round((255.0 * i) / (base.Height - 8.0));
                }
                else
                {
                    num2 = 1.0 - (((double)i) / ((double)(base.Height - 8)));
                }
                Color empty = Color.Empty;
                switch (this.ColorMode)
                {
                    case ColorModes.Red:
                        empty = Color.FromArgb((int)num2, this.ColorRGB.G, this.ColorRGB.B);
                        break;

                    case ColorModes.Green:
                        empty = Color.FromArgb(this.ColorRGB.R, (int)num2, this.ColorRGB.B);
                        break;

                    case ColorModes.Blue:
                        empty = Color.FromArgb(this.ColorRGB.R, this.ColorRGB.G, (int)num2);
                        break;

                    case ColorModes.Hue:
                        color.H = num2;
                        empty = color.RgbValue;
                        break;

                    case ColorModes.Saturation:
                        color.S = num2;
                        empty = color.RgbValue;
                        break;

                    case ColorModes.Luminance:
                        color.L = num2;
                        empty = color.RgbValue;
                        break;
                }

                using (Pen pen = new Pen(empty))
                {
                    e.Graphics.DrawLine(pen, 11, i + 4, base.Width - 11, i + 4);
                }
            }
            this.DrawSlider(e.Graphics);
        }


        #endregion

        #region Private Methods

        private void DrawSlider(Graphics g)
        {
            using (Pen pen = new Pen(Color.FromArgb(0x74, 0x72, 0x6a)))
            {
                SolidBrush fill = new SolidBrush(this.nubColor);
                Point[] points = new Point[] { new Point(1, this.position), new Point(3, this.position), new Point(7, this.position + 4), new Point(3, this.position + 8), new Point(1, this.position + 8), new Point(0, this.position + 7), new Point(0, this.position + 1) };
                g.FillPolygon(fill, points);
                g.DrawPolygon(pen, points);
                
                points[0] = new Point(base.Width - 2, this.position);
                points[1] = new Point(base.Width - 4, this.position);
                points[2] = new Point(base.Width - 8, this.position + 4);
                points[3] = new Point(base.Width - 4, this.position + 8);
                points[4] = new Point(base.Width - 2, this.position + 8);
                points[5] = new Point(base.Width - 1, this.position + 7);
                points[6] = new Point(base.Width - 1, this.position + 1);
                
                g.FillPolygon(fill, points);
                g.DrawPolygon(pen, points);
            }
        }

        private void ResetSlider()
        {
            double h = 0.0;
            switch (this.ColorMode)
            {
                case ColorModes.Red:
                    h = ((double)this.colorRGB.R) / 255.0;
                    break;

                case ColorModes.Green:
                    h = ((double)this.colorRGB.G) / 255.0;
                    break;

                case ColorModes.Blue:
                    h = ((double)this.colorRGB.B) / 255.0;
                    break;

                case ColorModes.Hue:
                    h = this.colorHSL.H;
                    break;

                case ColorModes.Saturation:
                    h = this.colorHSL.S;
                    break;

                case ColorModes.Luminance:
                    h = this.colorHSL.L;
                    break;
            }
            this.position = (base.Height - 8) - MathExtensions.Round((base.Height - 8) * h);
        }

        private void ResetHSLRGB()
        {
            this.setHueSilently = true;
            switch (this.ColorMode)
            {
                case ColorModes.Red:
                    this.ColorRGB = Color.FromArgb(0xff - MathExtensions.Round((255.0 * this.position) / ((double)(base.Height - 9))), this.ColorRGB.G, this.ColorRGB.B);
                    this.ColorHSL = HslColor.FromColor(this.ColorRGB);
                    break;

                case ColorModes.Green:
                    this.ColorRGB = Color.FromArgb(this.ColorRGB.R, 0xff - MathExtensions.Round((255.0 * this.position) / ((double)(base.Height - 9))), this.ColorRGB.B);
                    this.ColorHSL = HslColor.FromColor(this.ColorRGB);
                    break;

                case ColorModes.Blue:
                    this.ColorRGB = Color.FromArgb(this.ColorRGB.R, this.ColorRGB.G, 0xff - MathExtensions.Round((255.0 * this.position) / ((double)(base.Height - 9))));
                    this.ColorHSL = HslColor.FromColor(this.ColorRGB);
                    break;

                case ColorModes.Hue:
                    this.colorHSL.H = 1.0 - (((double)this.position) / ((double)(base.Height - 9)));
                    this.ColorRGB = this.ColorHSL.RgbValue;
                    break;

                case ColorModes.Saturation:
                    this.colorHSL.S = 1.0 - (((double)this.position) / ((double)(base.Height - 9)));
                    this.ColorRGB = this.ColorHSL.RgbValue;
                    break;

                case ColorModes.Luminance:
                    this.colorHSL.L = 1.0 - (((double)this.position) / ((double)(base.Height - 9)));
                    this.ColorRGB = this.ColorHSL.RgbValue;
                    break;
            }
            this.setHueSilently = false;
        }

        #endregion

    }
}
