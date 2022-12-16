#region Imports

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace CBH.Controls
{
    #region CrEaTiiOn_FlatProgressBar

    public class CrEaTiiOn_FlatProgressBar : Control
    {
        public CrEaTiiOn_FlatProgressBar()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            base.Size = new Size(300, 5);
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress bar style")]
        public Style BarStyle
        {
            get => barStyle;
            set
            {
                barStyle = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress value")]
        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                if (this.value < 0)
                {
                    this.value = 0;
                }
                if (this.value > maxValue)
                {
                    this.value = maxValue;
                }
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress complete color")]
        public Color CompleteColor
        {
            get => completeColor;
            set
            {
                completeColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress complete ios back color")]
        public Color CompleteBackColor
        {
            get => completeBackColor;
            set
            {
                completeBackColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress bar border color")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("Show the progress bar border")]
        public bool ShowBorder
        {
            get => showBorder;
            set
            {
                showBorder = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress incompleted color")]
        public Color InocmpletedColor
        {
            get => incompletedColor;
            set
            {
                incompletedColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The progress incompleted ios back color")]
        public Color IncompletedBackColor
        {
            get => incompletedBackColor;
            set
            {
                incompletedBackColor = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The maximum value")]
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                if (Value > maxValue)
                {
                    Value = maxValue;
                }
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The positions")]
        public List<float> Positions
        {
            get => _Positions;
            set
            {
                _Positions = value;
                base.Invalidate();
            }
        }

        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [Description("The colors")]
        public List<Color> Colors
        {
            get => _Colors;
            set
            {
                _Colors = value;
                base.Invalidate();
            }
        }

        private SmoothingMode _SmoothingType = SmoothingMode.HighQuality;
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

        private CompositingQuality _CompositingQualityType = CompositingQuality.HighQuality;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public CompositingQuality CompositingQualityType
        {
            get => _CompositingQualityType;
            set
            {
                _CompositingQualityType = value;
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

        private InterpolationMode _InterpolationType = InterpolationMode.HighQualityBilinear;
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        public InterpolationMode InterpolationType
        {
            get => _InterpolationType;
            set
            {
                _InterpolationType = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            BufferedGraphicsContext bufferedGraphicsContext = BufferedGraphicsManager.Current;
            bufferedGraphicsContext.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            bufferedGraphics = bufferedGraphicsContext.Allocate(base.CreateGraphics(), base.ClientRectangle);

            bufferedGraphics.Graphics.SmoothingMode = SmoothingType;
            bufferedGraphics.Graphics.InterpolationMode = InterpolationType;
            bufferedGraphics.Graphics.CompositingQuality = CompositingQualityType;
            bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetType;
            bufferedGraphics.Graphics.TextRenderingHint = TextRenderingType;

            bufferedGraphics.Graphics.Clear(BackColor);

            if (barStyle == Style.Flat)
            {
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(incompletedColor), 0, 0, base.Width, base.Height);
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(completeColor), 0, 0, value * base.Width / maxValue, base.Height);
            }

            if (barStyle == Style.IOS)
            {
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(incompletedBackColor), 0, 0, base.Width, base.Height);
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(completeBackColor), 0, 0, value * base.Width / maxValue, base.Height);
            }

            if (barStyle == Style.Material && Positions.Count == Colors.Count)
            {
                LinearGradientBrush linearGradientBrush = new(new Rectangle(0, 0, base.Width, base.Height), Color.Black, Color.Black, 0f, false)
                {
                    InterpolationColors = new ColorBlend
                    {
                        Positions = Positions.ToArray(),
                        Colors = Colors.ToArray()
                    }
                };

                linearGradientBrush.RotateTransform(1f);
                bufferedGraphics.Graphics.FillRectangle(linearGradientBrush, new Rectangle(0, 0, base.Width, base.Height));
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(incompletedColor), value * base.Width / maxValue, 0, base.Width - value * base.Width / maxValue, base.Height);
            }

            if (ShowBorder)
            {
                bufferedGraphics.Graphics.DrawRectangle(new Pen(BorderColor, 1f), new Rectangle(1, 1, base.Width - 2, base.Height - 2));
            }

            bufferedGraphics.Render(e.Graphics);
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.Invalidate();
        }

        private BufferedGraphics bufferedGraphics;

        private Style barStyle = Style.Material;

        private int value = 50;

        private Color completeColor = Color.FromArgb(250, 36, 38);

        private Color completeBackColor = Color.FromArgb(15, 15, 15);

        private Color borderColor = Color.FromArgb(20, 20, 20);

        private bool showBorder = true;

        private Color incompletedColor = Color.DarkGray;

        private Color incompletedBackColor = Color.FromArgb(100, 100, 100);

        private int maxValue = 100;

        private List<float> _Positions = new()
        {
            0f,
            0.2f,
            0.4f,
            0.6f,
            0.8f,
            1f
        };

        private List<Color> _Colors = new()
        {
            Color.FromArgb(76, 217, 100),
            Color.FromArgb(85, 205, 205),
            Color.FromArgb(2, 124, 255),
            Color.FromArgb(130, 75, 180),
            Color.FromArgb(255, 0, 150),
            Color.FromArgb(255, 45, 85)
        };

        public enum Style
        {
            Flat,
            Material,
            IOS
        }
    }

    #endregion
}