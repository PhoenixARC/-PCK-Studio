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
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PckStudio.Core
{
    internal sealed class AtlasAnimation : AtlasGroup
    {
        [JsonProperty("frameCount", Required = Required.Always)]
        private int _frameCount;

        [JsonProperty("frameTime")]
        public int FrameTime { get; } = Animation.MINIMUM_FRAME_TIME;

        public override int Count => _frameCount;

        [JsonProperty("direction")]
        public override ImageLayoutDirection Direction { get; }

        [JsonIgnore]
        protected override bool isAnimation => true;

        [JsonIgnore]
        protected override bool isLargeTile => false;

        protected override bool isComposedOfMultipleTiles => false;

        public override Size GetSize(Size tileSize) => new Size(tileSize.Width * (Direction == ImageLayoutDirection.Horizontal ? Count : 1), tileSize.Height * (Direction == ImageLayoutDirection.Vertical ? Count : 1));

        public AtlasAnimation(string name, int row, int column, int frameCount, ImageLayoutDirection direction, int frameTime = Animation.MINIMUM_FRAME_TIME, bool allowCustomColor = default)
            : base(name, row, column, allowCustomColor)
        {
            _frameCount = frameCount;
            Direction = direction;
            FrameTime = frameTime;
        }
    }
}