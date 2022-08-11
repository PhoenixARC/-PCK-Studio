using PckStudio.Classes.FileTypes;
using System;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.COL
{
    internal class COLFileWriter : StreamDataWriter
    {
        public static void Write(Stream stream, COLFile file)
        {
            new COLFileWriter().WriteToStream(stream, file);
        }

        private COLFileWriter() : base(false)
        {}

        private void WriteToStream(Stream stream, COLFile colourFile)
        {
            WriteInt(stream, Convert.ToInt32(colourFile.waterEntries.Count > 0));
            WriteInt(stream, colourFile.entries.Count);
            foreach (var colorEntry in colourFile.entries)
            {
                WriteString(stream, colorEntry.name);
                WriteUInt(stream, colorEntry.color);
            }
            if (colourFile.waterEntries.Count > 0)
            {
                WriteInt(stream, colourFile.waterEntries.Count);
                foreach (var colorEntry in colourFile.waterEntries)
                {
                    WriteString(stream, colorEntry.name);
                    WriteUInt(stream, colorEntry.color);
                    WriteUInt(stream, colorEntry.rgbcolor);
                    WriteUInt(stream, colorEntry.unk);
                }
            }
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.ASCII);
        }
    }
}
