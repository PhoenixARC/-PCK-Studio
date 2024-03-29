using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PckStudio.Internal.FileFormats
{
    #region File Structure
/*
	Magic - 8 bytes[uint64]
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
    #endregion
    class CSMBFile
    {
		internal static readonly string HEADER_MAGIC = "psm";

        public readonly byte Version;

        internal CSMBFile(byte version)
        {
            Version = version;
        }

        internal CSMBFile(byte version, SkinANIM skinANIM)
			: this(version)
        {
			SkinANIM = skinANIM;
        }

		public SkinANIM SkinANIM { get; private set; }
		public readonly List<SkinBOX> Parts = new List<SkinBOX>();
		public readonly List<SkinPartOffset> Offsets = new List<SkinPartOffset>();

		public static CSMBFile FromSkin(Skin skin)
		{
			var csmb = new CSMBFile(1);
			csmb.SkinANIM = skin.ANIM;
			csmb.Parts.AddRange(skin.AdditionalBoxes);
			csmb.Offsets.AddRange(skin.PartOffsets);
			return csmb;
		}
	}

	public enum CSMBOffsetType : byte
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

	public enum CSMBParentType : byte
	{
		HEAD = 0,
		BODY = 1,
		ARM0 = 2,
		ARM1 = 3,
		LEG0 = 4,
		LEG1 = 5,
	}
}
