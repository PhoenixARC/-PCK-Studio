#region Imports

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace CBH.Controls
{
    #region CrEaTiiOn_TickIcon

    public class CrEaTiiOn_TickIcon : Control
    {
        #region Settings

        private Color _BaseColor = Color.FromArgb(246, 246, 246);
        public Color BaseColor
        {
            get => _BaseColor;
            set => _BaseColor = value;
        }

        private Color _CircleColor = Color.Gray;
        public Color CircleColor
        {
            get => _CircleColor;
            set => _CircleColor = value;
        }

        private string _String = "ü";
        public string String
        {
            get => _String;
            set => _String = value;
        }

        #endregion

        public CrEaTiiOn_TickIcon()
        {
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            ForeColor = Color.Gray;
            Font = new("Wingdings", 25, FontStyle.Bold);
            Size = new(33, 33);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            e.Graphics.FillEllipse(new SolidBrush(_CircleColor), new Rectangle(1, 1, 29, 29));
            e.Graphics.FillEllipse(new SolidBrush(_BaseColor), new Rectangle(3, 3, 25, 25));

            e.Graphics.DrawString(_String, Font, new SolidBrush(ForeColor), new Rectangle(0, -3, Width, 43), new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            });
        }
    }

    #endregion
}