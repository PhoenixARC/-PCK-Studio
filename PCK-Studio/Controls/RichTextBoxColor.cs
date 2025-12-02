using System.Drawing;

namespace PckStudio.Controls
{
    public class RichTextBoxColor
    {
        public Color Foreground;
        public Color Background;

        public RichTextBoxColor(Color foreground, Color background)
        {
            Foreground = foreground;
            Background = background;
        }

        public RichTextBoxColor(Color foreground) : this(foreground, Color.Transparent) { }
    }
}
