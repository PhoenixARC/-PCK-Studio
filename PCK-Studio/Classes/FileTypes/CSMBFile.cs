using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.FileFormats
{
    #region File Structure
    /*
	Version - 4 bytes[int32]
	NumberOfParts - 4 bytes[int32]
	{
		Part name length - 2 bytes[int16]
		part name - x bytes
		part parent - 4 bytes[int32] (HEAD=1, BODY=2, LEG0=3, LEG1=4, ARM0=5, ARM1=6)
		Position-X - 4 bytes[float]
		Position-Y - 4 bytes[float]
		Position-Z - 4 bytes[float]
		Size-X - 4 bytes[float]
		Size-Y - 4 bytes[float]
		Size-Z - 4 bytes[float]
		UV-Y - 4 bytes[int32]
		UV-X - 4 bytes[int32]
		mirror texture - 1 byte[bool]
		Hide with armour - 1 byte[bool]
		inflation/scale value - 4 bytes[float]
	}
	NumberOfOffsets - 4 bytes[int32]
	{
		offset part - 4 bytes[int32]
		vertical offset - 4 bytes[float]
	}
	 */
    #endregion
    class CSMBFile
    {
		public List<CSMBPart> Parts = new List<CSMBPart>();
		public List<CSMBOffset> Offsets = new List<CSMBOffset>();
    }

	public class CSMBPart
    {
		public string Name = "Partname";
		public CSMBParentPart Parent = 0;
		public float posX, posY, posZ = 0.0f;
		public float sizeX, sizeY, sizeZ = 0.0f;
		public int uvX, uvY = 0;
		public bool HideWArmour, MirrorTexture = false;
		public float Inflation = 0.0f;
	}
	public class CSMBOffset
    {
		public CSMBOffsetPart offsetPart = 0;
		public float VerticalOffset = 0.0f;
	}

	public enum CSMBOffsetPart
	{
		HEAD = 0,
		BODY = 1,
		ARM0 = 2,
		ARM1 = 3,
		LEG0 = 4,
		LEG1 = 5,
		HEADWEAR = 6,
		JACKET = 7,
		SLEEVE0 = 8,
		SLEEVE1 = 9,
		PANTS0 = 10,
		PANTS1 = 11,
		WAIST = 12,
		LEGGING0 = 13,
		LEGGING1 = 14,
		SOCK0 = 15,
		SOCK1 = 16,
		BOOT0 = 17,
		BOOT1 = 18,
		ARMARMOR1 = 19,
		ARMARMOR0 = 20,
		BODYARMOR = 21,
		BELT = 22,
		TOOL0 = 23,
		TOOL1 = 24,
		HELMET = 25,
		SHOULDER0 = 26,
		SHOULDER1 = 27,
		CHEST = 28
	}

	public enum CSMBParentPart
	{
		HEAD = 0,
		BODY = 1,
		ARM0 = 2,
		ARM1 = 3,
		LEG0 = 4,
		LEG1 = 5,
	}
}