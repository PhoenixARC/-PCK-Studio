/* Copyright (c) 2025-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Drawing;

namespace PckStudio.Core
{
    public abstract class AtlasGroup
    {
        public string Name { get; set; }
        public int Row { get; }
        public int Column { get; }

        public virtual int Count => 1;

        protected abstract bool isAnimation { get; }
        protected abstract bool isLargeTile { get; }
        public virtual ImageLayoutDirection Direction => Column > Row ? ImageLayoutDirection.Vertical : ImageLayoutDirection.Horizontal;

        public AtlasGroup(string name, int row, int column)
        {
            Name = name;
            Row = Math.Max(0, row);
            Column = Math.Max(0, column);
        }

        public bool IsAnimation() => isAnimation;
        public bool IsLargeTile() => isLargeTile;

        public abstract Size GetSize(Size tileSize);

    }
}