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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;
using PckStudio.Core.Extensions;
using PckStudio.Core.IO.Java;
using PckStudio.Core.Properties;

namespace PckStudio.Core
{
    public sealed class Atlas
    {
        public string Name { get; set; }
        public int Rows { get; }
        public int Columns { get; }

        public Size TileSize { get; private set; }
        public int TileCount => _tiles.Length;

        private readonly AtlasTile[] _tiles;
        private readonly ImageLayoutDirection _layoutDirection;
        private readonly List<AtlasGroup> _groups;

        public static implicit operator Image(Atlas atlas) => atlas?.BuildFinalImage();
        

        private Atlas(string name, int rows, int columns, IEnumerable<AtlasTile> tiles, Size tileSize, ImageLayoutDirection layoutDirection)
        {
            Name = name;
            Rows = rows;
            Columns = columns;
            TileSize = tileSize;
            _tiles = tiles.Take(rows * columns).ToArray();
            _layoutDirection = layoutDirection;
            _groups = new List<AtlasGroup>();
        }

        public static Atlas FromResourceLocation(Image source, ResourceLocation resourceLocation, LCEGameVersion gameVersion = default, ImageLayoutDirection imageLayout = default)
        {
            AtlasResource atlasResource = resourceLocation as AtlasResource ?? throw new InvalidDataException(nameof(resourceLocation));
            Json.JsonTileInfo[] tilesInfo = atlasResource.TilesInfo.ToArray();
            Size tileArea = atlasResource.GetTileArea(source.Size);
            int rows = source.Width / tileArea.Width;
            int columns = GameConstants.GetColumnCountForGameVersion(atlasResource.Type, gameVersion);
            columns = columns == 0 ? source.Height / tileArea.Height : columns;
            IEnumerable <AtlasTile> tiles = source.Split(tileArea, imageLayout).Select((Image img, int index) => new AtlasTile(img, GetSelectedPoint(index, out int col, rows, columns, imageLayout), col, index: index, userData: tilesInfo.IndexInRange(index) ? tilesInfo[index] : default));
            var atlas = new Atlas(atlasResource.Path, rows, columns, tiles, tileArea, imageLayout);
            atlas.AddGroups(atlasResource.AtlasGroups);
            return atlas;
        }

        public static Atlas CreateEmpty(int rows, int columns, AtlasResource atlasResource, int res = 16)
        {
            ImageLayoutDirection layoutDirection = default;
            Image none = new Bitmap(res, res);
            IEnumerable<AtlasTile> tiles = Enumerable.Repeat(none, rows * columns).Select((v, i) => new AtlasTile(v, GetSelectedPoint(i, out int col, rows, columns, layoutDirection), col, i, default));
            return new Atlas(atlasResource.Path, rows, columns, tiles, none.Size, layoutDirection);
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
            if (group is AtlasLargeTileAnimation largeTileAnimation)
                return GetLargeAnimation(largeTileAnimation);
            return GetAnimation(group as AtlasAnimation);
        }

        private Animation GetLargeAnimation(AtlasLargeTileAnimation group)
        {
            return new Animation(GetLargeAnimationTiles(group).Select(largeTileParts => largeTileParts.Select(t => t.Texture).Combine(group.RowSpan, group.ColumnSpan, _layoutDirection)), true, group.FrameTime);
        }

        private IEnumerable<IEnumerable<AtlasTile>> GetLargeAnimationTiles(AtlasLargeTileAnimation group) => group.GetLargeTiles().Select(GetLargeTile);

        private Animation GetAnimation(AtlasAnimation groupAnimation) => new Animation(GetRange(groupAnimation.Row, groupAnimation.Column, groupAnimation.Direction == ImageLayoutDirection.Horizontal ? groupAnimation.Count : 1, groupAnimation.Direction == ImageLayoutDirection.Vertical ? groupAnimation.Count : 1).Select(t => t.Texture), true, groupAnimation.FrameTime);

        private Image BuildFinalImage()
        {
            return _tiles.Select(t => t.Texture).Combine(Rows, Columns, _layoutDirection);
        }

        public void SetTileSize(Size size)
        {
            if (size.Width != size.Height || size == Size.Empty)
                throw new Exception();
            TileSize = size;
            GraphicsConfig graphicsConfig = GraphicsConfig.PixelPerfect();
            foreach (AtlasTile tile in _tiles)
            {
                if (tile.Texture.Size == size)
                    continue;
                tile.Texture = tile.Texture.Resize(size, graphicsConfig);
            }
        }

        public IReadOnlyCollection<AtlasTile> GetTiles() => _tiles;

        public void SetGroupTilesFromAnimation(AtlasGroup group, Animation animation)
        {
            SetRange(group.Row, group.Column, group.Count, group.Direction, animation.GetFrames().Select(f => f.Texture));
        }

        private IEnumerable<AtlasTile> GetLargeTile(AtlasLargeTile group) => GetRange(group.Row, group.Column, group.RowSpan, group.ColumnSpan);

        public Image GetTileTexture(AtlasTile tile, Color blendColor)
        {
            if (!tile.IsPartOfGroup)
                return tile;
            AtlasGroup atlasGroup = tile.GetGroup();

            if (atlasGroup.IsComposedOfMultipleTiles())
                return ComposeTileTexture(atlasGroup, blendColor);

            if (!atlasGroup.IsLargeTile())
                return tile;

            AtlasLargeTile largeTile = atlasGroup is AtlasLargeTileAnimation largeTileAnimation ? largeTileAnimation.GetTile(tile.Row, tile.Column) : atlasGroup as AtlasLargeTile;
            Image tileTexture = GetLargeTile(largeTile).Select(t => t.Texture).Combine(largeTile.RowSpan, largeTile.ColumnSpan, _layoutDirection);
            return atlasGroup.AllowCustomColor ? tileTexture.Blend(BlendOption.Multiply(blendColor)) : tileTexture;
        }

        private Image ComposeTileTexture(AtlasGroup atlasGroup, Color blendColor)
        {
            Image result = new Bitmap(TileSize.Width, TileSize.Height);
            using Graphics g = Graphics.FromImage(result);
            g.ApplyConfig(GraphicsConfig.PixelPerfect());

            AtlasTile[] textures = atlasGroup.GetTileArea(new Size(1, 1)).Select(r => this[r.X, r.Y]).ToArray();
            if (textures.Length > 1)
            {
                g.DrawImage(textures[0].Texture.Blend(BlendOption.Multiply(blendColor)), Point.Empty);
                g.DrawImage(textures[1], Point.Empty);
            }
            
            return result;
        }

        private IEnumerable<AtlasTile> InternalGetTilesFromGroup(AtlasGroup atlasGroup, out int rowSpan, out int columnSpan)
        {
            if (atlasGroup is AtlasLargeTileAnimation largeTileAnimation)
            {
                rowSpan = largeTileAnimation.RowSpan;
                columnSpan = largeTileAnimation.ColumnSpan;
                return largeTileAnimation.GetLargeTiles().SelectMany(GetLargeTile);
            }
            if (atlasGroup is AtlasLargeTile largeTile)
            {
                rowSpan = largeTile.RowSpan;
                columnSpan = largeTile.ColumnSpan;
                return GetLargeTile(largeTile);
            }
            rowSpan = 1;
            columnSpan = 1;
            if (atlasGroup is AtlasOverlayGroup multipleTiles)
                return multipleTiles.GetTileArea(new Size(1, 1)).Select(r => this[r.X, r.Y]);
            return GetRange(atlasGroup.Row, atlasGroup.Column, atlasGroup.Count, atlasGroup.Direction);
        }

        public Rectangle[] GetTileArea(AtlasTile tile)
        {
            if (!tile.IsPartOfGroup)
                return [tile.GetArea(TileSize)];
            AtlasGroup group = tile.GetGroup();
            return group.GetTileArea(TileSize);
        }

        public bool SetGroup(AtlasGroup group, Image texture)
        {
            Size splitSize = group.IsLargeTile() ? new Size(texture.Width / ((AtlasLargeTile)group).RowSpan, texture.Height /((AtlasLargeTile)group).ColumnSpan) : TileSize;
            Image[] images = texture.Split(splitSize, group.Direction).ToArray();
            if (!images.All(img => img.Size == splitSize))
                return false;
            if (images.Length != group.Count)
                return false;
            Size s = group.GetSize(new Size(1, 1));
            SetRange(group.Row, group.Column, s.Width, s.Height, images);
            return true;
        }

        public bool SetTile(AtlasTile tile, Image image)
        {
            if (tile.IsPartOfGroup)
                return SetGroup(tile.GetGroup(), image);
            tile.Texture = image;
            return true;
        }

        internal static Atlas CreateDefault(AtlasResource atlasResource, LCEGameVersion gameVersion)
        {
            Image defaultAtlas = atlasResource.Type switch
            {
                AtlasResource.AtlasType.ItemAtlas => Resources.items_atlas,
                AtlasResource.AtlasType.BlockAtlas => Resources.terrain_atlas,
                AtlasResource.AtlasType.ParticleAtlas => Resources.particles_atlas,
                AtlasResource.AtlasType.BannerAtlas => Resources.banners_atlas,
                AtlasResource.AtlasType.PaintingAtlas => Resources.paintings_atlas,
                AtlasResource.AtlasType.ExplosionAtlas => Resources.explosions_atlas,
                AtlasResource.AtlasType.ExperienceOrbAtlas => Resources.experience_orbs_atlas,
                AtlasResource.AtlasType.MoonPhaseAtlas => Resources.moon_phases_atlas,
                AtlasResource.AtlasType.MapIconAtlas => Resources.map_icons_atlas,
                AtlasResource.AtlasType.AdditionalMapIconsAtlas => Resources.additional_map_icons_atlas,
                _ => throw new InvalidOperationException()
            };
            return FromResourceLocation(defaultAtlas, atlasResource, gameVersion);
        }
    }
}