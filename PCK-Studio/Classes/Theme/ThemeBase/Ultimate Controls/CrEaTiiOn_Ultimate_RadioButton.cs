using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    public sealed class CrEaTiiOn_Ultimate_RadioButton : RadioButton
    {
        private Color _checkedColor = Color.FromArgb(250, 36, 38);
        private Color _unCheckedColor = Color.FromArgb(15, 15, 15);
        private Color _backgroundColor = Color.FromArgb(20, 20, 20);

        [Category("CrEaTiiOn")]
        public Color CheckedColor { get => _checkedColor; set { _checkedColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color UnCheckedColor { get => _unCheckedColor; set { _unCheckedColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color BackgroundColor { get => _backgroundColor; set { _backgroundColor = value; Invalidate(); } }

        public CrEaTiiOn_Ultimate_RadioButton()
        {
            MinimumSize = new Size(0, 21);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rbBorderSize = 18f;
            var rbCheckSize = 12f;
            var rectRbBorder = new RectangleF()
            {
                X = 0.5f,
                Y = (Height - rbBorderSize) / 2,
                Width = rbBorderSize,
                Height = rbBorderSize
            };
            var rectRbCheck = new RectangleF()
            {
                X = rectRbBorder.X + (rectRbBorder.Width - rbCheckSize) / 2,
                Y = (Height - rbCheckSize) / 2,
                Width = rbCheckSize,
                Height = rbCheckSize
            };

            using(var penBorder = new Pen(_checkedColor, 1.6f))
            using(var brushRbCheck = new SolidBrush(_checkedColor))
            using(var brushBgCheck = new SolidBrush(_backgroundColor))
            using (var brushText = new SolidBrush(ForeColor))
            {
                graphics.Clear(_backgroundColor);
                if (Checked)
                {
                    graphics.DrawEllipse(penBorder, rectRbBorder);
                    graphics.FillEllipse(brushRbCheck, rectRbCheck);
                }
                else
                {
                    penBorder.Color = _unCheckedColor;
                    graphics.DrawEllipse(penBorder, rectRbBorder);
                }
                graphics.DrawString(Text, Font, brushText, rbBorderSize + 8, (Height - TextRenderer.MeasureText(Text, Font).Height) / 2f);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = TextRenderer.MeasureText(Text, Font).Width + 30;
        }
    }
}
