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
            CompositingQuality = default;
            InterpolationMode = default;
            SmoothingMode = default;
            PixelOffsetMode = default;
            CompositingMode = default;
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

        internal static Graphics Fill(this Graphics graphics, Rectangle area, Color color)
        {
            Region clip = graphics.Clip;
            graphics.SetClip(area, CombineMode.Replace);
            graphics.Clear(color);
            graphics.SetClip(clip, CombineMode.Replace);
            return graphics;
        }
    }
}
