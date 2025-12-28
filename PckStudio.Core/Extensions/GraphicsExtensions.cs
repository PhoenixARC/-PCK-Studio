using System.Drawing;
using System.Drawing.Drawing2D;

namespace PckStudio.Core.Extensions
{
    public readonly struct GraphicsConfig(InterpolationMode interpolationMode, SmoothingMode smoothingMode, PixelOffsetMode pixelOffsetMode)
    {
        public InterpolationMode InterpolationMode { get; } = interpolationMode;
        public SmoothingMode SmoothingMode { get; } = smoothingMode;
        public PixelOffsetMode PixelOffsetMode { get; } = pixelOffsetMode;

        public static GraphicsConfig PixelPerfect()
            => new GraphicsConfig(InterpolationMode.NearestNeighbor, SmoothingMode.None, PixelOffsetMode.HighQuality);
    }

    public static class GraphicsExtensions
    {
        public static void ApplyConfig(this Graphics graphics, GraphicsConfig config)
        {
            graphics.InterpolationMode = config.InterpolationMode;
            graphics.SmoothingMode = config.SmoothingMode;
            graphics.PixelOffsetMode = config.PixelOffsetMode;
        }

        public static void Fill(this Graphics graphics, Rectangle area, Color color)
        {
            Region clip = graphics.Clip;
            graphics.SetClip(area, CombineMode.Replace);
            graphics.Clear(color);
            graphics.SetClip(clip, CombineMode.Replace);
        }
    }
}
