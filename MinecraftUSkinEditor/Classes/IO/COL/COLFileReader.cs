using PckStudio.Classes.FileTypes;
using System.IO;
using System.Text;
using static PckStudio.Classes.FileTypes.COLFile;

namespace PckStudio.Classes.IO.COL
{
    internal class COLFileReader : StreamDataReader
    {
        public static COLFile Read(Stream stream)
        {
            return new COLFileReader().ReadFromStream(stream);
        }

        private COLFileReader() : base(false)
        {}

        private COLFile ReadFromStream(Stream stream)
        {
            COLFile colourFile = new COLFile();
            int has_water_colors = ReadInt(stream);
            int color_entries = ReadInt(stream);
            for (int i = 0; i < color_entries; i++)
            {
                string name = ReadString(stream);
                uint color = ReadUInt(stream);
                colourFile.entries.Add(new ColorEntry(name, color));
            }
            if (has_water_colors > 0)
            {
                int water_color_entries = ReadInt(stream);
                for (int i = 0; i < water_color_entries; i++)
                {
                    string name = ReadString(stream);
                    uint color = ReadUInt(stream);
                    uint rgbcolor = ReadUInt(stream);
                    uint unk = ReadUInt(stream);
                    colourFile.waterEntries.Add(new ExtendedColorEntry(name, color, rgbcolor, unk));
                }
            }
            return colourFile;
        }

        private string ReadString(Stream stream)
        {
            short strlen = ReadShort(stream);
            return ReadString(stream, strlen, Encoding.ASCII);
        }
    }
}
