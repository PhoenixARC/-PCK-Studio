#region Imports
using CBH_Ultimate_Theme_Library.Theme.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_Message : Control
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Parent.BackColor);

            DrawingHelper dh = new DrawingHelper();
            MultiLineHandler mlh = new MultiLineHandler();
            int slope = Adjustments.Roundness;

            Rectangle mainRect = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath mainPath = dh.RoundRect(mainRect, slope);

            Color bgColor = new Color();
            Color borderColor = new Color();
            Color textColor = new Color();

            bgColor = dh.AdjustColor(BackColor, 30, DrawingHelper.ColorAdjustmentType.Lighten);

            ColorBlend bgCB = new ColorBlend(3);
            bgCB.Colors = new[] { bgColor, bgColor, dh.AdjustColor(bgColor, 15, DrawingHelper.ColorAdjustmentType.Darken) };
            bgCB.Positions = new[] { 0.0F, 0.7F, 1.0F };

            LinearGradientBrush bgLGB = new LinearGradientBrush(mainRect, Color.FromArgb(250, 36, 38), Color.FromArgb(250, 36, 38), 90.0F);
            bgLGB.InterpolationColors = bgCB;

            borderColor = dh.AdjustColor(bgColor, 20, DrawingHelper.ColorAdjustmentType.Darken);
            textColor = dh.AdjustColor(bgColor, 95, DrawingHelper.ColorAdjustmentType.Darken);

            g.FillPath(bgLGB, mainPath);
            g.DrawPath(new Pen(borderColor), mainPath);

            int descY = 0;

            if (Header != null)
            {
                g.DrawString(_header, _headerFont, new SolidBrush(textColor), new Point(8, 8));
                descY = (int)(8 + g.MeasureString(_header, _headerFont).Height + 6);
            }
            else
            {
                descY = 8;
            }

            foreach (string line in mlh.GetLines(g, Text, Font, Width - 16))
            {
                g.DrawString(line, Font, new SolidBrush(textColor), new Point(8, descY));
                descY += 13;
            }

            if (_sizeByText)
            {
                Size = new Size(Width, 8 + descY + 4);
            }

        }

        public CrEaTiiOn_Message()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(250, 36, 38);
            Font = new Font(Adjustments.DefaultFontFamily, 8);
            Size = new Size(600, 80);
        }

        private string _header = "Welcome!";
        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
                Invalidate();
            }
        }

        private Font _headerFont = new Font(Adjustments.DefaultFontFamily, 10F, FontStyle.Bold);
        public Font HeaderFont
        {
            get
            {
                return _headerFont;
            }
            set
            {
                _headerFont = value;
                Invalidate();
            }
        }

        private bool _sizeByText = false;
        public bool SizeByText
        {
            get
            {
                return _sizeByText;
            }
            set
            {
                _sizeByText = value;
                Invalidate();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Font = new Font(Font.FontFamily, 8);
        }

    }
}
