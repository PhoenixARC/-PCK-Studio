﻿/* Copyright (c) 2023-present miku-666, MattNL
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;

using OMI.Formats.Color;
using OMI.Formats.Pck;
using OMI.Workers.Color;

using PckStudio.Extensions;
using PckStudio.Internal;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Json;
using PckStudio.Internal.Serializer;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Editor
{
    internal partial class TextureAtlasEditor : ThemeForm
    {
        private Image _atlasTexture;
        public Image FinalTexture => DialogResult == DialogResult.OK ? _atlasTexture : null;

        private readonly PckFile _pckFile;
        private ColorContainer _colourTable;
        private readonly Size _tileAreaSize;
        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly ResourceLocation _resourceLocation;
        private readonly List<AtlasTile> _tiles;

        private AtlasTile _selectedTile;
        // the "parent" tile for tiles that share name; i.e. parts of water_flow
        private AtlasTile dataTile;

        private sealed class AtlasTile
        {
            internal readonly int Index;
            internal readonly Rectangle Area;
            internal readonly JsonTileInfo Tile;
            internal readonly Image Texture;

            public AtlasTile(int index, Rectangle area, JsonTileInfo tile, Image texture)
            {
                Index = index;
                Area = area;
                Tile = tile;
                Texture = texture;
            }
        }

        private int SelectedIndex
        {
            set {
                if (value < 0)
                {
                    value += _tiles.Count;
                }
                else if (value >= _tiles.Count)
                {
                    value -= _tiles.Count;
                }
                SetImageDisplayed(value);
            }
        }

        private const ImageLayoutDirection _imageLayout = ImageLayoutDirection.Horizontal;

        private readonly GraphicsConfig _graphicsConfig = new GraphicsConfig()
        {
            InterpolationMode = InterpolationMode.NearestNeighbor,
            PixelOffsetMode = PixelOffsetMode.HighQuality
        };

        public TextureAtlasEditor(PckFile pckFile, ResourceLocation resourceLocation, Image atlas)
        {
            InitializeComponent();

            if (!AcquireColorTable(pckFile))
            {
                MessageBox.Show("Failed to acquire color information", "Acquire failure", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            _atlasTexture = atlas;
            _tileAreaSize = resourceLocation.GetTileArea(atlas.Size);
            _pckFile = pckFile;
            _rowCount = atlas.Width / _tileAreaSize.Width;
            _columnCount = atlas.Height / _tileAreaSize.Height;
            _resourceLocation = resourceLocation;
            List<JsonTileInfo> tileInfos = resourceLocation.Category switch
            {
                ResourceCategory.BlockAtlas => Tiles.BlockTileInfos,
                ResourceCategory.ItemAtlas => Tiles.ItemTileInfos,
                ResourceCategory.ParticleAtlas => Tiles.ParticleTileInfos,
                ResourceCategory.MapIconAtlas => Tiles.MapIconTileInfos,
                ResourceCategory.AdditionalMapIconsAtlas => Tiles.AdditionalMapIconTileInfos,
                ResourceCategory.MoonPhaseAtlas => Tiles.MoonPhaseTileInfos,
                ResourceCategory.ExperienceOrbAtlas => Tiles.ExperienceOrbTileInfos,
                ResourceCategory.ExplosionAtlas => Tiles.ExplosionTileInfos,
                ResourceCategory.PaintingAtlas => Tiles.PaintingTileInfos,
                ResourceCategory.BannerAtlas => Tiles.BannerTileInfos,
                _ => null,
            };

            originalPictureBox.Image = new Bitmap(atlas);

            IEnumerable<Image> images = atlas.Split(_tileAreaSize, _imageLayout);

            AtlasTile MakeTile((int index, Image value) p)
            {
                int i = p.index;
                JsonTileInfo tileInfo = tileInfos.IndexInRange(i) ? tileInfos[i] : null;
                
                Rectangle atlasArea = GetAtlasArea(i, tileInfo?.TileWidth ?? 1, tileInfo?.TileHeight ?? 1, _rowCount, _columnCount, _tileAreaSize, _imageLayout);
                
                // get texture for tiles that are not 1x1 tiles
                Point selectedPoint = GetSelectedPoint(i, _rowCount, _columnCount, _imageLayout);
                
                var textureLocation = new Point(selectedPoint.X * _tileAreaSize.Width, selectedPoint.Y * _tileAreaSize.Height);
                var textureSize = new Size(tileInfos[i].TileWidth * _tileAreaSize.Width, tileInfos[i].TileHeight * _tileAreaSize.Height);
                var textureArea = new Rectangle(textureLocation, textureSize);
                
                Image texture = tileInfos.IndexInRange(i) ? atlas.GetArea(textureArea) : p.value;
                return new AtlasTile(i, atlasArea, tileInfo, texture);
            }

            _tiles = new List<AtlasTile>(images.enumerate().Select(MakeTile));

            SelectedIndex = 0;

            animationButton.Enabled =
                _resourceLocation.Category == ResourceCategory.BlockAtlas ||
                _resourceLocation.Category == ResourceCategory.ItemAtlas;

            // this is directly based on Java's source code for handling enchanted hits
            // the particle is assigned a random grayscale color between roughly 154 and 230
            // since critical hit is the only particle with this distinction, we just need to check the atlas type
            if (_resourceLocation.Category == ResourceCategory.ParticleAtlas)
            {
                colorSlider.Minimum = 154;
                colorSlider.Maximum = 230;
                colorSlider.Value = colorSlider.Maximum;
            }
        }

        private bool AcquireColorTable(PckFile pckFile)
        {
            if (pckFile.TryGetAsset("colours.col", PckAssetType.ColourTableFile, out PckAsset colAsset) &&
                colAsset.Size > 0)
            {
                using var ms = new MemoryStream(colAsset.Data);
                var reader = new COLFileReader();
                _colourTable = reader.FromStream(ms);
                return true;
            }
            _colourTable = null;
            return false;
        }

        private void UpdateAtlasDisplay()
        {
            using (var g = Graphics.FromImage(originalPictureBox.Image))
            {
                g.ApplyConfig(_graphicsConfig);
                g.Clear(Color.Transparent);
                g.DrawImage(_atlasTexture, 0, 0, _atlasTexture.Width, _atlasTexture.Height);

                SolidBrush brush = new SolidBrush(Color.FromArgb(127, Color.White));
                g.FillRectangle(brush, _selectedTile.Area);
            }

            originalPictureBox.Invalidate();
        }

        private void SetImageDisplayed(int index)
        {
            tileNameLabel.Text = string.Empty;
            internalTileNameLabel.Text = string.Empty;

            colorSlider.Visible = false;
            colorSliderLabel.Visible = false;
            variantComboBox.Visible = false;

            variantComboBox.SelectedItem = null;
            variantComboBox.Enabled = false;
            variantComboBox.Items.Clear();
            clearColorButton.Enabled = false;

            if (selectTilePictureBox.IsPlaying)
                selectTilePictureBox.Stop();
            selectTilePictureBox.UseBlendColor = false;
            selectTilePictureBox.Image = null;

            if (_tiles is null || !_tiles.IndexInRange(index) || (_selectedTile = _tiles[index]) is null)
                return;

            UpdateAtlasDisplay();

            dataTile = _selectedTile;
            if (string.IsNullOrEmpty(dataTile.Tile.DisplayName) && !string.IsNullOrEmpty(dataTile.Tile.InternalName))
            {
                dataTile = _tiles.Find(t => t.Tile.InternalName == _selectedTile.Tile.InternalName);
            }

            selectTilePictureBox.Image = dataTile.Texture;
            selectTilePictureBox.BlendColor = GetBlendColor();
            selectTilePictureBox.UseBlendColor = applyColorMaskToolStripMenuItem.Checked;

            tileNameLabel.Text = $"{dataTile.Tile.DisplayName}";
            internalTileNameLabel.Text = $"{dataTile.Tile.InternalName}";

            if (animationButton.Enabled)
            {
                PckAsset animationAsset;
                ResourceCategory animationResourceCategory = _resourceLocation.Category == ResourceCategory.ItemAtlas ? ResourceCategory.ItemAnimation : ResourceCategory.BlockAnimation;

                string animationAssetPath = $"{ResourceLocation.GetPathFromCategory(animationResourceCategory)}/{dataTile.Tile.InternalName}";
                bool hasAnimation =
                    _pckFile.TryGetValue($"{animationAssetPath}.png", PckAssetType.TextureFile, out animationAsset) ||
                    _pckFile.TryGetValue($"{animationAssetPath}.tga", PckAssetType.TextureFile, out animationAsset);
                animationButton.Text = hasAnimation ? "Edit Animation" : "Create Animation";

                // asset size check dont have to be done here the deserializer handles it. -Miku
                if (playAnimationsToolStripMenuItem.Checked && hasAnimation)
                {
                    Animation animation = animationAsset.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);
                    selectTilePictureBox.Image = animation.CreateAnimationImage();
                    selectTilePictureBox.Start();
                }
            }

            setColorButton.Enabled = dataTile.Tile.AllowCustomColour;

            variantComboBox.Enabled = variantComboBox.Visible = dataTile.Tile.HasColourEntry && dataTile.Tile.ColourEntry?.Variants?.Length > 1;
            if (variantComboBox.Enabled)
            {
                if (dataTile.Tile.ColourEntry.IsWaterColour)
                {
                    foreach (ColorContainer.WaterColor col in _colourTable.WaterColors)
                    {
                        if(!variantComboBox.Items.Contains(col.Name))
                            variantComboBox.Items.Add(col.Name);
                    }
                }

                // TODO: only add variants that are available in the color table
                variantComboBox.Items.AddRange(dataTile.Tile.ColourEntry.Variants);
                
                if (variantComboBox.Items.Count > 0)
                    variantComboBox.SelectedIndex = 0;
            }
        }
        
        private static int GetSelectedImageIndex(
            Size pictureBoxSize,
            Size imageSize,
            Size areaSize,
            Point clickLocation,
            PictureBoxSizeMode sizeMode,
            ImageLayoutDirection imageLayout)
        {
            Point result = new Point();
            int rowCount = imageSize.Width / areaSize.Width;
            int columnCount = imageSize.Height / areaSize.Height;
            switch (sizeMode)
            {
                case PictureBoxSizeMode.Normal:
                case PictureBoxSizeMode.AutoSize:
                    {
                        var imageArea = new Rectangle(Point.Empty, imageSize);
                        if (!imageArea.Contains(clickLocation))
                            return -1;
                        result.X = clickLocation.X / areaSize.Width;
                        result.Y = clickLocation.Y / areaSize.Height;
                        break;
                    }
                case PictureBoxSizeMode.StretchImage:
                    {
                        float widthDiff = (float)pictureBoxSize.Width / imageSize.Width;
                        float heightDiff = (float)pictureBoxSize.Height / imageSize.Height;
                        Size scaledArea = Size.Round(new SizeF(areaSize.Width * widthDiff, areaSize.Height * heightDiff));

                        result.X = clickLocation.X / scaledArea.Width;
                        result.Y = clickLocation.Y / scaledArea.Height;
                        break;
                    }
                case PictureBoxSizeMode.CenterImage:
                    {
                        Rectangle imageArea = new Rectangle(Point.Empty, imageSize);
                        imageArea.X = (pictureBoxSize.Width - imageArea.Width) / 2;
                        imageArea.Y = (pictureBoxSize.Height - imageArea.Height) / 2;

                        if (!imageArea.Contains(clickLocation))
                            return -1;

                        result.X = (clickLocation.X - imageArea.X) / (clickLocation.X * areaSize.Width);
                        result.Y = (clickLocation.Y - imageArea.Y) / (clickLocation.Y * areaSize.Height);
                        break;
                    }
                case PictureBoxSizeMode.Zoom:
                    {
                        Rectangle imageArea = new Rectangle();
                        float widthDiff = (float)pictureBoxSize.Width / imageSize.Width;
                        float heightDiff = (float)pictureBoxSize.Height / imageSize.Height;
                        float scale = Math.Min(widthDiff, heightDiff);

                        imageArea.Width = (int)(imageSize.Width * scale);
                        imageArea.Height = (int)(imageSize.Height * scale);
                        imageArea.X = (pictureBoxSize.Width - imageArea.Width) / 2;
                        imageArea.Y = (pictureBoxSize.Height - imageArea.Height) / 2;

                        if (!imageArea.Contains(clickLocation))
                            return -1;

                        var scaledArea = new SizeF(areaSize.Width * scale, areaSize.Height * scale);
                        result.X = (int)((clickLocation.X - imageArea.X) / scaledArea.Width);
                        result.Y = (int)((clickLocation.Y - imageArea.Y) / scaledArea.Height);
                    }
                    break;
                
                default:
                    break;
            }
            return GetSelectedIndex(result.X, result.Y, rowCount, columnCount, imageLayout);
        }

        private static int GetSelectedIndex(int x, int y, int rowCount, int columnCount, ImageLayoutDirection imageLayout)
        {
            return imageLayout switch
            {
                ImageLayoutDirection.Horizontal => x + y * rowCount,
                ImageLayoutDirection.Vertical   => y + x * columnCount,
                _ => throw new ArgumentOutOfRangeException(nameof(imageLayout)),
            };
        }

        private static Rectangle GetAtlasArea(int index, int tileWidth, int tileHeight, int rowCount, int columnCount, Size size, ImageLayoutDirection imageLayout)
        {
            Point p = GetSelectedPoint(index, rowCount, columnCount, imageLayout);
            var ap = new Point(p.X * size.Width, p.Y * size.Height);
            return new Rectangle(ap, new Size(size.Width * tileWidth, size.Height * tileHeight));
        }

        private static Point GetSelectedPoint(int index, int rowCount, int columnCount, ImageLayoutDirection imageLayout)
        {
            int y = Math.DivRem(index, rowCount, out int x);
            if (imageLayout == ImageLayoutDirection.Vertical)
                x = Math.DivRem(index, columnCount, out y);
            return new Point(x, y);
        }

        private void SetTile(Image texture)
        {
            if (texture.Size != _tileAreaSize)
                texture = texture.Resize(_tileAreaSize, _graphicsConfig);

            using (var g = Graphics.FromImage(_atlasTexture))
            {
                g.ApplyConfig(_graphicsConfig);
                g.Fill(dataTile.Area, Color.Transparent);
                g.DrawImage(texture, dataTile.Area);
            }

            AtlasTile tile = _selectedTile != dataTile ? dataTile : _selectedTile;
            _tiles[tile.Index] = new AtlasTile(tile.Index, tile.Area, tile.Tile, texture);
            selectTilePictureBox.Image = texture;
            UpdateAtlasDisplay();
        }

        private Color GetBlendColor()
        {
            if (dataTile.Tile.HasColourEntry && dataTile.Tile.ColourEntry is not null)
            {
                Color col = FindBlendColorByKey(dataTile.Tile.ColourEntry.DefaultName);
                return col;
            }
            
            return Color.White;
        }

        private Color GetSpecificBlendColor(string colorKey)
        {
            colorSlider.Visible = colorSliderLabel.Visible = true;

            // Simply, Experience orbs red value is just sliding between 255 and 0
            if (colorKey == "experience_orb")
                return Color.FromArgb(colorSlider.Value, 255, 0);

            // Similar story for critical hits, but for all values
            var final_color = Color.FromArgb(colorSlider.Value, colorSlider.Value, colorSlider.Value);

            // Enchanted hits are modified critical hit particles
            if (dataTile.Tile.InternalName == "enchanted_hit")
            {
                // This is directly based on Java's source code for handling enchanted hits
                // it just multiplies the red by 0.3 and green by .8 of the color assigned to the critical hit particle
                final_color = Color.FromArgb((int)(final_color.R * 0.3f), (int)(final_color.R * 0.8f), final_color.B);
            }
            return final_color;
        }

        private Color FindBlendColorByKey(string colorKey)
        {
            // The following tiles are hardcoded within a range and do not have color table entries
            if (colorKey == "experience_orb" || colorKey == "critical_hit") 
                return GetSpecificBlendColor(colorKey);

            if (dataTile.Tile.HasColourEntry && dataTile.Tile.ColourEntry is not null)
            {
                // basic way to check for classic water colors
                if(!dataTile.Tile.ColourEntry.IsWaterColour || colorKey.StartsWith("Water_"))
                {
                    if (_colourTable.Colors.FirstOrDefault(entry => entry.Name == colorKey) is ColorContainer.Color color)
                    {
                        return color.ColorPallette;
                    }
                }
                else if (_colourTable.WaterColors.FirstOrDefault(entry => entry.Name == colorKey) is ColorContainer.WaterColor waterColor)
                {
                    return waterColor.SurfaceColor;
                }
            }

            Debug.WriteLine("Could not find: " + colorKey);
            return Color.White;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.R:
                    // Refreshes the specific tile
                    SelectedIndex = _selectedTile.Index;
                    return true;
                case Keys.Left:
                    SelectedIndex = _selectedTile.Index - 1;
                    return true;
                case Keys.Right:
                    SelectedIndex = _selectedTile.Index + 1;
                    return true;
                case Keys.Up:
                    SelectedIndex = _selectedTile.Index - _rowCount;
                    return true;
                case Keys.Down:
                    SelectedIndex = _selectedTile.Index + _rowCount;
                    return true;
            }

            return false;
        }

        private void originalPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            ActiveControl = null;

            int index = GetSelectedImageIndex(
                originalPictureBox.Size,
                _atlasTexture.Size,
                _tileAreaSize,
                e.Location,
                originalPictureBox.SizeMode,
                _imageLayout);

            if (index != -1)
            {
                SelectedIndex = index;
            }
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Tile Texture(*.png)|*.png",
                Title = "Select Texture"
            };

            if (fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var img = Image.FromFile(fileDialog.FileName);
                SetTile(img);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void animationButton_Click(object sender, EventArgs e)
        {
            ResourceCategory animationResourceCategory = _resourceLocation.Category == ResourceCategory.ItemAtlas ? ResourceCategory.ItemAnimation : ResourceCategory.BlockAnimation;
            string animationAssetPath = $"{ResourceLocation.GetPathFromCategory(animationResourceCategory)}/{_selectedTile.Tile.InternalName}.png";
            PckAsset asset = _pckFile.GetOrCreate(animationAssetPath, PckAssetType.TextureFile);

            Animation animation = asset.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);

            var animationEditor = new AnimationEditor(animation, _selectedTile.Tile.DisplayName);
            if (animationEditor.ShowDialog(this) == DialogResult.OK)
            {
                asset.SetSerializedData(animationEditor.Result, AnimationSerializer.DefaultSerializer);
                // so animations can automatically update upon saving
                SelectedIndex = _selectedTile.Index;
            }
        }

        private void extractTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Tile Texture|*.png",
                FileName = _selectedTile.Tile.InternalName
            };
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                dataTile.Texture.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void variantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataTile.Tile.ColourEntry is not null && variantComboBox.SelectedItem is not null)
            {
                string colorKey = variantComboBox.SelectedItem.ToString();

                selectTilePictureBox.BlendColor = FindBlendColorByKey(colorKey);
                selectTilePictureBox.Image = dataTile.Texture;
            }
        }

        private void applyColorMaskToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SelectedIndex = _selectedTile.Index;
        }

        private void playAnimationsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SelectedIndex = _selectedTile.Index;
        }

        private void TextureAtlasEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (selectTilePictureBox.IsPlaying)
                selectTilePictureBox.Stop();
        }

        private void setColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorPick = new ColorDialog();
            colorPick.AllowFullOpen = true;
            colorPick.AnyColor = true;
            colorPick.SolidColorOnly = true;

            //Debug.Assert(Color.FromArgb(0xf9fffe).ToBGR() == 0xfefff9); // White
            //Debug.Assert(Color.FromArgb(0xf9801d).ToBGR() == 0x1d80f9); // Orange
            //Debug.Assert(Color.FromArgb(0xc74ebd).ToBGR() == 0xbd4ec7); // Magenta
            //Debug.Assert(Color.FromArgb(0x3ab3da).ToBGR() == 0xdab33a); // Light Blue
            //Debug.Assert(Color.FromArgb(0xfed83d).ToBGR() == 0x3dd8fe); // Yellow
            //Debug.Assert(Color.FromArgb(0x80c71f).ToBGR() == 0x1fc780); // Lime
            //Debug.Assert(Color.FromArgb(0xf38baa).ToBGR() == 0xaa8bf3); // Pink
            //Debug.Assert(Color.FromArgb(0x474f52).ToBGR() == 0x524f47); // Gray
            //Debug.Assert(Color.FromArgb(0x9d9d97).ToBGR() == 0x979d9d); // Light Gray
            //Debug.Assert(Color.FromArgb(0x169c9c).ToBGR() == 0x9c9c16); // Cyan
            //Debug.Assert(Color.FromArgb(0x8932b8).ToBGR() == 0xb83289); // Purple
            //Debug.Assert(Color.FromArgb(0x3c44aa).ToBGR() == 0xaa443c); // Blue
            //Debug.Assert(Color.FromArgb(0x835432).ToBGR() == 0x325483); // Brown
            //Debug.Assert(Color.FromArgb(0x5e7c16).ToBGR() == 0x167c5e); // Green
            //Debug.Assert(Color.FromArgb(0xb02e26).ToBGR() == 0x262eb0); // Red
            //Debug.Assert(Color.FromArgb(0x1d1d21).ToBGR() == 0x211d1d); // Black

            colorPick.CustomColors = GameConstants.DyeColors.Select(c => c.ToBGR()).ToArray();
            
            if (colorPick.ShowDialog(this) != DialogResult.OK)
                return;

            selectTilePictureBox.BlendColor = colorPick.Color;
            selectTilePictureBox.Image = dataTile.Texture;
            variantComboBox.Enabled = false;
            clearColorButton.Enabled = true;
        }

        private void clearColorButton_Click(object sender, EventArgs e)
        {
            variantComboBox.Enabled = true;

            variantComboBox_SelectedIndexChanged(sender, e);

            clearColorButton.Enabled = false;
        }

        private void colorSlider_ValueChanged(object sender, EventArgs e)
        {
            selectTilePictureBox.BlendColor = GetBlendColor();
            selectTilePictureBox.Image = dataTile.Texture;
        }
    }
}