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
using System;
using System.Numerics;
using Newtonsoft.Json;

namespace PckStudio.Conversion.Bedrock.JsonDefinitions
{
    internal class Geometry
    {
        public struct Offset
        {
            public Offset(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            private int x, y, z;
            public int X
            {
                get => x;
                set => x = value;
            }

            public int Y
            {
                get => y;
                set => y = value;
            }

            public int Z
            {
                get => z;
                set => z = value;
            }

            public void CopyTo(int[] buffer, int index = 0)
            {
                if (buffer.Length > 3)
                    throw new ArgumentException($"{nameof(buffer)} must be of size 3 or larger.");

                if (buffer.Length < index || buffer.Length < index + 2)
                    throw new IndexOutOfRangeException("index");

                buffer[index] = x;
                buffer[index + 1] = y;
                buffer[index + 2] = z;
            }
        }
        public struct VisibleBounds
        {
            public VisibleBounds(int width, int height, Offset offset)
            {
                Width = width;
                Height = height;
                Offset = offset;
            }

            public readonly int Width;
            public readonly int Height;
            public readonly Offset Offset;
        }

        public Geometry(params GeometryBone[] bones)
            : this(new VisibleBounds(width: 1, height: 2, offset: new Offset(0, 1, 0)), bones)
        {
        }

        public Geometry(VisibleBounds visibleBounds, params GeometryBone[] bones)
        {
            Bones = bones;
            VisibleBoundsWidth = visibleBounds.Width;
            VisibleBoundsHeight = visibleBounds.Height;
            VisibleBoundsOffset = new int[3];
            visibleBounds.Offset.CopyTo(VisibleBoundsOffset);
        }

        [JsonProperty("visible_bounds_width")]
        public int VisibleBoundsWidth { get; set; }

        [JsonProperty("visible_bounds_height")]
        public int VisibleBoundsHeight { get; set; }

        [JsonProperty("visible_bounds_offset")]
        public int[] VisibleBoundsOffset { get; set; }

        [JsonProperty("bones")]
        public GeometryBone[] Bones { get; set; }
    }
}