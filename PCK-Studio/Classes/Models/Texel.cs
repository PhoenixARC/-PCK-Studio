using System;

namespace PckStudio.Models
{
	internal struct Texel
	{
		internal Texel(TexturePlane texturePlane, int x, int y, System.Drawing.Color color)
		{
			TexturePlane = texturePlane;
			X = x;
			Y = y;
			this.color = color;
			brush = new System.Drawing.SolidBrush(color);
			pen = new System.Drawing.Pen(System.Drawing.Color.White, 0.01f);
		}

		internal double Z
		{
			get
			{
				return TexturePlane.ZOrder[X + 1, Y + 1];
			}
		}

		internal void Draw(System.Drawing.Graphics g)
		{
			System.Drawing.PointF[] points = new System.Drawing.PointF[]
			{
				TexturePlane.Points[X, Y],
				TexturePlane.Points[X + 1, Y],
				TexturePlane.Points[X + 1, Y + 1],
				TexturePlane.Points[X, Y + 1]
			};
			g.FillPolygon(brush, points);
		}

		internal TexturePlane TexturePlane;

		internal int X;

		internal int Y;

		private System.Drawing.Color color;

		private System.Drawing.Brush brush;

		private System.Drawing.Pen pen;
	}
}
