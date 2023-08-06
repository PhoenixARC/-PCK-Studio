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
using PckStudio.Forms.Editor;
using PckStudio.Forms.Utilities;

namespace PckStudio
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

        private struct SelectedTile
        {
            internal readonly int Index;
            internal readonly string Name;
            internal readonly string TextureName;
            internal readonly Rectangle Area;
            internal readonly AnimationResources.JsonTileInfo Tile;

            public SelectedTile(int index, string name, string textureName, Rectangle area, AnimationResources.JsonTileInfo tile)
            {
                Index = index;
                Name = name;
                TextureName = textureName;
                Area = area;
                Tile = tile;
            }
        }

        private readonly PckFile _pckFile;
        private readonly Size _areaSize;
        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly string _atlasType;
        private readonly List<Image> _textures;
        private readonly List<AnimationResources.JsonTileInfo> _textureInfos;

        private SelectedTile _selectedItem = new SelectedTile();
        private ColorContainer _colourTable;

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
            (_textureInfos, _atlasType) = Path.GetFileNameWithoutExtension(path) switch
            {
                "terrain" => (AnimationResources.BlockTileInfos, "blocks"),
                "items" => (AnimationResources.ItemTileInfos, "items"),
                _ => (null, null),
            };
            originalPictureBox.Image = atlas;
            var images = atlas.CreateImageList(_areaSize, _imageLayout);
            _textures = new List<Image>(images);
            SelectedIndex = 0;
        }

        private bool AcquireColorTable(PckFile pckFile)
        {
            if (pckFile.TryGetFile("colours.col", PckFile.FileData.FileType.ColourTableFile, out var colFile) &&
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
            if (selectTilePictureBox.IsPlaying)
                selectTilePictureBox.Stop();
            prevButton.Enabled = index > 0;
            nextButton.Enabled = index < _textures.Count - 1;
            infoTextBox.Text = string.Empty;

            if (_textureInfos is not null && _textureInfos.IndexInRange(index))
            {
                var info = _textureInfos[index];
                var pos = GetSelectedPoint(index, _rowCount, _columnCount, _imageLayout);
                var selectedArea = new Rectangle(pos.X * _areaSize.Width, pos.Y * _areaSize.Height, _areaSize.Width, _areaSize.Height);
                _selectedItem = new SelectedTile(index, info.DisplayName, info.InternalName, selectedArea, info);
                
                infoTextBox.Text = $"{_selectedItem.Name}\n{_selectedItem.TextureName}";
                bool hasAnimation = _pckFile.Files.TryGetValue($"res/textures/{_atlasType}/{_selectedItem.TextureName}.png", PckFile.FileData.FileType.TextureFile, out var animationFile);
                animationButton.Text = hasAnimation ? "Edit Animation" : "Create Animation";
                if (hasAnimation && animationFile.Size > 0)
                {
                    using var ms = new MemoryStream(animationFile.Data);
                    var img = Image.FromStream(ms);
                    var textures = img.CreateImageList(ImageLayoutDirection.Vertical);
                    selectTilePictureBox.Start(new Internal.Animation(textures, animationFile.Properties.GetPropertyValue("ANIM")));
                    return;
                }
            }

            if (_textures.IndexInRange(index))
            {
                var img = _textures[index];
                if (_selectedItem.Tile.HasColourEntry &&
                    _selectedItem.Tile.HasColourEntry &&
                    !string.IsNullOrWhiteSpace(_selectedItem.Tile.ColourEntry.DefaultName) &&
                    _colourTable is not null &&
                    _colourTable.Colors.FirstOrDefault(entry => entry.Name == _selectedItem.Tile.ColourEntry.DefaultName) is ColorContainer.Color color)
                {
                    img = img.Blend(color.ColorPallette, BlendMode.Multiply);
                }
                selectTilePictureBox.Image = img;
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            SelectedIndex = _selectedItem.Index - 1;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            SelectedIndex = _selectedItem.Index + 1;
        }

        private void originalPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var index = GetSelectedImageIndex(
                originalPictureBox.Size,
                originalPictureBox.Image.Size,
                _areaSize,
                e.Location,
                originalPictureBox.SizeMode,
                _imageLayout);

            Debug.WriteLine(index);
            if (index != -1)
            {
                SelectedIndex = index;
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

                        Size scaledArea = Size.Round(new SizeF(areaSize.Width * scale, areaSize.Height * scale));
                        result.X = (clickLocation.X - imageArea.X) / scaledArea.Width;
                        result.Y = (clickLocation.Y - imageArea.Y) / scaledArea.Height;
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

        private static Point GetSelectedPoint(int index, int rowCount, int columnCount, ImageLayoutDirection imageLayout)
        {
            int y = Math.DivRem(index, rowCount, out int x);
            if (imageLayout == ImageLayoutDirection.Vertical)
                x = Math.DivRem(index, columnCount, out y);
            return new Point(x, y);
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
                g.Fill(_selectedItem.Area, Color.Transparent);
                g.DrawImage(texture, _selectedItem.Area);
            }
            _textures[_selectedItem.Index] = texture;
            selectTilePictureBox.Image = texture;
            originalPictureBox.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void animationButton_Click(object sender, EventArgs e)
        {
            bool isNewFile;
            if (isNewFile = !_pckFile.Files.TryGetValue(
                    $"res/textures/{_atlasType}/{_selectedItem.TextureName}.png",
                    PckFile.FileData.FileType.TextureFile, out var file
                ))
            {
                file = new PckFile.FileData($"res/textures/{_atlasType}/{_selectedItem.TextureName}.png", PckFile.FileData.FileType.TextureFile);
            }

            AnimationEditor animationEditor;
            if (_selectedItem.Tile.HasColourEntry &&
                !string.IsNullOrWhiteSpace(_selectedItem.Tile.ColourEntry.DefaultName) &&
                _colourTable is not null)
            {
                Color blenColor = Color.White;
                if (_selectedItem.Tile.ColourEntry.IsWaterColour &&
                    _colourTable.WaterColors.FirstOrDefault(entry => entry.Name == _selectedItem.Tile.ColourEntry.DefaultName) is ColorContainer.WaterColor waterColor)
                {
                    blenColor = waterColor.SurfaceColor;
                }
                else if (_colourTable.Colors.FirstOrDefault(entry => entry.Name == _selectedItem.Tile.ColourEntry.DefaultName) is ColorContainer.Color color)
                {
                    blenColor = color.ColorPallette;
                }
                animationEditor = new AnimationEditor(file, blenColor);
            }
            else
            {
                animationEditor = new AnimationEditor(file);
            }

            if (animationEditor.ShowDialog() == DialogResult.OK && isNewFile)
            {
                _pckFile.Files.Add(file);
            }
        }

        private void extractTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Tile Texture|*.png",
                FileName = _selectedItem.TextureName
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectTilePictureBox.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    if (prevButton.Enabled)
                    {
                        prevButton_Click(this, EventArgs.Empty);
                        return true;
                    }
                    break;
                case Keys.Right:
                    if (nextButton.Enabled)
                    {
                        nextButton_Click(this, EventArgs.Empty);
                        return true;
                    }
                    break;
                case Keys.Up:
                    if (_textures.IndexInRange(_selectedItem.Index - _rowCount))
                    {
                        SelectedIndex = _selectedItem.Index - _rowCount;
                        return true;
                    }
                    break;

                case Keys.Down:
                    if (_textures.IndexInRange(_selectedItem.Index + _rowCount))
                    {
                        SelectedIndex = _selectedItem.Index + _rowCount;
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}