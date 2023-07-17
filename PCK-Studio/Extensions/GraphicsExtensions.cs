using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace PckStudio.Extensions
{
    internal struct GraphicsConfig
    {
        public GraphicsConfig()
        {
            CompositingQuality = CompositingQuality.Default;
            InterpolationMode = InterpolationMode.Default;
            SmoothingMode = SmoothingMode.Default;
            PixelOffsetMode = PixelOffsetMode.Default;
            CompositingMode = CompositingMode.SourceOver;
        }

        public CompositingMode CompositingMode { get; set; }
        public CompositingQuality CompositingQuality { get; set; }
        public InterpolationMode InterpolationMode { get; set; }
        public SmoothingMode SmoothingMode { get; set; }
        public PixelOffsetMode PixelOffsetMode { get; set; }
    }

    internal static class GraphicsExtensions
    {
        public static void ApplyConfig(this Graphics graphics, GraphicsConfig config)
        {
            graphics.CompositingMode = config.CompositingMode;
            graphics.CompositingQuality = config.CompositingQuality;
            graphics.InterpolationMode = config.InterpolationMode;
            graphics.SmoothingMode = config.SmoothingMode;
            graphics.PixelOffsetMode = config.PixelOffsetMode;
        }
    }
}
