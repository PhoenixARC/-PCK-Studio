#region Imports
using System;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    
    [DefaultEvent("CheckedChanged")]
    public class CrEaTiiOn_RoundCheckBox : Control
    {

        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        private bool _checked;
        public bool Checked {
            get { return _checked; }
            set {
                _checked = value;
                Invalidate();
            }
        }

        public CrEaTiiOn_RoundCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(120, 17);
            Font = new Font("Segoe UI", 9);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.Clear(Parent.BackColor);

            Height = 17;

            Rectangle boxRect = new Rectangle(1, 1, Height - 3, Height - 3);
            G.DrawEllipse(new Pen(Color.FromArgb(250, 36, 38), 2), boxRect);

            int textY = ((Height - 1) / 2) - Convert.ToInt32((G.MeasureString(Text, Font).Height / 2));
            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(160, 160, 160)), new Point((Height - 1) + 4, textY));

            if (_checked)
                G.DrawString("a", new Font("Marlett", 17), new SolidBrush(Color.FromArgb(120, 180, 255)), new Point(-3, -5));

        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_checked) {
                _checked = false;
            } else {
                _checked = true;
            }

            if (CheckedChanged != null) {
                CheckedChanged(this);
            }
            Invalidate();

        }

    }

}