#region Imports
using CBH_Ultimate_Theme_Library.Theme.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_ProcessingControl : Control
    {
        #region Variables
        int n, circleIndex, radius, interval;
        bool spin;
        System.Windows.Forms.Timer timer;
        Color other, index;
        #endregion

        #region ProcessingControl
        public CrEaTiiOn_ProcessingControl()
        {
            Width = 85;
            Height = 85;
            n = 6;
            circleIndex = 0;
            interval = 100;
            radius = 10;
            spin = true;
            other = Color.LightGray;
            index = Color.Gray;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = interval;
            timer.Tick += timer_Tick;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            timer.Enabled = spin;
            BackColor = Color.Transparent;
        }
        #endregion

        #region Events


        protected override void OnPaint(PaintEventArgs e)
        {
            Transparenter.MakeTransparent(this, e.Graphics);
            #region Drawing
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            int length = Math.Min(Width, Height);
            PointF center = new PointF(length / 2, length / 2);
            int bigRadius = length / 2 - radius;
            float unitAngle = 360 / n;
            if (spin)
            {
                circleIndex++;
                circleIndex = circleIndex >= n ? circleIndex % n : circleIndex;
            }
            for (int i = 0; i < n; i++)
            {
                float c1X = center.X + (float)(bigRadius * Math.Cos(unitAngle * i * Math.PI / 180));
                float c1Y = center.Y + (float)(bigRadius * Math.Sin(unitAngle * i * Math.PI / 180));
                PointF loc1 = new PointF(c1X - radius, c1Y - radius);
                if (i == circleIndex)
                    using (SolidBrush brush = new SolidBrush(index))
                        e.Graphics.FillEllipse(brush, loc1.X, loc1.Y, 2 * radius, 2 * radius);
                else
                    using (SolidBrush brush = new SolidBrush(other))
                        e.Graphics.FillEllipse(brush, loc1.X, loc1.Y, 2 * radius, 2 * radius);
            }
            #endregion
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
            if (!spin) timer.Stop();
        }
        #endregion

        #region Properties
        public int NCircle
        {
            get
            {
                return n;
            }
            set
            {
                n = value > 1 ? value : 2;
                Invalidate();
            }
        }
        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value >= 1 ? value : 1;
                timer.Interval = interval;
            }
        }
        public bool Spin
        {
            get
            {
                return spin;
            }
            set
            {
                spin = value;
                timer.Enabled = spin;
            }
        }
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value >= 1 ? value : 1;
                Invalidate();

            }
        }
        public Color Others
        {
            get
            {
                return other;
            }
            set
            {
                other = value;
                if (!spin) Invalidate();
            }
        }
        public Color IndexColor
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                if (!spin) Invalidate();
            }
        }
        #endregion
    }
}
