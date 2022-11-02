using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.CSMB
{
    internal class CSMBFileReader : StreamDataReader
    {
        public CSMBFile Read(Stream stream)
        {
            return new CSMBFileReader().ReadFromStream(stream);
        }
        private CSMBFileReader() : base(false)
        { }

        private CSMBFile ReadFromStream(Stream stream)
        {
            CSMBFile BinFile = new CSMBFile();
            ReadInt(stream);
            int NumOfParts = ReadInt(stream);
            for(int i = 0; i < NumOfParts; i++)
            {
                CSMBPart part = new CSMBPart();
                part.Name = ReadString(stream);
                part.Parent = (ParentPart)ReadInt(stream);
                part.posX = ReadFloat(stream);
                part.posY = ReadFloat(stream);
                part.posZ = ReadFloat(stream);
                part.sizeX = ReadFloat(stream);
                part.sizeY = ReadFloat(stream);
                part.sizeZ = ReadFloat(stream);
                part.uvX = ReadInt(stream);
                part.uvY = ReadInt(stream);
                part.MirrorTexture = ReadBool(stream);
                part.HideWArmour = ReadBool(stream);
                part.Inflation = ReadFloat(stream);
                BinFile.Parts.Add(part);
            }
            int NumOfOffsets = ReadInt(stream);
            for (int i = 0; i < NumOfOffsets; i++)
            {
                CSMBOffset offset = new CSMBOffset();
                offset.offsetPart = (OffsetPart)ReadInt(stream);
                offset.VerticalOffset = ReadFloat(stream);
                BinFile.Offsets.Add(offset);
            }
                return BinFile;
        }

        private string ReadString(Stream stream)
        {
            short strlen = ReadShort(stream);
            return ReadString(stream, strlen, Encoding.ASCII);
        }
    }
}
