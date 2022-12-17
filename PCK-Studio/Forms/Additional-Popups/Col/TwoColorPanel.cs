using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColorPicker
{
    /// <summary>A panel to display two colors</summary>
    public partial class TwoColorPanel : UserControl
    {
        #region Private Constant/Read-Only Fields

        /// <summary>Checkered  size</summary>
        private readonly int CHECK = 10;

        #endregion Private Constant/Read-Only Fields

        #region Private Fields

        private Bitmap _bkBuff1 = null;
        private Bitmap _bkBuff2 = null;
        private int _bkHeight;
        private int _bkWidth;
        private Color _c1 = Color.Red;
        private Color _c2 = Color.Orange;

        #endregion Private Fields

        #region Constructors

        /// <summary>Constructor</summary>
        public TwoColorPanel()
            : this(Color.Red, Color.Orange)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        public TwoColorPanel(Color c1, Color c2)
        {
            InitializeComponent();
            this.Disposed += twoColorPanel_Disposed;
            _c1 = c1;
            _c2 = c2;
            createBkBuff();
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>Color 1</summary>
        public Color Color1
        {
            get => _c1;
            set
            {
                _c1 = value;
                createBkBuff();
                draw();
                this.Invalidate();
            }
        }

        /// <summary>Color 2</summary>
        public Color Color2
        {
            get => _c2;
            set
            {
                _c2 = value;
                draw();
                this.Invalidate();
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>Create the backgroubd Bitmaps</summary>
        private void createBkBuff()
        {
            _bkHeight = this.Height;
            _bkWidth = this.Width;
            if (_bkBuff1 != null)
                _bkBuff1.Dispose();
            _bkBuff1 = new Bitmap(_bkWidth, _bkHeight);
            using (Graphics g = Graphics.FromImage(_bkBuff1))
            {
                for (int x = 0; x < _bkWidth; x += CHECK)
                {
                    for (int y = 0; y < _bkHeight; y += CHECK)
                    {
                        g.FillRectangle((x + y) / CHECK % 2 == 0 ? Brushes.White : Brushes.Black, x, y, CHECK, CHECK);
                    }
                }
                using (SolidBrush b1 = new SolidBrush(Color1))
                {
                    g.FillRectangle(b1, 0, 0, _bkWidth, _bkHeight / 2);
                }
            }
            if (_bkBuff2 != null)
                _bkBuff2.Dispose();
            _bkBuff2 = new Bitmap(_bkWidth, _bkHeight);
        }

        /// <summary>Draw then background Bitmap</summary>
        private void draw()
        {
            using (Graphics g = Graphics.FromImage(_bkBuff2))
            {
                g.DrawImage(_bkBuff1, 0, 0);
                int h2 = _bkHeight / 2;
                using (SolidBrush b2 = new SolidBrush(Color2))
                {
                    g.FillRectangle(b2, 0, h2, _bkWidth, _bkHeight - h2);
                }
            }
        }

        /// <summary>Dispose the background Bitmaps</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void twoColorPanel_Disposed(object sender, System.EventArgs e)
        {
            if (_bkBuff1 != null)
            {
                _bkBuff1.Dispose();
                _bkBuff1 = null;
            }
            if (_bkBuff2 != null)
            {
                _bkBuff2.Dispose();
                _bkBuff2 = null;
            }
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>Remove flickering</summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>Paint the control</summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_bkBuff2, 0, 0);
        }

        /// <summary>Size changed</summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            createBkBuff();
        }

        #endregion Protected Methods
    }
}