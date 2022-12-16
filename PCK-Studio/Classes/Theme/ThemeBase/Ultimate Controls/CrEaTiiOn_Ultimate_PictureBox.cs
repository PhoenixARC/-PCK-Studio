using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    public sealed class CrEaTiiOn_Ultimate_PictureBox : PictureBox
    {
        private int _borderSize = 2;
        private Color _gradientColorPrimary = Color.FromArgb(250, 36, 38);
        private Color _gradientColorSecondary = Color.FromArgb(25, 25, 25);
        private DashStyle _borderLineStyle = DashStyle.Solid;
        private DashCap _borderCapStyle = DashCap.Flat;
        private float _gradientAngle = 50f;

        public CrEaTiiOn_Ultimate_PictureBox()
        {
            Size = new Size(100, 100);
            SizeMode = PictureBoxSizeMode.StretchImage;

        }

        [Category("CrEaTiiOn")]
        public int BorderSize { get => _borderSize; set { _borderSize = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color GradientColorPrimary { get => _gradientColorPrimary; set { _gradientColorPrimary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color GradientColorSecondary { get => _gradientColorSecondary; set { _gradientColorSecondary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public DashStyle BorderLineStyle { get => _borderLineStyle; set { _borderLineStyle = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public DashCap BorderCapStyle { get => _borderCapStyle; set { _borderCapStyle = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public float GradientAngle { get => _gradientAngle; set { _gradientAngle = value; Invalidate(); } }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, Width);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            var graphics = pe.Graphics;
            var rectContourSmooth = Rectangle.Inflate(ClientRectangle, -1, -1);
            var rectBorder = Rectangle.Inflate(ClientRectangle, -_borderSize, -_borderSize);
            var smoothSize = _borderSize > 0 ? _borderSize * 3 : 1;

            using (var borderGradientColor = new LinearGradientBrush(rectBorder, _gradientColorPrimary, _gradientColorSecondary, _gradientAngle))
            using (var pathRegion = new GraphicsPath())
            using (var penSmooth = new Pen(Parent.BackColor, smoothSize))
            using (var penBorder = new Pen(borderGradientColor, _borderSize))
            {
                penBorder.DashStyle = _borderLineStyle;
                penBorder.DashCap = _borderCapStyle;
                pathRegion.AddEllipse(rectContourSmooth);
                Region = new Region(pathRegion);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                graphics.DrawEllipse(penSmooth, rectContourSmooth);
                if(_borderSize > 0)
                    graphics.DrawEllipse(penBorder, rectBorder);
            }
        }
    }
}
