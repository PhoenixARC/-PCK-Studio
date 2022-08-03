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
		STATIC_ARMS = 1 << 0,
		ZOMBIE_ARMS = 1 << 1,
		STATIC_LEGS = 1 << 2,
		BAD_SANTA = 1 << 3,

		unk_BIT4 = 1 << 4,  // Whatever effect this is should be a simple one as it's existed for a while
		SYNCED_LEGS = 1 << 5,
		SYNCED_ARMS = 1 << 6,
		STATUE_OF_LIBERTY = 1 << 7,

		ALL_ARMOR_DISABLED = 1 << 8,
		HEAD_BOBBING_DISABLED = 1 << 9,
		HEAD_DISABLED = 1 << 10,
		RIGHT_ARM_DISABLED = 1 << 11,

		LEFT_ARM_DISABLED = 1 << 12,
		BODY_DISABLED = 1 << 13,
		RIGHT_LEG_DISABLED = 1 << 14,
		LEFT_LEG_DISABLED = 1 << 15,

		HEAD_OVERLAY_DISABLED = 1 << 16,
		DO_BACKWARDS_CROUCH = 1 << 17,
		RESOLUTION_64x64 = 1 << 18,
		SLIM_MODEL = 1 << 19,

		LEFT_ARM_OVERLAY_DISABLED = 1 << 20,
		RIGHT_ARM_OVERLAY_DISABLED = 1 << 21,
		LEFT_LEG_OVERLAY_DISABLED = 1 << 22,
		RIGHT_LEG_OVERLAY_DISABLED = 1 << 23,

		BODY_OVERLAY_DISABLED = 1 << 24,
		FORCE_HEAD_ARMOR = 1 << 25,
		FORCE_RIGHT_ARM_ARMOR = 1 << 26,
		FORCE_LEFT_ARM_ARMOR = 1 << 27,

		FORCE_BODY_ARMOR = 1 << 28,
		FORCE_RIGHT_LEG_ARMOR = 1 << 29,
		FORCE_LEFT_LEG_ARMOR = 1 << 30,
		DINNERBONE = 1 << 31,
	}

	internal struct SkinANIM
	{
		eANIM_EFFECTS _ANIM = 0;
		static readonly Regex animRegex = new Regex(@"^0x[0-9a-f]{1,8}\b", RegexOptions.IgnoreCase);

		public SkinANIM(string anim)
		{
			_ANIM = Parse(anim);
		}

		public SkinANIM(eANIM_EFFECTS anim)
		{
			_ANIM = anim;
		}

		public override string ToString() => "0x" + _ANIM.ToString("x");

		public static bool IsValidANIM(string anim) => animRegex.IsMatch(anim);

		public static eANIM_EFFECTS Parse(string anim)
			=> IsValidANIM(anim)
				? (eANIM_EFFECTS)Convert.ToInt32(anim, 16)
				: 0;

		public void SetANIM(int anim) => SetANIM((eANIM_EFFECTS)anim);
		public void SetANIM(eANIM_EFFECTS anim) => _ANIM = anim;

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
	}
}
