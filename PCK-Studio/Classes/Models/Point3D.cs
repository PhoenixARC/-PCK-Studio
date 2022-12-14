using System;

namespace PckStudio.Models
{
	public struct Point3D
	{
        public float X;
        public float Y;
        public float Z;

        public Point3D(float x, float y, float z)
		{
			(X, Y, Z) = (x, y, z);
		}

		public static Point3D Zero => default(Point3D);

        public override string ToString()
		{
			return string.Format("({0};{1};{2})", X, Y, Z);
		}

		public static Point3D operator +(Point3D a, Point3D b) => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static Point3D operator -(Point3D a, Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static Point3D operator *(Point3D p, float s) => new Point3D(p.X * s, p.Y * s, p.Z * s);

		public static Point3D operator /(Point3D p, float s) => new Point3D(p.X / s, p.Y / s, p.Z / s);		
	}
}
