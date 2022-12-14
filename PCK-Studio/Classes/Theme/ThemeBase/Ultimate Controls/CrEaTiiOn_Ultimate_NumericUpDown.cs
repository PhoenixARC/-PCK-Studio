using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    public sealed class CrEaTiiOn_Ultimate_NumericUpDown : Control
    {
        private int _w;
		private int _h;
        private int _x;
		private int _y;
		private long _value;
		private long _min;
		private long _max;
		private bool _bool;

        private Color _baseColor = Color.FromArgb(15, 15, 15);
        private Color _buttonColor = Color.FromArgb(250, 36, 38);
        private Color _stringColor = Color.White;

		public long Value { get => _value; set { if (value <= _max & value >= _min) _value = value; Invalidate(); } }
        public long Maximum { get => _max; set { if (value > _min) _max = value; if (_value > _max) _value = _max; Invalidate(); } }
        public long Minimum { get => _min; set { if (value < _max) _min = value; if (_value < _min) _value = Minimum; Invalidate(); } }

		[Category("CrEaTiiOn")]
		public Color BaseColor { get { return _baseColor; } set { _baseColor = value; Invalidate(); } }
		[Category("CrEaTiiOn")]
		public Color ButtonColor { get { return _buttonColor; } set { _buttonColor = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
		public Color StringColor { get => _stringColor; set => _stringColor = value; }

        public CrEaTiiOn_Ultimate_NumericUpDown()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            _min = 0;
            _max = 9999999;
        }

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			_x = e.Location.X;
			_y = e.Location.Y;
			Invalidate();
			Cursor = e.X < Width - 23 ? Cursors.IBeam : Cursors.Hand;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (_x > Width - 21 && _x < Width - 3)
			{
				if (_y < 15)
				{
					if ((Value + 1) <= _max)
						_value += 1;
				}
				else
				{
					if ((Value - 1) >= _min)
						_value -= 1;
				}
			}
			else
			{
				_bool = !_bool;
				Focus();
			}
			Invalidate();
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			try
			{
				if (_bool)
					_value = Convert.ToInt64(_value + e.KeyChar.ToString());
				if (_value > _max)
					_value = _max;
				Invalidate();
			}
            catch
            {
                // ignored
            }
        }

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Back)
			{
				Value = 0;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			Height = 30;
		}

        public static readonly StringFormat CenterSf = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };


		protected override void OnPaint(PaintEventArgs e)
		{
            var bitmap = new Bitmap(Width, Height);
			var graphics = Graphics.FromImage(bitmap);
			_w = Width;
			_h = Height;

			var rectBase = new Rectangle(0, 0, _w, _h);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			graphics.Clear(BackColor);

			graphics.FillRectangle(new SolidBrush(_baseColor), rectBase);
			graphics.FillRectangle(new SolidBrush(_buttonColor), new Rectangle(Width - 24, 0, 24, _h));
            graphics.DrawString("+", Font, new SolidBrush(_stringColor), new Point(Width - 12, 8), CenterSf);
            graphics.DrawString("-", Font, new SolidBrush(_stringColor), new Point(Width - 12, 22), CenterSf);
            graphics.DrawString(Value.ToString(), Font, new SolidBrush(_stringColor), new Rectangle(5, 1, _w, _h), new StringFormat { LineAlignment = StringAlignment.Center });

			base.OnPaint(e);
			graphics.Dispose();
			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			e.Graphics.DrawImageUnscaled(bitmap, 0, 0);
			bitmap.Dispose();
		}
	}
}
