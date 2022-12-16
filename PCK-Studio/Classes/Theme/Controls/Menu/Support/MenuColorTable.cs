#region Imports
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
  public class MenuColorTable : ProfessionalColorTable
  {
    private Color backColor;
    private Color leftColumnColor;
    private Color borderColor;
    private Color menuItemBorderColor;
    private Color menuItemSelectedColor;

    public MenuColorTable(bool isMainMenu, Color primaryColor)
    {
      if (isMainMenu)
      {
        this.backColor = Color.FromArgb(37, 39, 60);
        this.leftColumnColor = Color.FromArgb(32, 33, 51);
        this.borderColor = Color.FromArgb(32, 33, 51);
        this.menuItemBorderColor = primaryColor;
        this.menuItemSelectedColor = primaryColor;
      }
      else
      {
        this.backColor = Color.White;
        this.leftColumnColor = Color.LightGray;
        this.borderColor = Color.LightGray;
        this.menuItemBorderColor = primaryColor;
        this.menuItemSelectedColor = primaryColor;
      }
    }

    public override Color ToolStripDropDownBackground => this.backColor;

    public override Color MenuBorder => this.borderColor;

    public override Color MenuItemBorder => this.menuItemBorderColor;

    public override Color MenuItemSelected => this.menuItemSelectedColor;

    public override Color ImageMarginGradientBegin => this.leftColumnColor;

    public override Color ImageMarginGradientMiddle => this.leftColumnColor;

    public override Color ImageMarginGradientEnd => this.leftColumnColor;
  }
}
