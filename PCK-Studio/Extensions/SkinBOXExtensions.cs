using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Internal;

namespace PckStudio.Extensions
{
    internal static class SkinBOXExtensions
    {
        public static GraphicsPath GetUVGraphicsPath(this SkinBOX skinBOX, Vector2 tillingFactor)
        {
            var types = new byte[9];
            var points = new PointF[9];

            types[0] = (byte)PathPointType.Start;
            types[1] = (byte)PathPointType.Line;
            types[2] = (byte)PathPointType.Line;
            types[3] = (byte)PathPointType.Line;
            types[4] = (byte)PathPointType.Line;
            types[5] = (byte)PathPointType.Line;
            types[6] = (byte)PathPointType.Line;
            types[7] = (byte)PathPointType.Line;
            types[8] = (byte)PathPointType.Line;

            points[0] = new PointF(skinBOX.UV.X, skinBOX.UV.Y + skinBOX.Size.Z);
            points[1] = new PointF(skinBOX.UV.X + skinBOX.Size.Z, skinBOX.UV.Y + skinBOX.Size.Z);
            points[2] = new PointF(skinBOX.UV.X + skinBOX.Size.Z, skinBOX.UV.Y);
            points[3] = new PointF(skinBOX.UV.X + skinBOX.Size.Z + skinBOX.Size.X * 2, skinBOX.UV.Y);
            points[4] = new PointF(skinBOX.UV.X + skinBOX.Size.Z + skinBOX.Size.X * 2, skinBOX.UV.Y + skinBOX.Size.Z);
            points[5] = new PointF(skinBOX.UV.X + skinBOX.Size.Z * 2 + skinBOX.Size.X * 2, skinBOX.UV.Y + skinBOX.Size.Z);
            points[6] = new PointF(skinBOX.UV.X + skinBOX.Size.Z * 2 + skinBOX.Size.X * 2, skinBOX.UV.Y + skinBOX.Size.Z + skinBOX.Size.Y);
            points[7] = new PointF(skinBOX.UV.X, skinBOX.UV.Y + skinBOX.Size.Z + skinBOX.Size.Y);
            points[8] = points[0];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF(points[i].X * tillingFactor.X, points[i].Y * tillingFactor.Y);
            }

            return new GraphicsPath(points, types);

        }

        public static GraphicsPath GetUVGraphicsPath(this SkinBOX skinBox)
        {
            return skinBox.GetUVGraphicsPath(Vector2.One);
        }

        public static string GetOverlayType(this SkinBOX skinBox)
        {
            if (!skinBox.IsValidType())
                return "";
            int index = Array.IndexOf(SkinBOX.BaseTypes, skinBox.Type);
            return SkinBOX.OverlayTypes.IndexInRange(index) ? SkinBOX.OverlayTypes[index] : "";
        }

        public static string GetBaseType(this SkinBOX skinBox)
        {
            if (!skinBox.IsValidType())
                return "";
            int index = Array.IndexOf(SkinBOX.OverlayTypes, skinBox.Type);
            return SkinBOX.BaseTypes.IndexInRange(index) ? SkinBOX.BaseTypes[index] : "";
        }

        public static bool IsBasePart(this SkinBOX skinBox)
        {
            return SkinBOX.BaseTypes.Contains(skinBox.Type);
        }

        public static bool IsOverlayPart(this SkinBOX skinBox)
        {
            return SkinBOX.OverlayTypes.Contains(skinBox.Type);
        }
    }
}
