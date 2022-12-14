#region Imports
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
  public class CrEaTiiOn_Ultimate_ToggleButton : CheckBox
  {
    private Color onBackColor = Color.FromArgb(250, 36, 38);
    private Color onToggleColor = Color.Black;
    private Color offBackColor = Color.Gray;
    private Color offToggleColor = Color.Gainsboro;
    private bool solidStyle = true;

    [Category("CrEaTiiOn")]
    public Color OnBackColor
    {
      get => this.onBackColor;
      set
      {
        this.onBackColor = value;
        this.Invalidate();
      }
    }

    [Category("CrEaTiiOn")]
    public Color OnToggleColor
    {
      get => this.onToggleColor;
      set
      {
        this.onToggleColor = value;
        this.Invalidate();
      }
    }

    [Category("CrEaTiiOn")]
    public Color OffBackColor
    {
      get => this.offBackColor;
      set
      {
        this.offBackColor = value;
        this.Invalidate();
      }
    }

    [Category("CrEaTiiOn")]
    public Color OffToggleColor
    {
      get => this.offToggleColor;
      set
      {
        this.offToggleColor = value;
        this.Invalidate();
      }
    }

    [Browsable(false)]
    public override string Text
    {
      get => base.Text;
      set
      {
      }
    }

    [Category("CrEaTiiOn")]
    [DefaultValue(true)]
    public bool SolidStyle
    {
      get => this.solidStyle;
      set
      {
        this.solidStyle = value;
        this.Invalidate();
      }
    }

    public CrEaTiiOn_Ultimate_ToggleButton() => this.MinimumSize = new Size(45, 22);

    private GraphicsPath GetFigurePath()
    {
      int num = this.Height - 1;
      Rectangle rect1 = new Rectangle(0, 0, num, num);
      Rectangle rect2 = new Rectangle(this.Width - num - 2, 0, num, num);
      GraphicsPath figurePath = new GraphicsPath();
      figurePath.StartFigure();
      figurePath.AddArc(rect1, 90f, 180f);
      figurePath.AddArc(rect2, 270f, 180f);
      figurePath.CloseFigure();
      return figurePath;
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
      int num = this.Height - 5;
      pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
      pevent.Graphics.Clear(this.Parent.BackColor);
      if (this.Checked)
      {
        if (this.solidStyle)
          pevent.Graphics.FillPath((Brush) new SolidBrush(this.onBackColor), this.GetFigurePath());
        else
          pevent.Graphics.DrawPath(new Pen(this.onBackColor, 2f), this.GetFigurePath());
        pevent.Graphics.FillEllipse((Brush) new SolidBrush(this.onToggleColor), new Rectangle(this.Width - this.Height + 1, 2, num, num));
      }
      else
      {
        if (this.solidStyle)
          pevent.Graphics.FillPath((Brush) new SolidBrush(this.offBackColor), this.GetFigurePath());
        else
          pevent.Graphics.DrawPath(new Pen(this.offBackColor, 2f), this.GetFigurePath());
        pevent.Graphics.FillEllipse((Brush) new SolidBrush(this.offToggleColor), new Rectangle(2, 2, num, num));
      }
    }
  }
}
