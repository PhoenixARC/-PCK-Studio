using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

	internal class SkinANIM
	{
		eANIM_EFFECTS _ANIM;
		public bool isValid;

		public SkinANIM()
		{
			_ANIM = 0;
		}

		public SkinANIM(string anim)
		{
			// Port of my ANIM Generator found at https://mattnl.com/lce/anim-generator - MattNL
			if (anim.StartsWith("0x")) anim = anim.Substring(2);
			isValid = anim.Length <= 8 && Regex.IsMatch(anim, @"\A\b[0-9a-fA-F]+\b\Z");
			if (isValid)
			{
				anim = anim.PadLeft(8, '0');
				string bits = String.Join("", anim.Select(
					b => Convert.ToString(Convert.ToInt32(b.ToString(), 16), 2).PadLeft(4, '0')
				  )
				);
				int current_bit = 31;
				foreach (char bit in bits)
				{
					SetANIMFlag((eANIM_EFFECTS)(1 << current_bit), bit == '1');
					current_bit--;
				}
			}
		}

		public SkinANIM(eANIM_EFFECTS anim)
		{
			_ANIM = anim;
		}

		public override string ToString()
		{
			// Thanks miku :D - MattNL
			return "0x" + ((int)_ANIM).ToString("x08");
		}

		/// <summary>
		/// Sets the desired flag in the bitfield
		/// </summary>
		/// <param name="flag">ANIM Flag to be set</param>
		/// <param name="state">wether to enable the flag</param>
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
			return ((int)_ANIM & (int)flag) != 0;
		}
	}
}
