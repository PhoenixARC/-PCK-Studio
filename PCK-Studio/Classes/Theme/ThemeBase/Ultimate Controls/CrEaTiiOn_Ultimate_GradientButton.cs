using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    public sealed class CrEaTiiOn_Ultimate_GradientButton : Button
    {
        private int _borderSize = 1;
        private int _borderRadius = 25;
        private Color _gradientColorPrimary = Color.FromArgb(250, 36, 38);
        private Color _gradientColorSecondary = Color.FromArgb(75, 75, 75);

        [Category("CrEaTiiOn")]
        public int BorderSize { get => _borderSize; set { _borderSize = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public int BorderRadius { get => _borderRadius; set { if (value <= Height) _borderRadius = value; else BorderRadius = Height; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color BackgroundColor { get => BackColor; set => BackColor = value; }
        [Category("CrEaTiiOn")]
        public Color TextColor { get => ForeColor; set => ForeColor = value; }
        [Category("CrEaTiiOn")]
        public Color GradientColorPrimary { get => _gradientColorPrimary; set { _gradientColorPrimary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color GradientColorSecondary { get => _gradientColorSecondary; set { _gradientColorSecondary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color HoverOverColor { get => FlatAppearance.MouseOverBackColor; set { FlatAppearance.MouseOverBackColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color ClickedColor { get => FlatAppearance.MouseDownBackColor; set { FlatAppearance.MouseDownBackColor = value; Invalidate(); } }

        public CrEaTiiOn_Ultimate_GradientButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(150, 40);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 25, 25);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 15, 15);
            Resize += OnResize;
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (_borderRadius > Height)
                BorderRadius = Height;

        }

        private GraphicsPath GetFigurePath(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rectSurface = new RectangleF(0, 0, Width, Height);
            var rectBorder = new RectangleF(1, 1, Width - 0.8f, Height - 1);


            if (BorderRadius > 2)
            {
                using (var pathSurface = GetFigurePath(rectSurface, BorderRadius))
                using (var pathBorder = GetFigurePath(rectBorder, BorderRadius - 1))
                using (var penSurface = new Pen(Parent.BackColor, 2))
                using (var penBorder = new Pen(new LinearGradientBrush(new PointF(0, Height / 2f), new PointF(Width, Height / 2f), _gradientColorPrimary, _gradientColorSecondary), BorderSize))
                {
                    penBorder.Alignment = PenAlignment.Inset;
                    Region = new Region(pathSurface);
                    pevent.Graphics.DrawPath(penSurface, pathSurface);
                    if (BorderSize >= 1)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else
            {
                Region = new Region(rectSurface);
                if (BorderSize >= 1)
                {
                    using (var penBorder = new Pen(new LinearGradientBrush(new PointF(0, Height / 2f), new PointF(Width, Height / 2f), _gradientColorPrimary, _gradientColorSecondary), BorderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += ParentOnBackColorChanged;
        }

        private void ParentOnBackColorChanged(object sender, EventArgs e)
        {
            if (DesignMode)
                Invalidate();
        }
    }
}
