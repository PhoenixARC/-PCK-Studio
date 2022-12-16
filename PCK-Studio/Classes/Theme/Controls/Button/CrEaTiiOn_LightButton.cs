#region Imports
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_LightButton : Control
    {

        #region "CreateRound"
        private GraphicsPath CreateRoundPath;

        private Rectangle CreateRoundRectangle;
        public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
        {
            CreateRoundRectangle = new Rectangle(x, y, width, height);
            return CreateRound(CreateRoundRectangle, slope);
        }

        public GraphicsPath CreateRound(Rectangle r, int slope)
        {
            CreateRoundPath = new GraphicsPath(FillMode.Winding);
            CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
            CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
            CreateRoundPath.CloseFigure();
            return CreateRoundPath;
        }
        #endregion

        #region "Mouse states"
        private MouseStates State;
        public enum MouseStates
        {
            None = 0,
            Over = 1,
            Down = 2
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            State = MouseStates.Over;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            State = MouseStates.None;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            State = MouseStates.Down;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            State = MouseStates.Over;
            Invalidate();
            if (e.Button == MouseButtons.Left)
                base.OnClick(null);
            //This fixes some fucked up lag you get...
            base.OnMouseDown(e);
        }

        //Do nothing here or it fires twice
        protected override void OnClick(EventArgs e)
        {
        }
        #endregion

        #region "Properties"

        public enum ImageAlignments
        {
            Left = 0,
            Center = 1,
            Right = 2
        }

        ImageAlignments _ImageAlignment = ImageAlignments.Left;
        public ImageAlignments ImageAlignment
        {
            get { return _ImageAlignment; }
            set
            {
                _ImageAlignment = value;
                Invalidate();
            }
        }

        Image _Image;
        public Image Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                Invalidate();
            }
        }

        #endregion

        public CrEaTiiOn_LightButton()
        {
            Size = new Size(120, 31);
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.FromArgb(250, 36, 38);
            Font = new Font("Segoe UI", 9);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            Brush LGB = default(Brush);

            switch (State)
            {
                case MouseStates.None:
                    LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(25, 25, 25), 90f);
                    break;
                case MouseStates.Over:
                    LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(25, 25, 25), 90f);
                    break;
                default:
                    LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(18, 18, 18), 90f);
                    break;
            }
            if (!Enabled)
            {
                LGB = new SolidBrush(Color.FromArgb(20, 20, 20));
            }
            e.Graphics.FillPath(LGB, CreateRound(0, 0, Width - 1, Height - 1, 6));

            if ((_Image == null))
            {
                e.Graphics.DrawString(Text, Font, Brushes.White, new Rectangle(3, 2, Width - 7, Height - 5), new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                });
            }
            else
            {
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                switch (_ImageAlignment)
                {
                    case ImageAlignments.Left:
                        Rectangle ImageRect = new Rectangle(9, 6, Height - 13, Height - 13);
                        e.Graphics.DrawImage(_Image, ImageRect);
                        e.Graphics.DrawString(Text, Font, Brushes.White, new Rectangle(ImageRect.X + ImageRect.Width + 6, 2, Width - ImageRect.Width - 22, Height - 5), new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        });
                        break;
                    case ImageAlignments.Center:
                        Rectangle ImageRect1 = new Rectangle(((Width - 1) / 2) - (Height - 13) / 2, 6, Height - 13, Height - 13);
                        e.Graphics.DrawImage(_Image, ImageRect1);
                        break;
                    case ImageAlignments.Right:
                        Rectangle ImageRect2 = new Rectangle(Width - Height + 3, 6, Height - 13, Height - 13);
                        e.Graphics.DrawImage(_Image, ImageRect2);
                        e.Graphics.DrawString(Text, Font, Brushes.White, new Rectangle(3, 2, Width - ImageRect2.Width - 22, Height - 5), new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        });
                        break;
                }
            }

            base.OnPaint(e);
        }

        public void PerformClick()
        {
            base.OnClick(null);
        }
    }
}