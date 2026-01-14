using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Extensions;

namespace PckStudio.Core.IO.Java
{
    public class JavaConstants
    {
        public static Dictionary<string, (Color foreground, Color background)> JavaColorCodeToColor { get; } = new Dictionary<string, (Color foreground, Color background)>()
        {
            ["§0"] = (Color.FromArgb(0x00, 0x00, 0x00), Color.FromArgb(18, 18, 18)),
            ["§1"] = (Color.FromArgb(0x00, 0x00, 0xAA), Color.FromArgb(0x00, 0x00, 0x2A)),
            ["§2"] = (Color.FromArgb(0x00, 0xAA, 0x00), Color.FromArgb(0x00, 0x2A, 0x00)),
            ["§3"] = (Color.FromArgb(0x00, 0xAA, 0xAA), Color.FromArgb(0x00, 0x2A, 0x2A)),
            ["§4"] = (Color.FromArgb(0xAA, 0x00, 0x00), Color.FromArgb(0x2A, 0x00, 0x00)),
            ["§5"] = (Color.FromArgb(0xAA, 0x00, 0xAA), Color.FromArgb(0x2A, 0x00, 0x2A)),
            ["§6"] = (Color.FromArgb(0xFF, 0xAA, 0x00), Color.FromArgb(0x2A, 0x2A, 0x00)),
            ["§7"] = (Color.FromArgb(0xAA, 0xAA, 0xAA), Color.FromArgb(0x2A, 0x2A, 0x2A)),
            ["§8"] = (Color.FromArgb(0x55, 0x55, 0x55), Color.FromArgb(0x15, 0x15, 0x15)),
            ["§9"] = (Color.FromArgb(0x55, 0x55, 0xFF), Color.FromArgb(0x15, 0x15, 0x3F)),
            ["§a"] = (Color.FromArgb(0x55, 0xFF, 0x55), Color.FromArgb(0x15, 0x3F, 0x15)),
            ["§b"] = (Color.FromArgb(0x55, 0xFF, 0xFF), Color.FromArgb(0x15, 0x3F, 0x3F)),
            ["§c"] = (Color.FromArgb(0xFF, 0x55, 0x55), Color.FromArgb(0x3F, 0x15, 0x15)),
            ["§d"] = (Color.FromArgb(0xFF, 0x55, 0xFF), Color.FromArgb(0x3F, 0x15, 0x3F)),
            ["§e"] = (Color.FromArgb(0xFF, 0xFF, 0x55), Color.FromArgb(0x3F, 0x3F, 0x15)),
            ["§f"] = (Color.FromArgb(0xFF, 0xFF, 0xFF), Color.FromArgb(0x3F, 0x3F, 0x3F)),
            ["§g"] = (Color.FromArgb(0xDD, 0xD6, 0x05), Color.FromArgb(0x37, 0x35, 0x01)),
            ["§h"] = (Color.FromArgb(0xE3, 0xD4, 0xD1), Color.FromArgb(0x38, 0x35, 0x34)),
            ["§i"] = (Color.FromArgb(0xCE, 0xCA, 0xCA), Color.FromArgb(0x33, 0x32, 0x32)),
            ["§j"] = (Color.FromArgb(0x44, 0x3A, 0x3B), Color.FromArgb(0x11, 0x0E, 0x0E)),
            ["§m"] = (Color.FromArgb(0x97, 0x16, 0x07), Color.FromArgb(0x25, 0x05, 0x01)),
            ["§n"] = (Color.FromArgb(0xB4, 0x68, 0x4D), Color.FromArgb(0x2D, 0x1A, 0x13)),
            ["§p"] = (Color.FromArgb(0xDE, 0xB1, 0x2D), Color.FromArgb(0x37, 0x2C, 0x0B)),
            ["§q"] = (Color.FromArgb(0x47, 0xA0, 0x36), Color.FromArgb(0x04, 0x28, 0x0D)),
            ["§s"] = (Color.FromArgb(0x2C, 0xBA, 0xA8), Color.FromArgb(0x0B, 0x2E, 0x2A)),
            ["§t"] = (Color.FromArgb(0x21, 0x49, 0x7B), Color.FromArgb(0x08, 0x12, 0x1E)),
            ["§u"] = (Color.FromArgb(0x9A, 0x5C, 0xC6), Color.FromArgb(0x26, 0x17, 0x31)),
        };

        public static readonly Color BirchLeaves = Color.FromArgb(unchecked((int)0xFF_80A755));
        public static readonly Color SpruceLeaves = Color.FromArgb(unchecked((int)0xFF_619961));
        public static readonly Color LilyPad = Color.FromArgb(unchecked((int)0xFF_208030));
        public static readonly Color AttachedMelonStem = Color.FromArgb(unchecked((int)0xFF_E0C71C));
        public static readonly Color AttachedPumpkinStem = Color.FromArgb(unchecked((int)0xFF_E0C71C));

        public static string EsapceMiencarftJavaFormat(string text) 
            => text.Split(['§'], StringSplitOptions.RemoveEmptyEntries).ToString("");


        public struct HTMLString(string value)
        {
            private string value = value;

            private List<string> _front = new();
            private List<string> _back = new();

            public void AddTag(string tagName, params string[] parameters)
            {
                string param = string.Join(" ", parameters);
                _front.Add($"<{new string[] { tagName, param }.ToString(" ")}>");
                _back.Insert(0, $"</{tagName}>");
            }

            public override string ToString()
            {
                return string.Join("", _front) + value + string.Join("", _back);
            }
        }

        public static string ConvertJavaTextFormatToHTML(string text)
        {
            string[] sections = text.Split(['§'], StringSplitOptions.RemoveEmptyEntries);
            if (sections.Length == 0)
                return text;
            Color textColor = Color.White;
            string formatText = string.Join("", sections
                .Select(s =>
                {
                    if (string.IsNullOrWhiteSpace(s))
                        return "";
                    string colorFormat = "§" + s[0];
                    if (JavaColorCodeToColor.TryGetValue(colorFormat, out (Color foreground, Color background) color))
                    {
                        textColor = color.foreground;
                    }

                    if (s.Length == 1)
                        return "";

                    string colorText = s.Substring(1);
                    bool hasNewLineAtEnd = colorText.EndsWith("\n");
                    string text = hasNewLineAtEnd ? colorText.Substring(0, colorText.Length - 1) : colorText;
                    string cr = hasNewLineAtEnd ? "\n" : "";
                    HTMLString htmlString = new HTMLString(colorText);
                    if (colorFormat == "§r")
                    {
                        textColor = Color.White;
                    }
                    if (textColor != Color.White)
                    {
                        string htmlForegroundColor = textColor.ToHTMLColor();
                        htmlString.AddTag("font", $"color=\"{htmlForegroundColor}\"");
                    }
                    // italic
                    if (colorFormat == "§o")
                        htmlString.AddTag("i");
                    // bold
                    if (colorFormat == "§l")
                        htmlString.AddTag("b");
                    // strikethrough
                    if (colorFormat == "§m")
                        htmlString.AddTag("s");
                    // underline
                    if (colorFormat == "§n")
                        htmlString.AddTag("u");

                    return $"{htmlString}{cr}";
                }
                ));
            return formatText;
        }
    }
}
