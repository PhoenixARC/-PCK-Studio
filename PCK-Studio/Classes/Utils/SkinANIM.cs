using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PckStudio.Classes.Utils
{
	[Flags]
	public enum eANIM_EFFECTS : int
	{
		NONE                  = 0,       // 0x00
		STATIC_ARMS           = 1 << 0,  // 0x01
		ZOMBIE_ARMS           = 1 << 1,  // 0x02
		STATIC_LEGS           = 1 << 2,  // 0x04
		BAD_SANTA             = 1 << 3,  // 0x08

		// Whatever effect this is should be a simple one as it's existed for a while
        unk_BIT4              = 1 << 4,  // 0x10
        SYNCED_LEGS           = 1 << 5,  // 0x20
        SYNCED_ARMS           = 1 << 6,  // 0x40
        STATUE_OF_LIBERTY     = 1 << 7,  // 0x80

        ALL_ARMOR_DISABLED    = 1 << 8,  // 0x100
		HEAD_BOBBING_DISABLED = 1 << 9,  // 0x200
		HEAD_DISABLED         = 1 << 10, // 0x400
		RIGHT_ARM_DISABLED    = 1 << 11, // 0x800

        LEFT_ARM_DISABLED     = 1 << 12, // 0x1000
		BODY_DISABLED         = 1 << 13, // 0x2000
		RIGHT_LEG_DISABLED    = 1 << 14, // 0x4000
		LEFT_LEG_DISABLED     = 1 << 15, // 0x8000

        HEAD_OVERLAY_DISABLED = 1 << 16, // 0x10000
		DO_BACKWARDS_CROUCH   = 1 << 17, // 0x20000
		RESOLUTION_64x64      = 1 << 18, // 0x40000
		SLIM_MODEL            = 1 << 19, // 0x80000

        LEFT_ARM_OVERLAY_DISABLED  = 1 << 20, // 0x100000
		RIGHT_ARM_OVERLAY_DISABLED = 1 << 21, // 0x200000
		LEFT_LEG_OVERLAY_DISABLED  = 1 << 22, // 0x400000
		RIGHT_LEG_OVERLAY_DISABLED = 1 << 23, // 0x800000

        BODY_OVERLAY_DISABLED = 1 << 24, // 0x1000000
		FORCE_HEAD_ARMOR      = 1 << 25, // 0x2000000
		FORCE_RIGHT_ARM_ARMOR = 1 << 26, // 0x4000000
		FORCE_LEFT_ARM_ARMOR  = 1 << 27, // 0x8000000

        FORCE_BODY_ARMOR      = 1 << 28, // 0x10000000
		FORCE_RIGHT_LEG_ARMOR = 1 << 29, // 0x20000000
		FORCE_LEFT_LEG_ARMOR  = 1 << 30, // 0x40000000
		DINNERBONE            = 1 << 31, // 0x80000000
    }

	public struct SkinANIM
	{
		eANIM_EFFECTS _ANIM = 0;
		public static readonly Regex animRegex = new Regex(@"^0x[0-9a-f]{1,8}\b", RegexOptions.IgnoreCase);

		public SkinANIM(string anim)
		{
			_ANIM = Parse(anim);
		}

		public SkinANIM(eANIM_EFFECTS anim)
		{
			_ANIM = anim;
		}

		public override string ToString() => "0x" + ((int)_ANIM).ToString("x8");

		public static bool IsValidANIM(string anim) => animRegex.IsMatch(anim);

		public static eANIM_EFFECTS Parse(string anim)
			=> IsValidANIM(anim)
				? (eANIM_EFFECTS)Convert.ToInt32(anim.TrimEnd(' ', '\n', '\r'), 16)
				: eANIM_EFFECTS.NONE;

		public void SetANIM(int anim) => SetANIM((eANIM_EFFECTS)anim);
		public void SetANIM(eANIM_EFFECTS anim) => _ANIM = anim;

		public static SkinANIM operator |(SkinANIM a, SkinANIM b) => new SkinANIM(a._ANIM | b._ANIM);
		public static SkinANIM operator |(SkinANIM a, eANIM_EFFECTS anim) => new SkinANIM(a._ANIM | anim);
		public static implicit operator SkinANIM(eANIM_EFFECTS anim) => new SkinANIM(anim);

		public static bool operator ==(SkinANIM a, eANIM_EFFECTS b)
		{
			return a._ANIM == b;
		}
		
		public static bool operator !=(SkinANIM a, eANIM_EFFECTS b)
		{
			return !(a == b);
		}


		/// <summary>
		/// Sets the desired flag in the bitfield
		/// </summary>
		/// <param name="flag">ANIM Flag to set</param>
		/// <param name="state">Wether to enable the flag</param>
		public void SetANIMFlag(eANIM_EFFECTS flag, bool state)
		{
			if (state) _ANIM |= flag;
			else _ANIM &= ~flag;
		}

		/// <summary>
		/// Returns true if the desired flag is set in the bitfield, otherwise false
		/// </summary>
		/// <param name="flag">ANIM Flag to check</param>
		/// <returns>Bool wether its set or not</returns>
		public bool GetANIMFlag(eANIM_EFFECTS flag)
		{
			return (_ANIM & flag) != 0;
		}

		public override bool Equals(object obj)
		{
			return obj is SkinANIM a && _ANIM == a._ANIM;
		}

		public override int GetHashCode()
		{
			return (int)_ANIM;
		}
	}
}
