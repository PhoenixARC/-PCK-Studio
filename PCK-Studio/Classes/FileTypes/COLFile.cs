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
            public uint rgbcolor;
            public uint unk;

            public ExtendedColorEntry(string name, uint color, uint rgbcolor, uint unk) : base(name, color)
            {
                this.rgbcolor = rgbcolor;
                this.unk = unk;
            }
        }

        public List<ColorEntry> entries = new List<ColorEntry>();
        public List<ExtendedColorEntry> waterEntries = new List<ExtendedColorEntry>();
    }
}
