using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.CSMB
{
    internal class CSMBFileWriter : StreamDataWriter
    {
        public static void Write(Stream stream, CSMBFile file)
        {
            new CSMBFileWriter().WriteToStream(stream, file);
        }
        private CSMBFileWriter() : base(false)
        { }

        private void WriteToStream(Stream stream, CSMBFile CSMB)
        {
            WriteInt(stream, 0);
            WriteInt(stream, CSMB.Parts.Count);
            foreach(CSMBPart part in CSMB.Parts)
            {
                WriteString(stream, part.Name);
                WriteInt(stream, (int)part.Parent);
                WriteFloat(stream, part.posX);
                WriteFloat(stream, part.posY);
                WriteFloat(stream, part.posZ);
                WriteFloat(stream, part.sizeX);
                WriteFloat(stream, part.sizeY);
                WriteFloat(stream, part.sizeZ);
                WriteInt(stream, part.uvX);
                WriteInt(stream, part.uvY);
                WriteBool(stream, part.MirrorTexture);
                WriteBool(stream, part.HideWArmour);
                WriteFloat(stream, part.Inflation);
            }
            WriteInt(stream, CSMB.Offsets.Count);
            foreach (CSMBOffset offset in CSMB.Offsets)
            {
                WriteInt(stream, (int)offset.offsetPart);
                WriteFloat(stream, offset.VerticalOffset);
            }
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.ASCII);
        }
    }
}
