using System;

namespace MinecraftUSkinEditor.Models
{
	public class Box : global::MinecraftUSkinEditor.Models.Object3D
	{
		public override global::System.Drawing.Image Image
		{
			set
			{
				this.SetImage(value);
			}
		}

		internal override global::MinecraftUSkinEditor.Models.MinecraftModelView Viewport
		{
			set
			{
				base.Viewport = value;
				this.top.Viewport = value;
				this.bottom.Viewport = value;
				this.front.Viewport = value;
				this.back.Viewport = value;
				this.left.Viewport = value;
				this.right.Viewport = value;
				this.Update();
			}
		}

		internal override void Update()
		{
			global::MinecraftUSkinEditor.Models.Matrix3D a = this.globalTransformation * this.localTransformation;
			this.top.LocalTransformation = a * this.topLocalTransformation;
			this.bottom.LocalTransformation = a * this.bottomLocalTransformation;
			this.front.LocalTransformation = a * this.frontLocalTransformation;
			this.back.LocalTransformation = a * this.backLocalTransformation;
			this.left.LocalTransformation = a * this.leftLocalTransformation;
			this.right.LocalTransformation = a * this.rightLocalTransformation;
		}

		public Box(global::System.Drawing.Image image, global::System.Drawing.Rectangle srcTopBottom, global::System.Drawing.Rectangle srcSides, global::MinecraftUSkinEditor.Models.Point3D origin, global::MinecraftUSkinEditor.Models.Effects effects)
		{
			this.effects = effects;
			base.Origin = origin;
			this.SetImage(image, srcTopBottom, srcSides);
		}

		private void SetImage(global::System.Drawing.Image image, global::System.Drawing.Rectangle srcTopBottom, global::System.Drawing.Rectangle srcSides)
		{
			int num = srcTopBottom.Width / 2;
			int height = srcSides.Height;
			int height2 = srcTopBottom.Height;
			this.srcTop = new global::System.Drawing.Rectangle(srcTopBottom.Location, new global::System.Drawing.Size(num, height2));
			this.srcBottom = new global::System.Drawing.Rectangle(srcTopBottom.X + num, srcTopBottom.Y, num, height2);
			this.srcFront = new global::System.Drawing.Rectangle(srcSides.X + height2, srcSides.Y, num, height);
			this.srcBack = new global::System.Drawing.Rectangle(srcSides.X + height2 + num + height2, srcSides.Y, num, height);
			this.srcLeft = new global::System.Drawing.Rectangle(srcSides.Location, new global::System.Drawing.Size(height2, height));
			this.srcRight = new global::System.Drawing.Rectangle(srcSides.X + height2 + num, srcSides.Y, height2, height);
			this.SetImage(image);
		}

		private void SetImage(global::System.Drawing.Image image)
		{
			bool flag = (byte)(this.effects & global::MinecraftUSkinEditor.Models.Effects.FlipHorizontally) == 1;
			bool flag2 = (byte)(this.effects & global::MinecraftUSkinEditor.Models.Effects.FlipVertically) == 2;
			int width = this.srcFront.Width;
			int height = this.srcFront.Height;
			int width2 = this.srcLeft.Width;
			this.top = new global::MinecraftUSkinEditor.Models.TexturePlane(image, flag2 ? this.srcBottom : this.srcTop, new global::MinecraftUSkinEditor.Models.Point3D((float)width * 0.5f, (float)width2 * 0.5f, (float)(-(float)height) * 0.5f), new global::MinecraftUSkinEditor.Models.Point3D(0f, 1f, 0f), this.effects & global::MinecraftUSkinEditor.Models.Effects.FlipHorizontally);
			this.bottom = new global::MinecraftUSkinEditor.Models.TexturePlane(image, flag2 ? this.srcTop : this.srcBottom, new global::MinecraftUSkinEditor.Models.Point3D((float)width / 2f, (float)width2 / 2f, (float)height / 2f), new global::MinecraftUSkinEditor.Models.Point3D(0f, -1f, 0f), this.effects & global::MinecraftUSkinEditor.Models.Effects.FlipHorizontally);
			this.front = new global::MinecraftUSkinEditor.Models.TexturePlane(image, this.srcFront, new global::MinecraftUSkinEditor.Models.Point3D((float)width * 0.5f, (float)height * 0.5f, (float)(-(float)width2) * 0.5f), new global::MinecraftUSkinEditor.Models.Point3D(0f, 0f, 1f), this.effects);
			this.back = new global::MinecraftUSkinEditor.Models.TexturePlane(image, this.srcBack, new global::MinecraftUSkinEditor.Models.Point3D((float)width * 0.5f, (float)height * 0.5f, (float)(-(float)width2) * 0.5f), new global::MinecraftUSkinEditor.Models.Point3D(0f, 0f, -1f), this.effects);
			this.left = new global::MinecraftUSkinEditor.Models.TexturePlane(image, flag ? this.srcRight : this.srcLeft, new global::MinecraftUSkinEditor.Models.Point3D((float)width2 * 0.5f, (float)height * 0.5f, (float)(-(float)width) * 0.5f), new global::MinecraftUSkinEditor.Models.Point3D(-1f, 0f, 0f), this.effects);
			this.right = new global::MinecraftUSkinEditor.Models.TexturePlane(image, flag ? this.srcLeft : this.srcRight, new global::MinecraftUSkinEditor.Models.Point3D((float)width2 * 0.5f, (float)height * 0.5f, (float)(-(float)width) * 0.5f), new global::MinecraftUSkinEditor.Models.Point3D(1f, 0f, 0f), this.effects);
			this.top.Viewport = this.viewport;
			this.bottom.Viewport = this.viewport;
			this.front.Viewport = this.viewport;
			this.back.Viewport = this.viewport;
			this.left.Viewport = this.viewport;
			this.right.Viewport = this.viewport;
		}

		public override float HitTest(global::System.Drawing.PointF location)
		{
			float num = -1000f;
			float num2 = this.top.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = this.bottom.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = this.front.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = this.back.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = this.left.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			num2 = this.right.HitTest(location);
			if (num2 > num)
			{
				num = num2;
			}
			return num;
		}

		private global::MinecraftUSkinEditor.Models.TexturePlane top;

		private global::MinecraftUSkinEditor.Models.TexturePlane bottom;

		private global::MinecraftUSkinEditor.Models.TexturePlane front;

		private global::MinecraftUSkinEditor.Models.TexturePlane back;

		private global::MinecraftUSkinEditor.Models.TexturePlane left;

		private global::MinecraftUSkinEditor.Models.TexturePlane right;

		private global::System.Drawing.Rectangle srcTop;

		private global::System.Drawing.Rectangle srcBottom;

		private global::System.Drawing.Rectangle srcFront;

		private global::System.Drawing.Rectangle srcBack;

		private global::System.Drawing.Rectangle srcLeft;

		private global::System.Drawing.Rectangle srcRight;

		private global::MinecraftUSkinEditor.Models.Matrix3D topLocalTransformation = global::MinecraftUSkinEditor.Models.Matrix3D.CreateRotationX(-1.57079637f);

		private global::MinecraftUSkinEditor.Models.Matrix3D bottomLocalTransformation = global::MinecraftUSkinEditor.Models.Matrix3D.CreateRotationX(-1.57079637f);

		private global::MinecraftUSkinEditor.Models.Matrix3D frontLocalTransformation = global::MinecraftUSkinEditor.Models.Matrix3D.Identity;

		private global::MinecraftUSkinEditor.Models.Matrix3D backLocalTransformation = global::MinecraftUSkinEditor.Models.Matrix3D.CreateRotationY(3.14159274f);

		private global::MinecraftUSkinEditor.Models.Matrix3D leftLocalTransformation = global::MinecraftUSkinEditor.Models.Matrix3D.CreateRotationY(-1.57079637f);

		private global::MinecraftUSkinEditor.Models.Matrix3D rightLocalTransformation = global::MinecraftUSkinEditor.Models.Matrix3D.CreateRotationY(1.57079637f);

		private global::MinecraftUSkinEditor.Models.Effects effects;
	}
}
