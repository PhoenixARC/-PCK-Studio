#region Imports


#endregion

using System.Drawing;
using System.Windows.Forms;

namespace CBH.Controls
{
    #region Label5

    public class CrEaTiiOn_ClasicLabel : System.Windows.Forms.Label
    {
        public CrEaTiiOn_ClasicLabel()
        {
            Font = new("Segoe UI", 9, FontStyle.Regular);
            BackColor = Color.Transparent;
            ForeColor = ColorTranslator.FromHtml("#FFFFFF");
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }
    }

    #endregion
}