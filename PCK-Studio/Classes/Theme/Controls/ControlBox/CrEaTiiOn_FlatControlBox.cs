#region Imports
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Zeroit.CBH.Controls
{
    public class CrEaTiiOn_FlatControlBox : Control
    {
        public enum ColorSchemes
        {
            Dark
        }
        public event ColorSchemeChangedEventHandler ColorSchemeChanged;
        public delegate void ColorSchemeChangedEventHandler();
        private ColorSchemes _ColorScheme;
        public ColorSchemes ColorScheme
        {
            get { return _ColorScheme; }
            set
            {
                _ColorScheme = value;
                if (ColorSchemeChanged != null)
                {
                    ColorSchemeChanged();
                }
            }
        }
        protected void OnColorSchemeChanged()
        {
            Invalidate();
            switch (ColorScheme)
            {
                case ColorSchemes.Dark:
                    BackColor = Color.FromArgb(15, 15, 15);
                    ForeColor = Color.White;
                    AccentColor = Color.FromArgb(60, 60, 60);
                    break;
            }
        }
        private Color _AccentColor;
        public Color AccentColor
        {
            get { return _AccentColor; }
            set
            {
                _AccentColor = value;
                Invalidate();
            }
        }

        public CrEaTiiOn_FlatControlBox() : base()
        {
            ColorSchemeChanged += OnColorSchemeChanged;
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ForeColor = Color.FromArgb(50, 50, 50);
            BackColor = Color.FromArgb(15, 15, 15);
            AccentColor = Color.FromArgb(250, 36, 38);
            ColorScheme = ColorSchemes.Dark;
            Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(100, 25);
        }
        public enum ButtonHover
        {
            Minimize,
            Maximize,
            Close,
            None
        }
        ButtonHover ButtonState = ButtonHover.None;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int X = e.Location.X;
            int Y = e.Location.Y;
            if (Y > 0 && Y < (Height - 2))
            {
                if (X > 0 && X < 34)
                {
                    ButtonState = ButtonHover.Minimize;
                }
                else if (X > 33 && X < 65)
                {
                    ButtonState = ButtonHover.Maximize;
                }
                else if (X > 64 && X < Width)
                {
                    ButtonState = ButtonHover.Close;
                }
                else
                {
                    ButtonState = ButtonHover.None;
                }
            }
            else
            {
                ButtonState = ButtonHover.None;
            }
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            base.OnPaint(e);

            G.Clear(BackColor);
            switch (ButtonState)
            {
                case ButtonHover.None:
                    G.Clear(BackColor);
                    break;
                case ButtonHover.Minimize:
                    G.FillRectangle(new SolidBrush(_AccentColor), new Rectangle(3, 0, 30, Height));
                    break;
                case ButtonHover.Maximize:
                    G.FillRectangle(new SolidBrush(_AccentColor), new Rectangle(34, 0, 30, Height));
                    break;
                case ButtonHover.Close:
                    G.FillRectangle(new SolidBrush(_AccentColor), new Rectangle(65, 0, 35, Height));
                    break;
            }

            Font ButtonFont = new Font("Marlett", 9.75f);
            //Close
            G.DrawString("r", ButtonFont, new SolidBrush(Color.FromArgb(210, 210, 210)), new Point(Width - 16, 7), new StringFormat { Alignment = StringAlignment.Center });
            //Maximize
            switch (Parent.FindForm().WindowState)
            {
                case FormWindowState.Maximized:
                    G.DrawString("2", ButtonFont, new SolidBrush(Color.FromArgb(210, 210, 210)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    break;
                case FormWindowState.Normal:
                    G.DrawString("1", ButtonFont, new SolidBrush(Color.FromArgb(210, 210, 210)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    break;
            }
            //Minimize
            G.DrawString("0", ButtonFont, new SolidBrush(Color.FromArgb(210, 210, 210)), new Point(20, 7), new StringFormat { Alignment = StringAlignment.Center });


            e.Graphics.DrawImage(B, new Point(0, 0));
            G.Dispose();
            B.Dispose();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            switch (ButtonState)
            {
                case ButtonHover.Close:
                    Parent.FindForm().Close();
                    break;
                case ButtonHover.Minimize:
                    Parent.FindForm().WindowState = FormWindowState.Minimized;
                    break;
                case ButtonHover.Maximize:
                    if (Parent.FindForm().WindowState == FormWindowState.Normal)
                    {
                        Parent.FindForm().WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        Parent.FindForm().WindowState = FormWindowState.Normal;
                    }

                    break;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ButtonState = ButtonHover.None;
            Invalidate();
        }
    }


}


