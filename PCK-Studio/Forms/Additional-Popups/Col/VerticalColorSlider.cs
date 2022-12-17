using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColorPicker
{
    /// <summary>
    /// A vertical slider showing a range for a color property (Hue, Saturation, Value-Brightness, Red, Green, Blue)
    /// and sends an event when the slider is changed
    /// </summary>
    public partial class VerticalColorSlider : UserControl
    {
        #region Private Constant/Read-Only Fields

        private const int XMARGIN0 = 8; // Left and Right margin
        private const int XMARGIN1 = 11; // Starting color image position -> Left margin + 3d border size + 1 (XMARGIN0 + 2 + 1)
        private const int XMARGIN2 = 22; // Complete right margin (to find the color image width) (XMARGIN1 * 2);

        private const int YMARGIN0 = 2; // Top and bottom margin
        private const int YMARGIN1 = 4; // Top margin + 3d border size (YMARGIN0 + 2)
        private const int YMARGIN2 = 9; // YMARGIN1 * 2 + 1

        private readonly Point[] lArrow = null;
        private readonly Point[] rArrow = null;

        #endregion Private Constant/Read-Only Fields

        #region Private Fields

        private Bitmap _bkBuff = null; // Buffer where to draw the colors
        private int _bkHeight, _bkWidth; // Buffer size
        private DrawStyles _drawStyle = DrawStyles.Hue;
        private double _h, _s, _v;
        private bool _isDragging = false;
        private int _markerStartY = 0;
        private int _r, _g, _b;

        #endregion Private Fields

        #region Constructors

        /// <summary>Constructor</summary>
        public VerticalColorSlider()
        {
            InitializeComponent();
            this.Disposed += verticalColorSlider_Disposed;
            // Initialize Colors
            _h = 360.0;
            _s = 1.0;
            _v = 1.0;
            ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
            _drawStyle = DrawStyles.Hue;

            int w = this.Width;
            lArrow = new Point[7];
            lArrow[0] = new Point(1, 0);
            lArrow[1] = new Point(3, 0);
            lArrow[2] = new Point(7, 4);
            lArrow[3] = new Point(3, 8);
            lArrow[4] = new Point(1, 8);
            lArrow[5] = new Point(0, 7);
            lArrow[6] = new Point(0, 1);
            rArrow = new Point[7];
            rArrow[0] = new Point(w - 2, 0);
            rArrow[1] = new Point(w - 4, 0);
            rArrow[2] = new Point(w - 8, 4);
            rArrow[3] = new Point(w - 4, 8);
            rArrow[4] = new Point(w - 2, 8);
            rArrow[5] = new Point(w - 1, 7);
            rArrow[6] = new Point(w - 1, 1);
            createBkBuff();
        }

        #endregion Constructors

        #region Public Events

        /// <summary>It fires when we move the Slider</summary>
        public event EventHandler Scrolled;

        #endregion Public Events

        #region Public Properties

        /// <summary>Blue value</summary>
        public int B => _b;

        /// <summary>Control value as a System.Drawing.Color</summary>
        public Color Color
        {
            get => Color.FromArgb(_r, _g, _b);
            set
            {
                if (_r != value.R || _g != value.G || B != value.B)
                {
                    _r = value.R;
                    _g = value.G;
                    _b = value.B;
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    resetSlider(true);
                    drawContent();
                    this.Invalidate();
                }
            }
        }

        /// <summary>The DrawStyle of the control (Hue, Saturation, Brightness, Red, Green or Blue)</summary>
        public DrawStyles DrawStyle
        {
            get => _drawStyle;
            set
            {
                if (_drawStyle != value)
                {
                    _drawStyle = value;
                    resetSlider(true);
                    drawContent();
                    this.Invalidate();
                }
            }
        }

        /// <summary>Green value</summary>
        public int G => _g;

        /// <summary>Hue value</summary>
        public double H => _h;

        /// <summary>Red value</summary>
        public int R => _r;

        /// <summary>Control value as an int</summary>
        public int RGB
        {
            get => ((_r & 0xFF) << 16) + ((_g & 0xFF) << 8) + (_b & 0xFF);
            set
            {
                if (_r != ((value >> 16) & 0xFF) || _g != ((value >> 8) & 0xFF) || _b != (value & 0xFF))
                {
                    _r = (value >> 16) & 0xFF;
                    _g = (value >> 8) & 0xFF;
                    _b = value & 0xFF;
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    // Redibuixem el control
                    resetSlider(true);
                    drawContent();
                    this.Invalidate();
                }
            }
        }

        /// <summary>Saturation value</summary>
        public double S => _s;

        /// <summary>Value-Brightness value</summary>
        public double V => _v;

        #endregion Public Properties

        #region Private Methods

        /// <summary>Repaints the background of the sliders zones</summary>
        /// <param name="g">Graphics to draw on</param>
        private void clearSlider(Graphics g)
        {
            Brush brush = SystemBrushes.Control;
            int h = this.Height;
            g.FillRectangle(brush, 0, 0, XMARGIN0, h); // Left slider
            g.FillRectangle(brush, this.Width - XMARGIN0, 0, XMARGIN0, h);  // Right slider
        }

        /// <summary>Creates the background Bitmap</summary>
        private void createBkBuff()
        {
            _bkHeight = this.Height - YMARGIN2;
            _bkWidth = this.Width - XMARGIN2;
            if (_bkBuff != null)
                _bkBuff.Dispose();
            _bkBuff = new Bitmap(_bkWidth, _bkHeight + 1);
        }

        /// <summary>Draws the 3d border</summary>
        /// <param name="g">Graphics to draw on</param>
        private void drawBorder(Graphics g)
        {
            ControlPaint.DrawBorder3D(g, XMARGIN0 + 1, YMARGIN0, this.Width - (XMARGIN2 - 4), this.Height - YMARGIN1, Border3DStyle.Sunken);
        }

        /// <summary>Draws the content</summary>
        private void drawContent()
        {
            switch (_drawStyle)
            {
                case DrawStyles.Hue:
                    drawStyleHue();
                    break;
                case DrawStyles.Saturation:
                    drawStyleSaturation();
                    break;
                case DrawStyles.Brightness:
                    drawStyleBrightness();
                    break;
                case DrawStyles.Red:
                    drawStyleRed();
                    break;
                case DrawStyles.Green:
                    drawStyleGreen();
                    break;
                case DrawStyles.Blue:
                    drawStyleBlue();
                    break;
            }
        }

        /// <summary>Draws the sliders</summary>
        /// <param name="g">Graphics to draw on</param>
        /// <param name="y">Slider position</param>
        private void drawSlider(Graphics g, int y)
        {
            // Delete old slider
            this.clearSlider(g);

            // Adjust y position
            _markerStartY = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Draw sliders
            lArrow[0].Y = _markerStartY;
            lArrow[1].Y = _markerStartY;
            lArrow[2].Y = _markerStartY + 4;
            lArrow[3].Y = _markerStartY + 8;
            lArrow[4].Y = _markerStartY + 8;
            lArrow[5].Y = _markerStartY + 7;
            lArrow[6].Y = _markerStartY + 1;
            g.FillPolygon(Brushes.White, lArrow);
            g.DrawPolygon(Pens.DarkGray, lArrow);
            rArrow[0].Y = _markerStartY;
            rArrow[1].Y = _markerStartY;
            rArrow[2].Y = _markerStartY + 4;
            rArrow[3].Y = _markerStartY + 8;
            rArrow[4].Y = _markerStartY + 8;
            rArrow[5].Y = _markerStartY + 7;
            rArrow[6].Y = _markerStartY + 1;
            g.FillPolygon(Brushes.White, rArrow);
            g.DrawPolygon(Pens.DarkGray, rArrow);
        }

        /// <summary>Draws the sliders</summary>
        /// <param name="y">Slider position</param>
        /// <param name="force">Draw the slider even if y value has not changed</param>
        private void drawSlider(int y, bool force)
        {
            if (force || _markerStartY != (y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y)))
            {
                using (Graphics g = this.CreateGraphics())
                {
                    drawSlider(g, y);
                }
            }
        }

        /// <summary>Draw all the Blue colors</summary>
        private void drawStyleBlue()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkHeight; i++)
                {
                    int b = 255 - (int)Math.Round(255.0 * i / _bkHeight);
                    using (Pen pen = new Pen(Color.FromArgb(_r, _g, b)))
                    {
                        gr.DrawLine(pen, 0, i, _bkWidth, i);
                    }
                }
            }
        }

        /// <summary>Draw all the Value/Brightness colors</summary>
        private void drawStyleBrightness()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkHeight; i++)
                {
                    double v = 1.0 - (double)i / _bkHeight;
                    ColorUtil.HSV2RGB(_h, _s, v, out int r, out int g, out int b);
                    using (Pen pen = new Pen(Color.FromArgb(r, g, b)))
                    {
                        gr.DrawLine(pen, 0, i, _bkWidth, i);
                    }
                }
            }
        }

        /// <summary>Draw all the Green colors</summary>
        private void drawStyleGreen()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkHeight; i++)
                {
                    int g = 255 - (int)Math.Round(255.0 * i / _bkHeight);
                    using (Pen pen = new Pen(Color.FromArgb(_r, g, _b)))
                    {
                        gr.DrawLine(pen, 0, i, _bkWidth, i);
                    }
                }
            }
        }

        /// <summary>Draw all the Hue colors</summary>
        private void drawStyleHue()
        {
            using(Graphics gr = Graphics.FromImage(_bkBuff))
            {
                double s = 1.0;
                double l = 1.0;
                for (int i = 0; i <= _bkHeight; i++)
                {
                    double h = 360.0 * (1.0 - (double)i / _bkHeight);
                    ColorUtil.HSV2RGB(h, s, l, out int r, out int g, out int b);
                    using (Pen pen = new Pen(Color.FromArgb(r, g, b)))
                    {
                        gr.DrawLine(pen, 0, i, _bkWidth, i);
                    }
                }
            }
        }

        /// <summary>Draw all the Red colors</summary>
        private void drawStyleRed()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkHeight; i++)
                {
                    int r = 255 - (int)Math.Round(255.0 * i / _bkHeight);
                    using (Pen pen = new Pen(Color.FromArgb(r, _g, _b)))
                    {
                        gr.DrawLine(pen, 0, i, _bkWidth, i);
                    }
                }
            }
        }

        /// <summary>Draw all the Saturation colors</summary>
        private void drawStyleSaturation()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkHeight; i++)
                {
                    double s = 1.0 - (double)i / _bkHeight;
                    ColorUtil.HSV2RGB(_h, s, _v, out int r, out int g, out int b);
                    using (Pen pen = new Pen(Color.FromArgb(r, g, b)))
                    {
                        gr.DrawLine(pen, 0, i, _bkWidth, i);
                    }
                }
            }
        }

        /// <summary>Set the color from the slider position</summary>
        private void resetHSVRGB()
        {
            switch (_drawStyle)
            {
                case DrawStyles.Hue:
                    _h = 360.0 * (1.0 - (double)_markerStartY / _bkHeight);
                    ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                    break;
                case DrawStyles.Saturation:
                    _s = 1.0 - (double)_markerStartY / _bkHeight;
                    ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                    break;
                case DrawStyles.Brightness:
                    _v = 1.0 - (double)_markerStartY / _bkHeight;
                    ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                    break;
                case DrawStyles.Red:
                    _r = 255 - (int)Math.Round(255.0 * _markerStartY / _bkHeight);
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    break;
                case DrawStyles.Green:
                    _g = 255 - (int)Math.Round(255.0 * _markerStartY / _bkHeight);
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    break;
                case DrawStyles.Blue:
                    _b = 255 - (int)Math.Round(255.0 * _markerStartY / _bkHeight);
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    break;
            }
        }

        /// <summary>Set the slider position from the color</summary>
        /// <param name="redraw">Redraw the control after setting the slider position</param>
        private void resetSlider(bool redraw)
        {
            switch (_drawStyle)
            {
                case DrawStyles.Hue:
                    _markerStartY = _bkHeight - (int)Math.Round(_bkHeight * _h / 360.0);
                    break;
                case DrawStyles.Saturation:
                    _markerStartY = _bkHeight - (int)Math.Round(_bkHeight * _s);
                    break;
                case DrawStyles.Brightness:
                    _markerStartY = _bkHeight - (int)Math.Round(_bkHeight * _v);
                    break;
                case DrawStyles.Red:
                    _markerStartY = _bkHeight - (int)Math.Round(_bkHeight * _r / 255.0);
                    break;
                case DrawStyles.Green:
                    _markerStartY = _bkHeight - (int)Math.Round(_bkHeight * _g / 255.0);
                    break;
                case DrawStyles.Blue:
                    _markerStartY = _bkHeight - (int)Math.Round(_bkHeight * _b / 255.0);
                    break;
            }
            if (redraw)
                drawSlider(_markerStartY, true);
        }

        /// <summary>Dispose the background Bitmap</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void verticalColorSlider_Disposed(object sender, EventArgs e)
        {
            if (_bkBuff != null)
            {
                _bkBuff.Dispose();
                _bkBuff = null;
            }
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>The control has been loaded</summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            drawContent();
            this.Invalidate();
        }

        /// <summary>Process MouseDown event</summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Only check left button
            if (e.Button != MouseButtons.Left)
                return;

            // Start dragging
            _isDragging = true;

            // Get the slider position
            int y = e.Y - YMARGIN1;
            y = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Check that there has been a change in the position
            if (y == _markerStartY)
                return;

            drawSlider(y, false); // Redraw the slider
            resetHSVRGB(); // Redraw the colors

            Scrolled?.Invoke(this, e); // Send "Scrolled" event
        }

        /// <summary>Process MouseMove event</summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Check we are "dragging"
            if (!_isDragging)
                return;

            // Get the slider position
            int y = e.Y - YMARGIN1;
            y = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Check that there has been a change in the position
            if (y == _markerStartY)
                return;

            drawSlider(y, false); // Redraw the slider
            resetHSVRGB(); // Redraw the colors

            Scrolled?.Invoke(this, e); // Send "Scrolled" event
        }

        /// <summary>Process MouseUp event</summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Only check left button and "dragging"
            if (e.Button != MouseButtons.Left || !_isDragging)
                return;

            // End "dragging"
            _isDragging = false;

            // Get the slider position
            int y = e.Y - YMARGIN1;
            y = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Check that there has been a change in the position
            if (y == _markerStartY)
                return;

            drawSlider(y, false); // Redraw the slider
            resetHSVRGB(); // Redraw the colors

            Scrolled?.Invoke(this, e); // Send "Scrolled" event
        }

        /// <summary>Repaint the control</summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            drawSlider(e.Graphics, _markerStartY);
            drawBorder(e.Graphics);
            e.Graphics.DrawImage(_bkBuff, XMARGIN1, YMARGIN1);
        }

        /// <summary>Repaint the background</summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(this.BackColor))
            {
                int w = this.Width;
                int h = this.Height;
                e.Graphics.FillRectangle(b, 0, 0, w, YMARGIN0);
                e.Graphics.FillRectangle(b, XMARGIN0, YMARGIN0, 1, h - YMARGIN0 * 2);
                e.Graphics.FillRectangle(b, w - XMARGIN0 - 1, YMARGIN0, 1, h - YMARGIN0 * 2);
                e.Graphics.FillRectangle(b, 0, h - YMARGIN0, w, YMARGIN0);
            }
        }

        /// <summary>The control has been resized</summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            if (rArrow != null)
            {
                int w = this.Width;
                rArrow[0].X = w - 2;
                rArrow[1].X = w - 4;
                rArrow[2].X = w - 8;
                rArrow[3].X = w - 4;
                rArrow[4].X = w - 2;
                rArrow[5].X = w - 1;
                rArrow[6].X = w - 1;
                createBkBuff();
                drawContent();
                this.Invalidate();
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>Set the control color from HSV values</summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        public void SetHSV(double h, double s, double v)
        {
            if (_h != (h < 0.0 ? 0.0 : h > 360.0 ? 360.0 : h) || _s != (s < 0.0 ? 0.0 : s > 1.0 ? 1.0 : s) || _v != (v < 0.0 ? 0.0 : v > 1.0 ? 1.0 : v))
            {
                _h = (h < 0.0 ? 0.0 : h > 360.0 ? 360.0 : h);
                _s = s < 0.0 ? 0.0 : s > 1.0 ? 1.0 : s;
                _v = v < 0.0 ? 0.0 : v > 1.0 ? 1.0 : v;
                ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                resetSlider(true);
                drawContent();
                this.Invalidate();
            }
        }

        /// <summary>Set the control color from RGB values</summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public void SetRGB(int r, int g, int b)
        {
            if (_r != (r & 0xFF) || _g != (g & 0xFF) || _b != (b & 0xFF))
            {
                _r = r & 0xFF;
                _g = g & 0xFF;
                _b = b & 0xFF;
                ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                resetSlider(true);
                drawContent();
                this.Invalidate();
            }
        }

        #endregion Public Methods
    }
}