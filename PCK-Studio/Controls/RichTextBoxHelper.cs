using System;
using System.Drawing;
using System.Windows.Forms;

namespace PckStudio.Controls
{
    public static class RichTextBoxHelper
    {
        private static void _AppendText(RichTextBox box, string text, Color foreColor, Color backColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            if (foreColor == Color.Transparent)
                foreColor = box.ForeColor;
            box.SelectionColor = foreColor;
            if (backColor == Color.Transparent)
                backColor = box.BackColor;
            box.SelectionBackColor = backColor;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            
        }

        public static void AppendLine(this RichTextBox box, string text)
        {
            if (!text.EndsWith("\n") && !text.EndsWith("\r\n"))
                text += Environment.NewLine;
            box.AppendText(text);
        }

        public static void AppendText(this RichTextBox box, string text, RichTextBoxColor colors)
        {
            _AppendText(box, text, colors.Foreground, colors.Background);
        }

        public static void AppendLine(this RichTextBox box, string text, Color color)
            => box.AppendLine(text, new RichTextBoxColor(color));
        public static void AppendLine(this RichTextBox box, string text, RichTextBoxColor colors)
        {
            if (!text.EndsWith("\n") && !text.EndsWith("\r\n"))
                text += Environment.NewLine;
            box.AppendText(text, colors);
        }
    }
}
