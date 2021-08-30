using System;

namespace PckStudio.Models
{
	public struct Point3D
	{
		public Point3D(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public static global::PckStudio.Models.Point3D Zero
		{
			get
			{
				return default(global::PckStudio.Models.Point3D);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.X,
				";",
				this.Y,
				";",
				this.Z,
				")"
			});
		}

		public static global::PckStudio.Models.Point3D operator +(global::PckStudio.Models.Point3D a, global::PckStudio.Models.Point3D b)
		{
			return new global::PckStudio.Models.Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static global::PckStudio.Models.Point3D operator -(global::PckStudio.Models.Point3D a, global::PckStudio.Models.Point3D b)
		{
			return new global::PckStudio.Models.Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static global::PckStudio.Models.Point3D operator *(global::PckStudio.Models.Point3D p, float s)
		{
			return new global::PckStudio.Models.Point3D(p.X * s, p.Y * s, p.Z * s);
		}

		public static global::PckStudio.Models.Point3D operator /(global::PckStudio.Models.Point3D p, float s)
		{
			return new global::PckStudio.Models.Point3D(p.X / s, p.Y / s, p.Z / s);
		}

		public float X;

		public float Y;

		public float Z;
	}
}
