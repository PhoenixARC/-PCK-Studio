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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using OMI.Formats.Color;
using OMI.Workers.Color;
using PckStudio.Controls;
using PckStudio.Core;
using PckStudio.Core.Extensions;
using PckStudio.Core.IO;
using PckStudio.Core.Json;
using PckStudio.Interfaces;
using PckStudio.Internal;
using PckStudio.Json;
using PckStudio.Properties;

namespace PckStudio.Forms.Editor
{
    internal partial class TextureAtlasEditor : EditorForm<Atlas>
    {
        private readonly ITryGet<string, Animation> _tryGetAnimation;
        private readonly ITryGet<string, ISaveContext<Animation>> _tryGetAnimationSaveContext;
        private readonly AtlasResource _atlasResource;
        private readonly ColorContainer _colorTable;
        private readonly ResourceCategory _resourceLocationCategory;
        private readonly Atlas _atlas;

        private AtlasTile _selectedTile;
        private static readonly ColorDialog _colorPick = new ColorDialog
        {
            AllowFullOpen = true,
            AnyColor = true,
            SolidColorOnly = true,
            CustomColors = GameConstants.DyeColors.Select(ColorExtensions.ToBGR).ToArray()
        };
        private readonly FileObserver _fileObserver = new FileObserver();

        public TextureAtlasEditor(Atlas atlas, ISaveContext<Atlas> saveContext, ResourceLocation resourceLocation, ColorContainer colorContainer,
            ITryGet<string, Animation> tryGetAnimation, ITryGet<string, ISaveContext<Animation>> tryGetAnimationSaveContext)
            : base(atlas, saveContext)
        {
            InitializeComponent();

            _ = atlas ?? throw new ArgumentNullException(nameof(atlas));
            _ = resourceLocation ?? throw new ArgumentNullException(nameof(resourceLocation));
            _atlas = atlas;
            Text += " - ";
            Text += _atlas.Name;
            originalPictureBox.Image = atlas;

            _colorTable = colorContainer ?? AppResourceManager.Default.GetData(Resources.tu69colours, new COLFileReader());
            _tryGetAnimation = tryGetAnimation;
            _tryGetAnimationSaveContext = tryGetAnimationSaveContext;
            _atlasResource = resourceLocation as AtlasResource;
            _resourceLocationCategory = resourceLocation.Category;

            SelectedIndex = 0;

            animationButton.Enabled =
                _resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas) ||
                _resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas);

            // this is directly based on Java's source code for handling enchanted hits
            // the particle is assigned a random grayscale color between roughly 154 and 230
            // since critical hit is the only particle with this distinction, we just need to check the atlas type
            if (_resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.ParticleAtlas))
            {
                colorSlider.Minimum = 154;
                colorSlider.Maximum = 230;
                colorSlider.Value = colorSlider.Maximum;
            }
        }

        private int SelectedIndex
        {
            set
            {
                if (value < 0)
                {
                    value += _atlas.TileCount;
                }
                else if (value >= _atlas.TileCount)
                {
                    value -= _atlas.TileCount;
                }
                SetImageDisplayed(value);
            }
        }

        private readonly ImageLayoutDirection _imageLayout = ImageLayoutDirection.Horizontal;

        private readonly GraphicsConfig _graphicsConfig = GraphicsConfig.PixelPerfect();

        private void UpdateAtlasDisplay()
        {
            using Graphics g = Graphics.FromImage(originalPictureBox.Image);
            g.ApplyConfig(_graphicsConfig);
            g.Clear(Color.Transparent);
            Image image = EditorValue;
            g.DrawImage(image, 0, 0, image.Width, image.Height);

            SolidBrush brush = new SolidBrush(Color.FromArgb(127, Color.White));
            if (!allowGroupsToolStripMenuItem.Checked)
            {
                g.FillRectangle(brush, _selectedTile.GetArea(_atlas.TileSize));
                originalPictureBox.Invalidate();
                return;
            }
            foreach (Rectangle area in _atlas.GetTileArea(_selectedTile))
            {
                g.FillRectangle(brush, area);
            }
            originalPictureBox.Invalidate();
        }

        private void SetImageDisplayed(int index)
        {
            ResetView();
            _selectedTile = _atlas[index];

            if (_selectedTile is null)
                return;

            UpdateAtlasDisplay();

            selectTilePictureBox.Image = _selectedTile.Texture;
            selectTilePictureBox.BlendColor = GetBlendColor();

            JsonTileInfo tileInfo = _selectedTile.GetUserDataOfType<JsonTileInfo>();

            tileNameLabel.Text = $"{tileInfo?.DisplayName}";
            internalTileNameLabel.Text = $"{tileInfo?.InternalName}";
            setColorButton.Enabled = tileInfo.AllowCustomColour;
            variantComboBox.Enabled = Tiles.ColorEntries.TryGetValue(tileInfo.ColorKey, out JsonColorEntry colorEntry) && colorEntry?.Variants?.Length > 1 || _colorTable.Colors.Any(c => c.Name == tileInfo.ColorKey);

            if (variantComboBox.Enabled)
            {
                if (colorEntry?.IsWaterColour ?? default)
                {
                    foreach (ColorContainer.WaterColor col in _colorTable.WaterColors)
                    {
                        if (!variantComboBox.Items.Contains(col.Name))
                            variantComboBox.Items.Add(col.Name);
                    }
                }

                // TODO: only add variants that are available in the color table
                if (_colorTable.Colors.Where(c => c.Name == tileInfo.ColorKey).FirstOrDefault() is ColorContainer.Color c)
                    variantComboBox.Items.Add(c.Name);
                if (_colorTable.WaterColors.Where(c => c.Name == tileInfo.ColorKey).FirstOrDefault() is ColorContainer.WaterColor wc)
                    variantComboBox.Items.Add(wc.Name);
                if (tileInfo.HasColourEntry)
                {
                    if (!string.IsNullOrWhiteSpace(colorEntry.DefaultName))
                        variantComboBox.Items.Add(colorEntry.DefaultName);
                    variantComboBox.Items.AddRange(colorEntry.Variants.Where(colorName => _colorTable.Colors.Any(c => c.Name == colorName) || _colorTable.WaterColors.Any(c => c.Name == colorName)).ToArray());
                }

                if (variantComboBox.Items.Count > 0)
                {
                    variantComboBox.Visible = variantComboBox.Items.Count > 1;
                    variantComboBox.SelectedIndex = 0;
                }
            }

            CheckForAnimation();
        }

        private void CheckForAnimation()
        {
            selectTilePictureBox.Image = _selectedTile.Texture;
            JsonTileInfo tileInfo = _selectedTile.GetUserDataOfType<JsonTileInfo>();
            if (animationButton.Enabled &&
                           (_resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas) ||
                            _resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas)))
            {
                ResourceCategory animationResourceCategory = _resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas)
                    ? ResourceCategory.ItemAnimation
                    : ResourceCategory.BlockAnimation;

                string animationAssetPath = $"{ResourceLocations.GetPathFromCategory(animationResourceCategory)}/{tileInfo.InternalName}";
                bool hasAnimation = _tryGetAnimation.TryGet(animationAssetPath, out Animation animation);
                animationButton.Text = hasAnimation ? "Edit Animation" : "Create Animation";

                if (playAnimationsToolStripMenuItem.Checked && hasAnimation)
                {
                    selectTilePictureBox.UseBlendColor = false;
                    selectTilePictureBox.Image = animation.CreateAnimationImage(selectTilePictureBox.BlendColor);
                    selectTilePictureBox.Start();
                    return;
                }
            }

            if (_selectedTile.IsPartOfGroup && allowGroupsToolStripMenuItem.Checked)
            {
                AtlasGroup group = _selectedTile.GetGroup();
                if (!string.IsNullOrWhiteSpace(group.Name))
                    tileNameLabel.Text = group.Name;
                internalTileNameLabel.Text = string.Empty;
                if (group.IsAnimation())
                {
                    animationButton.Enabled = true;
                    animationButton.Text = "Edit as Animation";
                    if (playAnimationsToolStripMenuItem.Checked)
                    {
                        selectTilePictureBox.UseBlendColor = false;
                        selectTilePictureBox.Image = _atlas.GetAnimationFromGroup(group).CreateAnimationImage(selectTilePictureBox.BlendColor);
                        selectTilePictureBox.Start();
                        return;
                    }
                }
                selectTilePictureBox.UseBlendColor = false;
                setColorButton.Enabled |= group.AllowCustomColor;
                selectTilePictureBox.Image = _atlas.GetTileTexture(_selectedTile, selectTilePictureBox.BlendColor);
            }
        }

        private void ResetView()
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
            selectTilePictureBox.UseBlendColor = true;
            selectTilePictureBox.Image = null;
        }

        private void ResetView()
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
            Debug.WriteLine(result);
            return GetSelectedIndex(result.X, result.Y, rowCount, columnCount, imageLayout);
        }

        private static int GetSelectedIndex(int x, int y, int rowCount, int columnCount, ImageLayoutDirection imageLayout)
        {
            return imageLayout switch
            {
                ImageLayoutDirection.Horizontal => x + y * rowCount,
                ImageLayoutDirection.Vertical => y + x * columnCount,
                _ => throw new ArgumentOutOfRangeException(nameof(imageLayout)),
            };
        }

        private void SetTile(Image texture)
        {
            if (_selectedTile.IsPartOfGroup && allowGroupsToolStripMenuItem.Checked)
            {
                AtlasGroup group = _selectedTile.GetGroup();
                _atlas.SetGroup(group, texture);
                selectTilePictureBox.Image = _atlas.GetTileTexture(_selectedTile, selectTilePictureBox.BlendColor);
                UpdateAtlasDisplay();
                return;
            }

            if (texture.Size != _atlas.TileSize)
                texture = texture.Resize(_atlas.TileSize, _graphicsConfig);

            _selectedTile.Texture = texture;
            selectTilePictureBox.Image = texture;
            UpdateAtlasDisplay();
        }

        private Color GetBlendColor()
        {
            if (_selectedTile.TryGetUserDataOfType(out JsonTileInfo tileInfo))
            {
                string colorKey = tileInfo.InternalName;

                colorSlider.Visible = colorSliderLabel.Visible = true;
                // Simply, Experience orbs red value is just sliding between 255 and 0
                if (colorKey.StartsWith("experience_orb"))
                    return Color.FromArgb(colorSlider.Value, 255, 0);

                // Similar story for critical hits, but for all values
                if (colorKey == "critical_hit")
                    return Color.FromArgb(colorSlider.Value, colorSlider.Value, colorSlider.Value);

                // Enchanted hits are modified critical hit particles
                if (colorKey == "enchanted_hit")
                {
                    // This is directly based on Java's source code for handling enchanted hits
                    // it just multiplies the red by 0.3 and green by .8 of the color assigned to the critical hit particle
                    var c = Color.FromArgb(colorSlider.Value, colorSlider.Value, colorSlider.Value);
                    return Color.FromArgb((byte)(c.R * 0.3f), (byte)(c.G * 0.8f), c.B);
                }
                colorSlider.Visible = colorSliderLabel.Visible = false;
                if (tileInfo.HasColourEntry)
                    return FindBlendColorByKey(tileInfo.ColorEntry.DefaultName);
            }
            return Color.White;
        }

        private Color FindBlendColorByKey(string colorKey, bool isWaterColour = default)
        {
            if (string.IsNullOrWhiteSpace(colorKey))
                return Color.White;

            // Enchanted hits are modified critical hit particles
            if (tileInfo.InternalName == "enchanted_hit")
            {
                // This is directly based on Java's source code for handling enchanted hits
                // it just multiplies the red by 0.3 and green by .8 of the color assigned to the critical hit particle
                var c = Color.FromArgb(colorSlider.Value, colorSlider.Value, colorSlider.Value);
                return Color.FromArgb((int)(c.R * 0.3f), (int)(c.R * 0.8f), c.B);
            }

            // basic way to check for classic water colors
            if (!isWaterColour && !colorKey.StartsWith("Water_") && _colorTable.Colors.FirstOrDefault(entry => entry.Name == colorKey) is ColorContainer.Color color)
            {
                return color.ColorPallette;
            }
            if (_colorTable.WaterColors.FirstOrDefault(entry => entry.Name == colorKey) is ColorContainer.WaterColor waterColor)
            {
                return waterColor.SurfaceColor;
            }

            Debug.WriteLine("Could not find: " + colorKey);
            return Color.White;
        }

        // TODO(null): check for large tile and get skip length
        protected override bool ProcessDialogKey(Keys keyData)
        {
            int up = -_atlas.Rows;
            int down = _atlas.Rows;
            int left = -1;
            int right = 1;
            if (_selectedTile.IsPartOfGroup)
            {
                AtlasGroup group = _selectedTile.GetGroup();
                if (!group.IsComposedOfMultipleTiles())
                {
                    Size s = group.GetSize(new Size(1, 1));
                    up = -_atlas.Rows * (group.Column - _selectedTile.Column + 1);
                    down = _atlas.Rows * (group.Column - _selectedTile.Column + s.Height);
                    left = group.Row - _selectedTile.Row - 1;
                    right = group.Row - _selectedTile.Row + s.Width;
                }
            }
            switch (keyData)
            {
                case Keys.R:
                    // Refreshes the specific tile
                    SelectedIndex = _selectedTile.Index;
                    return true;
                case Keys.Left:
                    SelectedIndex = _selectedTile.Index + left;
                    return true;
                case Keys.Right:
                    SelectedIndex = _selectedTile.Index + right;
                    return true;
                case Keys.Up:
                    SelectedIndex = _selectedTile.Index + up;
                    return true;
                case Keys.Down:
                    SelectedIndex = _selectedTile.Index + down;
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
                ((Image)_atlas).Size,
                _atlas.TileSize,
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
                Image img = Image.FromFile(fileDialog.FileName).ReleaseFromFile();
                SetTile(img);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
            DialogResult = DialogResult.OK;
        }

        private void animationButton_Click(object sender, EventArgs e)
        {
            if (_selectedTile.IsPartOfGroup)
            {
                AtlasGroup group = _selectedTile.GetGroup();
                Animation anim = _atlas.GetAnimationFromGroup(group);
                ISaveContext<Animation> saveContext = new DelegatedSaveContext<Animation>(false, (animation) =>
                {
                    //! TODO(null): Test for functionallity
                    _atlas.SetGroupTilesFromAnimation(group, animation);
                });
                var aEditor = new AnimationEditor(anim, saveContext, group.Name, false);
                aEditor.ShowDialog(this);
                return;
            }
            JsonTileInfo tileInfo = _selectedTile.GetUserDataOfType<JsonTileInfo>();
            ResourceCategory animationResourceCategory = _resourceLocationCategory == AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas) ? ResourceCategory.ItemAnimation : ResourceCategory.BlockAnimation;
            string animationAssetPath = $"{ResourceLocations.GetPathFromCategory(animationResourceCategory)}/{tileInfo.InternalName}";
            bool hasAnimation = _tryGetAnimation.TryGet(animationAssetPath, out Animation animation);
            bool isValidAnimationSaveContext = _tryGetAnimationSaveContext.TryGet(animationAssetPath, out ISaveContext<Animation> animationSaveContext);

            Debug.Assert(isValidAnimationSaveContext, "Couldn't get valid animation save context.");

            var animationEditor = new AnimationEditor(hasAnimation ? animation : Animation.CreateEmpty(), animationSaveContext, tileInfo.DisplayName);
            if (animationEditor.ShowDialog(this) == DialogResult.OK)
            {
                // so animations can automatically update upon saving
                SelectedIndex = _selectedTile.Index;
            }
        }

        private void extractTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = GetSanitizedFilename();
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Tile Texture|*.png",
                FileName = filename
            };
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _atlas.GetTileTexture(_selectedTile, selectTilePictureBox.BlendColor).Save(saveFileDialog.FileName, ImageFormat.Png);
                Action<AtlasTile, FileInfo> onChange = new Action<AtlasTile, FileInfo>((tile, fileInfo) =>
                {
                    Image img = Image.FromFile(fileInfo.FullName).ReleaseFromFile();
                    bool success = _atlas.SetTile(tile, img);
                    if (!success)
                    {
                        Size s = _atlas.GetTileArea(tile).FirstOrDefault().Size;
                        new ToastContentBuilder()
                            .AddText("Invalid Image Dimensions")
                            .AddText($"Required: {s.Width}x{s.Height} Recived: {img.Width}x{img.Height}")
                            .AddInlineImage(new Uri(fileInfo.FullName))
                            .Show();
                        return;
                    }
                    UpdateAtlasDisplay();
                });
                _fileObserver.AddFileWatcher(saveFileDialog.FileName, onChange, _selectedTile);
            }
        }

        private string GetSanitizedFilename()
        {
            if (_selectedTile is null)
                return "tile";

            if (_selectedTile.IsPartOfGroup)
            {
                AtlasGroup group = _selectedTile.GetGroup();
                return group.Name.Replace(' ', '_').Trim().ToLower();
            }
            if (_selectedTile.TryGetUserDataOfType(out JsonTileInfo tileInfo) && !string.IsNullOrWhiteSpace(tileInfo.InternalName))
                return tileInfo.InternalName;
            return "tile";
        }

        private void variantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedTile.TryGetUserDataOfType(out JsonTileInfo tileInfo) && variantComboBox.SelectedItem is not null)
            {
                string colorKey = variantComboBox.SelectedItem.ToString();
                Color blendColor = FindBlendColorByKey(colorKey);
                selectTilePictureBox.BlendColor = blendColor;
                CheckForAnimation();
            }
        }

        private void applyColorMaskToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            selectTilePictureBox.UseBlendColor = applyColorMaskToolStripMenuItem.Checked;
        }

        private void playAnimationsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            CheckForAnimation();
        }

        private void TextureAtlasEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _fileObserver.Dispose();
            if (selectTilePictureBox.IsPlaying)
                selectTilePictureBox.Stop();
        }

        private void setColorButton_Click(object sender, EventArgs e)
        {
            if (_colorPick.ShowDialog(this) != DialogResult.OK)
                return;

            selectTilePictureBox.BlendColor = _colorPick.Color;
            variantComboBox.Enabled = false;
            clearColorButton.Enabled = true;
            CheckForAnimation();
        }

        private void clearColorButton_Click(object sender, EventArgs e)
        {
            variantComboBox.Enabled = true;

            selectTilePictureBox.BlendColor = Color.White;

            clearColorButton.Enabled = false;
            CheckForAnimation();
        }

        private void colorSlider_ValueChanged(object sender, EventArgs e)
        {
            selectTilePictureBox.BlendColor = GetBlendColor();
        }

        private void allowGroupsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            CheckForAnimation();
        }
    }
}