﻿using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace CBH.Ultimate.Controls
{
    public sealed class CrEaTiiOn_Ultimate_GradientToggleButton : CheckBox
    {

        private Color _gradientColorPrimary = Color.FromArgb(250, 36, 38);
        private Color _gradientColorSecondary = Color.Black;
        private Color _onToggleColor = Color.Black;
        private Color _offBackColor = Color.FromArgb(15, 15, 15);
        private Color _offToggleColor = Color.White;
        private bool _solidStyle = true;

        [Category("CrEaTiiOn")]
        public Color GradientColorPrimary { get => _gradientColorPrimary; set { _gradientColorPrimary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color GradientColorSecondary { get => _gradientColorSecondary; set { _gradientColorSecondary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color OnToggleColor { get => _onToggleColor; set { _onToggleColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color OffBackColor { get => _offBackColor; set { _offBackColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color OffToggleColor { get => _offToggleColor; set { _offToggleColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        [DefaultValue(true)]
        public bool SolidStyle { get => _solidStyle; set { _solidStyle = value; Invalidate(); } }

        public override string Text
        {
            get => base.Text;
            set { }
        }

        public CrEaTiiOn_Ultimate_GradientToggleButton()
        {
            MinimumSize = new Size(45, 22);
        }

        private GraphicsPath GetFigurePath()
        {
            var arcSize = Height - 1;
            var leftArc = new Rectangle(0, 0, arcSize, arcSize);
            var rightArc = new Rectangle(Width - arcSize - 2, 0, arcSize, arcSize);

            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();

            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var toggleSize = Height - 5;
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.Clear(BackColor);

            if (Checked)
            {
                if (_solidStyle)
                    pevent.Graphics.FillPath(new LinearGradientBrush(new PointF(0, Height / 2f), new PointF(Width, Height / 2f), _gradientColorPrimary, _gradientColorSecondary), GetFigurePath());
                else
                    pevent.Graphics.DrawPath(new Pen(new LinearGradientBrush(new PointF(0, Height / 2f), new PointF(Width, Height / 2f), _gradientColorPrimary, _gradientColorSecondary), 2), GetFigurePath());

                pevent.Graphics.FillEllipse(new SolidBrush(_onToggleColor), new Rectangle(Width - Height + 1, 2, toggleSize, toggleSize));
            }
            else
            {
                if (_solidStyle)
                    pevent.Graphics.FillPath(new SolidBrush(_offBackColor), GetFigurePath());
                else
                    pevent.Graphics.DrawPath(new Pen(_offBackColor), GetFigurePath());
                pevent.Graphics.FillEllipse(new SolidBrush(_offToggleColor), new Rectangle(2, 2, toggleSize, toggleSize));
            }
        }
    }
}
