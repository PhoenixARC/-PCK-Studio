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

namespace PckStudio.Internal
{
    /// <summary>
    /// For usage see <see cref="SkinANIM"/>
    /// </summary>
	public enum SkinAnimFlag : int
	{
		STATIC_ARMS           = 0,  // 0x01
		ZOMBIE_ARMS           = 1,  // 0x02
		STATIC_LEGS           = 2,  // 0x04
		BAD_SANTA             = 3,  // 0x08
										 //
		__BIT_4               = 4,  // 0x10 - Unused??
		SYNCED_LEGS           = 5,  // 0x20
        SYNCED_ARMS           = 6,  // 0x40
        STATUE_OF_LIBERTY     = 7,  // 0x80

        ALL_ARMOR_DISABLED    = 8,  // 0x100
		HEAD_BOBBING_DISABLED = 9,  // 0x200
		HEAD_DISABLED         = 10, // 0x400
		RIGHT_ARM_DISABLED    = 11, // 0x800

        LEFT_ARM_DISABLED     = 12, // 0x1000
		BODY_DISABLED         = 13, // 0x2000
		RIGHT_LEG_DISABLED    = 14, // 0x4000
		LEFT_LEG_DISABLED     = 15, // 0x8000

        HEAD_OVERLAY_DISABLED = 16, // 0x10000
		DO_BACKWARDS_CROUCH   = 17, // 0x20000
		RESOLUTION_64x64      = 18, // 0x40000
		SLIM_MODEL            = 19, // 0x80000

        LEFT_ARM_OVERLAY_DISABLED  = 20, // 0x100000
		RIGHT_ARM_OVERLAY_DISABLED = 21, // 0x200000
		LEFT_LEG_OVERLAY_DISABLED  = 22, // 0x400000
		RIGHT_LEG_OVERLAY_DISABLED = 23, // 0x800000

        BODY_OVERLAY_DISABLED = 24, // 0x1000000
		FORCE_HEAD_ARMOR      = 25, // 0x2000000
		FORCE_RIGHT_ARM_ARMOR = 26, // 0x4000000
		FORCE_LEFT_ARM_ARMOR  = 27, // 0x8000000

        FORCE_BODY_ARMOR      = 28, // 0x10000000
		FORCE_RIGHT_LEG_ARMOR = 29, // 0x20000000
		FORCE_LEFT_LEG_ARMOR  = 30, // 0x40000000
		DINNERBONE            = 31, // 0x80000000
    }
}
