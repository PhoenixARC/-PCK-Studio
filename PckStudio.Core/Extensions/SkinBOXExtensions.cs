using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Skin;

namespace PckStudio.Core.Extensions
{
    public static class SkinBOXExtensions
    {
        public static GraphicsPath GetUVGraphicsPath(this SkinBOX skinBOX, Vector2 tillingFactor)
        {
            var types = new byte[9];
            var points = new PointF[9];

            var path = new GraphicsPath(FillMode.Winding);

            Vector2 uv = skinBOX.UV;
            Vector3 size = skinBOX.Size;

            path.AddRectangle(new RectangleF(new PointF((uv.X                      ) * tillingFactor.X, (uv.Y + size.Z) * tillingFactor.Y), new SizeF(size.Z * tillingFactor.X, size.Y * tillingFactor.Y)));
            path.AddRectangle(new RectangleF(new PointF((uv.X + size.Z             ) * tillingFactor.X, (uv.Y + size.Z) * tillingFactor.Y), new SizeF(size.X * tillingFactor.X, size.Y * tillingFactor.Y)));
            path.AddRectangle(new RectangleF(new PointF((uv.X + size.Z + size.X    ) * tillingFactor.X, (uv.Y + size.Z) * tillingFactor.Y), new SizeF(size.Z * tillingFactor.X, size.Y * tillingFactor.Y)));
            path.AddRectangle(new RectangleF(new PointF((uv.X + size.Z * 2 + size.X) * tillingFactor.X, (uv.Y + size.Z) * tillingFactor.Y), new SizeF(size.X * tillingFactor.X, size.Y * tillingFactor.Y)));
            
            path.AddRectangle(new RectangleF(new PointF((uv.X + size.Z             ) * tillingFactor.X, (uv.Y         ) * tillingFactor.Y), new SizeF(size.X * tillingFactor.X, size.Z * tillingFactor.Y)));
            path.AddRectangle(new RectangleF(new PointF((uv.X + size.Z + size.X    ) * tillingFactor.X, (uv.Y         ) * tillingFactor.Y), new SizeF(size.X * tillingFactor.X, size.Z * tillingFactor.Y)));

            return path;
        }

        public static GraphicsPath GetUVGraphicsPath(this SkinBOX skinBox)
        {
            return skinBox.GetUVGraphicsPath(Vector2.One);
        }

        public static string GetOverlayType(this SkinBOX skinBox)
        {
            if (!skinBox.IsValidType())
                return "";
            if (skinBox.IsOverlayPart())
                return skinBox.Type;
            int index = Array.IndexOf(SkinBOX.BaseTypes, skinBox.Type);
            return SkinBOX.OverlayTypes.IndexInRange(index) ? SkinBOX.OverlayTypes[index] : "";
        }

        public static string GetOverlayType(string type)
        {
            if (!SkinBOX.IsValidType(type))
                return "";
            if (SkinBOX.IsOverlayPart(type))
                return type;
            int index = Array.IndexOf(SkinBOX.BaseTypes, type);
            return SkinBOX.OverlayTypes.IndexInRange(index) ? SkinBOX.OverlayTypes[index] : "";
        }

        public static string GetBaseType(this SkinBOX skinBox)
        {
            if (!skinBox.IsValidType())
                return "";
            if (skinBox.IsBasePart())
                return skinBox.Type;
            int index = Array.IndexOf(SkinBOX.OverlayTypes, skinBox.Type);
            return SkinBOX.BaseTypes.IndexInRange(index) ? SkinBOX.BaseTypes[index] : "";
        }

        public static string GetBaseType(string type)
        {
            if (!SkinBOX.IsValidType(type))
                return "";
            if (SkinBOX.IsBasePart(type))
                return type;
            int index = Array.IndexOf(SkinBOX.OverlayTypes, type);
            return SkinBOX.BaseTypes.IndexInRange(index) ? SkinBOX.BaseTypes[index] : "";
        }

        public enum SkinBoxFace
        {
            Front,
            Back,
            Top,
            Bottom,
            Left,
            Right
        }

        public static Rectangle GetFaceArea(this SkinBOX skinBox, SkinBoxFace face) => new Rectangle(skinBox.GetPoint(face), skinBox.GetSize(face));

        public static Point GetPoint(this SkinBOX skinBox, SkinBoxFace face)
        {
            return Point.Truncate((face) switch
            {
                SkinBoxFace.Front  => new PointF(skinBox.UV.X + skinBox.Size.Z                     , skinBox.UV.Y + skinBox.Size.Z),
                SkinBoxFace.Back   => new PointF(skinBox.UV.X + skinBox.Size.Z * 2 + skinBox.Size.X, skinBox.UV.Y + skinBox.Size.Z),
                SkinBoxFace.Top    => new PointF(skinBox.UV.X + skinBox.Size.X                     , skinBox.UV.Y),
                SkinBoxFace.Bottom => new PointF(skinBox.UV.X + skinBox.Size.X * 2                 , skinBox.UV.Y),
                SkinBoxFace.Left   => new PointF(skinBox.UV.X + skinBox.Size.Z + skinBox.Size.X    , skinBox.UV.Y + skinBox.Size.Z),
                SkinBoxFace.Right  => new PointF(skinBox.UV.X + skinBox.Size.Z                     , skinBox.UV.Y + skinBox.Size.Z),
                _ => PointF.Empty,
            });
        }

        public static Size GetSize(this SkinBOX skinBox, SkinBoxFace face)
        {
            return Size.Truncate((face) switch
            {
                SkinBoxFace.Front  => new SizeF(skinBox.Size.X, skinBox.Size.Y),
                SkinBoxFace.Back   => new SizeF(skinBox.Size.X, skinBox.Size.Y),
                SkinBoxFace.Top    => new SizeF(skinBox.Size.X, skinBox.Size.Z),
                SkinBoxFace.Bottom => new SizeF(skinBox.Size.X, skinBox.Size.Z),
                SkinBoxFace.Left   => new SizeF(skinBox.Size.Z, skinBox.Size.Y),
                SkinBoxFace.Right  => new SizeF(skinBox.Size.Z, skinBox.Size.Y),
                _ => SizeF.Empty,
            });
        }

    }
}
