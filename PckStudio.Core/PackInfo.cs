using System;
using OMI.Formats.Pck;

namespace PckStudio.Core
{
    public sealed class PackInfo
    {
        public static readonly PackInfo Empty = new PackInfo(default, default, default);
        public bool IsValid { get; }
        public PckFile File { get; }
        public OMI.Endianness Endianness { get; }

        
        //public enum PackType
        //{
        //    Unknown = -1,
        //    SkinPack,
        //    TexturePack,
        //    MashUpPack
        //}

        //public PackType Type { get; }

        public bool AllowEndianSwap { get; }

        public static PackInfo Create(PckFile file, OMI.Endianness endianness, bool allowEndianSwap)
        {
            return new PackInfo(file, endianness, allowEndianSwap);
        }

        private PackInfo(PckFile file, OMI.Endianness endianness, bool allowEndianSwap)
        {
            File = file;
            Endianness = endianness;
            AllowEndianSwap = allowEndianSwap;
            //Type = GetPackType();
            IsValid = file is not null && Enum.IsDefined(typeof(OMI.Endianness), endianness); // && Type != PackType.Unknown;
        }

        //private PackType GetPackType()
        //{
        //    return PackType.SkinPack;
        //}
    }
}