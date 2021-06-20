using System;

namespace MinecraftUSkinEditor.Models
{
	internal struct Texel
	{
		internal Texel(global::MinecraftUSkinEditor.Models.TexturePlane texturePlane, int x, int y, global::System.Drawing.Color color)
		{
			this.TexturePlane = texturePlane;
			this.X = x;
			this.Y = y;
			this.color = color;
			this.brush = new global::System.Drawing.SolidBrush(color);
			this.pen = new global::System.Drawing.Pen(global::System.Drawing.Color.White, 0.01f);
		}

		internal double Z
		{
			get
			{
				return this.TexturePlane.ZOrder[this.X + 1, this.Y + 1];
			}
		}

		internal void Draw(global::System.Drawing.Graphics g)
		{
			global::System.Drawing.PointF[] points = new global::System.Drawing.PointF[]
			{
				this.TexturePlane.Points[this.X, this.Y],
				this.TexturePlane.Points[this.X + 1, this.Y],
				this.TexturePlane.Points[this.X + 1, this.Y + 1],
				this.TexturePlane.Points[this.X, this.Y + 1]
			};
			g.FillPolygon(this.brush, points);
		}

		internal global::MinecraftUSkinEditor.Models.TexturePlane TexturePlane;

		internal int X;

		internal int Y;

		private global::System.Drawing.Color color;

		private global::System.Drawing.Brush brush;

		private global::System.Drawing.Pen pen;
	}
}
