using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    [DefaultEvent("TextHasChanged")]
    public partial class CrEaTiiOn_Ultimate_GradientTextBox : UserControl
    {
        private Color _gradientColorPrimary = Color.FromArgb(250, 36, 38);
        private Color _gradientColorSecondary = Color.FromArgb(75, 75, 75);
        private int _borderSize = 2;
        private bool _underlinedStyle;
        private Color _borderFocusColor = Color.FromArgb(250, 36, 38);
        private bool _isFocused;
        private int _borderRadius;
        private Color _placeholderColor = Color.FromArgb(15, 15, 15);
        private string _placeholderText = string.Empty;
        private bool _isPlaceholder;
        private bool _isPasswordChar;

        public CrEaTiiOn_Ultimate_GradientTextBox()
        {
            InitializeComponent();

        }

        public event EventHandler TextHasChanged;


        [Category("CrEaTiiOn")]
        public Color GradientColorPrimary { get => _gradientColorPrimary; set { _gradientColorPrimary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color GradientColorSecondary { get => _gradientColorSecondary; set { _gradientColorSecondary = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public int BorderSize { get => _borderSize; set { _borderSize = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public bool UnderlinedStyle { get => _underlinedStyle; set { _underlinedStyle = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public bool PasswordChar { get => _isPasswordChar; set { _isPasswordChar = value; textBox1.UseSystemPasswordChar = value; } }
        [Category("CrEaTiiOn")]
        public bool Multiline { get => textBox1.Multiline; set => textBox1.Multiline = value; }
        [Category("CrEaTiiOn")]
        public string String { get { if (_isPlaceholder) return string.Empty; return textBox1.Text; } set { textBox1.Text = value; SetPlaceHolder(); } }
        [Category("CrEaTiiOn")]
        public override Color BackColor { get => base.BackColor; set { base.BackColor = value; textBox1.BackColor = value; } }
        [Category("CrEaTiiOn")]
        public override Color ForeColor { get => base.ForeColor; set { base.ForeColor = value; textBox1.ForeColor = value; } }
        [Category("CrEaTiiOn")]
        public override Font Font { get => base.Font; set { base.Font = value; textBox1.Font = value; if (DesignMode) UpdateControlHeight(); } }
        [Category("CrEaTiiOn")]
        public Color BorderFocusColor { get => _borderFocusColor; set => _borderFocusColor = value; }
        [Category("CrEaTiiOn")]
        public int BorderRadius { get => _borderRadius; set { if (value >= 0) _borderRadius = value; Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color PlaceholderColor { get => _placeholderColor; set { _placeholderColor = value; if (_isPlaceholder) textBox1.ForeColor = value; } }
        [Category("CrEaTiiOn")]
        public string PlaceholderText { get => _placeholderText; set { _placeholderText = value; textBox1.Text = String.Empty; SetPlaceHolder(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;

            if (_borderRadius > 1)
            {
                var rectBorderSmooth = ClientRectangle;
                var rectBorder = Rectangle.Inflate(rectBorderSmooth, -_borderSize, -_borderSize);
                int smoothSize = _borderSize > 0 ? _borderSize : 1;

                using (GraphicsPath pathBorderSmooth = GetFigurePath(rectBorderSmooth, _borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, _borderRadius - _borderSize))
                using (Pen penBorderSmooth = new Pen(Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(new LinearGradientBrush(new PointF(0, Height / 2f), new PointF(Width, Height / 2f), _gradientColorPrimary, _gradientColorSecondary), _borderSize))
                {
                    Region = new Region(pathBorderSmooth);
                    if (_borderSize > 15) SetTextBoxRoundedRegion();
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    penBorder.Alignment = PenAlignment.Center;

                    if (_isFocused)
                        penBorder.Color = _borderFocusColor;

                    if (_underlinedStyle)
                    {
                        graphics.DrawPath(penBorderSmooth, pathBorderSmooth);
                        graphics.SmoothingMode = SmoothingMode.None;
                        graphics.DrawLine(penBorder, 0, Height - 1, Width, Height - 1);
                    }
                    else
                    {
                        graphics.DrawPath(penBorderSmooth, pathBorderSmooth);
                        graphics.DrawPath(penBorder, pathBorder);
                    }
                }
            }
            else
            {
                using (Pen penBorder = new Pen(new LinearGradientBrush(new PointF(0, Height / 2f), new PointF(Width, Height / 2f), _gradientColorPrimary, _gradientColorSecondary), _borderSize))
                {
                    Region = new Region(ClientRectangle);
                    penBorder.Alignment = PenAlignment.Inset;

                    if (_isFocused)
                        penBorder.Color = _borderFocusColor;

                    if (_underlinedStyle)
                        graphics.DrawLine(penBorder, 0, Height - 1, Width, Height - 1);
                    else
                        graphics.DrawRectangle(penBorder, 0, 0, Width - 0.5f, Height - 0.5f);
                }
            }
        }

        private void SetTextBoxRoundedRegion()
        {
            GraphicsPath pathText;

            pathText = GetFigurePath(textBox1.ClientRectangle, Multiline ? _borderRadius - _borderSize : _borderRadius * 2);
            textBox1.Region = new Region(pathText);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode)
                UpdateControlHeight();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControlHeight();
        }

        private void UpdateControlHeight()
        {
            if (!textBox1.Multiline)
            {
                int textHeight = TextRenderer.MeasureText("Text", Font).Height + 1;
                textBox1.Multiline = true;
                textBox1.MinimumSize = new Size(0, textHeight);
                textBox1.Multiline = false;
                Height = textBox1.Height + Padding.Top + Padding.Bottom;
            }
        }

        private void SetPlaceHolder()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) && _placeholderText != String.Empty)
            {
                _isPlaceholder = true;
                textBox1.Text = _placeholderText;
                textBox1.ForeColor = _placeholderColor;
                if (_isPasswordChar)
                    textBox1.UseSystemPasswordChar = false;

            }
        }

        private void RemovePlaceHolder()
        {
            if (_isPlaceholder && _placeholderText != String.Empty)
            {
                _isPlaceholder = false;
                textBox1.Text = string.Empty;
                textBox1.ForeColor = ForeColor;
                if (_isPasswordChar)
                    textBox1.UseSystemPasswordChar = true;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextHasChanged?.Invoke(sender, e);
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            _isFocused = true;
            Invalidate();
            RemovePlaceHolder();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            _isFocused = false;
            Invalidate();
            SetPlaceHolder();
        }
    }
}
