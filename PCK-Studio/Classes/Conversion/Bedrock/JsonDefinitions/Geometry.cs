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
using Newtonsoft.Json;

namespace PckStudio.Classes.Conversion.Bedrock.JsonDefinitions
{
    internal class Geometry
    {
        public Geometry((int Width, int Height, int[] Offset) visibleBounds, params GeometryBone[] bones)
        {
            Bones = bones;
            VisibleBoundsWidth = visibleBounds.Width;
            VisibleBoundsHeight = visibleBounds.Height;
            VisibleBoundsOffset = visibleBounds.Offset;
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
