using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Internal
{
    internal static class GameConstants
    {
        // See: https://minecraft.fandom.com/wiki/Dye#Color_values for more information.
        public static readonly Color[] DyeColors = [
            Color.FromArgb(0xf9fffe), // White
            Color.FromArgb(0xf9801d), // Orange
            Color.FromArgb(0xc74ebd), // Magenta
            Color.FromArgb(0x3ab3da), // Light Blue
            Color.FromArgb(0xfed83d), // Yellow
            Color.FromArgb(0x80c71f), // Lime
            Color.FromArgb(0xf38baa), // Pink
            Color.FromArgb(0x474f52), // Gray
            Color.FromArgb(0x9d9d97), // Light Gray
            Color.FromArgb(0x169c9c), // Cyan
            Color.FromArgb(0x8932b8), // Purple
            Color.FromArgb(0x3c44aa), // Blue
            Color.FromArgb(0x835432), // Brown
            Color.FromArgb(0x5e7c16), // Green
            Color.FromArgb(0xb02e26), // Red
            Color.FromArgb(0x1d1d21), // Black
            ];
    }
}
