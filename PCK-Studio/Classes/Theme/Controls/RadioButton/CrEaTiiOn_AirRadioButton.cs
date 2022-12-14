#region Imports

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace CBH.Controls
{
    #region CrEaTiiOn_AirRadioButton

    public class CrEaTiiOn_AirRadioButton : Control
    {
        public CrEaTiiOn_AirRadioButton()
        {
            base.Size = new Size(100, 16);
            Text = base.Name;
            ForeColor = Color.White;
            currentColor = radioColor;
            Cursor = Cursors.Hand;
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("Checked or unchecked")]
        public bool Checked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("Radio color")]
        public Color RadioColor
        {
            get => radioColor;
            set
            {
                radioColor = value;
                currentColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("Radio color when hovering")]
        public Color RadioHoverColor
        {
            get => radioHoverColor;
            set
            {
                radioHoverColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The radio style")]
        public Style RadioStyle
        {
            get => radioStyle;
            set
            {
                radioStyle = value;
                base.Invalidate();
            }
        }

        private SmoothingMode _SmoothingType = SmoothingMode.AntiAlias;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public SmoothingMode SmoothingType
        {
            get => _SmoothingType;
            set
            {
                _SmoothingType = value;
                Invalidate();
            }
        }

        private PixelOffsetMode _PixelOffsetType = PixelOffsetMode.HighQuality;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public PixelOffsetMode PixelOffsetType
        {
            get => _PixelOffsetType;
            set
            {
                _PixelOffsetType = value;
                Invalidate();
            }
        }

        private TextRenderingHint _TextRenderingType = TextRenderingHint.ClearTypeGridFit;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public TextRenderingHint TextRenderingType
        {
            get => _TextRenderingType;
            set
            {
                _TextRenderingType = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            currentColor = radioHoverColor;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            currentColor = radioColor;
            base.Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            foreach (object obj in base.Parent.Controls)
            {
                Control control = (Control)obj;
                if (control is System.Windows.Forms.RadioButton)
                {
                    ((CrEaTiiOn_Ultimate_RadioButton)control).Checked = false;
                }
                if (control is CrEaTiiOn_AirRadioButton button)
                {
                    button.Checked = false;
                }
            }
            isChecked = true;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingType;
            if (radioStyle == Style.Material)
            {
                e.Graphics.DrawEllipse(new Pen(currentColor, 2f), 2, 2, base.Height - 4, base.Height - 4);
                if (isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(currentColor), new Rectangle(5, 5, base.Height - 2 - 8, base.Height - 2 - 8), 0f, 360f);
                }
                e.Graphics.FillPie(new SolidBrush(currentColor), new Rectangle(1, 1, base.Height - 2, base.Height - 2), 0f, 360f);
                if (isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.White), new Rectangle(4, 4, base.Height - 2 - 6, base.Height - 2 - 6), 0f, 360f);
                }
            }
            if (radioStyle == Style.iOS)
            {
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(250, 36, 38), 2f), 2, 2, base.Height - 4, base.Height - 4);
                if (isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(250, 36, 38)), new Rectangle(5, 5, base.Height - 2 - 8, base.Height - 2 - 8), 0f, 360f);
                }
            }
            if (radioStyle == Style.Android)
            {
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(255, 0, 0), 2f), 2, 2, base.Height - 4, base.Height - 4);
                if (isChecked)
                {
                    e.Graphics.FillPie(new SolidBrush(Color.FromArgb(255, 0, 0)), new Rectangle(5, 5, base.Height - 2 - 8, base.Height - 2 - 8), 0f, 360f);
                }
            }
            StringFormat stringFormat = new()
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };
            SolidBrush brush = new(ForeColor);
            RectangleF layoutRectangle = new(base.Height + 3, 0f, base.Width - base.Height - 2, Height);
            e.Graphics.PixelOffsetMode = PixelOffsetType;
            e.Graphics.TextRenderingHint = TextRenderingType;
            e.Graphics.DrawString(Text, Font, brush, layoutRectangle, stringFormat);
            base.OnPaint(e);
        }

        private bool isChecked;

        private Color radioColor = Color.FromArgb(15, 15, 15);

        private Color radioHoverColor = Color.FromArgb(155, 31, 33);

        private Style radioStyle = Style.Material;

        private Color currentColor;

        public enum Style
        {
            iOS,
            Android,
            Material
        }
    }

    #endregion
}