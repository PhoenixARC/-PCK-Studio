using System;
using System.Drawing;

namespace PckStudio.Models
{
	public abstract class Object3D
	{
		public abstract Image Image { set; }

		public float Angle1
		{
			get
			{
				return angle1;
			}
			set
			{
				angle1 = value;
				OnUpdateRotation();
			}
		}

		public float Angle2
		{
			get
			{
				return angle2;
			}
			set
			{
				angle2 = value;
				OnUpdateRotation();
			}
		}

		public float MinAngle1
		{
			get
			{
				return minAngle1;
			}
			set
			{
				minAngle1 = value;
			}
		}

		public float MinAngle2
		{
			get
			{
				return minAngle2;
			}
			set
			{
				minAngle2 = value;
			}
		}

		public float MaxAngle1
		{
			get
			{
				return maxAngle1;
			}
			set
			{
				maxAngle1 = value;
			}
		}

		public float MaxAngle2
		{
			get
			{
				return maxAngle2;
			}
			set
			{
				maxAngle2 = value;
			}
		}

		public float AngleRange1
		{
			get
			{
				return maxAngle1 - minAngle1;
			}
			set
			{
				minAngle1 = angle1 - value / 2f;
				maxAngle1 = angle1 + value / 2f;
			}
		}

		public float AngleRange2
		{
			get
			{
				return maxAngle2 - minAngle2;
			}
			set
			{
				minAngle2 = angle2 - value / 2f;
				maxAngle2 = angle2 + value / 2f;
			}
		}

		public float MinDegrees1
		{
			get
			{
				return minAngle1 / PIby180;
			}
			set
			{
				minAngle1 = value * PIby180;
			}
		}

		public float MinDegrees2
		{
			get
			{
				return minAngle2 / PIby180;
			}
			set
			{
				minAngle2 = value * PIby180;
			}
		}

		public float MaxDegrees1
		{
			get
			{
				return maxAngle1 / PIby180;
			}
			set
			{
				maxAngle1 = value * PIby180;
			}
		}

		public float MaxDegrees2
		{
			get
			{
				return maxAngle2 / PIby180;
			}
			set
			{
				maxAngle2 = value * PIby180;
			}
		}

		public float DegreesRange1
		{
			get
			{
				return AngleRange1 / PIby180;
			}
			set
			{
				AngleRange1 = value * PIby180;
			}
		}

		public float DegreesRange2
		{
			get
			{
				return AngleRange2 / PIby180;
			}
			set
			{
				AngleRange2 = value * PIby180;
			}
		}

		public float Scale
		{
			get
			{
				return scaleTransformation.M11;
			}
			set
			{
				scaleTransformation = Matrix3D.CreateScale(value);
				UpdateRotation();
			}
		}

		public RotationOrders RotationOrder
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
				switch (order)
				{
					case RotationOrders.XY:
						Rotate = new RotateMethod(RotateXY);
						OnUpdateRotation = UpdateRotationXY;
						return;
					case RotationOrders.YX:
						Rotate = new RotateMethod(RotateYX);
						OnUpdateRotation = UpdateRotationYX;
						return;
					case RotationOrders.XZ:
						Rotate = new RotateMethod(RotateXZ);
						OnUpdateRotation = UpdateRotationXZ;
						return;
					case RotationOrders.ZX:
						Rotate = new RotateMethod(RotateZX);
						OnUpdateRotation = UpdateRotationZX;
						return;
					case RotationOrders.YZ:
						Rotate = new RotateMethod(RotateYZ);
						OnUpdateRotation = UpdateRotationYZ;
						return;
					case RotationOrders.ZY:
						Rotate = new RotateMethod(RotateZY);
						OnUpdateRotation = UpdateRotationZY;
						return;
					default:
						return;
				}
			}
		}

		internal virtual MinecraftModelView Viewport
		{
			set
			{
				viewport = value;
			}
		}

		public Point3D Origin
		{
			get
			{
				return new Point3D(-originTranslation.M14, -originTranslation.M24, -originTranslation.M34);
			}
			set
			{
				originTranslation = Matrix3D.CreateTranslation(-value.X, -value.Y, -value.Z);
				UpdateRotation();
			}
		}

		public Point3D Position
		{
			get
			{
				return new Point3D(positionTranslation.M14, positionTranslation.M24, positionTranslation.M34);
			}
			set
			{
				positionTranslation = Matrix3D.CreateTranslation(value);
				UpdateRotation();
				Update();
			}
		}

		internal abstract void Update();

		public Matrix3D GlobalTransformation
		{
			get
			{
				return globalTransformation;
			}
			set
			{
				globalTransformation = value;
				Update();
			}
		}

		public Matrix3D LocalTransformation
		{
			get
			{
				return localTransformation;
			}
			set
			{
				localTransformation = value;
				Update();
			}
		}

		public void SetRotation(float angle1, float angle2)
		{
			this.angle1 = angle1;
			this.angle2 = angle2;
			OnUpdateRotation();
		}

		public void RotateByMouse(float deltaX, float deltaY)
		{
			if (Rotate != null)
			{
				Rotate(deltaX, deltaY);
				Update();
			}
		}

		private void CorrectAngles()
		{
			if (angle1 > maxAngle1)
			{
				angle1 = maxAngle1;
			}
			else if (angle1 < minAngle1)
			{
				angle1 = minAngle1;
			}
			if (angle2 > maxAngle2)
			{
				angle2 = maxAngle2;
				return;
			}
			if (angle2 < minAngle2)
			{
				angle2 = minAngle2;
			}
		}

		public abstract float HitTest(PointF location);

		private void RotateXY(float delta1, float delta2)
		{
			angle1 += delta1 * PIby180;
			angle2 += delta2 * PIby180 * (float)Math.Cos((double)(viewport.RotationY * PIby180));
			UpdateRotationXY();
		}

		private void RotateYX(float delta1, float delta2)
		{
			angle1 += delta1 * PIby180;
			angle2 += delta2 * PIby180 * (float)Math.Cos(viewport.RotationY * 3.1415926535897931 / 180.0);
			UpdateRotationYX();
		}

		private void RotateXZ(float delta1, float delta2)
		{
			angle1 += delta1 * PIby180 * (float)Math.Cos((double)(viewport.RotationY * PIby180)) + delta2 * PIby180 * (float)Math.Sin((double)(viewport.RotationY * PIby180));
			angle2 += delta2 * PIby180 * (float)Math.Cos((double)(viewport.RotationY * PIby180)) - delta1 * PIby180 * (float)Math.Sin((double)(viewport.RotationY * PIby180));
			UpdateRotationXZ();
		}

		private void RotateZX(float delta1, float delta2)
		{
			angle1 += delta1 * PIby180 * (float)Math.Cos((double)(viewport.RotationY * PIby180)) + delta2 * PIby180 * (float)Math.Sin((double)(viewport.RotationY * PIby180));
			angle2 += delta2 * PIby180 * (float)Math.Cos((double)(viewport.RotationY * PIby180)) - delta1 * PIby180 * (float)Math.Sin((double)(viewport.RotationY * PIby180));
			UpdateRotationZX();
		}

		private void RotateZY(float delta1, float delta2)
		{
			angle1 -= delta2 * PIby180;
			angle2 += delta1 * PIby180;
			UpdateRotationZY();
		}

		private void RotateYZ(float delta1, float delta2)
		{
			angle1 += delta1 * PIby180;
			angle2 += delta2 * PIby180 * (float)Math.Sin((double)(viewport.RotationY * PIby180));
			UpdateRotationYZ();
		}

		private void UpdateRotationXY()
		{
			CorrectAngles();
			localRotation = Matrix3D.CreateRotationY(angle1) * Matrix3D.CreateRotationX(angle2);
			UpdateRotation();
		}

		private void UpdateRotationYX()
		{
			CorrectAngles();
			localRotation = Matrix3D.CreateRotationX(angle2) * Matrix3D.CreateRotationY(angle1);
			UpdateRotation();
		}

		private void UpdateRotationXZ()
		{
			CorrectAngles();
			localRotation = Matrix3D.CreateRotationZ(angle1) * Matrix3D.CreateRotationX(angle2);
			UpdateRotation();
		}

		private void UpdateRotationZX()
		{
			CorrectAngles();
			localRotation = Matrix3D.CreateRotationX(angle2) * Matrix3D.CreateRotationZ(angle1);
			UpdateRotation();
		}

		private void UpdateRotationZY()
		{
			CorrectAngles();
			localRotation = Matrix3D.CreateRotationY(angle2) * Matrix3D.CreateRotationZ(angle1);
			UpdateRotation();
		}

		private void UpdateRotationYZ()
		{
			CorrectAngles();
			localRotation = Matrix3D.CreateRotationZ(angle2) * Matrix3D.CreateRotationY(angle1);
			UpdateRotation();
		}

		private void UpdateRotation()
		{
            localTransformation = positionTranslation * localRotation * originTranslation * scaleTransformation;
        }

		public const float PIby180 = 0.0174532924f;

		protected Matrix3D originTranslation = Matrix3D.Identity;

		protected Matrix3D positionTranslation = Matrix3D.Identity;

		protected Matrix3D scaleTransformation = Matrix3D.Identity;

		protected Matrix3D localRotation = Matrix3D.Identity;

		protected Matrix3D localTransformation = Matrix3D.Identity;

		protected Matrix3D globalTransformation = Matrix3D.Identity;

		private float angle1;

		private float angle2;

		private float maxAngle1 = (float)Math.PI;
		private float minAngle1 = (float)-Math.PI;

		private float maxAngle2 = (float)Math.PI;
		private float minAngle2 = (float)-Math.PI;

		private RotationOrders order;

		protected MinecraftModelView viewport;

		private RotateMethod Rotate;

		private Action OnUpdateRotation;

		private delegate void RotateMethod(float deltaX, float deltaY);
	}
}
