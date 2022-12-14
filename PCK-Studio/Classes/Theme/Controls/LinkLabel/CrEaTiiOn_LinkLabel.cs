#region Imports


#endregion

using System.Drawing;

namespace CBH.Controls
{
    #region CrEaTiiOn_LinkLabel

    public class CrEaTiiOn_LinkLabel : System.Windows.Forms.LinkLabel
    {
        public CrEaTiiOn_LinkLabel()
        {
            Font = new("Segoe UI", 11, FontStyle.Regular);
            BackColor = Color.Transparent;
            LinkColor = Color.FromArgb(240, 119, 70);
            ActiveLinkColor = Color.FromArgb(221, 72, 20);
            VisitedLinkColor = Color.FromArgb(240, 119, 70);
        }
    }

    #endregion
}