#region Imports
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_FlatPanel : Panel
    {
        private int borderSize = 0;
        private int borderRadius = 6;
        private Color borderColor = Color.FromArgb(250, 36, 38);

        public CrEaTiiOn_FlatPanel()
        {
            this.Size = new Size(150, 40);
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
            this.Resize += new EventHandler(this.Panel_Resize);
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            if (this.borderRadius <= this.Height)
                return;
            this.borderRadius = this.Height;
        }

        private GraphicsPath GetFigurePath(Rectangle rect, float radius)
        {
            GraphicsPath figurePath = new GraphicsPath();
            float num = radius * 2f;
            figurePath.StartFigure();
            figurePath.AddArc((float)rect.X, (float)rect.Y, num, num, 180f, 90f);
            figurePath.AddArc((float)rect.Right - num, (float)rect.Y, num, num, 270f, 90f);
            figurePath.AddArc((float)rect.Right - num, (float)rect.Bottom - num, num, num, 0.0f, 90f);
            figurePath.AddArc((float)rect.X, (float)rect.Bottom - num, num, num, 90f, 90f);
            figurePath.CloseFigure();
            return figurePath;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Rectangle clientRectangle = this.ClientRectangle;
            Rectangle rect = Rectangle.Inflate(clientRectangle, -this.borderSize, -this.borderSize);
            int width = 2;
            if (this.borderSize > 0)
                width = this.borderSize;
            if (this.borderRadius > 2)
            {
                using (GraphicsPath figurePath1 = this.GetFigurePath(clientRectangle, (float)this.borderRadius))
                {
                    using (GraphicsPath figurePath2 = this.GetFigurePath(rect, (float)(this.borderRadius - this.borderSize)))
                    {
                        using (Pen pen1 = new Pen(this.Parent.BackColor, (float)width))
                        {
                            using (Pen pen2 = new Pen(this.borderColor, (float)this.borderSize))
                            {
                                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                this.Region = new Region(figurePath1);
                                pevent.Graphics.DrawPath(pen1, figurePath1);
                                if (this.borderSize < 1)
                                    return;
                                pevent.Graphics.DrawPath(pen2, figurePath2);
                            }
                        }
                    }
                }
            }
            else
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.None;
                this.Region = new Region(clientRectangle);
                if (this.borderSize >= 1)
                {
                    using (Pen pen = new Pen(this.borderColor, (float)this.borderSize))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                    }
                }
            }
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
        public int BorderRadius
        {
            get => this.borderRadius;
            set
            {
                this.borderRadius = value;
                this.Invalidate();
            }
        }

        protected override void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        private void Container_BackColorChanged(object sender, EventArgs e) => this.Invalidate();

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
    }
}
