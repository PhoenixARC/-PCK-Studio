/* Copyright (c) 2023-present miku-666
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

namespace PckStudio.Core
{
    [Flags]
    public enum ResourceCategory : int
    {
        Unknown                 = -1,
        Animation               = (1 << 28),
        ItemAnimation           = 1 | Animation,
        BlockAnimation          = 2 | Animation,

        Atlas                   = (2 << 28),

        Textures                = (3 << 28),
        MobEntityTextures       = 1  | Textures,
        ItemEntityTextures      = 2 | Textures,
        ArmorTextures           = 3 | Textures,
    }
}
