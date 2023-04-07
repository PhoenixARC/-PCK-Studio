using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;
using OMI.Workers;
using OMI;

namespace PckStudio.Classes.IO.CSMB
{
    internal class CSMBFileWriter : IDataFormatWriter
    {
        CSMBFile _CSMB;

        public CSMBFileWriter(CSMBFile csmb)
        {
            _CSMB = csmb;
        }

        public void WriteToFile(string filename)
        {
            using(var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian))
            {
                writer.Write(0);
                writer.Write(_CSMB.Parts.Count);
                foreach (CSMBPart part in _CSMB.Parts)
                {
                    writer.Write((short)part.Name.Length);
                    writer.WriteString(part.Name);
                    writer.Write((int)part.Parent);
                    writer.Write(part.posX);
                    writer.Write(part.posY);
                    writer.Write(part.posZ);
                    writer.Write(part.sizeX);
                    writer.Write(part.sizeY);
                    writer.Write(part.sizeZ);
                    writer.Write(part.uvX);
                    writer.Write(part.uvY);
                    writer.Write(part.MirrorTexture);
                    writer.Write(part.HideWArmour);
                    writer.Write(part.Inflation);
                }
                writer.Write(_CSMB.Offsets.Count);
                foreach (CSMBOffset offset in _CSMB.Offsets)
                {
                    writer.Write((int)offset.offsetPart);
                    writer.Write(offset.VerticalOffset);
                }
            }
        }
    }
}
