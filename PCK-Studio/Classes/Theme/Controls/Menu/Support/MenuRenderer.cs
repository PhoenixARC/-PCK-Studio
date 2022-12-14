#region Imports
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
  public class MenuRenderer : ToolStripProfessionalRenderer
  {
    private Color primaryColor;
    private Color textColor;
    private int arrowThickness;

    public MenuRenderer(bool isMainMenu, Color primaryColor, Color textColor)
      : base((ProfessionalColorTable) new MenuColorTable(isMainMenu, primaryColor))
    {
      this.primaryColor = primaryColor;
      if (isMainMenu)
      {
        this.arrowThickness = 3;
        if (textColor == Color.Empty)
          this.textColor = Color.Gainsboro;
        else
          this.textColor = textColor;
      }
      else
      {
        this.arrowThickness = 2;
        this.textColor = !(textColor == Color.Empty) ? textColor : Color.DimGray;
      }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
      base.OnRenderItemText(e);
      e.Item.ForeColor = e.Item.Selected ? Color.Black : this.textColor;
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
      Graphics graphics = e.Graphics;
      Size size = new Size(5, 12);
      Color color = e.Item.Selected ? Color.Blue : this.primaryColor;
      Rectangle rectangle = new Rectangle(e.ArrowRectangle.Location.X, (e.ArrowRectangle.Height - size.Height) / 2, size.Width, size.Height);
      using (GraphicsPath path = new GraphicsPath())
      {
        using (Pen pen = new Pen(color, (float) this.arrowThickness))
        {
          graphics.SmoothingMode = SmoothingMode.AntiAlias;
          path.AddLine(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Top + rectangle.Height / 2);
          path.AddLine(rectangle.Right, rectangle.Top + rectangle.Height / 2, rectangle.Left, rectangle.Top + rectangle.Height);
          graphics.DrawPath(pen, path);
        }
      }
    }
  }
}
