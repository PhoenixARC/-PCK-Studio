using CBH_Ultimate_Theme_Library.Theme.Helpers;
using System.Drawing;
using System.Windows.Forms;

namespace CBH.Controls
{
    public class CrEaTiiOn_CheckBox : CheckBox
    {
        private Graphics G;
        public CrEaTiiOn_CheckBox()
        {
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 9);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);

            if (Enabled)
            {
                using (SolidBrush Back = new SolidBrush(Color.Transparent))
                {
                    G.FillPath(Back, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 1));
                }

                using (Pen Border = new Pen(Color.FromArgb(250, 36, 38)))
                {
                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 1));
                }

                using (SolidBrush TextBrush = new SolidBrush(Color.White))
                {
                    using (Font TextFont = new Font("Segoe UI", 9))
                    {
                        G.DrawString(Text, TextFont, TextBrush, new Point(22, 0));
                    }
                }

                if (Checked)
                {
                    using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(20, 20, 20)))
                    {
                        using (Font TextFont = new Font("Marlett", 12))
                        {
                            G.DrawString("b", TextFont, TextBrush, new Point(-2, 1));
                        }
                    }

                }

            }
            else
            {
                using (SolidBrush Back = new SolidBrush(Color.FromArgb(20, 20, 20)))
                {
                    G.FillPath(Back, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 1));
                }

                using (Pen Border = new Pen(Color.FromArgb(36, 36, 35)))
                {
                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 1));
                }

                using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(20, 20, 20)))
                {
                    using (Font TextFont = new Font("Segoe UI", 9))
                    {
                        G.DrawString(Text, TextFont, TextBrush, new Point(22, 0));
                    }
                }

                if (Checked)
                {
                    using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(20, 20, 20)))
                    {
                        using (Font TextFont = new Font("Marlett", 12))
                        {
                            G.DrawString("b", TextFont, TextBrush, new Point(-2, 1));
                        }
                    }
                }
            }
        }
    }
}
