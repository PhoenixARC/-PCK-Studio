#region Imports

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CBH.Controls
{
    #region CrEaTiiOn_Separator

    public class CrEaTiiOn_Separator : Control
    {
        #region Properties

        private Color _LineColor = Color.Gray;
        public Color LineColor
        {
            get => _LineColor;
            set
            {
                _LineColor = value;
                Invalidate();
            }
        }

        #endregion

        public CrEaTiiOn_Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            Size = new(120, 10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(new(LineColor), 0, 5, Width, 5);
        }
    }

    #endregion
}