using System;

namespace PckStudio.Models
{
	public struct Matrix3D
	{
		public Matrix3D(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M14 = m14;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M24 = m24;
			M31 = m31;
			M32 = m32;
			M33 = m33;
			M34 = m34;
			M41 = m41;
			M42 = m42;
			M43 = m43;
			M44 = m44;
		}

		public static Matrix3D CreateRotationX(float radians)
		{
			float num = (float)Math.Sin((double)radians);
			float num2 = (float)Math.Cos((double)radians);
			return new Matrix3D(1f, 0f, 0f, 0f, 0f, num2, -num, 0f, 0f, num, num2, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateRotationX(float sin, float cos)
		{
			return new Matrix3D(1f, 0f, 0f, 0f, 0f, cos, -sin, 0f, 0f, sin, cos, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateRotationY(float radians)
		{
			float num = (float)Math.Sin((double)radians);
			float num2 = (float)Math.Cos((double)radians);
			return new Matrix3D(num2, 0f, num, 0f, 0f, 1f, 0f, 0f, -num, 0f, num2, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateRotationY(float sin, float cos)
		{
			return new Matrix3D(cos, 0f, sin, 0f, 0f, 1f, 0f, 0f, -sin, 0f, cos, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateRotationZ(float radians)
		{
			float num = (float)Math.Sin((double)radians);
			float num2 = (float)Math.Cos((double)radians);
			return new Matrix3D(num2, -num, 0f, 0f, num, num2, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateRotationZ(float sin, float cos)
		{
			return new Matrix3D(cos, -sin, 0f, 0f, sin, cos, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateTranslation(float x, float y, float z)
		{
			return new Matrix3D(1f, 0f, 0f, x, 0f, 1f, 0f, y, 0f, 0f, 1f, z, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateTranslation(Point3D point)
		{
			return new Matrix3D(1f, 0f, 0f, point.X, 0f, 1f, 0f, point.Y, 0f, 0f, 1f, point.Z, 0f, 0f, 0f, 1f);
		}

		public static Matrix3D CreateScale(float s)
		{
			return new Matrix3D(s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, 1f);
		}

		public float Determinant
		{
			get
			{
				return M11 * M22 * M33 * M44 + M21 * M32 * M43 * M14 + M31 * M42 * M13 * M24 + M41 * M12 * M23 * M34 - M14 * M23 * M32 * M41 - M24 * M33 * M42 * M11 - M34 * M43 * M12 * M21 - M44 * M13 * M22 * M31;
			}
		}

		public static Matrix3D Identity
		{
			get
			{
				return new Matrix3D(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
			}
		}

		public static Matrix3D Transpose(Matrix3D m)
		{
			return new Matrix3D(m.M11, m.M21, m.M31, m.M41, m.M12, m.M22, m.M32, m.M42, m.M13, m.M23, m.M33, m.M43, m.M14, m.M24, m.M34, m.M44);
		}

		public static Matrix3D Invert(Matrix3D m)
		{
			float determinant = m.Determinant;
			return new Matrix3D((m.M22 * m.M33 * m.M44 + m.M32 * m.M43 * m.M24 + m.M42 * m.M23 * m.M34 - m.M24 * m.M33 * m.M42 - m.M34 * m.M43 * m.M22 - m.M44 * m.M23 * m.M32) / determinant, -(m.M12 * m.M33 * m.M44 + m.M32 * m.M43 * m.M14 + m.M42 * m.M13 * m.M34 - m.M14 * m.M33 * m.M42 - m.M34 * m.M43 * m.M12 - m.M44 * m.M13 * m.M32) / determinant, (m.M12 * m.M23 * m.M44 + m.M22 * m.M43 * m.M14 + m.M42 * m.M13 * m.M24 - m.M14 * m.M23 * m.M42 - m.M24 * m.M43 * m.M12 - m.M44 * m.M13 * m.M22) / determinant, -(m.M12 * m.M23 * m.M34 + m.M22 * m.M33 * m.M14 + m.M32 * m.M13 * m.M24 - m.M14 * m.M23 * m.M32 - m.M24 * m.M33 * m.M12 - m.M34 * m.M13 * m.M22) / determinant, -(m.M21 * m.M33 * m.M44 + m.M31 * m.M43 * m.M24 + m.M41 * m.M23 * m.M34 - m.M24 * m.M33 * m.M41 - m.M34 * m.M43 * m.M21 - m.M44 * m.M23 * m.M31) / determinant, (m.M11 * m.M33 * m.M44 + m.M31 * m.M43 * m.M14 + m.M41 * m.M13 * m.M34 - m.M14 * m.M33 * m.M41 - m.M34 * m.M43 * m.M11 - m.M44 * m.M13 * m.M31) / determinant, -(m.M11 * m.M23 * m.M44 + m.M21 * m.M43 * m.M14 + m.M41 * m.M13 * m.M24 - m.M14 * m.M23 * m.M41 - m.M24 * m.M43 * m.M11 - m.M44 * m.M13 * m.M21) / determinant, (m.M11 * m.M23 * m.M34 + m.M21 * m.M33 * m.M14 + m.M31 * m.M13 * m.M24 - m.M14 * m.M23 * m.M31 - m.M24 * m.M33 * m.M11 - m.M34 * m.M13 * m.M21) / determinant, (m.M21 * m.M32 * m.M44 + m.M31 * m.M42 * m.M24 + m.M41 * m.M22 * m.M34 - m.M24 * m.M32 * m.M41 - m.M34 * m.M42 * m.M21 - m.M44 * m.M22 * m.M31) / determinant, -(m.M11 * m.M32 * m.M44 + m.M31 * m.M42 * m.M14 + m.M41 * m.M12 * m.M34 - m.M14 * m.M32 * m.M41 - m.M34 * m.M42 * m.M11 - m.M44 * m.M12 * m.M31) / determinant, (m.M11 * m.M32 * m.M44 + m.M21 * m.M42 * m.M14 + m.M41 * m.M12 * m.M24 - m.M14 * m.M22 * m.M41 - m.M24 * m.M42 * m.M11 - m.M44 * m.M12 * m.M21) / determinant, -(m.M11 * m.M22 * m.M34 + m.M21 * m.M32 * m.M14 + m.M31 * m.M12 * m.M24 - m.M14 * m.M22 * m.M31 - m.M24 * m.M32 * m.M11 - m.M34 * m.M12 * m.M21) / determinant, -(m.M21 * m.M32 * m.M43 + m.M31 * m.M42 * m.M23 + m.M41 * m.M22 * m.M33 - m.M23 * m.M32 * m.M41 - m.M33 * m.M42 * m.M21 - m.M43 * m.M22 * m.M31) / determinant, (m.M11 * m.M32 * m.M43 + m.M31 * m.M42 * m.M13 + m.M41 * m.M12 * m.M33 - m.M13 * m.M32 * m.M41 - m.M33 * m.M42 * m.M11 - m.M43 * m.M12 * m.M31) / determinant, -(m.M11 * m.M22 * m.M43 + m.M21 * m.M42 * m.M13 + m.M41 * m.M12 * m.M23 - m.M13 * m.M22 * m.M41 - m.M23 * m.M42 * m.M11 - m.M43 * m.M12 * m.M21) / determinant, (m.M11 * m.M22 * m.M33 + m.M21 * m.M32 * m.M13 + m.M31 * m.M12 * m.M23 - m.M13 * m.M22 * m.M31 - m.M23 * m.M32 * m.M11 - m.M33 * m.M12 * m.M21) / determinant);
		}

		public static Matrix3D operator +(Matrix3D a, Matrix3D b)
		{
			return new Matrix3D(a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M14 + b.M14, a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M24 + b.M24, a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33, a.M34 + b.M34, a.M41 + b.M41, a.M42 + b.M42, a.M43 + b.M43, a.M44 + b.M44);
		}

		public static Matrix3D operator -(Matrix3D a, Matrix3D b)
		{
			return new Matrix3D(a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13, a.M14 - b.M14, a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23, a.M24 - b.M24, a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33, a.M34 - b.M34, a.M41 - b.M41, a.M42 - b.M42, a.M43 - b.M43, a.M44 - b.M44);
		}

		public static Matrix3D operator *(Matrix3D a, Matrix3D b)
		{
			return new Matrix3D(a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41, a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42, a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43, a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44, a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41, a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42, a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43, a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44, a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41, a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42, a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43, a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44, a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41, a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42, a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43, a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44);
		}

		public static Point3D operator *(Matrix3D m, Point3D p)
		{
			return new Point3D(p.X * m.M11 + p.Y * m.M12 + p.Z * m.M13 + m.M14, p.X * m.M21 + p.Y * m.M22 + p.Z * m.M23 + m.M24, p.X * m.M31 + p.Y * m.M32 + p.Z * m.M33 + m.M34);
		}

		public static Matrix3D operator +(Matrix3D m, float n)
		{
			return new Matrix3D(m.M11 + n, m.M12 + n, m.M13 + n, m.M14 + n, m.M21 + n, m.M22 + n, m.M23 + n, m.M24 + n, m.M31 + n, m.M32 + n, m.M33 + n, m.M34 + n, m.M41 + n, m.M42 + n, m.M43 + n, m.M44 + n);
		}

		public static Matrix3D operator -(Matrix3D m, float n)
		{
			return new Matrix3D(m.M11 - n, m.M12 - n, m.M13 - n, m.M14 - n, m.M21 - n, m.M22 - n, m.M23 - n, m.M24 - n, m.M31 - n, m.M32 - n, m.M33 - n, m.M34 - n, m.M41 - n, m.M42 - n, m.M43 - n, m.M44 - n);
		}

		public static Matrix3D operator *(Matrix3D m, float n)
		{
			return new Matrix3D(m.M11 * n, m.M12 * n, m.M13 * n, m.M14 * n, m.M21 * n, m.M22 * n, m.M23 * n, m.M24 * n, m.M31 * n, m.M32 * n, m.M33 * n, m.M34 * n, m.M41 * n, m.M42 * n, m.M43 * n, m.M44 * n);
		}

		public static Matrix3D operator /(Matrix3D m, float n)
		{
			return new Matrix3D(m.M11 / n, m.M12 / n, m.M13 / n, m.M14 / n, m.M21 / n, m.M22 / n, m.M23 / n, m.M24 / n, m.M31 / n, m.M32 / n, m.M33 / n, m.M34 / n, m.M41 / n, m.M42 / n, m.M43 / n, m.M44 / n);
		}

		public float M11;

		public float M12;

		public float M13;

		public float M14;

		public float M21;

		public float M22;

		public float M23;

		public float M24;

		public float M31;

		public float M32;

		public float M33;

		public float M34;

		public float M41;

		public float M42;

		public float M43;

		public float M44;
	}
}
