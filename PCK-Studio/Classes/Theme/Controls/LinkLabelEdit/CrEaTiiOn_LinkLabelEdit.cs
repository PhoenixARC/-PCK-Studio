#region Imports


#endregion

using System.Drawing;

namespace CBH.Controls
{
    #region CrEaTiiOn_LinkLabelEdit

    public class CrEaTiiOn_LinkLabelEdit : System.Windows.Forms.LinkLabel
    {
        public CrEaTiiOn_LinkLabelEdit()
        {
            Font = new("Microsoft Sans Serif", 9, FontStyle.Regular);
            BackColor = Color.Transparent;
            LinkColor = Color.FromArgb(32, 34, 37);
            ActiveLinkColor = Color.FromArgb(153, 34, 34);
            VisitedLinkColor = Color.FromArgb(32, 34, 37);
        }
    }

    #endregion
}