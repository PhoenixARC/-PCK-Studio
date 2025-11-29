using System;
using System.Collections.Generic;
using PckStudio.Core.Skin;

namespace PckStudio.ModelSupport.Internal.Format
{
/*
	Magic - 3 bytes("psm")
	Version - 1 byte [u8]
	Anim - 4 bytes[int32]
	NumberOfParts - 4 bytes[int32]
	{
		part parent - 1 byte (HEAD=0, BODY=1, LEG0=2, LEG1=3, ARM0=4, ARM1=5)
		Position-X - 4 bytes (float32)
		Position-Y - 4 bytes (float32)
		Position-Z - 4 bytes (float32)
		Size-X - 4 bytes (float32)
		Size-Y - 4 bytes (float32)
		Size-Z - 4 bytes (float32)
		MirrorAndUvX - 1 bit flag 7 bits uv.x value(0-64) (s8)
		HideWithArmorAndUvY - 1 bit flag 7 bits uv.y value(0-64) (s8)
		inflation/scale value - 4 bytes (float32)
	}
	NumberOfOffsets - 4 bytes[int32]
	{
		offset part - 1 byte 
		vertical offset - 4 bytes[float]
	}
*/
    public sealed class PSMFile
    {
		internal static readonly string HEADER_MAGIC = "psm";
		internal const byte CURRENT_VERSION = 2;

        public readonly byte Version;

        internal PSMFile(byte version)
        {
            Version = version;
        }

        internal PSMFile(byte version, SkinANIM skinANIM)
			: this(version)
        {
			SkinANIM = skinANIM;
        }

		public SkinANIM SkinANIM { get; private set; }

        public readonly List<SkinBOX> Parts = new List<SkinBOX>();
		public readonly List<SkinPartOffset> Offsets = new List<SkinPartOffset>();
	}

	public enum PSMOffsetType : byte
    {
		HEAD = 0,
        BODY = 1,
        ARM0 = 2,
        ARM1 = 3,
        LEG0 = 4,
        LEG1 = 5,

        TOOL0 = 6,
        TOOL1 = 7,

        HELMET = 8,
        SHOULDER0 = 9,
        SHOULDER1 = 10,
        CHEST = 11,
        WAIST = 12,
        PANTS0 = 13,
        PANTS1 = 14,
        BOOT0 = 15,
        BOOT1 = 16,
    }

	public enum PSMParentType : byte
	{
		HEAD = 0,
		BODY = 1,
		ARM0 = 2,
		ARM1 = 3,
		LEG0 = 4,
		LEG1 = 5,
	}
}
