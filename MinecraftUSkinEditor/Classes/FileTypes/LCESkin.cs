using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
	internal class LCESkin
	{
		// The effects that can be achieved with an ANIM tag. Each one is set to its respective position in the bitfield
		public enum ANIM_EFFECTS
		{
			// BIT 8
			STATIC_ARMS = 31, ZOMBIE_ARMS = 30, STATIC_LEGS = 29, BAD_SANTA = 28,
			// BIT 7
			SYNCED_ARMS = 26, SYNCED_LEGS = 25, STATUE_OF_LIBERTY = 24,
			// BIT 6
			ARMOR_DISABLED = 23, HEAD_BOBBING_DISABLED = 22, HEAD_DISABLED = 21, RIGHT_ARM_DISABLED = 20,
			// BIT 5
			LEFT_ARM_DISABLED = 19, BODY_DISABLED = 18, RIGHT_LEG_DISABLED = 17, LEFT_LEG_DISABLED = 16,
			// BIT 4
			HEAD_OVERLAY_DISABLED = 15, DO_BACKWARDS_CROUCH = 14, RESOLUTION_64x64 = 13, SLIM_MODEL = 12,
			// BIT 3
			LEFT_ARM_OVERLAY_DISABLED = 11, RIGHT_ARM_OVERLAY_DISABLED = 10, LEFT_LEG_OVERLAY_DISABLED = 09, RIGHT_LEG_OVERLAY_DISABLED = 08,
			// BIT 2
			BODY_OVERLAY_DISABLED = 07,
			// BIT 1
			DINNERBONE = 03
		};

		// the ANIM value represented as a bitfield
		BitArray ANIM_FLAGS = new BitArray(32, false);

		// The flags represented by a string
		public string Bits
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (bool b in ANIM_FLAGS) sb.Append(b ? "1" : "0");
				return sb.ToString();
			}
		}

		// The actual ANIM string. For example: "0x40008"
		public string ANIM
		{
			get
			{
				return Convert.ToInt32(Bits, 2).ToString("X");
			}   // get method
			set
			{
				long temp_long = Convert.ToInt64(value, 16);
				string bits = Convert.ToString(temp_long, 2);
				int bitIndex = 0;
				foreach (char bit in bits)
				{
					ANIM_FLAGS[bitIndex] = bit == '1' ? true : false;
					bitIndex++;
				}
			}  // set method
		}

		public LCESkin()
		{
			// TODO: finish constructor
		}

		// Sets the desired flag in the bitfield to either true or false
		public void SetFlag(ANIM_EFFECTS flagToSet, bool value)
		{
			ANIM_FLAGS[(int)flagToSet] = value;
		}

		// Returns the value of the desired flag in the bitfield
		public bool GetFlag(ANIM_EFFECTS flagToGet)
		{
			return ANIM_FLAGS[(int)flagToGet];
		}
	}
}
