using System;

namespace PckStudio.Models
{
	public abstract class Object3D
	{
		public abstract global::System.Drawing.Image Image { set; }

		public float Angle1
		{
			get
			{
				return this.angle1;
			}
			set
			{
				this.angle1 = value;
				this.UpdateRotation();
			}
		}

		public float Angle2
		{
			get
			{
				return this.angle2;
			}
			set
			{
				this.angle2 = value;
				this.UpdateRotation();
			}
		}

		public float MinAngle1
		{
			get
			{
				return this.minAngle1;
			}
			set
			{
				this.minAngle1 = value;
			}
		}

		public float MinAngle2
		{
			get
			{
				return this.minAngle2;
			}
			set
			{
				this.minAngle2 = value;
			}
		}

		public float MaxAngle1
		{
			get
			{
				return this.maxAngle1;
			}
			set
			{
				this.maxAngle1 = value;
			}
		}

		public float MaxAngle2
		{
			get
			{
				return this.maxAngle2;
			}
			set
			{
				this.maxAngle2 = value;
			}
		}

		public float AngleRange1
		{
			get
			{
				return this.maxAngle1 - this.minAngle1;
			}
			set
			{
				this.minAngle1 = this.angle1 - value / 2f;
				this.maxAngle1 = this.angle1 + value / 2f;
			}
		}

		public float AngleRange2
		{
			get
			{
				return this.maxAngle2 - this.minAngle2;
			}
			set
			{
				this.minAngle2 = this.angle2 - value / 2f;
				this.maxAngle2 = this.angle2 + value / 2f;
			}
		}

		public float MinDegrees1
		{
			get
			{
				return this.minAngle1 / 0.0174532924f;
			}
			set
			{
				this.minAngle1 = value * 0.0174532924f;
			}
		}

		public float MinDegrees2
		{
			get
			{
				return this.minAngle2 / 0.0174532924f;
			}
			set
			{
				this.minAngle2 = value * 0.0174532924f;
			}
		}

		public float MaxDegrees1
		{
			get
			{
				return this.maxAngle1 / 0.0174532924f;
			}
			set
			{
				this.maxAngle1 = value * 0.0174532924f;
			}
		}

		public float MaxDegrees2
		{
			get
			{
				return this.maxAngle2 / 0.0174532924f;
			}
			set
			{
				this.maxAngle2 = value * 0.0174532924f;
			}
		}

		public float DegreesRange1
		{
			get
			{
				return this.AngleRange1 / 0.0174532924f;
			}
			set
			{
				this.AngleRange1 = value * 0.0174532924f;
			}
		}

		public float DegreesRange2
		{
			get
			{
				return this.AngleRange2 / 0.0174532924f;
			}
			set
			{
				this.AngleRange2 = value * 0.0174532924f;
			}
		}

		public float Scale
		{
			get
			{
				return this.scaleTransformation.M11;
			}
			set
			{
				this.scaleTransformation = global::PckStudio.Models.Matrix3D.CreateScale(value);
				this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
			}
		}

		public global::PckStudio.Models.RotationOrders RotationOrder
		{
			get
			{
				return this.order;
			}
			set
			{
				this.order = value;
				switch (this.order)
				{
				case global::PckStudio.Models.RotationOrders.XY:
					this.Rotate = new global::PckStudio.Models.Object3D.RotateMethod(this.RotateXY);
					this.UpdateRotation = new global::PckStudio.Models.Object3D.UpdateRotationMethod(this.UpdateRotationXY);
					return;
				case global::PckStudio.Models.RotationOrders.YX:
					this.Rotate = new global::PckStudio.Models.Object3D.RotateMethod(this.RotateYX);
					this.UpdateRotation = new global::PckStudio.Models.Object3D.UpdateRotationMethod(this.UpdateRotationYX);
					return;
				case global::PckStudio.Models.RotationOrders.XZ:
					this.Rotate = new global::PckStudio.Models.Object3D.RotateMethod(this.RotateXZ);
					this.UpdateRotation = new global::PckStudio.Models.Object3D.UpdateRotationMethod(this.UpdateRotationXZ);
					return;
				case global::PckStudio.Models.RotationOrders.ZX:
					this.Rotate = new global::PckStudio.Models.Object3D.RotateMethod(this.RotateZX);
					this.UpdateRotation = new global::PckStudio.Models.Object3D.UpdateRotationMethod(this.UpdateRotationZX);
					return;
				case global::PckStudio.Models.RotationOrders.YZ:
					this.Rotate = new global::PckStudio.Models.Object3D.RotateMethod(this.RotateYZ);
					this.UpdateRotation = new global::PckStudio.Models.Object3D.UpdateRotationMethod(this.UpdateRotationYZ);
					return;
				case global::PckStudio.Models.RotationOrders.ZY:
					this.Rotate = new global::PckStudio.Models.Object3D.RotateMethod(this.RotateZY);
					this.UpdateRotation = new global::PckStudio.Models.Object3D.UpdateRotationMethod(this.UpdateRotationZY);
					return;
				default:
					return;
				}
			}
		}

		internal virtual global::PckStudio.Models.MinecraftModelView Viewport
		{
			set
			{
				this.viewport = value;
			}
		}

		public global::PckStudio.Models.Point3D Origin
		{
			get
			{
				return new global::PckStudio.Models.Point3D(-this.originTranslation.M14, -this.originTranslation.M24, -this.originTranslation.M34);
			}
			set
			{
				this.originTranslation = global::PckStudio.Models.Matrix3D.CreateTranslation(-value.X, -value.Y, -value.Z);
				this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
			}
		}

		public global::PckStudio.Models.Point3D Position
		{
			get
			{
				return new global::PckStudio.Models.Point3D(this.positionTranslation.M14, this.positionTranslation.M24, this.positionTranslation.M34);
			}
			set
			{
				this.positionTranslation = global::PckStudio.Models.Matrix3D.CreateTranslation(value);
				this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
				this.Update();
			}
		}

		internal abstract void Update();

		public global::PckStudio.Models.Matrix3D GlobalTransformation
		{
			get
			{
				return this.globalTransformation;
			}
			set
			{
				this.globalTransformation = value;
				this.Update();
			}
		}

		public global::PckStudio.Models.Matrix3D LocalTransformation
		{
			get
			{
				return this.localTransformation;
			}
			set
			{
				this.localTransformation = value;
				this.Update();
			}
		}

		public void SetRotation(float angle1, float angle2)
		{
			this.angle1 = angle1;
			this.angle2 = angle2;
			this.UpdateRotation();
		}

		public void RotateByMouse(float deltaX, float deltaY)
		{
			if (this.Rotate != null)
			{
				this.Rotate(deltaX, deltaY);
				this.Update();
			}
		}

		private void CorrectAngles()
		{
			if (this.angle1 > this.maxAngle1)
			{
				this.angle1 = this.maxAngle1;
			}
			else if (this.angle1 < this.minAngle1)
			{
				this.angle1 = this.minAngle1;
			}
			if (this.angle2 > this.maxAngle2)
			{
				this.angle2 = this.maxAngle2;
				return;
			}
			if (this.angle2 < this.minAngle2)
			{
				this.angle2 = this.minAngle2;
			}
		}

		public abstract float HitTest(global::System.Drawing.PointF location);

		private void RotateXY(float delta1, float delta2)
		{
			this.angle1 += delta1 * 0.0174532924f;
			this.angle2 += delta2 * 0.0174532924f * (float)global::System.Math.Cos((double)(this.viewport.RotationY * 0.0174532924f));
			this.UpdateRotationXY();
		}

		private void RotateYX(float delta1, float delta2)
		{
			this.angle1 += delta1 * 0.0174532924f;
			this.angle2 += delta2 * 0.0174532924f * (float)global::System.Math.Cos((double)this.viewport.RotationY * 3.1415926535897931 / 180.0);
			this.UpdateRotationYX();
		}

		private void RotateXZ(float delta1, float delta2)
		{
			this.angle1 += delta1 * 0.0174532924f * (float)global::System.Math.Cos((double)(this.viewport.RotationY * 0.0174532924f)) + delta2 * 0.0174532924f * (float)global::System.Math.Sin((double)(this.viewport.RotationY * 0.0174532924f));
			this.angle2 += delta2 * 0.0174532924f * (float)global::System.Math.Cos((double)(this.viewport.RotationY * 0.0174532924f)) - delta1 * 0.0174532924f * (float)global::System.Math.Sin((double)(this.viewport.RotationY * 0.0174532924f));
			this.UpdateRotationXZ();
		}

		private void RotateZX(float delta1, float delta2)
		{
			this.angle1 += delta1 * 0.0174532924f * (float)global::System.Math.Cos((double)(this.viewport.RotationY * 0.0174532924f)) + delta2 * 0.0174532924f * (float)global::System.Math.Sin((double)(this.viewport.RotationY * 0.0174532924f));
			this.angle2 += delta2 * 0.0174532924f * (float)global::System.Math.Cos((double)(this.viewport.RotationY * 0.0174532924f)) - delta1 * 0.0174532924f * (float)global::System.Math.Sin((double)(this.viewport.RotationY * 0.0174532924f));
			this.UpdateRotationZX();
		}

		private void RotateZY(float delta1, float delta2)
		{
			this.angle1 -= delta2 * 0.0174532924f;
			this.angle2 += delta1 * 0.0174532924f;
			this.UpdateRotationZY();
		}

		private void RotateYZ(float delta1, float delta2)
		{
			this.angle1 += delta1 * 0.0174532924f;
			this.angle2 += delta2 * 0.0174532924f * (float)global::System.Math.Sin((double)(this.viewport.RotationY * 0.0174532924f));
			this.UpdateRotationYZ();
		}

		private void UpdateRotationXY()
		{
			this.CorrectAngles();
			this.localRotation = global::PckStudio.Models.Matrix3D.CreateRotationY(this.angle1) * global::PckStudio.Models.Matrix3D.CreateRotationX(this.angle2);
			this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
		}

		private void UpdateRotationYX()
		{
			this.CorrectAngles();
			this.localRotation = global::PckStudio.Models.Matrix3D.CreateRotationX(this.angle2) * global::PckStudio.Models.Matrix3D.CreateRotationY(this.angle1);
			this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
		}

		private void UpdateRotationXZ()
		{
			this.CorrectAngles();
			this.localRotation = global::PckStudio.Models.Matrix3D.CreateRotationZ(this.angle1) * global::PckStudio.Models.Matrix3D.CreateRotationX(this.angle2);
			this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
		}

		private void UpdateRotationZX()
		{
			this.CorrectAngles();
			this.localRotation = global::PckStudio.Models.Matrix3D.CreateRotationX(this.angle2) * global::PckStudio.Models.Matrix3D.CreateRotationZ(this.angle1);
			this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
		}

		private void UpdateRotationZY()
		{
			this.CorrectAngles();
			this.localRotation = global::PckStudio.Models.Matrix3D.CreateRotationY(this.angle2) * global::PckStudio.Models.Matrix3D.CreateRotationZ(this.angle1);
			this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
		}

		private void UpdateRotationYZ()
		{
			this.CorrectAngles();
			this.localRotation = global::PckStudio.Models.Matrix3D.CreateRotationZ(this.angle2) * global::PckStudio.Models.Matrix3D.CreateRotationY(this.angle1);
			this.localTransformation = this.positionTranslation * this.localRotation * this.originTranslation * this.scaleTransformation;
		}

		protected Object3D()
		{
		}

		public const float PIby180 = 0.0174532924f;

		protected global::PckStudio.Models.Matrix3D originTranslation = global::PckStudio.Models.Matrix3D.Identity;

		protected global::PckStudio.Models.Matrix3D positionTranslation = global::PckStudio.Models.Matrix3D.Identity;

		protected global::PckStudio.Models.Matrix3D scaleTransformation = global::PckStudio.Models.Matrix3D.Identity;

		protected global::PckStudio.Models.Matrix3D localRotation = global::PckStudio.Models.Matrix3D.Identity;

		protected global::PckStudio.Models.Matrix3D localTransformation = global::PckStudio.Models.Matrix3D.Identity;

		protected global::PckStudio.Models.Matrix3D globalTransformation = global::PckStudio.Models.Matrix3D.Identity;

		private float angle1;

		private float angle2;

		private float maxAngle1 = 3.14159274f;

		private float minAngle1 = -3.14159274f;

		private float maxAngle2 = 3.14159274f;

		private float minAngle2 = -3.14159274f;

		private global::PckStudio.Models.RotationOrders order;

		protected global::PckStudio.Models.MinecraftModelView viewport;

		private global::PckStudio.Models.Object3D.RotateMethod Rotate;

		private global::PckStudio.Models.Object3D.UpdateRotationMethod UpdateRotation;

		private delegate void RotateMethod(float deltaX, float deltaY);

		private delegate void UpdateRotationMethod();
	}
}
