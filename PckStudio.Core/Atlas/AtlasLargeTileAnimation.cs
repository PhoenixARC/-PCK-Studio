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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace PckStudio.Core
{
    internal sealed class AtlasLargeTileAnimation : AtlasGroup
    {
        [JsonIgnore]
        private AtlasLargeTile[] _largeTiles;

        [JsonProperty("frameCount")]
        private int _frameCount;

        [JsonProperty("rowSpan")]
        public int RowSpan { get; }
        [JsonProperty("columnSpan")]
        public int ColumnSpan { get; }
        [JsonProperty("frameTime")]
        public int FrameTime { get; }

        [JsonIgnore]
        public override int Count => RowSpan * ColumnSpan * _frameCount;

        [JsonProperty("direction")]
        public override ImageLayoutDirection Direction { get; }

        [JsonIgnore]
        protected override bool isAnimation => true;

        [JsonIgnore]
        protected override bool isLargeTile => true;

        protected override bool isComposedOfMultipleTiles => false;

        public override Size GetSize(Size tileSize) => new Size(RowSpan * tileSize.Width * (Direction == ImageLayoutDirection.Horizontal ? _frameCount : 1), ColumnSpan * tileSize.Height * (Direction == ImageLayoutDirection.Vertical ? _frameCount : 1));

        public AtlasLargeTileAnimation(string name, int row, int column, int rowSpan, int columnSpan, int frameCount, ImageLayoutDirection direction, int frameTime = Animation.MINIMUM_FRAME_TIME, bool allowCustomColor = default)
            : base(name, row, column, allowCustomColor)
        {
            _frameCount = Math.Abs(frameCount);
            RowSpan = Math.Max(1, rowSpan);
            ColumnSpan = Math.Max(1, columnSpan);
            Direction = direction;
            FrameTime = frameTime;
            _largeTiles = InternalGetLargeTiles().ToArray();
        }

        private IEnumerable<AtlasLargeTile> InternalGetLargeTiles()
        {
            for (int i = 0; i < _frameCount; i++)
            {
                yield return new AtlasLargeTile($"{Name}_{i}", Row + (Direction == ImageLayoutDirection.Horizontal ? i * RowSpan : 0), Column + (Direction == ImageLayoutDirection.Vertical ? i * ColumnSpan : 0), RowSpan, ColumnSpan, AllowCustomColor);
            }
            yield break;
        }

        internal AtlasLargeTile GetTile(int row, int col)
        {
            if (!IsInRange(row, col))
                return default;
            int i = (Direction == ImageLayoutDirection.Horizontal) ? Math.DivRem((row - Row), RowSpan, out _) : Math.DivRem((col - Column), ColumnSpan, out _);
            return (i >= 0 && i < _frameCount) ? _largeTiles[i] : _largeTiles[0];
        }

        private bool IsInRange(int row, int col)
        {
            return Row <= row && row < (Row + (RowSpan * (Direction == ImageLayoutDirection.Horizontal ? _frameCount : 1))) && Column <= col && col < (Column + (ColumnSpan * (Direction == ImageLayoutDirection.Horizontal ? _frameCount : 1)));
        }

        internal IEnumerable<AtlasLargeTile> GetLargeTiles() => _largeTiles;
    }
}