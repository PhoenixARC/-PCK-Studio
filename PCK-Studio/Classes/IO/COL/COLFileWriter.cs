using PckStudio.Classes.FileTypes;
using System;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.COL
{
    internal class COLFileWriter : StreamDataWriter
    {
        private COLFile colourFile;

        public static void Write(Stream stream, COLFile file)
        {
            new COLFileWriter(file).WriteToStream(stream);
        }

        public COLFileWriter(COLFile file) : base(false)
        {
            colourFile = file;
        }

        protected override void WriteToStream(Stream stream)
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
                    WriteUInt(stream, colorEntry.color_b);
                    WriteUInt(stream, colorEntry.color_c);
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
