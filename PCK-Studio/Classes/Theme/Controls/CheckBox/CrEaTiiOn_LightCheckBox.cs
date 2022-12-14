#region Imports
using System;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    [DefaultEvent("CheckedChanged")]
    public class CrEaTiiOn_LightCheckBox : Control
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
        private int X;
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
            X = -1;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            State = MouseStates.Down;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            X = e.X;
            Invalidate();
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            State = MouseStates.Over;
            Invalidate();
            if (e.Button == MouseButtons.Left)
            {
                base.OnClick(null);
                _Checked = !_Checked;
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        //Do nothing here or it fires twice
        protected override void OnClick(EventArgs e)
        {
        }
        #endregion

        #region "Properties"

        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        public enum SliderLocations
        {
            Left = 0,
            Right = 1
        }

        private SliderLocations _SliderLocation = SliderLocations.Right;
        public SliderLocations SliderLocation
        {
            get { return _SliderLocation; }
            set
            {
                _SliderLocation = value;
                Invalidate();
            }
        }

        #endregion

        public CrEaTiiOn_LightCheckBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Font = new Font("Segoe UI", 9);
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            Size = new Size(150, 21);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            Brush LGB = default(Brush);

            if (X >= 0 & X < 17)
            {
                switch (State)
                {
                    case MouseStates.None:
                        LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(25, 25, 25), 90f);
                        break;
                    case MouseStates.Over:
                        LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(25, 25, 25), 90f);
                        break;
                    default:
                        LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(10, 10, 10), 90f);
                        break;
                }
            }
            else
            {
                LGB = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(20, 20, 20), Color.FromArgb(25, 25, 25), 90f);
            }
            e.Graphics.FillPath(LGB, CreateRound(1, 2, 15, 15, 7));

            if (_Checked)
            {
                e.Graphics.DrawString("a", new Font("Marlett", 13), Brushes.Red, new Point(-2, 1));
            }

            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(20, 0, Width - 21, Height - 1), new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            });
            base.OnPaint(e);
        }
    }
}