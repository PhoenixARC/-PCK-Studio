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
using PckStudio.Core.Extensions;

namespace PckStudio.Core
{
    public sealed class Atlas
    {
        public string Name { get; set; }
        public int Rows { get; }
        public int Columns { get; }

        public Size TileSize { get; }
        public int TileCount => _tiles.Length;

        private readonly AtlasTile[] _tiles;
        private readonly ImageLayoutDirection _layoutDirection;
        private readonly List<AtlasGroup> _groups;

        public static implicit operator Image(Atlas atlas) => atlas.BuildFinalImage();
        
        private Atlas(string name, int rows, int columns)
        {
            Name = name;
            Rows = rows;
            Columns = columns;
            _tiles = new AtlasTile[rows * columns];
            _groups = new List<AtlasGroup>();
        }

        private Atlas(string name, int rows, int columns, IEnumerable<AtlasTile> tiles, Size tileSize, ImageLayoutDirection layoutDirection)
            : this(name, rows, columns)
        {
            _tiles = tiles.Take(rows * columns).ToArray();
            TileSize = tileSize;
            _layoutDirection = layoutDirection;
        }

        public static Atlas FromResourceLocation(Image source, ResourceLocation resourceLocation, ImageLayoutDirection imageLayout = default)
        {
            Json.JsonTileInfo[] tilesInfo = resourceLocation.TilesInfo.ToArray();
            Size tileArea = resourceLocation.GetTileArea(source.Size);
            int rows = source.Width / tileArea.Width;
            int columns = source.Height / tileArea.Height;
            IEnumerable<AtlasTile> tiles = source.Split(tileArea, imageLayout).enumerate().Select(((int index, Image img) data) => new AtlasTile(data.img, GetSelectedPoint(data.index, out int col, rows, columns, imageLayout), col, index: data.index, userData: tilesInfo.IndexInRange(data.index) ? tilesInfo[data.index] : default));
            var atlas = new Atlas(resourceLocation.Path, rows, columns, tiles, tileArea, imageLayout);
            atlas.AddGroups(resourceLocation.AtlasGroups);
            return atlas;
        }

        private static int GetSelectedPoint(int index, out int col, int rows, int columns, ImageLayoutDirection layoutDirection)
        {
            int y = Math.DivRem(index, rows, out int x);
            if (layoutDirection == ImageLayoutDirection.Vertical)
                x = Math.DivRem(index, columns, out y);
            col = y;
            return x;
        }

        public void AddGroups(IEnumerable<AtlasGroup> groups)
        {
            foreach (AtlasGroup group in groups)
            {
                AddGroup(group);
            }
        }

        public AtlasTile this[int row, int col]
        {
            get => this[(col * Rows) + row];
            set => this[(col * Rows) + row] = value;
        }

        public AtlasTile this[int index]
        {
            get => _tiles.IndexInRange(index) ? _tiles[index] : throw new IndexOutOfRangeException(index.ToString());
            set
            {
                if (_tiles.IndexInRange(index))
                    _tiles[index] = value;
            }
        }

        public IEnumerable<AtlasTile> GetRange(int row, int col, int count, ImageLayoutDirection direction)
        {
            return GetRange(row, col, direction == ImageLayoutDirection.Horizontal ? count : 1, direction == ImageLayoutDirection.Vertical ? count : 1);
        }

        public IEnumerable<AtlasTile> GetRange(int row, int col, int rowCount, int columnCount)
        {
            for (int j = 0; j < columnCount; j++)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    if ((row + i) < Rows && (col + j) < Columns)
                    {
                        yield return this[row + i, col + j];
                    }
                }
            }
            yield break;
        }

        private void SetRange(int row, int col, int count, ImageLayoutDirection direction, IEnumerable<Image> tiles)
            => SetRange(row, col, direction == ImageLayoutDirection.Horizontal ? count : 1, direction == ImageLayoutDirection.Vertical ? count : 1, tiles);
        private void SetRange(int row, int col, int rowCount, int columnCount, IEnumerable<Image> tiles)
        {
            Image[] atlasTiles = tiles.ToArray();
            for (int j = 0; j < columnCount; j++)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    if ((row + i) < Rows && (col + j) < Columns)
                    {
                        this[row + i, col + j].Texture = atlasTiles[(j * rowCount) + i];
                    }
                }
            }
        }

        private int AddGroup(AtlasGroup group)
        {
            IEnumerable<AtlasTile> tiles = InternalGetTilesFromGroup(group, out int _, out _);
            foreach (AtlasTile tile in tiles)
            {
                tile.SetGroup(group);
            }
            int groupId = _groups.Count;
            _groups.Add(group);
            return groupId;
        }


        public Animation GetAnimationFromGroup(AtlasGroup group)
        {
            if (!group.IsAnimation())
                return Animation.CreateEmpty();
            if (group is AtlasGroupLargeTileAnimation largeTileAnimation)
                return GetLargeAnimation(largeTileAnimation);
            return GetAnimation(group as AtlasGroupAnimation);
        }

        private Animation GetLargeAnimation(AtlasGroupLargeTileAnimation group)
        {
            return new Animation(GetLargeAnimationTiles(group).Select(largeTileParts => largeTileParts.Select(t => t.Texture).Combine(group.RowSpan, group.ColumnSpan, _layoutDirection)), true, group.FrameTime);
        }

        private IEnumerable<IEnumerable<AtlasTile>> GetLargeAnimationTiles(AtlasGroupLargeTileAnimation group) => group.GetLargeTiles().Select(GetLargeTile);

        private Animation GetAnimation(AtlasGroupAnimation groupAnimation) => new Animation(GetRange(groupAnimation.Row, groupAnimation.Column, groupAnimation.Direction == ImageLayoutDirection.Horizontal ? groupAnimation.Count : 1, groupAnimation.Direction == ImageLayoutDirection.Vertical ? groupAnimation.Count : 1).Select(t => t.Texture), true, groupAnimation.FrameTime);

        private Image BuildFinalImage() => _tiles.Select(t => t.Texture).Combine(Rows, Columns, _layoutDirection);

        public IReadOnlyCollection<AtlasTile> GetTiles() => _tiles;

        public void SetGroupTilesFromAnimation(AtlasGroup group, Animation animation)
        {
            SetRange(group.Row, group.Column, group.Count, group.Direction, animation.GetFrames().Select(f => f.Texture));
        }

        private IEnumerable<AtlasTile> GetLargeTile(AtlasGroupLargeTile group) => GetRange(group.Row, group.Column, group.RowSpan, group.ColumnSpan);

        public Image GetTileTexture(AtlasTile tile)
        {
            if (!tile.IsPartOfGroup)
                return tile;
            AtlasGroup atlasGroup = tile.GetGroup();

            if (!atlasGroup.IsLargeTile())
                return tile;

            AtlasGroupLargeTile largeTile = atlasGroup is AtlasGroupLargeTileAnimation largeTileAnimation ? largeTileAnimation.GetTile(tile.Row, tile.Column) : atlasGroup as AtlasGroupLargeTile;
            return GetLargeTile(largeTile).Select(t => t.Texture).Combine(largeTile.RowSpan, largeTile.ColumnSpan, _layoutDirection);
        }

        private IEnumerable<AtlasTile> InternalGetTilesFromGroup(AtlasGroup atlasGroup, out int rowSpan, out int columnSpan)
        {
            if (atlasGroup is AtlasGroupLargeTileAnimation largeTileAnimation)
            {
                rowSpan = largeTileAnimation.RowSpan;
                columnSpan = largeTileAnimation.ColumnSpan;
                return largeTileAnimation.GetLargeTiles().SelectMany(GetLargeTile);
            }
            if (atlasGroup is AtlasGroupLargeTile largeTile)
            {
                rowSpan = largeTile.RowSpan;
                columnSpan = largeTile.ColumnSpan;
                return GetLargeTile(largeTile);
            }
            rowSpan = 1;
            columnSpan = 1;
            return GetRange(atlasGroup.Row, atlasGroup.Column, atlasGroup.Count, atlasGroup.Direction);
        }

        public Rectangle GetTileArea(AtlasTile tile)
        {
            if (!tile.IsPartOfGroup)
                return tile.GetArea(TileSize);
            AtlasGroup group = tile.GetGroup();
            return new Rectangle(new Point(group.Row * TileSize.Width, group.Column * TileSize.Height), group.GetSize(TileSize));
        }

        public void SetGroup(AtlasGroup group, Image texture)
        {
            IEnumerable<Image> images = texture.Split(TileSize, group.Direction);
            if (!images.All(img => img.Size == TileSize))
                return;
            Size s = group.GetSize(new Size(1, 1));
            SetRange(group.Row, group.Column, s.Width, s.Height, images);
        }
    }
}