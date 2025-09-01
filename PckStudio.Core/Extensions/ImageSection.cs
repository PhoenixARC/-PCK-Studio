using System.Drawing;

namespace PckStudio.Core.Extensions
{
    internal struct ImageSection
    {
        public readonly Size Size;
        public readonly Point Point;
        public readonly Rectangle Area;

        internal ImageSection(Size originalSize, int index, ImageLayoutDirection layoutDirection)
        {
            switch(layoutDirection)
            {
                case ImageLayoutDirection.Horizontal:
                    {
                        Size = new Size(originalSize.Height, originalSize.Height);
                        Point = new Point(index * originalSize.Height, 0);
                    }
                    break;

                case ImageLayoutDirection.Vertical:
                    {
                        Size = new Size(originalSize.Width, originalSize.Width);
                        Point = new Point(0, index * originalSize.Width);
                    }
                    break;

                default:
                    Size = Size.Empty;
                    Point = new Point(-1, -1);
                    break;
            }
            Area = new Rectangle(Point, Size);
        }
    }
}
