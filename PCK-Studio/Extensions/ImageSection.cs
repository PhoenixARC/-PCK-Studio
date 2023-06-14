using System.Drawing;

namespace PckStudio.Extensions
{
    struct ImageSection
    {
        public readonly Size Size;
        public readonly Point Point;
        public readonly Rectangle Area;

        internal ImageSection(Size sectionSize, int index, ImageLayoutDirection layoutDirection)
        {
            switch(layoutDirection)
            {
                case ImageLayoutDirection.Horizontal:
                    {
                        Size = new Size(sectionSize.Height, sectionSize.Height);
                        Point = new Point(index * sectionSize.Height, 0);
                    }
                    break;

                case ImageLayoutDirection.Vertical:
                    {
                        Size = new Size(sectionSize.Width, sectionSize.Width);
                        Point = new Point(0, index * sectionSize.Width);
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
