#region Imports
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    internal class CrEaTiiOn_CircularPictureBox : CrEaTiiOn_Ultimate_PictureBox
    {
        private int borderSize = 2;
        private Color borderColor = Color.RoyalBlue;
        private Color borderColor2 = Color.HotPink;
        private DashStyle borderLineStyle = DashStyle.Solid;
        private DashCap borderCapStyle = DashCap.Flat;
        private float gradientAngle = 50f;

        public CrEaTiiOn_CircularPictureBox()
        {
            this.Size = new Size(100, 100);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        [Category("Sipaa")]
        public int BorderSize
        {
            get => this.borderSize;
            set
            {
                this.borderSize = value;
                this.Invalidate();
            }
        }

        [Category("Sipaa")]
        public Color BorderColor
        {
            get => this.borderColor;
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        [Category("Sipaa")]
        public Color BorderColor2
        {
            get => this.borderColor2;
            set
            {
                this.borderColor2 = value;
                this.Invalidate();
            }
        }

        [Category("Sipaa")]
        public DashStyle BorderLineStyle
        {
            get => this.borderLineStyle;
            set
            {
                this.borderLineStyle = value;
                this.Invalidate();
            }
        }

        [Category("Sipaa")]
        public DashCap BorderCapStyle
        {
            get => this.borderCapStyle;
            set
            {
                this.borderCapStyle = value;
                this.Invalidate();
            }
        }

        [Category("Sipaa")]
        public float GradientAngle
        {
            get => this.gradientAngle;
            set
            {
                this.gradientAngle = value;
                this.Invalidate();
            }
        }

        public PictureBoxSizeMode SizeMode { get; private set; }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(this.Width, this.Width);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics graphics = pe.Graphics;
            Rectangle rect1 = Rectangle.Inflate(this.ClientRectangle, -1, -1);
            Rectangle rect2 = Rectangle.Inflate(rect1, -this.borderSize, -this.borderSize);
            int width = this.borderSize > 0 ? this.borderSize * 3 : 1;
            using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect2, this.borderColor, this.borderColor2, this.gradientAngle))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    using (Pen pen1 = new Pen(this.Parent.BackColor, (float)width))
                    {
                        using (Pen pen2 = new Pen((Brush)linearGradientBrush, (float)this.borderSize))
                        {
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            pen2.DashStyle = this.borderLineStyle;
                            pen2.DashCap = this.borderCapStyle;
                            path.AddEllipse(rect1);
                            this.Region = new Region(path);
                            graphics.DrawEllipse(pen1, rect1);
                            if (this.borderSize <= 0)
                                return;
                            graphics.DrawEllipse(pen2, rect2);
                        }
                    }
                }
            }
        }
    }
}
