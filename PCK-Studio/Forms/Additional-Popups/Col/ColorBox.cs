using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ColorPicker
{
    /// <summary>
    /// A box showing a 2D range for a color property (Hue, Saturation, Value-Brightness, Red, Green, Blue)
    /// and sends an event when the marker position is changed
    /// </summary>
    public partial class ColorBox : UserControl
    {
        #region Private Constant/Read-Only Fields

        private const int MARGIN0 = 2; // Margin
        private const int MARGIN1 = 4; // Starting color image position -> Margin + 3d border (MARGIN0 + 2)
        private const int MARGIN2 = 9; //  Complete margin (to find the color image width) (MARGIN1 * 2) + 1;
        private const int MARKER0 = 10; // Marker width 
        private const int MARKER2 = 5; // Half marker width

        #endregion Private Constant/Read-Only Fields

        #region Private Fields

        private Bitmap _bkBuff = null; // Buffer where to draw the colors
        private int _bkHeight, _bkWidth; // Buffer size
        private DrawStyles _drawStyle = DrawStyles.Hue;
        private double _h, _s, _v;
        private bool _isDragging = false;
        private int _markerX = 0;
        private int _markerY = 0;
        private int _r, _g, _b;

        #endregion Private Fields

        #region Constructors

        /// <summary>Contructor</summary>
        public ColorBox()
        {
            InitializeComponent();
            this.Disposed += colorBox_Disposed;
            //	Initialize Colors
            _h = 360.0;
            _s = 1.0;
            _v = 1.0;
            ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
            _drawStyle = DrawStyles.Hue;
            createBkBuff();
        }

        #endregion Constructors

        #region Public Events

        /// <summary>It fires when we move the marker</summary>
        public event EventHandler Scrolled;

        #endregion Public Events

        #region Public Properties

        /// <summary>Blue value</summary>
        public int B => _b;

        /// <summary>Control value as a System.Drawing.Color</summary>
        public System.Drawing.Color Color
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
                    resetMarker(true);
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
                    resetMarker(true);
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
                    resetMarker(true);
                    drawContent();
                    this.Invalidate();
                }
            }
        }

        /// <summary>Saturation value</summary>
        public double S => _s;

        /// <summary>Value/Brightness value</summary>
        public double V => _v;

        #endregion Public Properties

        #region Private Methods

        /// <summary>Clear the marker</summary>
        /// <param name="g">Graphics to draw on</param>
        private void clearMarker(Graphics g)
        {
            int ix = _markerX - MARKER2;
            int iy = _markerY - MARKER2;
            g.DrawImage(_bkBuff, new Rectangle(ix + MARGIN1, iy + MARGIN1, MARKER0 + 1, MARKER0 + 1), ix, iy, MARKER0 + 1, MARKER0 + 1, GraphicsUnit.Pixel);
        }

        /// <summary>Dispose the background Bitmap</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorBox_Disposed(object sender, EventArgs e)
        {
            if (_bkBuff != null)
            {
                _bkBuff.Dispose();
                _bkBuff = null;
            }
        }

        /// <summary>Creates the background Bitmap</summary>
        private void createBkBuff()
        {
            _bkHeight = this.Height - MARGIN2;
            _bkWidth = this.Width - MARGIN2;
            if (_bkBuff != null)
                _bkBuff.Dispose();
            _bkBuff = new Bitmap(_bkWidth + 1, _bkHeight + 1);
        }

        /// <summary>Draws the 3d border</summary>
        /// <param name="g">Graphics to draw on</param>
        private void drawBorder(Graphics g)
        {
            ControlPaint.DrawBorder3D(g, MARGIN0, MARGIN0, this.Width - MARGIN1, this.Height - MARGIN1, Border3DStyle.Sunken);
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

        /// <summary>Draws the Marker</summary>
        /// <param name="g">Graphics to draw on</param>
        /// <param name="x">X Marker position</param>
        /// <param name="y">Y Marker position</param>
        private void drawMarker(Graphics g, int x, int y)
        {
            // Delete old marker
            this.clearMarker(g);

            // Adjust la position
            _markerX = x < 0 ? 0 : (x > _bkWidth ? _bkWidth : x);
            _markerY = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Get the color of the actual position to draw the marker in black or white
            getColorHSV(x, y, out double hue, out double sat, out double val);

            Pen pen;
            if (val < 200.0 / 255.0)
                pen = Pens.White;
            else if (hue < 26.0 || hue > 200.0)
                if (sat > 70 / 255.0)
                    pen = Pens.White;
                else
                    pen = Pens.Black;
            else
                pen = Pens.Black;

            g.DrawEllipse(pen, x + MARGIN1 - MARKER2, y + MARGIN1 - MARKER2, MARKER0, MARKER0);
            if (x <= 5 || y <= 5 || x > _bkWidth - 5 || y > _bkHeight - 5)
            {
                drawBorder(g);
                if (x < 3 || y < 3 || x > _bkWidth - 3 || y > _bkHeight - 3)
                {
                    Brush brush = SystemBrushes.Control;
                    int w = this.Width;
                    int h = this.Height;
                    g.FillRectangle(brush, 0, 0, w, MARGIN0);
                    g.FillRectangle(brush, 0, MARGIN0, MARGIN0, h - MARGIN0);
                    g.FillRectangle(brush, w - MARGIN0, MARGIN0, MARGIN0, h - MARGIN0);
                    g.FillRectangle(brush, MARGIN0, h - MARGIN0, w - (MARGIN0 * 2), h);
                }
            }
        }

        /// <summary>Draws the Marker</summary>
        /// <param name="x">X Marker position</param>
        /// <param name="y">Y Marker position</param>
        /// <param name="force">Draw the marker even if x and y value have not changed</param>
        private void drawMarker(int x, int y, bool force)
        {
            if (force ||
                _markerX != (x < 0 ? 0 : (y > _bkWidth ? _bkWidth : x)) ||
                _markerY != (y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y))
                )
            {
                using (Graphics g = this.CreateGraphics())
                {
                    drawMarker(g, x, y);
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
                    int g = (int)Math.Round(255.0 - ((255.0 * i) / _bkHeight));
                    using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, _bkWidth + 1, 1), Color.FromArgb(0, g, _b), Color.FromArgb(255, g, _b), 0, false))
                    {
                        gr.FillRectangle(br, new Rectangle(0, i, _bkWidth + 1, 1));
                    }
                }
            }
        }

        /// <summary>Draw all the Value/Brightness colors</summary>
        private void drawStyleBrightness()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkWidth; i++)
                {
                    double h = (360.0 * i) / _bkWidth;

                    ColorUtil.HSV2RGB(h, 1.0, _v, out int rS, out int gS, out int bS);
                    ColorUtil.HSV2RGB(h, 0.0, _v, out int rE, out int gE, out int bE);

                    using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, 1, _bkHeight + 1), Color.FromArgb(rS, gS, bS), Color.FromArgb(rE, gE, bE), 90, false))
                    {
                        gr.FillRectangle(br, new Rectangle(i, 0, 1, _bkHeight + 1));
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
                    int r = (int)Math.Round(255.0 - ((255.0 * i) / _bkHeight));
                    using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, _bkWidth + 1, 1), Color.FromArgb(r, _g, 0), Color.FromArgb(r, _g, 255), 0, false))
                    {
                        gr.FillRectangle(br, new Rectangle(0, i, _bkWidth + 1, 1));
                    }
                }
            }
        }

        /// <summary>Draw all the Hue colors</summary>
        private void drawStyleHue()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkHeight; i++)
                {
                    double v = 1.0 - (double)i / _bkHeight;

                    ColorUtil.HSV2RGB(_h, 0.0, v, out int rS, out int gS, out int bS);
                    ColorUtil.HSV2RGB(_h, 1.0, v, out int rE, out int gE, out int bE);

                    using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, _bkWidth +1 , 1), Color.FromArgb(rS, gS, bS), Color.FromArgb(rE, gE, bE), 0, false))
                    {
                        gr.FillRectangle(br, new Rectangle(0, i, _bkWidth + 1, 1));
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
                    int g = (int)Math.Round(255.0 - ((255.0 * i) / _bkHeight));
                    using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, _bkWidth + 1, 1), Color.FromArgb(_r, g, 0), Color.FromArgb(_r, g, 255), 0, false))
                    {
                        gr.FillRectangle(br, new Rectangle(0, i, _bkWidth + 1, 1));
                    }
                }
            }
        }

        /// <summary>Draw all the Saturation colors</summary>
        private void drawStyleSaturation()
        {
            using (Graphics gr = Graphics.FromImage(_bkBuff))
            {
                for (int i = 0; i <= _bkWidth; i++)
                {
                    double h = (360.0 * i) / _bkWidth;

                    ColorUtil.HSV2RGB(h, _s, 1.0, out int rS, out int gS, out int bS);
                    ColorUtil.HSV2RGB(h, _s, 0.0, out int rE, out int gE, out int bE);

                    using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, 1, _bkHeight + 1), Color.FromArgb(rS, gS, bS), Color.FromArgb(rE, gE, bE), 90, false))
                    {
                        gr.FillRectangle(br, new Rectangle(i, 0, 1, _bkHeight + 1));
                    }
                }
            }
        }

        /// <summary>Get the color at X, Y position</summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="h">Returned Hue value at X,Y position</param>
        /// <param name="s">Returned Saturation value at X,Y position</param>
        /// <param name="v">Returned Value/Brightness value at X,Y position</param>
        private void getColorHSV(int x, int y, out double h, out double s, out double v)
        {
            switch (_drawStyle)
            {
                case DrawStyles.Hue:
                    h = _h;
                    s = (double)x / _bkWidth;
                    v = 1.0 - y / _bkHeight;
                    break;
                case DrawStyles.Saturation:
                    h = (360.0 * x) / _bkWidth;
                    s = _s;
                    v = 1.0 - (double)y / _bkHeight;
                    break;
                case DrawStyles.Brightness:
                    h = (360.0 * x) / _bkWidth;
                    s = 1.0 - (double)y / _bkHeight;
                    v = _v;
                    break;
                case DrawStyles.Red:
                    ColorUtil.RGB2HSV(_r, (int)Math.Round(255.0 * (1.0 - (double)y / _bkHeight)), (int)Math.Round((255.0 * x) / _bkWidth), out h, out s, out v);
                    break;
                case DrawStyles.Green:
                    ColorUtil.RGB2HSV((int)Math.Round(255.0 * (1.0 - (double)y / _bkHeight)), _g, (int)Math.Round((255.0 * x) / _bkWidth), out h, out s, out v);
                    break;
                default: // case DrawStyles.Blue:
                    ColorUtil.RGB2HSV((int)Math.Round((255.0 * x) / _bkWidth), (int)Math.Round(255.0 * (1.0 - (double)y / _bkHeight)), _b, out h, out s, out v);
                    break;
            }
        }

        /// <summary>Set the color from the marker position</summary>
        private void resetHSVRGB()
        {
            switch (_drawStyle)
            {
                case DrawStyles.Hue:
                    _s = (double)_markerX / _bkWidth;
                    _v = 1.0 - (double)_markerY / _bkHeight;
                    ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                    break;
                case DrawStyles.Saturation:
                    _h = (360.0 * _markerX) / _bkWidth;
                    _v = 1.0 - (double)_markerY / _bkHeight;
                    ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                    break;
                case DrawStyles.Brightness:
                    _h = (360.0 * _markerX) / _bkWidth;
                    _s = 1.0 - (double)_markerY / _bkHeight;
                    ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                    break;
                case DrawStyles.Red:
                    _b = (int)Math.Round((255.0 * _markerX) / _bkWidth);
                    _g = (int)Math.Round(255.0 * (1.0 - (double)_markerY / _bkHeight));
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    break;
                case DrawStyles.Green:
                    _b = (int)Math.Round((255.0 * _markerX) / _bkWidth);
                    _r = (int)Math.Round(255.0 * (1.0 - (double)_markerY / _bkHeight));
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    break;
                case DrawStyles.Blue:
                    _r = (int)Math.Round((255.0 * _markerX) / _bkWidth);
                    _g = (int)Math.Round(255.0 * (1.0 - (double)_markerY / _bkHeight));
                    ColorUtil.RGB2HSV(_r, _g, _b, out _h, out _s, out _v);
                    break;
            }
        }

        /// <summary>Set the marker position from the color</summary>
        /// <param name="redraw">Redraw the control after setting the marker position</param>
        private void resetMarker(bool redraw)
        {
            switch (_drawStyle)
            {
                case DrawStyles.Hue:
                    _markerX = (int)Math.Round(_bkWidth * _s);
                    _markerY = (int)Math.Round(_bkHeight * (1.0 - _v));
                    break;
                case DrawStyles.Saturation:
                    _markerX = (int)Math.Round(_bkWidth * _h / 360.0);
                    _markerY = (int)Math.Round(_bkHeight * (1.0 - _v));
                    break;
                case DrawStyles.Brightness:
                    _markerX = (int)Math.Round(_bkWidth * _h / 360.0);
                    _markerY = (int)Math.Round(_bkHeight * (1.0 - _s));
                    break;
                case DrawStyles.Red:
                    _markerX = (int)Math.Round(_bkWidth * _b / 255.0);
                    _markerY = (int)Math.Round(_bkHeight * (1.0 - _g / 255.0));
                    break;
                case DrawStyles.Green:
                    _markerX = (int)Math.Round(_bkWidth * _b / 255.0);
                    _markerY = (int)Math.Round(_bkHeight * (1.0 - _r / 255.0));
                    break;
                case DrawStyles.Blue:
                    _markerX = (int)Math.Round(_bkWidth * _r / 255.0);
                    _markerY = (int)Math.Round(_bkHeight * (1.0 - _g / 255.0));
                    break;
            }
            if (redraw)
                drawMarker(_markerX, _markerY, true);
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

            // Get the marker position
            int x = e.X - MARGIN0;
            x = x < 0 ? 0 : (x > _bkWidth ? _bkWidth : x);
            int y = e.Y - MARGIN0;
            y = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Check that there has been a change in the position
            if (x == _markerX && y == _markerY)
                return;

            drawMarker(x, y, false); // Redraw the slider
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

            // Get the marker position
            int x = e.X - MARGIN0;
            x = x < 0 ? 0 : (x > _bkWidth ? _bkWidth : x);
            int y = e.Y - MARGIN0;
            y = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Check that there has been a change in the position
            if (x == _markerX && y == _markerY)
                return;

            drawMarker(x, y, false); // Redraw the slider
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

            // Get the marker position
            int x = e.X - MARGIN0;
            x = x < 0 ? 0 : (x > _bkWidth ? _bkWidth : x);
            int y = e.Y - MARGIN0;
            y = y < 0 ? 0 : (y > _bkHeight ? _bkHeight : y);

            // Check that there has been a change in the position
            if (x == _markerX && y == _markerY)
                return;

            drawMarker(x, y, false); // Redraw the slider
            resetHSVRGB(); // Redraw the colors

            Scrolled?.Invoke(this, e); // Send "Scrolled" event
        }

        /// <summary>Repaint the control</summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            drawBorder(e.Graphics);
            e.Graphics.DrawImage(_bkBuff, MARGIN1, MARGIN1);
            drawMarker(e.Graphics, _markerX, _markerY);
        }

        /// <summary>Repaint the background</summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(this.BackColor))
            {
                int w = this.Width;
                int h = this.Height;
                e.Graphics.FillRectangle(b, 0, 0, w, MARGIN0);
                e.Graphics.FillRectangle(b, 0, MARGIN0, MARGIN0, h - MARGIN0);
                e.Graphics.FillRectangle(b, w - MARGIN0, MARGIN0, MARGIN0, h - MARGIN0);
                e.Graphics.FillRectangle(b, MARGIN0, h - MARGIN0, w - (MARGIN0 * 2), h);
            }
        }

        /// <summary>The control has been resized</summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            createBkBuff();
            drawContent();
            this.Invalidate();
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
                _h = h < 0.0 ? 0.0 : h > 360.0 ? 360.0 : h;
                _s = s < 0.0 ? 0.0 : s > 1.0 ? 1.0 : s;
                _v = v < 0.0 ? 0.0 : v > 1.0 ? 1.0 : v;
                ColorUtil.HSV2RGB(_h, _s, _v, out _r, out _g, out _b);
                resetMarker(true);
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
                resetMarker(true);
                drawContent();
                this.Invalidate();
            }
        }

        #endregion Public Methods
    }
}