using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace PckStudio.Models
{
    public partial class MinecraftModelView : Control
    {
		private const int IsometricFOV = 46;

		internal float cameraZ = 31.4285717f;

		private int Fov = 70;

		private float scale = 1f;

		internal float RotationX;

		internal float RotationY;

		private bool perspective = true;

		private float PiBy180 = 0.0174532924f;

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			backgroundColor = BackColor;
			UpdateBackgroundBrush();
			Invalidate();
		}

		[Browsable(true)]
		[Category("Appearance")]
		public System.Drawing.Color BackGradientColor1
		{
			get
			{
				return backgroundGradientColor1;
			}
			set
			{
				backgroundGradientColor1 = value;
				UpdateBackgroundBrush();
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("Appearance")]
		public System.Drawing.Color BackGradientColor2
		{
			get
			{
				return backgroundGradientColor2;
			}
			set
			{
				backgroundGradientColor2 = value;
				UpdateBackgroundBrush();
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("Appearance")]
		public new System.Drawing.Image BackgroundImage
		{
			get
			{
				return backgroundTexture;
			}
			set
			{
				backgroundTexture = value;
				UpdateBackgroundBrush();
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("Appearance")]
		public BackgroundTypes BackgroundType
		{
			get
			{
				return backgroundType;
			}
			set
			{
				backgroundType = value;
				UpdateBackgroundBrush();
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("View")]
		public ProjectionTypes Projection
		{
			get
			{
				return perspective ? ProjectionTypes.Perspective : ProjectionTypes.Isometric;
			}
			set
			{
				perspective = value == ProjectionTypes.Perspective;
				SetupProjection();
				foreach (Object3D object3D in object3DList)
				{
					object3D.Update();
				}
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("View")]
		public int FOV
		{
			get
			{
				return Fov;
			}
			set
			{
				Fov = value;
				SetupProjection();
				foreach (Object3D object3D in object3DList)
				{
					object3D.Update();
				}
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("View")]
		public int DegreesX
		{
			get
			{
				return (int)RotationX;
			}
			set
			{
				RotationX = value;
				Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * PiBy180) * Matrix3D.CreateRotationY(RotationY * PiBy180);
				foreach (Object3D object3D in object3DList)
				{
					object3D.GlobalTransformation = globalTransformation;
				}
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("View")]
		public int DegreesY
		{
			get
			{
				return (int)RotationY;
			}
			set
			{
				RotationY = value;
				Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * PiBy180) * Matrix3D.CreateRotationY(RotationY * PiBy180);
				foreach (Object3D object3D in object3DList)
				{
					object3D.GlobalTransformation = globalTransformation;
				}
				Invalidate();
			}
		}

		public ModelBase Model
		{
			set
			{
				Clear();
				value.AddToModelView(this);
				Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * 3.14159274f / 180f) * Matrix3D.CreateRotationY(RotationY * 3.14159274f / 180f);
				foreach (Object3D object3D in object3DList)
				{
					object3D.GlobalTransformation = globalTransformation;
				}
				Invalidate();
			}
		}

		protected virtual void OnSkinDownloaded(EventArgs args)
		{
			if (SkinDowloaded != null)
			{
				SkinDowloaded(this, args);
			}
		}

		private void Clear()
		{
			texelList.Clear();
			object3DList.Clear();
			dynamicObject3DtList.Clear();
			rotatingObject3D = null;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			base.OnPaintBackground(pevent);
			System.Drawing.Graphics graphics = pevent.Graphics;
			graphics.CompositingQuality = CompositingQuality.HighSpeed;
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
			graphics.SmoothingMode = SmoothingMode.HighSpeed;
			graphics.CompositingMode = CompositingMode.SourceCopy;
			graphics.FillRectangle(backgroundBrush, ClientRectangle);
			graphics.CompositingMode = CompositingMode.SourceOver;
			if (versionImage == null)
			{
				versionImage = RenderVersionText();
			}
			graphics.DrawImage(versionImage, 3, 3);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			texelList.Sort(texelComparer);
			System.Drawing.Graphics graphics = pe.Graphics;
			graphics.TranslateTransform(Width / 2, Height / 2);
			graphics.ScaleTransform(scale, -scale);
			graphics.CompositingQuality = CompositingQuality.HighSpeed;
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
			graphics.SmoothingMode = SmoothingMode.HighSpeed;
			graphics.CompositingMode = CompositingMode.SourceCopy;
			for (int i = 0; i < texelList.Count; i++)
			{
				texelList[i].Draw(graphics);
			}
		}

		public System.Drawing.Image RenderToImage(System.Drawing.Size size, System.Drawing.RectangleF crop)
		{
			if (size.Width / size.Height != crop.Width / crop.Height)
			{
				throw new ArgumentException("Aspect ratio is ambiguous");
			}
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(size.Width, size.Height);
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
			{
				System.Drawing.Size size2 = new System.Drawing.Size((int)(size.Width / crop.Width), (int)(size.Height / crop.Height));
				texelList.Sort(texelComparer);
				graphics.TranslateTransform(-crop.Left * size2.Width, -crop.Top * size2.Height);
				graphics.TranslateTransform(size2.Width / 2, size2.Height / 2);
				float num = Math.Min(size2.Width, size2.Height) * 0.01f / (float)Math.Tan((perspective ? Fov : IsometricFOV) * 3.1415926535897931 / 360.0);
				graphics.ScaleTransform(num, -num);
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
				graphics.SmoothingMode = SmoothingMode.HighSpeed;
				graphics.Clear(System.Drawing.Color.Transparent);
				for (int i = 0; i < texelList.Count; i++)
				{
					texelList[i].Draw(graphics);
				}
			}
			return bitmap;
		}

		private System.Drawing.Image RenderVersionText()
		{
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(0x154, 0x12);
			Version version = new Version(Application.ProductVersion);
			string s = string.Format("{0} {1}.{2}{3}", Application.ProductName, version.Major, version.Minor, (version.Build != 0) ? ("." + version.Build) : "");
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
			{
				using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0x7F, System.Drawing.Color.Gray)))
				{
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
					graphics.DrawString(s, Font, brush, 1f, 1f);
					graphics.DrawString(s, Font, System.Drawing.Brushes.White, 0f, 0f);
				}
			}
			return bitmap;
		}

		private void UpdateBackgroundBrush()
		{
			backgroundBrush = ((backgroundType == BackgroundTypes.Texture) ? new System.Drawing.TextureBrush(backgroundTexture) : ((backgroundType == BackgroundTypes.Gradient) ? new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, System.Math.Max(1, base.Height)), backgroundGradientColor1, backgroundGradientColor2) : new System.Drawing.SolidBrush(backgroundColor)));
		}

		public System.Drawing.Brush GetBackgroundBrush(System.Drawing.Size size)
		{
			if (backgroundType == BackgroundTypes.Texture)
			{
				return new System.Drawing.TextureBrush(backgroundTexture);
			}
			if (backgroundType != BackgroundTypes.Gradient)
			{
				return new System.Drawing.SolidBrush(backgroundColor);
			}
			return new LinearGradientBrush(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, Math.Max(1, size.Height)), backgroundGradientColor1, backgroundGradientColor2);
		}

		private void SetupProjection()
		{
			cameraZ = 2400f / Fov;
			scale = Math.Min(Width, Height) * 0.01f / (float)Math.Tan((perspective ? Fov : IsometricFOV) * Math.PI / 360.0);
		}

		protected override void OnResize(EventArgs e)
		{
			SetupProjection();
			UpdateBackgroundBrush();
			base.OnResize(e);
		}

		internal void RemoveTexelsOf(TexturePlane texturePlane)
		{
			for (int i = 0; i < texelList.Count; i++)
			{
				if (texelList[i].TexturePlane == texturePlane)
				{
					texelList.RemoveAt(i);
					i--;
				}
			}
		}

		internal void AddTexel(Texel texel)
		{
			texelList.Add(texel);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			System.Drawing.PointF location = new System.Drawing.PointF((e.X - Width * 0.5f) / scale, -(e.Y - Height * 0.5f) / scale);
			rotatingObject3D = null;
			Object3D item = null;
			float num = -1000f;
			foreach (Object3D object3D in object3DList)
			{
				float num2 = object3D.HitTest(location);
				if (num2 > num)
				{
					num = num2;
					item = object3D;
				}
			}
			if (num > -1000f && dynamicObject3DtList.Contains(item))
			{
				rotatingObject3D = item;
			}
			mouseLastLocation = e.Location;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			rotatingObject3D = null;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			if (rotatingObject3D != null)
			{
				rotatingObject3D.RotateByMouse((e.X - mouseLastLocation.X) * 400f / Height, (e.Y - mouseLastLocation.Y) * 400f / Height);
				mouseLastLocation = e.Location;
				Invalidate();
				return;
			}
			RotationY += (e.X - mouseLastLocation.X) * 400f / Height;
			RotationX += (e.Y - mouseLastLocation.Y) * 400f / Height;
			mouseLastLocation = e.Location;
			Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * 3.14159274f / 180f) * Matrix3D.CreateRotationY(RotationY * 3.14159274f / 180f);
			foreach (Object3D object3D in object3DList)
			{
				object3D.GlobalTransformation = globalTransformation;
			}
			Invalidate();
		}

		public void AddStatic(Object3D object3D)
		{
			object3D.Viewport = this;
			object3DList.Add(object3D);
			foreach (Object3D object3D2 in object3DList)
			{
				object3D2.Update();
			}
		}

		public void AddDynamic(Object3D object3D)
		{
			AddStatic(object3D);
			dynamicObject3DtList.Add(object3D);
		}

		internal System.Drawing.PointF Point3DTo2D(Point3D point3D)
		{
			if (perspective)
			{
				return new System.Drawing.PointF(point3D.X * (-50f / (point3D.Z - cameraZ)), point3D.Y * (-50f / (point3D.Z - cameraZ)));
			}
			return new System.Drawing.PointF(point3D.X, point3D.Y);
		}

		internal float GetZOrder(Point3D point3D)
		{
			if (perspective)
			{
				return point3D.X * point3D.X + point3D.Y * point3D.Y + (cameraZ - point3D.Z) * (cameraZ - point3D.Z);
			}
			return -point3D.Z;
		}

        public MinecraftModelView()
		{
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			texelComparer = new TexelComparer();
		}

        public MinecraftModelView(IContainer container) : this()
        {
            container.Add(this);
        }
    }
}
