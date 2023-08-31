﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Internal
{
    [Flags]
    public enum SkinAnimMask : int
    {
        NONE                        = 0,       // 0x00
        STATIC_ARMS                 = 1 << 0,  // 0x01
        ZOMBIE_ARMS                 = 1 << 1,  // 0x02
        STATIC_LEGS                 = 1 << 2,  // 0x04
        BAD_SANTA                   = 1 << 3,  // 0x08

        __BIT_4                     = 1 << 4,  // 0x10 - Unused??
        SYNCED_LEGS                 = 1 << 5,  // 0x20
        SYNCED_ARMS                 = 1 << 6,  // 0x40
        STATUE_OF_LIBERTY           = 1 << 7,  // 0x80

        ALL_ARMOR_DISABLED          = 1 << 8,  // 0x100
        HEAD_BOBBING_DISABLED       = 1 << 9,  // 0x200
        HEAD_DISABLED               = 1 << 10, // 0x400
        RIGHT_ARM_DISABLED          = 1 << 11, // 0x800

        LEFT_ARM_DISABLED           = 1 << 12, // 0x1000
        BODY_DISABLED               = 1 << 13, // 0x2000
        RIGHT_LEG_DISABLED          = 1 << 14, // 0x4000
        LEFT_LEG_DISABLED           = 1 << 15, // 0x8000

        HEAD_OVERLAY_DISABLED       = 1 << 16, // 0x10000
        DO_BACKWARDS_CROUCH         = 1 << 17, // 0x20000
        RESOLUTION_64x64            = 1 << 18, // 0x40000
        SLIM_MODEL                  = 1 << 19, // 0x80000

        LEFT_ARM_OVERLAY_DISABLED   = 1 << 20, // 0x100000
        RIGHT_ARM_OVERLAY_DISABLED  = 1 << 21, // 0x200000
        LEFT_LEG_OVERLAY_DISABLED   = 1 << 22, // 0x400000
        RIGHT_LEG_OVERLAY_DISABLED  = 1 << 23, // 0x800000

        BODY_OVERLAY_DISABLED       = 1 << 24, // 0x1000000
        FORCE_HEAD_ARMOR            = 1 << 25, // 0x2000000
        FORCE_RIGHT_ARM_ARMOR       = 1 << 26, // 0x4000000
        FORCE_LEFT_ARM_ARMOR        = 1 << 27, // 0x8000000

        FORCE_BODY_ARMOR            = 1 << 28, // 0x10000000
        FORCE_RIGHT_LEG_ARMOR       = 1 << 29, // 0x20000000
        FORCE_LEFT_LEG_ARMOR        = 1 << 30, // 0x40000000
        DINNERBONE                  = 1 << 31, // 0x80000000
    }
}
