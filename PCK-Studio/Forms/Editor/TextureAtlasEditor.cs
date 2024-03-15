/* Copyright (c) 2023-present miku-666, MattNL
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
        private Image _workingTexture;
        public Image FinalTexture
        {
            get
            {
                if (DialogResult != DialogResult.OK)
                    return null;
                return _workingTexture;
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
                    value = _tiles.Count + value;
                }
                else if (value >= _tiles.Count)
                {
                    value = value - _tiles.Count;
                }
                SetImageDisplayed(value);
            }
        }

        private const ImageLayoutDirection _imageLayout = ImageLayoutDirection.Horizontal;

        public TextureAtlasEditor(PckFile pckFile, string path, Image atlas, Size areaSize)
        {
            InitializeComponent();

            AcquireColorTable(pckFile);

            _workingTexture = atlas;

            _areaSize = areaSize;
            _pckFile = pckFile;
            _rowCount = atlas.Width / areaSize.Width;
            _columnCount = atlas.Height / areaSize.Height;
            (var tileInfos, _atlasType) = Path.GetFileNameWithoutExtension(path) switch
            {
                "terrain" => (Tiles.BlockTileInfos, "blocks"),
                "items" => (Tiles.ItemTileInfos, "items"),
                "particles" => (Tiles.ParticleTileInfos, "particles"),
                "mapicons" => (Tiles.MapIconTileInfos, "map_icons"),
                "additionalmapicons" => (Tiles.AdditionalMapIconTileInfos, "additional_map_icons"),
                "moon_phases" => (Tiles.MoonPhaseTileInfos, "moon_phases"),
                "xporb" => (Tiles.ExperienceOrbTileInfos, "experience_orbs"),
                "explosion" => (Tiles.ExplosionTileInfos, "explosion"),
                _ => (null, null),
            };

            originalPictureBox.Image = atlas.GetArea(new Rectangle(0, 0, atlas.Width, atlas.Height));

            var images = atlas.Split(_areaSize, _imageLayout);

            var tiles = images.enumerate().Select(
                    p => new AtlasTile(
                        p.index, 

                        GetAtlasArea(
                            p.index, 
                            tileInfos.IndexInRange(p.index) 
                                ? tileInfos[p.index].Width : 1,                        
                            tileInfos.IndexInRange(p.index) 
                                ? tileInfos[p.index].Height : 1, 
                            _rowCount, 
                            _columnCount, 
                            _areaSize, 
                            _imageLayout),

                        tileInfos.IndexInRange(p.index)
                            ? tileInfos[p.index] : null,

                        // get texture for tiles that are not 1x1 tiles
                        tileInfos.IndexInRange(p.index)
                            ? atlas.GetArea(
                                new Rectangle(
                                    GetSelectedPoint(p.index, _rowCount, _columnCount, _imageLayout).X * _areaSize.Width,
                                    GetSelectedPoint(p.index, _rowCount, _columnCount, _imageLayout).Y * _areaSize.Height,
                                tileInfos[p.index].Width * _areaSize.Width,
                                tileInfos[p.index].Height * _areaSize.Height))
                            : p.value)
                );
            _tiles = new List<AtlasTile>(tiles);

            SelectedIndex = 0;

            bool isParticles = _atlasType == "particles";

            // this is directly based on Java's source code for handling enchanted hits
            // the particle is assigned a random grayscale color between roughly 154 and 230
            // since critical hit is the only particle with this distinction, we just need to check the atlas type
            colorSlider.Maximum = isParticles ? 230 : 255;
            colorSlider.Minimum = isParticles ? 154 : 0;
            colorSlider.Value = isParticles ? colorSlider.Maximum : colorSlider.Minimum;
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

        private void UpdateAtlasDisplay()
        {
            var graphicsConfig = new GraphicsConfig()
            {
                InterpolationMode = selectTilePictureBox.InterpolationMode,
                PixelOffsetMode = PixelOffsetMode.HighQuality
            };
            using (var g = Graphics.FromImage(originalPictureBox.Image))
            {
                g.ApplyConfig(graphicsConfig);
                g.Clear(Color.Transparent);
                g.DrawImage(_workingTexture, 0, 0, _workingTexture.Width, _workingTexture.Height);

                SolidBrush brush = new SolidBrush(Color.FromArgb(127, 255, 255, 255));

                var rect = new Rectangle(_selectedTile.Area.X, _selectedTile.Area.Y,
                    _areaSize.Width, _areaSize.Height);

                g.FillRectangle(brush, rect);
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
            variantComboBox.Items.Clear();
            variantComboBox.SelectedItem = null;
            variantComboBox.Enabled = false;

            if (selectTilePictureBox.IsPlaying)
                selectTilePictureBox.Stop();
            selectTilePictureBox.UseBlendColor = false;
            selectTilePictureBox.Image = null;

            if (_tiles is null || !_tiles.IndexInRange(index) || (_selectedTile = _tiles[index]) is null)
                return;

            dataTile = _selectedTile;

            UpdateAtlasDisplay();

            if (string.IsNullOrEmpty(dataTile.Tile.DisplayName) && !string.IsNullOrEmpty(dataTile.Tile.InternalName))
            {
                dataTile = _tiles.Find(t => t.Tile.InternalName == _selectedTile.Tile.InternalName);
            }

            selectTilePictureBox.Image = dataTile.Texture;
            tileNameLabel.Text = $"{dataTile.Tile.DisplayName}";
            internalTileNameLabel.Text = $"{dataTile.Tile.InternalName}";
            selectTilePictureBox.BlendColor = GetBlendColor();
            selectTilePictureBox.UseBlendColor = applyColorMaskToolStripMenuItem.Checked;

            if (animationButton.Enabled = _atlasType == "blocks" || _atlasType == "items")
            {
                bool hasAnimation =
                    _pckFile.TryGetValue($"res/textures/{_atlasType}/{dataTile.Tile.InternalName}.png", PckFileType.TextureFile, out var animationFile);
                animationButton.Text = hasAnimation ? "Edit Animation" : "Create Animation";

                if (playAnimationsToolStripMenuItem.Checked &&
                    hasAnimation &&
                    animationFile.Size > 0)
                {
                    var animation = AnimationHelper.GetAnimationFromFile(animationFile);
                    selectTilePictureBox.Start(animation);
                }
            }

            if (setColorButton.Enabled = clearColorButton.Enabled = dataTile.Tile.HasColourEntry)
            {
                setColorButton.Enabled = clearColorButton.Enabled = dataTile.Tile.ColourEntry.HasCustomColour;
                clearColorButton.Enabled = false;

                variantComboBox.Enabled = variantComboBox.Visible = dataTile.Tile.ColourEntry.Variants.Length > 1;

                if (dataTile.Tile.ColourEntry.IsWaterColour && _colourTable.WaterColors.Count > 0)
                {
                    foreach (var col in _colourTable.WaterColors)
                    {
                        if(!variantComboBox.Items.Contains(col.Name))
                            variantComboBox.Items.Add(col.Name);
                    }

                    dataTile.Tile.ColourEntry.DefaultName = _colourTable.WaterColors[0].Name;
                }

                variantComboBox.Items.AddRange(dataTile.Tile.ColourEntry.Variants);

                variantComboBox.SelectedItem = dataTile.Tile.ColourEntry.DefaultName;
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

        private static Rectangle GetAtlasArea(int index, int width, int height, int rowCount, int columnCount, Size size, ImageLayoutDirection imageLayout)
        {
            var p = GetSelectedPoint(index, rowCount, columnCount, imageLayout);
            var ap = new Point(p.X * size.Width, p.Y * size.Height);
            return new Rectangle(ap, new Size(size.Width * width, size.Height * height));
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
            using (var g = Graphics.FromImage(_workingTexture))
            {
                g.ApplyConfig(graphicsConfig);
                g.Fill(dataTile.Area, Color.Transparent);
                g.DrawImage(texture, dataTile.Area);
            }

            _tiles[_selectedTile.Index] = new AtlasTile(_selectedTile.Index, _selectedTile.Area, _selectedTile.Tile, texture);
            selectTilePictureBox.Image = texture;

            UpdateAtlasDisplay();
        }

        private Color GetBlendColor()
        {
            if (dataTile.Tile.HasColourEntry && dataTile.Tile.ColourEntry is not null)
            {
                var col = FindBlendColorByKey(dataTile.Tile.ColourEntry.DefaultName);
                return col;
            }
            
            return Color.White;
        }

        private Color HandleSpecialTiles(string colorKey)
        {
            colorSlider.Visible = colorSliderLabel.Visible = true;

            // Simply, Experience orbs red value is just sliding between 255 and 0
            if (colorKey == "experience_orb") return Color.FromArgb(colorSlider.Value, 255, 0);

            //similar story for critical hits, but for all values
            var final_color = Color.FromArgb(colorSlider.Value, colorSlider.Value, colorSlider.Value);

            // enchanted hits are modified critical hit particles
            if (dataTile.Tile.InternalName == "enchanted_hit")
                // this is directly based on Java's source code for handling enchanted hits
                // it just multiplies the red by 0.3 and green by .8 of the color assigned to the critical hit particle
                final_color = Color.FromArgb((int)(final_color.R * 0.3f), (int)(final_color.R * 0.8f), final_color.B);

            return final_color;
        }

        private Color FindBlendColorByKey(string colorKey)
        {
            // The following tiles are hardcoded within a range and do not have color table entries
            if (colorKey == "experience_orb" || colorKey == "critical_hit") 
                return HandleSpecialTiles(colorKey);

            if (_colourTable is not null &&
                dataTile.Tile.HasColourEntry &&
                dataTile.Tile.ColourEntry is not null)
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
                _workingTexture.Size,
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
            var file = _pckFile.GetOrCreate(
                    $"res/textures/{_atlasType}/{_selectedTile.Tile.InternalName}.png",
                    PckFileType.TextureFile
                );

            var animation = AnimationHelper.GetAnimationFromFile(file);

            var animationEditor = new AnimationEditor(animation, _selectedTile.Tile.InternalName, GetBlendColor());
            if (animationEditor.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            AnimationHelper.SaveAnimationToFile(file, animation);
            // so animations can automatically update upon saving
            SelectedIndex = _selectedTile.Index;
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
                dataTile.Texture.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void variantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataTile.Tile.ColourEntry is not null)
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

            // custom colors are read as BGR for some reason, so hex values are "backwards"
            // values below are the default Minecraft dyed leather armor values for convenience

            colorPick.CustomColors = new int[] {
                0xfefff9, // White
                0x1d80f9, // Orange
                0xbd4ec7, // Magenta
                0xdab33a, // Light Blue
                0x3dd8fe, // Yellow
                0x1fc780, // Lime
                0xaa8bf3, // Pink
                0x524f47, // Gray
                0x979d9d, // Light Gray
                0x9c9c16, // Cyan
                0xb83289, // Purple
                0xaa443c, // Blue
                0x325483, // Brown
                0x167c5e, // Green
                0x262eb0, // Red
                0x211d1d  // Black
            };
            
            if (colorPick.ShowDialog() != DialogResult.OK) return;

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