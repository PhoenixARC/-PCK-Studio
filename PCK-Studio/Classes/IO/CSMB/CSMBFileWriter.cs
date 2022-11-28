using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.CSMB
{
    internal class CSMBFileWriter : StreamDataWriter
    {
        CSMBFile _CSMB;
        public static void Write(Stream stream, CSMBFile file)
        {
            new CSMBFileWriter(file).WriteToStream(stream);
        }

        public CSMBFileWriter(CSMBFile csmb) : base(false)
        {
            _CSMB = csmb;
        }

        protected override void WriteToStream(Stream stream)
        {
            WriteInt(stream, 0);
            WriteInt(stream, _CSMB.Parts.Count);
            foreach(CSMBPart part in _CSMB.Parts)
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
            WriteInt(stream, _CSMB.Offsets.Count);
            foreach (CSMBOffset offset in _CSMB.Offsets)
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
