#region Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_LightSeperator : Control
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Parent.BackColor);

            Point p1 = new Point();
            Point p2 = new Point();
            p1 = new Point(0, (Height - 1) / 2);
            p2 = new Point(Width, (Height - 1) / 2);

            ColorBlend lineCB = new ColorBlend(3);
            lineCB.Colors = new[] { Color.Transparent, _lineColor, Color.Transparent };
            lineCB.Positions = new[] { 0.0F, 0.5F, 1.0F };

            LinearGradientBrush lineLGB = new LinearGradientBrush(p1, p2, Color.Transparent, Color.Transparent);
            lineLGB.InterpolationColors = lineCB;

            g.DrawLine(new Pen(lineLGB), p1, p2);

        }

        public CrEaTiiOn_LightSeperator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Size = new Size(400, 9);
        }

        private Color _lineColor = Color.Silver;
        public Color LineColor
        {
            get
            {
                return _lineColor;
            }
            set
            {
                _lineColor = value;
                Invalidate();
            }
        }

    }
}
