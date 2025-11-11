using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PckStudio.ToolboxItems
{
    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// </summary>
    public class InterpolationPictureBox : PictureBox
    {
        public InterpolationMode InterpolationMode { get; set; }
        
        public InterpolationMode BackgroundInterpolationMode { get; set; }

        protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = BackgroundInterpolationMode;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            base.OnPaintBackground(paintEventArgs);
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            base.OnPaint(paintEventArgs);
        }
    }
}
