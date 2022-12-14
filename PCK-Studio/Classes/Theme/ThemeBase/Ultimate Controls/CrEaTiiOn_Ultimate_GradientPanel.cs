#region Imports

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace CBH_Ultimate_Theme_Library.Theme.ThemeBase
{
    #region CrEaTiiOn_GradientPanel

    public class CrEaTiiOn_Ultimate_GradientPanel : Panel
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public CrEaTiiOn_Ultimate_GradientPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            BackColor = Color.White;
            Size = new Size(200, 200);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Color BackColor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Color ForeColor { get; set; }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The primer color")]
        public Color PrimerColor
        {
            get => primerColor;
            set
            {
                primerColor = value;
                Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The top left color")]
        public Color TopLeft
        {
            get => topLeft;
            set
            {
                topLeft = value;
                Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The top right color")]
        public Color TopRight
        {
            get => topRight;
            set
            {
                topRight = value;
                Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The bottom left color")]
        public Color BottomLeft
        {
            get => bottomLeft;
            set
            {
                bottomLeft = value;
                Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The bottom right color")]
        public Color BottomRight
        {
            get => bottomRight;
            set
            {
                bottomRight = value;
                Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The gradient orientation")]
        public GradientStyle Style
        {
            get => style;
            set
            {
                style = value;
                Refresh();
            }
        }

        private SmoothingMode _SmoothingType = SmoothingMode.AntiAlias;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public SmoothingMode SmoothingType
        {
            get => _SmoothingType;
            set
            {
                _SmoothingType = value;
                Invalidate();
            }
        }

        private InterpolationMode _InterpolationType = InterpolationMode.HighQualityBilinear;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public InterpolationMode InterpolationType
        {
            get => _InterpolationType;
            set
            {
                _InterpolationType = value;
                Invalidate();
            }
        }

        private CompositingQuality _CompositingQualityType = CompositingQuality.HighQuality;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public CompositingQuality CompositingQualityType
        {
            get => _CompositingQualityType;
            set
            {
                _CompositingQualityType = value;
                Invalidate();
            }
        }

        private PixelOffsetMode _PixelOffsetType = PixelOffsetMode.HighQuality;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public PixelOffsetMode PixelOffsetType
        {
            get => _PixelOffsetType;
            set
            {
                _PixelOffsetType = value;
                Invalidate();
            }
        }

        private TextRenderingHint _TextRenderingType = TextRenderingHint.ClearTypeGridFit;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public TextRenderingHint TextRenderingType
        {
            get => _TextRenderingType;
            set
            {
                _TextRenderingType = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphicsContext bufferedGraphicsContext = BufferedGraphicsManager.Current;
            bufferedGraphicsContext.MaximumBuffer = new Size(Width + 1, Height + 1);
            bufferedGraphics = bufferedGraphicsContext.Allocate(CreateGraphics(), ClientRectangle);
            bufferedGraphics.Graphics.SmoothingMode = SmoothingType;
            bufferedGraphics.Graphics.InterpolationMode = InterpolationType;
            bufferedGraphics.Graphics.CompositingQuality = CompositingQualityType;
            bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetType;
            bufferedGraphics.Graphics.TextRenderingHint = TextRenderingType;
            bufferedGraphics.Graphics.Clear(primerColor);
            if (style == GradientStyle.Corners)
            {
                LinearGradientBrush linearGradientBrush = new(new Rectangle(0, 0, Width, Height), TopLeft, Color.Transparent, 45f);
                bufferedGraphics.Graphics.FillRectangle(linearGradientBrush, ClientRectangle);
                linearGradientBrush = new(new Rectangle(0, 0, Width, Height), topRight, Color.Transparent, 135f);
                bufferedGraphics.Graphics.FillRectangle(linearGradientBrush, ClientRectangle);
                linearGradientBrush = new(new Rectangle(0, 0, Width, Height), bottomRight, Color.Transparent, 225f);
                bufferedGraphics.Graphics.FillRectangle(linearGradientBrush, ClientRectangle);
                linearGradientBrush = new(new Rectangle(0, 0, Width, Height), bottomLeft, Color.Transparent, 315f);
                bufferedGraphics.Graphics.FillRectangle(linearGradientBrush, ClientRectangle);
                linearGradientBrush.Dispose();
            }
            else
            {
                Brush brush;
                if (style == GradientStyle.Vertical)
                {
                    brush = new LinearGradientBrush(ClientRectangle, topLeft, topRight, 720f);
                }
                else
                {
                    brush = new LinearGradientBrush(ClientRectangle, topLeft, topRight, 90f);
                }
                bufferedGraphics.Graphics.FillRectangle(brush, ClientRectangle);
                brush.Dispose();
            }
            bufferedGraphics.Render(e.Graphics);
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private BufferedGraphics bufferedGraphics;

        private Color primerColor = Color.FromArgb(20, 20, 20);

        private Color topLeft = Color.FromArgb(250, 36, 38);

        private Color topRight = Color.FromArgb(250, 36, 38);

        private Color bottomLeft = Color.FromArgb(15, 15, 15);

        private Color bottomRight = Color.FromArgb(250, 36, 38);

        private GradientStyle style = GradientStyle.Corners;

        public enum GradientStyle
        {
            Horizontal,
            Vertical,
            Corners
        }
    }

    #endregion
}