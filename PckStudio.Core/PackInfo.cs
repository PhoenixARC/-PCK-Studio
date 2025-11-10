using System;
using OMI.Formats.Pck;

namespace PckStudio.Core
{
    public sealed class PackInfo
    {
        public static readonly PackInfo Empty = new PackInfo(default, default, default);
        public bool IsValid { get; }
        public PckFile File { get; }
        public OMI.ByteOrder ByteOrder { get; }

        public bool AllowByteOrderSwap { get; }

        public static PackInfo Create(PckFile file, OMI.ByteOrder byteOrder, bool allowByteOrderSwap)
        {
            return new PackInfo(file, byteOrder, allowByteOrderSwap);
        }

        private PackInfo(PckFile file, OMI.ByteOrder byteOrder, bool allowByteOrderSwap)
        {
            File = file;
            ByteOrder = byteOrder;
            AllowByteOrderSwap = allowByteOrderSwap;
            IsValid = file is not null && Enum.IsDefined(typeof(OMI.ByteOrder), byteOrder);
        }
    }
}