#region Imports
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_FancyProgressBar : Control
    {
        int value;
        int maxValue;
        int minValue;
        int old, current;
        Color prColor;
        System.Windows.Forms.Timer timer;

        public CrEaTiiOn_FancyProgressBar()
        {
            prColor = Color.LightBlue;
            maxValue = 100;
            minValue = 0;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Width = 100;
            Height = 20;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += timer_Tick;
            timer.Enabled = true;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                int a = current - old;
                int k = maxValue / 100;
                int art = k < 1 ? 1 : k;
                old += Math.Abs(a) < 2 ? a : art * Math.Sign(a);
                this.value = old;
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int length = Width * value / maxValue;
            e.Graphics.DrawRectangle(Pens.Silver, 0, 0, Width - 1, Height - 1);
            using (SolidBrush brush = new SolidBrush(prColor))
                e.Graphics.FillRectangle(brush, 1, 1, length - 2, Height - 2);

            Size textSize = TextRenderer.MeasureText(value + "", this.Font);
            int x = (Width - textSize.Width) / 2;
            int y = (Height - textSize.Height) / 2;
            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
                e.Graphics.DrawString("%" + (value * 100 / maxValue), this.Font, textBrush, x, y);
            base.OnPaint(e);
        }
        public int Value
        {
            get
            {
                return current;
            }
            set
            {
                old = this.value;
                if (value > maxValue)
                    throw new Exception("The value is greater than the maximum value of progress");
                if (value < minValue)
                    throw new Exception("The value is lower than the maximum value of progress");
                current = value;
            }
        }
        public int MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                Invalidate();
            }
        }
        public int MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
                Invalidate();
            }
        }
        public Color ProgressColor
        {
            get
            {
                return prColor;
            }
            set
            {
                prColor = value;
                Invalidate();
            }
        }
    }
}
