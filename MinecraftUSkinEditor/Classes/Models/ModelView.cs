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

namespace MinecraftUSkinEditor.Models
{
    public partial class MinecraftModelView : Control
    {


		private const int IsometricFOV = 0x2E;

		private string skinFilename;

		internal float cameraZ = 31.4285717f;

		private int fov = 0x46;

		private float scale = 1f;

		internal float RotationX;

		internal float RotationY;

		private bool perspective = true;

		private bool showUsername;

		private string username = string.Empty;

		private string skinUrlBase = "http://s3.amazonaws.com/MinecraftSkins/";

		private string skinFileExt = ".png";

		private float PiBy180 = 0.0174532924f;

		private string info = "Minecraft Skin Viewer " + System.Windows.Forms.Application.ProductVersion + " Beta Prerelease";


		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("Appearance")]
		public bool ShowUsername
		{
			get
			{
				return showUsername;
			}
			set
			{
				showUsername = value;
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		public event EventHandler SkinDowloadedx
		{
			add
			{
				System.EventHandler eventHandler = SkinDowloaded;
				System.EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
					eventHandler = System.Threading.Interlocked.CompareExchange<System.EventHandler>(ref SkinDowloaded, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				System.EventHandler eventHandler = SkinDowloaded;
				System.EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
					eventHandler = System.Threading.Interlocked.CompareExchange<System.EventHandler>(ref SkinDowloaded, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("Skin")]
		public string Username
		{
			get
			{
				return username;
			}
			set
			{
				if (value.Length == 0)
				{
					username = string.Empty;
					return;
				}
				downloader.RunWorkerCompleted -= OnDownloaderRunWorkerCompleted;
				if (downloader.IsBusy)
				{
					downloader.CancelAsync();
				}
				downloader = new System.ComponentModel.BackgroundWorker();
				InitializeDownloader();
				username = value.Trim();
				downloader.RunWorkerAsync();
			}
		}

		protected override void OnBackColorChanged(System.EventArgs e)
		{
			base.OnBackColorChanged(e);
			backgroundColor = BackColor;
			UpdateBackgroundBrush();
			base.Invalidate();
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("Appearance")]
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
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("Appearance")]
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
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Category("Appearance")]
		[System.Obsolete]
		public new System.Windows.Forms.ImageLayout BackgroundImageLayout
		{
			get
			{
				return System.Windows.Forms.ImageLayout.Tile;
			}
			set
			{
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("Appearance")]
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
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("Appearance")]
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
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("View")]
		public ProjectionTypes Projection
		{
			get
			{
				if (!perspective)
				{
					return ProjectionTypes.Isometric;
				}
				return ProjectionTypes.Perspective;
			}
			set
			{
				perspective = (value == ProjectionTypes.Perspective);
				SetupProjection();
				foreach (Object3D object3D in object3DList)
				{
					object3D.Update();
				}
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("View")]
		public int FOV
		{
			get
			{
				return fov;
			}
			set
			{
				fov = value;
				SetupProjection();
				foreach (Object3D object3D in object3DList)
				{
					object3D.Update();
				}
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("View")]
		public int DegreesX
		{
			get
			{
				return (int)RotationX;
			}
			set
			{
				RotationX = (float)value;
				Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * PiBy180) * Matrix3D.CreateRotationY(RotationY * PiBy180);
				foreach (Object3D object3D in object3DList)
				{
					object3D.GlobalTransformation = globalTransformation;
				}
				base.Invalidate();
			}
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("View")]
		public int DegreesY
		{
			get
			{
				return (int)RotationY;
			}
			set
			{
				RotationY = (float)value;
				Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * PiBy180) * Matrix3D.CreateRotationY(RotationY * PiBy180);
				foreach (Object3D object3D in object3DList)
				{
					object3D.GlobalTransformation = globalTransformation;
				}
				base.Invalidate();
			}
		}

		public Models.ModelBase Model
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
				base.Invalidate();
			}
		}


		private void InitializeDownloader()
		{
			downloader.WorkerSupportsCancellation = true;
			downloader.DoWork += OnDownloaderDoWork;
			downloader.RunWorkerCompleted += OnDownloaderRunWorkerCompleted;
		}

		protected virtual void OnSkinDownloaded(System.EventArgs args)
		{
			if (SkinDowloaded != null)
			{
				SkinDowloaded(this, args);
			}
		}

		private void OnDownloaderRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			RenderUsernameImage();
			OnSkinDownloaded(System.EventArgs.Empty);
		}

		private void OnDownloaderDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			System.Net.WebClient webClient = new System.Net.WebClient();
			try
			{
				byte[] buffer = webClient.DownloadData(skinUrlBase + username + skinFileExt);
				if (webSkin != null)
				{
					webSkin.Dispose();
				}
				webSkin = System.Drawing.Image.FromStream(new System.IO.MemoryStream(buffer));
			}
			catch
			{
				webSkin = null;
				username = string.Empty;
				usernameImage = null;
			}
		}

		private void Clear()
		{
			texelList.Clear();
			object3DList.Clear();
			dynamicObject3DtList.Clear();
			rotatingObject3D = null;
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
			base.OnPaintBackground(pevent);
			System.Drawing.Graphics graphics = pevent.Graphics;
			graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			graphics.FillRectangle(backgroundBrush, base.ClientRectangle);
			graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
			if (showUsername && usernameImage != null)
			{
				graphics.DrawImage(usernameImage, base.Width / 2 - usernameImage.Width / 2, base.Height - 0x20);
			}
			if (versionImage == null)
			{
				versionImage = RenderVersionText();
			}
			graphics.DrawImage(versionImage, 3, 3);
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
		{
			texelList.Sort(texelComparer);
			System.Drawing.Graphics graphics = pe.Graphics;
			graphics.TranslateTransform((float)(base.Width / 2), (float)(base.Height / 2));
			graphics.ScaleTransform(scale, -scale);
			graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			for (int i = 0; i < texelList.Count; i++)
			{
				texelList[i].Draw(graphics);
			}
		}

		public System.Drawing.Image RenderToImage(System.Drawing.Size size, System.Drawing.RectangleF crop)
		{
			if ((float)(size.Width / size.Height) != crop.Width / crop.Height)
			{
				throw new System.ArgumentException("Aspect ratio is ambiguous");
			}
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(size.Width, size.Height);
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
			{
				System.Drawing.Size size2 = new System.Drawing.Size((int)((float)size.Width / crop.Width), (int)((float)size.Height / crop.Height));
				texelList.Sort(texelComparer);
				graphics.TranslateTransform(-crop.Left * (float)size2.Width, -crop.Top * (float)size2.Height);
				graphics.TranslateTransform((float)(size2.Width / 2), (float)(size2.Height / 2));
				float num = (float)System.Math.Min(size2.Width, size2.Height) * 0.01f / (float)System.Math.Tan((double)(perspective ? fov : 0x2E) * 3.1415926535897931 / 360.0);
				graphics.ScaleTransform(num, -num);
				graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
				graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
				graphics.Clear(System.Drawing.Color.Transparent);
				for (int i = 0; i < texelList.Count; i++)
				{
					texelList[i].Draw(graphics);
				}
			}
			return bitmap;
		}

		private System.Drawing.Image RenderText(string text)
		{
			int num = 0;
			using (System.Drawing.Graphics graphics = base.CreateGraphics())
			{
				num = (int)graphics.MeasureString(text, Font).Width;
			}
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(num + 2, 0x12);
			using (System.Drawing.Graphics graphics2 = System.Drawing.Graphics.FromImage(bitmap))
			{
				using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(MinecraftModelView.textShadowColor))
				{
					graphics2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
					graphics2.DrawString(text, Font, brush, 2f, 2f);
					graphics2.DrawString(text, Font, System.Drawing.Brushes.White, 0f, 0f);
				}
			}
			return bitmap;
		}

		private System.Drawing.Image RenderVersionText()
		{
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(0x154, 0x12);
			System.Version version = new System.Version(System.Windows.Forms.Application.ProductVersion);
			string s = string.Concat(new object[]
			{
				System.Windows.Forms.Application.ProductName,
				" ",
				version.Major,
				".",
				version.Minor,
				(version.Build != 0) ? ("." + version.Build) : ""
			});
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
			{
				using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0x7F, System.Drawing.Color.Gray)))
				{
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
					graphics.DrawString(s, Font, brush, 1f, 1f);
					graphics.DrawString(s, Font, System.Drawing.Brushes.White, 0f, 0f);
				}
			}
			return bitmap;
		}

		private void RenderUsernameImage()
		{
			if (username != null && username.Length > 0)
			{
				usernameImage = RenderText(username);
			}
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
			return new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, System.Math.Max(1, size.Height)), backgroundGradientColor1, backgroundGradientColor2);
		}

		private void SetupProjection()
		{
			cameraZ = 2400f / (float)fov;
			scale = (float)System.Math.Min(base.Width, base.Height) * 0.01f / (float)System.Math.Tan((double)(perspective ? fov : 0x2E) * 3.1415926535897931 / 360.0);
		}

		protected override void OnResize(System.EventArgs e)
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

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			System.Drawing.PointF location = new System.Drawing.PointF(((float)e.X - (float)base.Width * 0.5f) / scale, -((float)e.Y - (float)base.Height * 0.5f) / scale);
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
			base.Invalidate();
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);
			rotatingObject3D = null;
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
			{
				return;
			}
			if (rotatingObject3D != null)
			{
				rotatingObject3D.RotateByMouse((float)(e.X - mouseLastLocation.X) * 400f / (float)base.Height, (float)(e.Y - mouseLastLocation.Y) * 400f / (float)base.Height);
				mouseLastLocation = e.Location;
				base.Invalidate();
				return;
			}
			RotationY += (float)(e.X - mouseLastLocation.X) * 400f / (float)base.Height;
			RotationX += (float)(e.Y - mouseLastLocation.Y) * 400f / (float)base.Height;
			mouseLastLocation = e.Location;
			Matrix3D globalTransformation = Matrix3D.CreateRotationX(RotationX * 3.14159274f / 180f) * Matrix3D.CreateRotationY(RotationY * 3.14159274f / 180f);
			foreach (Object3D object3D in object3DList)
			{
				object3D.GlobalTransformation = globalTransformation;
			}
			base.Invalidate();
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
			object3D.Viewport = this;
			object3DList.Add(object3D);
			dynamicObject3DtList.Add(object3D);
			foreach (Object3D object3D2 in object3DList)
			{
				object3D2.Update();
			}
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

		private unsafe System.Drawing.Image CorrectSkin(System.Drawing.Image skin)
		{
			if (skin == null)
			{
				return null;
			}
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(skin);
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
			System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			for (int i = 0; i < 0x10; i++)
			{
				byte* ptr = (byte*)((void*)bitmapData.Scan0) + (int)i * (int)bitmapData.Stride;
				for (int j = 3; j < 0x80; j += 4)
				{
					ptr[j] = byte.MaxValue;
				}
			}
			for (int k = 0x10; k < 0x20; k++)
			{
				byte* ptr2 = (byte*)((void*)bitmapData.Scan0) + (int)k * (int)bitmapData.Stride;
				for (int l = 3; l < 0x100; l += 4)
				{
					ptr2[l] = byte.MaxValue;
				}
			}
			bool flag = false;
			for (int m = 0; m < 8; m++)
			{
				byte* ptr3 = (byte*)((void*)bitmapData.Scan0) + (int)m * (int)bitmapData.Stride;
				for (int n = 0xA3; n < 0xE0; n += 4)
				{
					if (ptr3[n] != 0xFF)
					{
						flag = true;
					}
				}
			}
			for (int num = 8; num < 0x10; num++)
			{
				byte* ptr4 = (byte*)((void*)bitmapData.Scan0) + (int)num * (int)bitmapData.Stride;
				for (int num2 = 0x83; num2 < 0x100; num2 += 4)
				{
					if (ptr4[num2] != 0xFF)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				for (int num3 = 0; num3 < 8; num3++)
				{
					byte* ptr5 = (byte*)((void*)bitmapData.Scan0) + (int)num3 * (int)bitmapData.Stride;
					for (int num4 = 0xA3; num4 < 0xE0; num4++)
					{
						ptr5[num4] = 0;
					}
				}
				for (int num5 = 8; num5 < 0x10; num5++)
				{
					byte* ptr6 = (byte*)((void*)bitmapData.Scan0) + (int)num5 * (int)bitmapData.Stride;
					for (int num6 = 0x83; num6 < 0x100; num6++)
					{
						ptr6[num6] = 0;
					}
				}
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		internal void UpdateImage()
		{
			SetSkinFromFile(skinFilename);
		}

		internal bool SetSkinFromFile(string filename)
		{
			skinFilename = filename;
			bool flag = true;
			using (System.IO.FileStream fileStream = System.IO.File.OpenRead(filename))
			{
				System.Drawing.Image image = System.Drawing.Image.FromStream(fileStream);
				if (image.Width != 0x40 || image.Height != 0x20)
				{
					System.Windows.Forms.MessageBox.Show("Image '" + System.IO.Path.GetFileName(filename) + "' is not Minecraft skin.", "Minecraft Skin Viewer", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
					flag = false;
				}
			}
			if (flag)
			{
				usernameImage = null;
				username = string.Empty;
			}
			return flag;
		}

		public MinecraftModelView()
		{
			base.SetStyle(System.Windows.Forms.ControlStyles.ResizeRedraw | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint | System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			texelComparer = new TexelComparer();
		}

        public MinecraftModelView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
