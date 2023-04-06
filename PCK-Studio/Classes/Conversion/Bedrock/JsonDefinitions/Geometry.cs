/* Copyright (c) 2023-present miku-666
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
using System.Numerics;
using Newtonsoft.Json;

namespace PckStudio.Conversion.Bedrock.JsonDefinitions
{
    internal class Geometry
    {
        public struct VisibleBounds
        {
            public VisibleBounds(int width, int height, int[] offset)
            {
                Width = width;
                Height = height;
                Offset = new Vector<int>(offset,offset.Length-3);
            }

            public readonly int Width;
            public readonly int Height;
            public readonly Vector<int> Offset;
        }

        public Geometry(params GeometryBone[] bones)
            : this(new VisibleBounds(width: 1, height: 2, offset: new int[3] { 0, 1, 0 }), bones)
        {
        }

        public Geometry(VisibleBounds visibleBounds, params GeometryBone[] bones)
        {
            Bones = bones;
            VisibleBoundsWidth = visibleBounds.Width;
            VisibleBoundsHeight = visibleBounds.Height;
            visibleBounds.Offset.CopyTo(VisibleBoundsOffset);
        }

        [JsonProperty("visible_bounds_width")]
        public int VisibleBoundsWidth = 1;

        [JsonProperty("visible_bounds_height")]
        public int VisibleBoundsHeight = 2;

        [JsonProperty("visible_bounds_offset")]
        public int[] VisibleBoundsOffset = { 0, 1, 0 };

        [JsonProperty("bones")]
        public GeometryBone[] Bones { get; set; }
    }
}