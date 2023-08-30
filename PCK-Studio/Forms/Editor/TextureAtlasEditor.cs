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
using System.Collections.Generic;
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
using PckStudio.Helper;
using PckStudio.Internal.Json;

namespace PckStudio.Forms.Editor
{
    internal partial class TextureAtlasEditor : MetroForm
    {
        public Image FinalTexture
        {
            get
            {
                if (DialogResult != DialogResult.OK)
                    return null;
                return (Image)originalPictureBox.Image.Clone();
            }
        }

        private readonly PckFile _pckFile;
        private ColorContainer _colourTable;
        private readonly Size _areaSize;
        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly string _atlasType;
        private readonly List<AtlasTile> _tiles;

        private AtlasTile _selectedTile;
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
            set => SetImageDisplayed(value);
        }

        private const ImageLayoutDirection _imageLayout = ImageLayoutDirection.Horizontal;

        public TextureAtlasEditor(PckFile pckFile, string path, Image atlas, Size areaSize)
        {
            InitializeComponent();

            AcquireColorTable(pckFile);
            _areaSize = areaSize;
            _pckFile = pckFile;
            _rowCount = atlas.Width / areaSize.Width;
            _columnCount = atlas.Height / areaSize.Height;
            (var tileInfos, _atlasType) = Path.GetFileNameWithoutExtension(path) switch
            {
                "terrain" => (Tiles.BlockTileInfos, "blocks"),
                "items" => (Tiles.ItemTileInfos, "items"),
                _ => (null, null),
            };
            originalPictureBox.Image = atlas;
            var images = atlas.Split(_areaSize, _imageLayout);

            var tiles = images.enumerate().Select(
                p => new AtlasTile(p.index, GetAtlasArea(p.index, _rowCount, _columnCount, _areaSize, _imageLayout), tileInfos.IndexInRange(p.index) ? tileInfos[p.index] : null, p.value)
                );
            _tiles = new List<AtlasTile>(tiles);

            SelectedIndex = 0;
        }

        private bool AcquireColorTable(PckFile pckFile)
        {
            if (pckFile.TryGetFile("colours.col", PckFileType.ColourTableFile, out var colFile) &&
                colFile.Size > 0)
            {
                using var ms = new MemoryStream(colFile.Data);
                var reader = new COLFileReader();
                _colourTable = reader.FromStream(ms);
                return true;
            }
            _colourTable = null;
            return false;
        }

        private void SetImageDisplayed(int index)
        {
            tileNameLabel.Text = string.Empty;
            
            variantLabel.Visible = false;
            variantComboBox.Visible = false;
            variantComboBox.Items.Clear();
            variantComboBox.SelectedItem = null;
            variantComboBox.Enabled = false;

            if (selectTilePictureBox.IsPlaying)
                selectTilePictureBox.Stop();
            selectTilePictureBox.UseBlendColor = false;
            selectTilePictureBox.Image = null;

            if (_tiles is null || !_tiles.IndexInRange(index) || (_selectedTile = _tiles[index]) is null)
                return;

            if(string.IsNullOrEmpty(_selectedTile.Tile.DisplayName))
			{
                // changes the selected tile to the base flowing tile (carries all properties over) - Matt
                _selectedTile = _tiles.Find(t => t.Tile.InternalName == _selectedTile.Tile.InternalName);
			}

            tileNameLabel.Text = $"{_selectedTile.Tile.DisplayName}";
            selectTilePictureBox.BlendColor = GetBlendColor();
            selectTilePictureBox.UseBlendColor = applyColorMaskToolStripMenuItem.Checked;

            bool hasAnimation =
                _pckFile.Files.TryGetValue($"res/textures/{_atlasType}/{_selectedTile.Tile.InternalName}.png", PckFileType.TextureFile, out var animationFile);
            animationButton.Text = hasAnimation ? "Edit Animation" : "Create Animation";
            replaceButton.Enabled = !hasAnimation;

            if (playAnimationsToolStripMenuItem.Checked &&
                hasAnimation &&
                animationFile.Size > 0)
            {
                var animation = AnimationHelper.GetAnimationFromFile(animationFile);
                selectTilePictureBox.Start(animation);
                return;
            }

            if (variantLabel.Visible = variantComboBox.Visible = _selectedTile.Tile.HasColourEntry && _selectedTile.Tile.ColourEntry.Variants.Length > 1)
            {
                variantComboBox.Items.AddRange(_selectedTile.Tile.ColourEntry.Variants);
                variantComboBox.SelectedItem = _selectedTile.Tile.ColourEntry.DefaultName;
            }
            
            selectTilePictureBox.Image = _selectedTile.Texture;
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
            };
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

        private static Rectangle GetAtlasArea(int index, int rowCount, int columnCount, Size size, ImageLayoutDirection imageLayout)
        {
            var p = GetSelectedPoint(index, rowCount, columnCount, imageLayout);
            var ap = new Point(p.X * size.Width, p.Y * size.Height);
            return new Rectangle(ap, size);
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
            var graphicsConfig = new GraphicsConfig()
            {
                InterpolationMode = selectTilePictureBox.InterpolationMode,
                PixelOffsetMode = PixelOffsetMode.HighQuality
            };
            if (texture.Size != _areaSize)
                texture = texture.Resize(_areaSize, graphicsConfig);
            using (var g = Graphics.FromImage(originalPictureBox.Image))
            {
                g.ApplyConfig(graphicsConfig);
                g.Fill(_selectedTile.Area, Color.Transparent);
                g.DrawImage(texture, _selectedTile.Area);
            }

            _tiles[_selectedTile.Index] = new AtlasTile(_selectedTile.Index, _selectedTile.Area, _selectedTile.Tile, texture);
            selectTilePictureBox.Image = texture;
            originalPictureBox.Invalidate();
        }

        private Color GetBlendColor()
        {
            if (_selectedTile.Tile.HasColourEntry && _selectedTile.Tile.ColourEntry is not null)
                return FindBlendColorByKey(_selectedTile.Tile.ColourEntry.DefaultName);
            return Color.White;
        }

        private Color FindBlendColorByKey(string colorKey)
        {
            if (_colourTable is not null &&
                _selectedTile.Tile.HasColourEntry &&
                _selectedTile.Tile.ColourEntry is not null)
            {
                if (_selectedTile.Tile.ColourEntry.IsWaterColour &&
                    _colourTable.WaterColors.FirstOrDefault(entry => entry.Name == colorKey) is ColorContainer.WaterColor waterColor)
                {
                    return waterColor.SurfaceColor;
                }
                else if (_colourTable.Colors.FirstOrDefault(entry => entry.Name == colorKey) is ColorContainer.Color color)
                {
                    return color.ColorPallette;
                }
            }
            return Color.White;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    if (_tiles.IndexInRange(_selectedTile.Index - 1))
                    {
                        SelectedIndex = _selectedTile.Index - 1;
                        return true;
                    }
                    break;
                case Keys.Right:
                    if (_tiles.IndexInRange(_selectedTile.Index + 1))
                    {
                        SelectedIndex = _selectedTile.Index + 1;
                        return true;
                    }
                    break;
                case Keys.Up:
                    if (_tiles.IndexInRange(_selectedTile.Index - _rowCount))
                    {
                        SelectedIndex = _selectedTile.Index - _rowCount;
                        return true;
                    }
                    break;

                case Keys.Down:
                    if (_tiles.IndexInRange(_selectedTile.Index + _rowCount))
                    {
                        SelectedIndex = _selectedTile.Index + _rowCount;
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void originalPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            int index = GetSelectedImageIndex(
                originalPictureBox.Size,
                originalPictureBox.Image.Size,
                _areaSize,
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
                Filter = "PNG Image|*.png",
                Title = "Select Texture"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
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
            bool isNewFile;
            if (isNewFile = !_pckFile.Files.TryGetValue(
                    $"res/textures/{_atlasType}/{_selectedTile.Tile.InternalName}.png",
                    PckFileType.TextureFile, out var file
                ))
            {
                file = new PckFileData($"res/textures/{_atlasType}/{_selectedTile.Tile.InternalName}.png", PckFileType.TextureFile);
            }

            var animation = AnimationHelper.GetAnimationFromFile(file);

            var animationEditor = new AnimationEditor(animation, _selectedTile.Tile.InternalName, GetBlendColor());
            if (animationEditor.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            AnimationHelper.SaveAnimationToFile(file, animation);
            
            if (isNewFile)
            {
                _pckFile.Files.Add(file);
            }
        }

        private void extractTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Tile Texture|*.png",
                FileName = _selectedTile.Tile.InternalName
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectTilePictureBox.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void variantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedTile.Tile.ColourEntry is not null &&
                _selectedTile.Tile.ColourEntry.Variants.IndexInRange(variantComboBox.SelectedIndex))
            {
                string colorKey = _selectedTile.Tile.ColourEntry.Variants[variantComboBox.SelectedIndex];
                selectTilePictureBox.BlendColor = FindBlendColorByKey(colorKey);
                selectTilePictureBox.Image = _selectedTile.Texture;
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
    }
}