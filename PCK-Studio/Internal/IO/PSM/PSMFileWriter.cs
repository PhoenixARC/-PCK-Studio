using System.IO;
using System.Text;
using PckStudio.Internal.FileFormats;
using OMI.Workers;
using OMI;
using PckStudio.Internal;
using System;
using OpenTK;

namespace PckStudio.Internal.IO.PSM
{
    internal class PSMFileWriter : IDataFormatWriter
    {
        PSMFile _PSM;

        public PSMFileWriter(PSMFile csmb)
        {
            _PSM = csmb;
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
                writer.WriteString(PSMFile.HEADER_MAGIC);
                writer.Write(_PSM.Version);
                writer.Write(_PSM.SkinANIM.ToValue());
                writer.Write(_PSM.Parts.Count);
                foreach (SkinBOX part in _PSM.Parts)
                {
                    WritePart(writer, part);
                }
                writer.Write(_PSM.Offsets.Count);
                foreach (SkinPartOffset offset in _PSM.Offsets)
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

        private static PSMParentType GetParentPart(string type)
        {
            switch (type)
            {
                case "HEAD":
                    return PSMParentType.HEAD;
                case "BODY":
                    return PSMParentType.BODY;
                case "ARM0":
                    return PSMParentType.ARM0;
                case "ARM1":
                    return PSMParentType.ARM1;
                case "LEG0":
                    return PSMParentType.LEG0;
                case "LEG1":
                    return PSMParentType.LEG1;
                default:
                    throw new InvalidDataException(type);
            }
        }

        private static PSMOffsetType GetOffsetPart(string type)
        {
            switch (type)
            {
                case "HEAD":
                    return PSMOffsetType.HEAD;
                case "BODY":
                    return PSMOffsetType.BODY;
                case "ARM0":
                    return PSMOffsetType.ARM0;
                case "ARM1":
                    return PSMOffsetType.ARM1;
                case "LEG0":
                    return PSMOffsetType.LEG0;
                case "LEG1":
                    return PSMOffsetType.LEG1;
                case "TOOL0":
                    return PSMOffsetType.TOOL0;
                case "TOOL1":
                    return PSMOffsetType.TOOL1;
                case "HELMET":
                    return PSMOffsetType.HELMET;
                case "SHOULDER0":
                    return PSMOffsetType.SHOULDER0;
                case "SHOULDER1":
                    return PSMOffsetType.SHOULDER1;
                case "CHEST":
                    return PSMOffsetType.CHEST;
                case "WAIST":
                    return PSMOffsetType.WAIST;
                case "PANTS0":
                    return PSMOffsetType.PANTS0;
                case "PANTS1":
                    return PSMOffsetType.PANTS1;
                case "BOOT0":
                    return PSMOffsetType.BOOT0;
                case "BOOT1":
                    return PSMOffsetType.BOOT1;
                default:
                    throw new InvalidDataException(type);
            }
        }
    }
}
