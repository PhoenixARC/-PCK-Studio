#region Imports
using System;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CBH_Ultimate_Theme_Library.Theme.Helpers;
#endregion

namespace CBH_Ultimate_Theme_Library
{
    
    [ToolboxItem(false)]
	public class ASCThemeContainer : ContainerControl
	{
	
		private int moveHeight = 38;
		private bool formCanMove = false;
		private int mouseX;
		private int mouseY;
		private bool overExit;
	
		private bool overMin;
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				Invalidate();
			}
		}
	
		public ASCThemeContainer()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
			Dock = DockStyle.Fill;
			Font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic);
			BackColor = Color.FromArgb(15, 15, 15);
		}
	
		protected override void CreateHandle()
		{
			base.CreateHandle();
			Parent.FindForm().FormBorderStyle = FormBorderStyle.None;
			if (Parent.FindForm().TransparencyKey == null)
				Parent.FindForm().TransparencyKey = Color.Fuchsia;
		}
	
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
	
			Graphics G = e.Graphics;
			G.Clear(Parent.FindForm().TransparencyKey);
	
			int slope = 8;
	
			Rectangle mainRect = new Rectangle(0, 0, Width - 1, Height - 1);
			GraphicsPath mainPath = Drawing.RoundRect(mainRect, slope);
			G.FillPath(new SolidBrush(BackColor), mainPath);
			G.DrawPath(new Pen(Color.FromArgb(30, 35, 45)), mainPath);
			G.FillPath(new SolidBrush(Color.FromArgb(30, 30, 40)), Drawing.RoundRect(new Rectangle(0, 0, Width - 1, moveHeight - slope), slope));
			G.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 40)), new Rectangle(0, moveHeight - (slope * 2), Width - 1, slope * 2));
			G.DrawLine(new Pen(Color.FromArgb(60, 60, 60)), new Point(1, moveHeight), new Point(Width - 2, moveHeight));
			G.SmoothingMode = SmoothingMode.HighQuality;
	
			int textX = 6;
			int textY = (moveHeight / 2) - Convert.ToInt32((G.MeasureString(Text, Font).Height / 2)) + 1;
			Size textSize = new Size(Convert.ToInt32(G.MeasureString(Text, Font).Width), Convert.ToInt32(G.MeasureString(Text, Font).Height));
			Rectangle textRect = new Rectangle(textX, textY, textSize.Width, textSize.Height);
			LinearGradientBrush textBrush = new LinearGradientBrush(textRect, Color.FromArgb(185, 190, 195), Color.FromArgb(125, 125, 125), 90f);
			G.DrawString(Text, Font, textBrush, new Point(textX, textY));
	
			if (overExit) {
				G.DrawString("r", new Font("Marlett", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(25, 100, 140)), new Point(Width - 27, 11));
			} else {
				G.DrawString("r", new Font("Marlett", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(205, 210, 215)), new Point(Width - 27, 11));
			}
			if (overMin) {
				G.DrawString("0", new Font("Marlett", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(25, 100, 140)), new Point(Width - 47, 10));
			} else {
				G.DrawString("0", new Font("Marlett", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(205, 210, 215)), new Point(Width - 47, 10));
			}
	
			if (DesignMode)
				
				Prevent.Prevents(G, Width, Height);
	
		}
	
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
	
			if (formCanMove == true) {
				Parent.FindForm().Location = new Point(MousePosition.X - mouseX, MousePosition.Y - mouseY);
			}
	
			if (e.Y > 11 && e.Y < 24) {
				if (e.X > Width - 23 && e.X < Width - 10)
					overExit = true;
				else
					overExit = false;
				if (e.X > Width - 44 && e.X < Width - 31)
					overMin = true;
				else
					overMin = false;
			} else {
				overExit = false;
				overMin = false;
			}
	
			Invalidate();
	
		}
	
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
	
			mouseX = e.X;
			mouseY = e.Y;
	
			if (e.Y <= moveHeight && overExit == false && overMin == false)
				formCanMove = true;
	
			if (overExit) {
				Parent.FindForm().Close();
			} else if (overMin) {
				Parent.FindForm().WindowState = FormWindowState.Minimized;
				overExit = false;
				overMin = false;
			} else {
				Focus();
			}
	
			Invalidate();
	
		}
	
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);
			formCanMove = false;
		}
	
	}

}