using System;
using System.Drawing;

namespace PckStudio.Models
{
	public class Box : Object3D
	{
		public override Image Image
		{
			set
			{
				SetImage(value);
			}
		}

		internal override MinecraftModelView Viewport
		{
			set
			{
				base.Viewport = value;
				top.Viewport = value;
				bottom.Viewport = value;
				front.Viewport = value;
				back.Viewport = value;
				left.Viewport = value;
				right.Viewport = value;
				Update();
			}
		}

		internal override void Update()
		{
			Matrix3D a = globalTransformation * localTransformation;
			top.LocalTransformation = a * topLocalTransformation;
			bottom.LocalTransformation = a * bottomLocalTransformation;
			front.LocalTransformation = a * frontLocalTransformation;
			back.LocalTransformation = a * backLocalTransformation;
			left.LocalTransformation = a * leftLocalTransformation;
			right.LocalTransformation = a * rightLocalTransformation;
		}

		public Box(Image image, Rectangle srcTopBottom, Rectangle srcSides, Point3D origin, Effects effects)
		{
			this.effects = effects;
			Origin = origin;
			SetImage(image, srcTopBottom, srcSides);
		}

		private void SetImage(Image image, Rectangle srcTopBottom, Rectangle srcSides)
		{
			int num = srcTopBottom.Width / 2;
			int height = srcSides.Height;
			int height2 = srcTopBottom.Height;
			srcTop = new Rectangle(srcTopBottom.Location, new Size(num, height2));
			srcBottom = new Rectangle(srcTopBottom.X + num, srcTopBottom.Y, num, height2);
			srcFront = new Rectangle(srcSides.X + height2, srcSides.Y, num, height);
			srcBack = new Rectangle(srcSides.X + height2 + num + height2, srcSides.Y, num, height);
			srcLeft = new Rectangle(srcSides.Location, new Size(height2, height));
			srcRight = new Rectangle(srcSides.X + height2 + num, srcSides.Y, height2, height);
			SetImage(image);
		}

		private void SetImage(Image image)
		{
			bool flag = (byte)(effects & Effects.FlipHorizontally) == 1;
			bool flag2 = (byte)(effects & Effects.FlipVertically) == 2;
			int width = srcFront.Width;
			int height = srcFront.Height;
			int width2 = srcLeft.Width;
			top = new TexturePlane(image, flag2 ? srcBottom : srcTop, new Point3D(width * 0.5f, width2 * 0.5f, (float)(-(float)height) * 0.5f), new Point3D(0f, 1f, 0f), effects & Effects.FlipHorizontally);
			bottom = new TexturePlane(image, flag2 ? srcTop : srcBottom, new Point3D(width / 2f, width2 / 2f, height / 2f), new Point3D(0f, -1f, 0f), effects & Effects.FlipHorizontally);
			front = new TexturePlane(image, srcFront, new Point3D(width * 0.5f, height * 0.5f, (float)(-(float)width2) * 0.5f), new Point3D(0f, 0f, 1f), effects);
			back = new TexturePlane(image, srcBack, new Point3D(width * 0.5f, height * 0.5f, (float)(-(float)width2) * 0.5f), new Point3D(0f, 0f, -1f), effects);
			left = new TexturePlane(image, flag ? srcRight : srcLeft, new Point3D(width2 * 0.5f, height * 0.5f, (float)(-(float)width) * 0.5f), new Point3D(-1f, 0f, 0f), effects);
			right = new TexturePlane(image, flag ? srcLeft : srcRight, new Point3D(width2 * 0.5f, height * 0.5f, (float)(-(float)width) * 0.5f), new Point3D(1f, 0f, 0f), effects);
			top.Viewport = viewport;
			bottom.Viewport = viewport;
			front.Viewport = viewport;
			back.Viewport = viewport;
			left.Viewport = viewport;
			right.Viewport = viewport;
		}

		public override float HitTest(PointF location)
		{
			float num = -1000f;
			float num2 = top.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = bottom.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = front.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = back.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = left.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = right.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			return num;
		}

		private TexturePlane top;

		private TexturePlane bottom;

		private TexturePlane front;

		private TexturePlane back;

		private TexturePlane left;

		private TexturePlane right;

		private Rectangle srcTop;

		private Rectangle srcBottom;

		private Rectangle srcFront;

		private Rectangle srcBack;

		private Rectangle srcLeft;

		private Rectangle srcRight;

		private Matrix3D topLocalTransformation = Matrix3D.CreateRotationX(-1.57079637f);

		private Matrix3D bottomLocalTransformation = Matrix3D.CreateRotationX(-1.57079637f);

		private Matrix3D frontLocalTransformation = Matrix3D.Identity;

		private Matrix3D backLocalTransformation = Matrix3D.CreateRotationY(3.14159274f);

		private Matrix3D leftLocalTransformation = Matrix3D.CreateRotationY(-1.57079637f);

		private Matrix3D rightLocalTransformation = Matrix3D.CreateRotationY(1.57079637f);

		private Effects effects;
	}
}
