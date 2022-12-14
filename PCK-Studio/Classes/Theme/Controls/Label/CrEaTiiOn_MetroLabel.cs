using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace CBH.Controls
{
    public class CrEaTiiOn_MetroLabel : Control
    {
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        public CrEaTiiOn_MetroLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Font = new Font("Segoe UI", 10);
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            Text = Text;
        }
    }
}