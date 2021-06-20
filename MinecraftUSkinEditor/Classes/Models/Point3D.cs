using System;

namespace MinecraftUSkinEditor.Models
{
	public struct Point3D
	{
		public Point3D(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public static global::MinecraftUSkinEditor.Models.Point3D Zero
		{
			get
			{
				return default(global::MinecraftUSkinEditor.Models.Point3D);
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

		public static global::MinecraftUSkinEditor.Models.Point3D operator +(global::MinecraftUSkinEditor.Models.Point3D a, global::MinecraftUSkinEditor.Models.Point3D b)
		{
			return new global::MinecraftUSkinEditor.Models.Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static global::MinecraftUSkinEditor.Models.Point3D operator -(global::MinecraftUSkinEditor.Models.Point3D a, global::MinecraftUSkinEditor.Models.Point3D b)
		{
			return new global::MinecraftUSkinEditor.Models.Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static global::MinecraftUSkinEditor.Models.Point3D operator *(global::MinecraftUSkinEditor.Models.Point3D p, float s)
		{
			return new global::MinecraftUSkinEditor.Models.Point3D(p.X * s, p.Y * s, p.Z * s);
		}

		public static global::MinecraftUSkinEditor.Models.Point3D operator /(global::MinecraftUSkinEditor.Models.Point3D p, float s)
		{
			return new global::MinecraftUSkinEditor.Models.Point3D(p.X / s, p.Y / s, p.Z / s);
		}

		public float X;

		public float Y;

		public float Z;
	}
}
