using System.IO;
using System.Text;
using OMI;
using OMI.Workers;
using PckStudio.FileFormats;

namespace PckStudio.Classes.IO.CSMB
{
    internal class CSMBFileReader : IDataFormatReader<CSMBFile>, IDataFormatReader
    {
        private CSMBFileReader()
        { }

        public CSMBFile FromFile(string filename)
        {
            throw new System.NotImplementedException();
        }

        public CSMBFile FromStream(Stream stream)
        {
            CSMBFile csmbFile = new CSMBFile();
            using (var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian))
            {
                reader.ReadInt32();
                int numOfParts = reader.ReadInt32();
                for (int i = 0; i < numOfParts; i++)
                {
                    CSMBPart part = new CSMBPart();
                    part.Name = ReadString(reader);
                    part.Parent = (CSMBParentPart)reader.ReadInt32();
                    part.posX = reader.ReadSingle();
                    part.posY = reader.ReadSingle();
                    part.posZ = reader.ReadSingle();
                    part.sizeX = reader.ReadSingle();
                    part.sizeY = reader.ReadSingle();
                    part.sizeZ = reader.ReadSingle();
                    part.uvX = reader.ReadInt32();
                    part.uvY = reader.ReadInt32();
                    part.MirrorTexture = reader.ReadBoolean();
                    part.HideWArmour = reader.ReadBoolean();
                    part.Inflation = reader.ReadSingle();
                    csmbFile.Parts.Add(part);
                }
                int numOfOffsets = reader.ReadInt32();
                for (int i = 0; i < numOfOffsets; i++)
                {
                    CSMBOffset offset = new CSMBOffset();
                    offset.offsetPart = (CSMBOffsetPart)reader.ReadInt32();
                    offset.VerticalOffset = reader.ReadSingle();
                    csmbFile.Offsets.Add(offset);
                }
            }
            return csmbFile;
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            ushort strlen = reader.ReadUInt16();
            return reader.ReadString(strlen);
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
