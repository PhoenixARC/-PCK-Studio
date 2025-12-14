using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Color;

namespace PckStudio.Core.Colors
{
    public class AbstractColorContainer
    {
        public AbstractColorContainer(IDictionary<string, Color> colors, IDictionary<string, (Color Surface, Color Underwater, Color Fog)> waterColors)
        {
            Colors = colors;
            WaterColors = waterColors;
        }

        public bool HasColor(string colorName) => Colors.ContainsKey(colorName) || WaterColors.ContainsKey(colorName);

        public static AbstractColorContainer FromColorContainer(ColorContainer colorContainer)
        {
            IDictionary<string, Color> colors = colorContainer.Colors
                .GroupBy(c => c.Name)
                .Select(grp => grp.FirstOrDefault())
                .ToDictionary(c => c.Name, c => c.ColorPallette);
            IDictionary<string, (Color, Color, Color)> waterColors = colorContainer.WaterColors
                .GroupBy(c => c.Name)
                .Select(grp => grp.FirstOrDefault())
                .ToDictionary(c => c.Name, c => (c.SurfaceColor, c.UnderwaterColor, c.FogColor));
            return new AbstractColorContainer(colors, waterColors);
        }

        public IDictionary<string, Color> Colors { get; }
        public IDictionary<string, (Color Surface, Color Underwater, Color Fog)> WaterColors { get; }
    } 
}
