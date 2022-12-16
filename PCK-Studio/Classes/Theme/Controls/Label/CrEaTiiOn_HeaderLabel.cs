#region Imports


#endregion

using System.Drawing;
namespace CBH.Controls
{
    #region CrEaTiiOn_HeaderLabel

    public class CrEaTiiOn_HeaderLabel : System.Windows.Forms.Label
    {
        public CrEaTiiOn_HeaderLabel()
        {
            Font = new("Segoe UI", 11, FontStyle.Bold);
            ForeColor = Color.FromArgb(255, 255, 255);
            BackColor = Color.Transparent;
        }
    }

    #endregion
}