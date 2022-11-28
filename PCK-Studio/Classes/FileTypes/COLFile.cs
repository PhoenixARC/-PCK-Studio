using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class COLFile
    {
        public class ColorEntry
        {
            public readonly string name;
            public uint color;

            public ColorEntry(string name, uint color)
            {
                this.name = name;
                this.color = color;
            }
        }

        public class ExtendedColorEntry : ColorEntry
        {
            public uint color_b;
            public uint color_c;

            // Water entries consist of three colors
            // color_a - the surface of the water
            // color_b - the color displayed underwater
            // color_c - the color for the distant "fog" displayed while underwater
            public ExtendedColorEntry(string name, uint color_a, uint color_b, uint color_c) : base(name, color_a)
            {
                this.color_b = color_b;
                this.color_c = color_c;
            }
        }

        public bool hasWaterTable;
        public List<ColorEntry> entries = new List<ColorEntry>();
        public List<ExtendedColorEntry> waterEntries = new List<ExtendedColorEntry>();
    }
}
