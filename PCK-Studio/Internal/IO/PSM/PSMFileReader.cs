using System.Diagnostics;
using System.IO;
using System.Text;
using OMI;
using OMI.Workers;
using PckStudio.Internal.FileFormats;
using PckStudio.Internal;

namespace PckStudio.Internal.IO.PSM
{
    internal class PSMFileReader : IDataFormatReader<PSMFile>, IDataFormatReader
    {
        public PSMFile FromFile(string filename)
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

        public PSMFile FromStream(Stream stream)
        {
            using var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian);
            
            var magic = reader.ReadString(3);
            if (magic != PSMFile.HEADER_MAGIC)
            {
                Trace.TraceError("PSMFileReader.FromStream - Failed to load csmb.\n\tReason: Header magic mismatch.");
                return new PSMFile(byte.MaxValue);
            }
            
            byte version = reader.ReadByte();
            if (version < 1 || version > 1)
            {
                Trace.TraceError("PSMFileReader.FromStream - Failed to load csmb.\n\tReason: Unsupported version.");
                return new PSMFile(byte.MaxValue);
            }

            var skinANIM = SkinANIM.FromValue(reader.ReadInt32());
            PSMFile csmbFile = new PSMFile(version, skinANIM);
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
            string type = GetParentType((PSMParentType)reader.ReadByte());
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
            PSMOffsetType type = (PSMOffsetType)reader.ReadByte();
            float value = reader.ReadSingle();
            return new SkinPartOffset(GetOffsetType(type), value);
        }

        private static string GetParentType(PSMParentType type)
        {
            switch (type)
            {
                case PSMParentType.HEAD:
                    return "HEAD";
                case PSMParentType.BODY:
                    return "BODY";
                case PSMParentType.ARM0:
                    return "ARM0";
                case PSMParentType.ARM1:
                    return "ARM1";
                case PSMParentType.LEG0:
                    return "LEG0";
                case PSMParentType.LEG1:
                    return "LEG1";
                default:
                    throw new InvalidDataException(type.ToString());
            }
        }

        private static string GetOffsetType(PSMOffsetType type)
        {
            switch (type)
            {
                case PSMOffsetType.HEAD:
                    return "HEAD";
                case PSMOffsetType.BODY:
                    return "BODY";
                case PSMOffsetType.ARM0:
                    return "ARM0";
                case PSMOffsetType.ARM1:
                    return "ARM1";
                case PSMOffsetType.LEG0:
                    return "LEG0";
                case PSMOffsetType.LEG1:
                    return "LEG1";
                case PSMOffsetType.TOOL0:
                    return "TOOL0";
                case PSMOffsetType.TOOL1:
                    return "TOOL1";
                case PSMOffsetType.HELMET:
                    return "HELMET";
                case PSMOffsetType.SHOULDER0:
                    return "SHOULDER0";
                case PSMOffsetType.SHOULDER1:
                    return "SHOULDER1";
                case PSMOffsetType.CHEST:
                    return "CHEST";
                case PSMOffsetType.WAIST:
                    return "WAIST";
                case PSMOffsetType.PANTS0:
                    return "PANTS0";
                case PSMOffsetType.PANTS1:
                    return "PANTS1";
                case PSMOffsetType.BOOT0:
                    return "BOOT0";
                case PSMOffsetType.BOOT1:
                    return "BOOT1";
                default:
                    throw new InvalidDataException(type.ToString());
            }
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
