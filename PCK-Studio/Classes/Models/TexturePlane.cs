using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace PckStudio.Models
{
	public class TexturePlane : Object3D
	{
		public override Image Image
		{
			set
			{
				Bitmap = (Bitmap)value;
			}
		}

		internal override MinecraftModelView Viewport
		{
			set
			{
				base.Viewport = value;
				if (bitmap != null && value != null)
				{
					UpdateBitmap();
				}
			}
		}

		internal override void Update()
		{
			if (Points == null || viewport == null)
			{
				return;
			}
			Matrix3D m = globalTransformation * localTransformation * originTranslation;
			for (int i = 0; i <= width; i++)
			{
				for (int j = 0; j <= height; j++)
				{
					Point3D point3D = m * new Point3D(i, j, 0f);
					Points[i, j] = viewport.Point3DTo2D(point3D);
					double num = (double)viewport.GetZOrder(point3D);
					ZOrder[i, j] += num;
					ZOrder[i + 1, j] += num;
					ZOrder[i, j + 1] += num;
					ZOrder[i + 1, j + 1] = num;
				}
			}
		}

		private Bitmap Bitmap
		{
			set
			{
				if (viewport == null)
				{
					bitmap = value;
					return;
				}
				texelList.Clear();
				if (bitmap != null)
				{
					viewport.RemoveTexelsOf(this);
					Points = null;
				}
				bitmap = value;
				if (bitmap != null)
				{
					UpdateBitmap();
					Update();
				}
			}
		}

		private void UpdateBitmap()
		{
			width = bitmap.Width;
			height = bitmap.Height;
			visibility = new bool[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Color pixel = bitmap.GetPixel(i, j);
					int num = flipHorizontally ? (width - i - 1) : i;
					int num2 = flipVertically ? j : (height - j - 1);
					if (pixel.A == 0)
					{
						visibility[num, num2] = false;
					}
					else
					{
						visibility[num, num2] = true;
						Texel texel = new Texel(this, num, num2, pixel);
						viewport.AddTexel(texel);
						texelList.Add(texel);
					}
				}
			}
			Points = new PointF[width + 1, height + 1];
			ZOrder = new double[width + 2, height + 2];
		}

		public TexturePlane(Image bitmap, Rectangle srcRect, Point3D origin, Point3D normal, Effects effects)
		{
			Origin = origin;
			this.normal = normal;
			if (bitmap == null)
			{
				Bitmap = null;
				return;
			}
			Bitmap bitmap2 = new Bitmap(srcRect.Width, srcRect.Height);
			using (Graphics graphics = Graphics.FromImage(bitmap2))
			{
				graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), srcRect, GraphicsUnit.Pixel);
			}
			flipHorizontally = (byte)(effects & Effects.FlipHorizontally) == 1;
			flipVertically = (byte)(effects & Effects.FlipVertically) == 2;
			Bitmap = bitmap2;
		}

		public override float HitTest(PointF location)
		{
			if (Points == null)
			{
				return -1000f;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddPolygon(new PointF[]
			{
				Points[0, 0],
				Points[Points.GetLength(0) - 1, 0],
				Points[Points.GetLength(0) - 1, Points.GetLength(1) - 1],
				Points[0, Points.GetLength(1) - 1]
			});
			Region region = new Region(graphicsPath);
			if (region.IsVisible(location))
			{
				for (int i = 0; i < Points.GetLength(0) - 1; i++)
				{
					for (int j = 0; j < Points.GetLength(1) - 1; j++)
					{
						if (visibility[i, j])
						{
							graphicsPath.Reset();
							graphicsPath.AddPolygon(new PointF[]
							{
								Points[i, j],
								Points[i + 1, j],
								Points[i + 1, j + 1],
								Points[i, j + 1]
							});
							if (graphicsPath.IsVisible(location))
							{
								return (globalTransformation * localTransformation * originTranslation * new Point3D(i, j, 0f)).Z;
							}
						}
					}
				}
			}
			return -1000f;
		}

		private List<Texel> texelList = new List<Texel>();

		internal PointF[,] Points;

		internal double[,] ZOrder;

		internal bool IsVisible = true;

		private bool[,] visibility;

		private Bitmap bitmap;

		private bool flipHorizontally;

		private bool flipVertically;

		private int width;

		private int height;

		private Point3D normal;
	}
}
