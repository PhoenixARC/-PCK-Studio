using System.Diagnostics;
using System.IO;
using System.Text;
using OMI;
using OMI.Workers;
using PckStudio.Internal.FileFormats;
using PckStudio.Internal;

namespace PckStudio.IO.CSMB
{
    internal class CSMBFileReader : IDataFormatReader<CSMBFile>, IDataFormatReader
    {
        public CSMBFile FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                using (var fs = File.OpenRead(filename))
                {
                    return FromStream(fs);
                }
            }
            throw new FileNotFoundException(filename);
        }

        public CSMBFile FromStream(Stream stream)
        {
            using var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian);
            
            var magic = reader.ReadString(3);
            if (magic != CSMBFile.HEADER_MAGIC)
            {
                Trace.TraceError("CSMBFileReader.FromStream - Failed to load csmb.\n\tReason: Header magic mismatch.");
                return new CSMBFile(byte.MaxValue);
            }
            
            byte version = reader.ReadByte();
            if (version < 1 || version > 1)
            {
                Trace.TraceError("CSMBFileReader.FromStream - Failed to load csmb.\n\tReason: Unsupported version.");
                return new CSMBFile(byte.MaxValue);
            }

            var skinANIM = SkinANIM.FromValue(reader.ReadInt32());
            CSMBFile csmbFile = new CSMBFile(version, skinANIM);
            int numOfParts = reader.ReadInt32();
            for (int i = 0; i < numOfParts; i++)
            {
                SkinBOX part = ReadPart(reader);
                csmbFile.Parts.Add(part);
            }
            int numOfOffsets = reader.ReadInt32();
            for (int i = 0; i < numOfOffsets; i++)
            {
                SkinPartOffset offset = ReadOffset(reader);
                csmbFile.Offsets.Add(offset);
            }
            
            return csmbFile;
        }

        private SkinBOX ReadPart(EndiannessAwareBinaryReader reader)
        {
            string type = GetParentType((CSMBParentType)reader.ReadByte());
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float posZ = reader.ReadSingle();
            float sizeX = reader.ReadSingle();
            float sizeY = reader.ReadSingle();
            float sizeZ = reader.ReadSingle();
            byte mirrorAndUvX = reader.ReadByte();
            byte hideWithArmorAndUvY = reader.ReadByte();
            int uvX = mirrorAndUvX & 0x7f;
            int uvY = hideWithArmorAndUvY & 0x7f;
            bool mirror = (mirrorAndUvX & 0x80) != 0;
            bool hideWithArmor = (hideWithArmorAndUvY & 0x80) != 0;
            float scale = reader.ReadSingle();
            return new SkinBOX(type, new System.Numerics.Vector3(posX, posY, posZ), new System.Numerics.Vector3(sizeX, sizeY, sizeZ), new System.Numerics.Vector2(uvX, uvY), hideWithArmor, mirror, scale);
        }

        private SkinPartOffset ReadOffset(EndiannessAwareBinaryReader reader)
        {
            CSMBOffsetType type = (CSMBOffsetType)reader.ReadByte();
            float value = reader.ReadSingle();
            return new SkinPartOffset(GetOffsetType(type), value);
        }

        private static string GetParentType(CSMBParentType type)
        {
            switch (type)
            {
                case CSMBParentType.HEAD:
                    return "HEAD";
                case CSMBParentType.BODY:
                    return "BODY";
                case CSMBParentType.ARM0:
                    return "ARM0";
                case CSMBParentType.ARM1:
                    return "ARM1";
                case CSMBParentType.LEG0:
                    return "LEG0";
                case CSMBParentType.LEG1:
                    return "LEG1";
                default:
                    throw new InvalidDataException(type.ToString());
            }
        }

        private static string GetOffsetType(CSMBOffsetType type)
        {
            switch (type)
            {
                case CSMBOffsetType.HEAD:
                    return "HEAD";
                case CSMBOffsetType.BODY:
                    return "BODY";
                case CSMBOffsetType.ARM0:
                    return "ARM0";
                case CSMBOffsetType.ARM1:
                    return "ARM1";
                case CSMBOffsetType.LEG0:
                    return "LEG0";
                case CSMBOffsetType.LEG1:
                    return "LEG1";
                case CSMBOffsetType.TOOL0:
                    return "TOOL0";
                case CSMBOffsetType.TOOL1:
                    return "TOOL1";
                case CSMBOffsetType.HELMET:
                    return "HELMET";
                case CSMBOffsetType.SHOULDER0:
                    return "SHOULDER0";
                case CSMBOffsetType.SHOULDER1:
                    return "SHOULDER1";
                case CSMBOffsetType.CHEST:
                    return "CHEST";
                case CSMBOffsetType.WAIST:
                    return "WAIST";
                case CSMBOffsetType.PANTS0:
                    return "PANTS0";
                case CSMBOffsetType.PANTS1:
                    return "PANTS1";
                case CSMBOffsetType.BOOT0:
                    return "BOOT0";
                case CSMBOffsetType.BOOT1:
                    return "BOOT1";
                default:
                    throw new InvalidDataException(type.ToString());
            }
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
