using System.IO;
using System.Text;
using PckStudio.Internal.FileFormats;
using OMI.Workers;
using OMI;
using PckStudio.Internal;
using System;
using OpenTK;

namespace PckStudio.IO.CSMB
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
                writer.Write(CSMBFile.HEADER_MAGIC);
                writer.Write(_CSMB.Version);
                writer.Write(_CSMB.SkinANIM.ToValue());
                writer.Write(_CSMB.Parts.Count);
                foreach (SkinBOX part in _CSMB.Parts)
                {
                    WritePart(writer, part);
                }
                writer.Write(_CSMB.Offsets.Count);
                foreach (SkinPartOffset offset in _CSMB.Offsets)
                {
                    writer.Write((byte)GetOffsetPart(offset.Type));
                    writer.Write(offset.Value);
                }
            }
        }

        private void WritePart(EndiannessAwareBinaryWriter writer, SkinBOX part)
        {
            writer.Write((byte)GetParentPart(part.Type));
            writer.Write(part.Pos.X);
            writer.Write(part.Pos.Y);
            writer.Write(part.Pos.Z);
            writer.Write(part.Size.X);
            writer.Write(part.Size.Y);
            writer.Write(part.Size.Z);

            byte uvX = (byte)MathHelper.Clamp((int)part.UV.X, 0, 64);
            byte uvY = (byte)MathHelper.Clamp((int)part.UV.Y, 0, 64);
            byte mirrorAndUvX = (byte)(Convert.ToByte(part.Mirror) << 7 | uvX);
            byte hideWithArmorAndUvY = (byte)(Convert.ToByte(part.HideWithArmor) << 7 | uvY);

            writer.Write(mirrorAndUvX);
            writer.Write(hideWithArmorAndUvY);
            writer.Write(part.Scale);
        }

        private static CSMBParentType GetParentPart(string type)
        {
            switch (type)
            {
                case "HEAD":
                    return CSMBParentType.HEAD;
                case "BODY":
                    return CSMBParentType.BODY;
                case "ARM0":
                    return CSMBParentType.ARM0;
                case "ARM1":
                    return CSMBParentType.ARM1;
                case "LEG0":
                    return CSMBParentType.LEG0;
                case "LEG1":
                    return CSMBParentType.LEG1;
                default:
                    throw new InvalidDataException(type);
            }
        }

        private static CSMBOffsetType GetOffsetPart(string type)
        {
            switch (type)
            {
                case "HEAD":
                    return CSMBOffsetType.HEAD;
                case "BODY":
                    return CSMBOffsetType.BODY;
                case "ARM0":
                    return CSMBOffsetType.ARM0;
                case "ARM1":
                    return CSMBOffsetType.ARM1;
                case "LEG0":
                    return CSMBOffsetType.LEG0;
                case "LEG1":
                    return CSMBOffsetType.LEG1;
                case "TOOL0":
                    return CSMBOffsetType.TOOL0;
                case "TOOL1":
                    return CSMBOffsetType.TOOL1;
                case "HELMET":
                    return CSMBOffsetType.HELMET;
                case "SHOULDER0":
                    return CSMBOffsetType.SHOULDER0;
                case "SHOULDER1":
                    return CSMBOffsetType.SHOULDER1;
                case "CHEST":
                    return CSMBOffsetType.CHEST;
                case "WAIST":
                    return CSMBOffsetType.WAIST;
                case "PANTS0":
                    return CSMBOffsetType.PANTS0;
                case "PANTS1":
                    return CSMBOffsetType.PANTS1;
                case "BOOT0":
                    return CSMBOffsetType.BOOT0;
                case "BOOT1":
                    return CSMBOffsetType.BOOT1;
                default:
                    throw new InvalidDataException(type);
            }
        }
    }
}
