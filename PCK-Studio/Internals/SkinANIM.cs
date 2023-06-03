/* Copyright (c) 2022-present miku-666, MattNL
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Text.RegularExpressions;

namespace PckStudio.Internal
{
	/// <summary>
	/// For usage see <see cref="SkinANIM"/>
	/// </summary>
	[Flags]
	public enum ANIM_EFFECTS : int
	{
		NONE                  = 0,       // 0x00
		STATIC_ARMS           = 1 << 0,  // 0x01
		ZOMBIE_ARMS           = 1 << 1,  // 0x02
		STATIC_LEGS           = 1 << 2,  // 0x04
		BAD_SANTA             = 1 << 3,  // 0x08
										 //
		__BIT_4               = 1 << 4,  // 0x10 - Unused??
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

	public class SkinANIM : ICloneable, IEquatable<SkinANIM>
    {
		private ANIM_EFFECTS _ANIM;
		public static readonly Regex animRegex = new Regex(@"^0x[0-9a-f]{1,8}\b", RegexOptions.IgnoreCase);

		public static readonly SkinANIM Empty = new SkinANIM();

		public SkinANIM()
			: this(ANIM_EFFECTS.NONE)
		{
		}

		public SkinANIM(ANIM_EFFECTS anim)
		{
			_ANIM = anim;
		}

		public override string ToString() => "0x" + ((int)_ANIM).ToString("x8");

		public static bool IsValidANIM(string anim) => animRegex.IsMatch(anim ?? string.Empty);

		public static SkinANIM FromString(string value)
			=> IsValidANIM(value)
				? new SkinANIM((ANIM_EFFECTS)Convert.ToInt32(value.TrimEnd(' ', '\n', '\r'), 16))
				: new SkinANIM();

		public void SetANIM(ANIM_EFFECTS anim) => _ANIM = anim;

		public static SkinANIM operator |(SkinANIM a, SkinANIM b) => new SkinANIM(a._ANIM | b._ANIM);
		
		public static SkinANIM operator |(SkinANIM a, ANIM_EFFECTS anim) => new SkinANIM(a._ANIM | anim);

		public static implicit operator SkinANIM(ANIM_EFFECTS anim) => new SkinANIM(anim);

		public static bool operator ==(SkinANIM a, ANIM_EFFECTS b) => a._ANIM == b;		
		public static bool operator !=(SkinANIM a, ANIM_EFFECTS b) => !(a == b);
		public static bool operator ==(SkinANIM a, SkinANIM b) => a.Equals(b);
		public static bool operator !=(SkinANIM a, SkinANIM b) => !a.Equals(b);

        public bool Equals(SkinANIM other)
        {
            return _ANIM == other._ANIM;
        }

		public override bool Equals(object obj) => obj is SkinANIM a && Equals(a);

		public override int GetHashCode() => (int)_ANIM;
		
		/// <summary>
		/// Sets the desired flag in the bitfield
		/// </summary>
		/// <param name="flag">ANIM Flag to set</param>
		/// <param name="state">State of the flag</param>
		public void SetFlag(ANIM_EFFECTS flag, bool state)
		{
			if (state) _ANIM |= flag;
			else _ANIM &= ~flag;
		}

		/// <summary>
		/// Gets a desired flags state
		/// </summary>
		/// <param name="flag">Flag to check</param>
		/// <returns>True if flag is set, otherwise false</returns>
		public bool GetFlag(ANIM_EFFECTS flag)
		{
			return (_ANIM & flag) != 0;
		}

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
