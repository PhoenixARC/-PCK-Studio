#region Imports

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CBH.Controls
{
    #region CrEaTiiOn_AirTextBox

    public class CrEaTiiOn_AirTextBox : System.Windows.Forms.TextBox
    {
        private Color _BorderColor = Color.FromArgb(250, 36, 38);
        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                _BorderColor = value;
                Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 15:
                    Invalidate();
                    base.WndProc(ref m);
                    CustomPaint();
                    break; // TODO: might not be correct. Was : Exit Select
                default:
                    base.WndProc(ref m);
                    break; // TODO: might not be correct. Was : Exit Select
            }
        }

        public CrEaTiiOn_AirTextBox()
        {
            Font = new("Segoe UI", 8);
            ForeColor = Color.Gray;
            BackColor = Color.FromArgb(15, 15, 15);
            BorderStyle = BorderStyle.FixedSingle;
            Size = new(76, 20);
        }

        private void CustomPaint()
        {
            Pen p = new(new SolidBrush(BorderColor));
            CreateGraphics().DrawLine(p, 0, 0, Width, 0);
            CreateGraphics().DrawLine(p, 0, Height - 1, Width, Height - 1);
            CreateGraphics().DrawLine(p, 0, 0, 0, Height - 1);
            CreateGraphics().DrawLine(p, Width - 1, 0, Width - 1, Height - 1);
        }
    }

    #endregion
}