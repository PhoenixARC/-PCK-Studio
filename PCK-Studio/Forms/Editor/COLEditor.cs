using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OMI.Formats.Color;

using OMI.Workers.Color;
using PckStudio.Core.Extensions;
using PckStudio.Properties;
using PckStudio.Controls;
using PckStudio.Interfaces;

namespace PckStudio.Forms.Editor
{
    public partial class COLEditor : EditorForm<ColorContainer>
    {
        ColorContainer _defaultColourfile;
        string _clipboard_color = "#FFFFFF";

        TreeView colorTreeView;
        TreeView waterTreeView;
        TreeView underwaterTreeView;
        TreeView fogTreeView;

        private static readonly ICollection<string> PS4Biomes = new List<string>
        {
            "bamboo_jungle",
            "bamboo_jungle_hills",
            "mesa_mutated",
            "mega_spruce_taiga_mutated",
            "mega_taiga_mutated"
        };

        public COLEditor(ColorContainer colorContainer, ISaveContext<ColorContainer> saveContext)
            : base(colorContainer, saveContext)
        {
            InitializeComponent();

            saveToolStripMenuItem1.Visible = !saveContext.AutoSave;

            TU12ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 0);
            TU13ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 1);
            TU14ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 2);
            TU19ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 3);
            TU31ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 4);
            TU32ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 5);
            TU43ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 6);
            TU46ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 7);
            TU51ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 8);
            TU53ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 9);
            TU54ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 10);
            TU69ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 11);
            _1_9_1ToolStripMenuItem.Click += (sender, e) => SetUpDefaultFile(sender, e, 12);

            colorTreeView = filterPrompt.AddFilterPage("All", null, null);
            waterTreeView = filterPrompt.AddFilterPage("Water", null, null);
            underwaterTreeView = filterPrompt.AddFilterPage("Underwater", null, null);
            fogTreeView = filterPrompt.AddFilterPage("Fog", null, null);

            colorTreeView.KeyDown += treeView1_KeyDown;
            waterTreeView.KeyDown += treeView1_KeyDown;
            underwaterTreeView.KeyDown += treeView1_KeyDown;
            fogTreeView.KeyDown += treeView1_KeyDown;

            BuildTreeViews(EditorValue);
        }

        private void BuildTreeViews(ColorContainer c)
        {
            colorTreeView.Nodes.Clear();
            waterTreeView.Nodes.Clear();
            underwaterTreeView.Nodes.Clear();
            fogTreeView.Nodes.Clear();
            int nodeDepth = Convert.ToInt32(compactViewToolStripMenuItem.Checked);
            c.Colors.ForEach(col =>
            {
                TreeNode n = colorTreeView.Nodes.BuildNodeTreeBySeperator(col.Name, '_', nodeDepth);
                //n.Text = n.Text.Replace('_', '.');
                n.Tag = col;
            });

            c.WaterColors.ForEach(col =>
            {
                waterTreeView.Nodes.BuildNodeTreeBySeperator(col.Name, '_', nodeDepth).Tag = col;
                underwaterTreeView.Nodes.BuildNodeTreeBySeperator(col.Name, '_', nodeDepth).Tag = col;
                fogTreeView.Nodes.BuildNodeTreeBySeperator(col.Name, '_', nodeDepth).Tag = col;
            });

            filterPrompt.Update();
        }

        private void SetUpDefaultFile(object sender, EventArgs e, int id, bool showMessageBox = true)
        {
            if (showMessageBox)
            {
                DialogResult result = MessageBox.Show(this, "This function will set up your colour table to match that of the chosen version. You may lose some entries in the table. Are you sure you would like to continue?", "Target update version?", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            byte[] colorData = id switch
            {
                0 => Resources.tu12colours,
                1 => Resources.tu13colours,
                2 => Resources.tu14colours,
                3 => Resources.tu19colours,
                4 => Resources.tu31colours,
                5 => Resources.tu32colours,
                6 => Resources.tu43colours,
                7 => Resources.tu46colours,
                8 => Resources.tu51colours,
                9 => Resources.tu53colours,
                10 => Resources.tu54colours,
                11 => Resources.tu69colours,
                12 => Resources._1_91_colours,
                _ => throw new ArgumentOutOfRangeException(nameof(id)),
            };
            var reader = new COLFileReader();
            using (var stream = new MemoryStream(colorData))
            {
                _defaultColourfile = reader.FromStream(stream);
            }

            BuildTreeViews(_defaultColourfile);
        }

        void SetUpValueChanged(bool add)
        {
            if (add)
            {
                redUpDown.ValueChanged += color_ValueChanged;
                greenUpDown.ValueChanged += color_ValueChanged;
                blueUpDown.ValueChanged += color_ValueChanged;
            }
            else
            {
                redUpDown.ValueChanged -= color_ValueChanged;
                greenUpDown.ValueChanged -= color_ValueChanged;
                blueUpDown.ValueChanged -= color_ValueChanged;
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Save();
            DialogResult = DialogResult.OK;
        }

        public void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TreeView treeView)
                return;
            TreeNode node = treeView.SelectedNode;
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    restoreOriginalColorToolStripMenuItem_Click(sender, e);
                    break;
                case Keys.C when e.Control:
                    copyColorToolStripMenuItem_Click(sender, e);
                    break;
                case Keys.V when e.Control:
                    pasteColorToolStripMenuItem_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void colorBox_TextChanged(object sender, EventArgs e)
        {
            if (IsValidHexColor(colorTextbox.Text) && int.TryParse(colorTextbox.Text, System.Globalization.NumberStyles.HexNumber, null, out int argbColor))
            {
                SetColor(Color.FromArgb(argbColor));
            }
        }

        private void color_ValueChanged(object sender, EventArgs e)
        {
            Color color = Color.FromArgb(filterPrompt.SelectedTabIndex == 1 ? (int)alphaUpDown.Value : 255, (int)redUpDown.Value, (int)greenUpDown.Value, (int)blueUpDown.Value);

            if (filterPrompt.SelectedTabIndex == 0 && filterPrompt.SelectedItem is ColorContainer.Color colorEntry)
            {
                colorEntry.ColorPallette = color;
            }
            else if (filterPrompt.SelectedItem is ColorContainer.WaterColor waterColorEntry)
            {
                if (filterPrompt.SelectedTabIndex == 1)
                    waterColorEntry.SurfaceColor = color;
                else if (filterPrompt.SelectedTabIndex == 2)
                    waterColorEntry.UnderwaterColor = color;
                else
                    waterColorEntry.FogColor = color;
            }

            pictureBox1.BackColor = color;
            colorTextbox.Text = color.ToArgb().ToString("X06");
        }

        private void setColorBtn_Click(object sender, EventArgs e)
        {
            using ColorDialog colorPick = new ColorDialog();
            colorPick.AllowFullOpen = true;
            colorPick.AnyColor = true;
            colorPick.SolidColorOnly = filterPrompt.SelectedTabIndex == 0;
            if (colorPick.ShowDialog(this) != DialogResult.OK)
                return;
            if (filterPrompt.SelectedTabIndex == 1 && filterPrompt.SelectedItem is ColorContainer.WaterColor waterColor)
            {
                // preserves the alpha so the user can handle it since the color picker doesn't support alpha
                waterColor.SurfaceColor = Color.FromArgb(waterColor.SurfaceColor.A, colorPick.Color);
                SetColor(waterColor.SurfaceColor);
            }
            else if (filterPrompt.SelectedTabIndex == 2 && filterPrompt.SelectedItem is ColorContainer.WaterColor underwaterColor)
            {
                underwaterColor.UnderwaterColor = Color.FromArgb(0, colorPick.Color);
                SetColor(underwaterColor.UnderwaterColor);
            }
            else if (filterPrompt.SelectedTabIndex == 3 && filterPrompt.SelectedItem is ColorContainer.WaterColor fogColor)
            {
                fogColor.FogColor = Color.FromArgb(0, colorPick.Color);
                SetColor(fogColor.FogColor);
            }
            else if (filterPrompt.SelectedTabIndex == 0 && filterPrompt.SelectedItem is ColorContainer.Color color)
            {
                color.ColorPallette = colorPick.Color;
                SetColor(color.ColorPallette);
            }
        }

        private void alpha_ValueChanged(object sender, EventArgs e)
        {
            var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
            pictureBox1.BackColor = colorEntry.SurfaceColor = Color.FromArgb((int)alphaUpDown.Value, colorEntry.SurfaceColor);
        }

        private void restoreOriginalColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (tabControl.SelectedTab is var tab)
            //{
            //    Color color = Color.Empty;

            //    if (tab == colorsTab)
            //    {
            //        ColorContainer.Color col_entry = _defaultColourfile.Colors.Find(color => color.Name == node.Text);

            //        if (col_entry == null)
            //            return;

            //        color = col_entry.ColorPallette;

            //        colorTextbox.Text = ColorTranslator.ToHtml(color).TrimStart('#');
            //    }
            //    else
            //    {
            //        ColorContainer.WaterColor waterEntry = _defaultColourfile.WaterColors.Find(color => color.Name == node.Text);

            //        if (waterEntry == null)
            //            return;

            //        color =
            //            tab == waterTab ? waterEntry.SurfaceColor :
            //            tab == underwaterTab ? waterEntry.UnderwaterColor : waterEntry.FogColor;

            //        if (tab == waterTab)
            //        {
            //            alphaUpDown.Value = color.A;
            //        }

            //        redUpDown.Value = color.R;
            //        greenUpDown.Value = color.G;
            //        blueUpDown.Value = color.B;
            //    }
            //    pictureBox1.BackColor = Color.FromArgb(tab == colorsTab ? 0xFF : color.A, color);
            //}
        }

        private void copyColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _clipboard_color = colorTextbox.Text;
            Clipboard.SetText(_clipboard_color);
        }

        private void pasteColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorTextbox.Text = _clipboard_color;
        }

        private void colorTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHexChar(e.KeyChar);
        }

        enum ColorSpace
        {
            RGB8,
            ARGB8,
        }

        private static int GetCharCountForColorSpace(ColorSpace colorSpace)
        {
            return colorSpace switch
            {
                ColorSpace.RGB8 => 6,
                ColorSpace.ARGB8 => 8,
                _ => 0,
            };
        }

        private static bool IsHexChar(char c) => char.IsDigit(c) || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        private static bool IsValidHexColor(string value, ColorSpace colorSpace = ColorSpace.RGB8) => value.All(IsHexChar) && value.Length == GetCharCountForColorSpace(colorSpace);

        private void stripPS4BiomesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorValue.WaterColors.RemoveAll(col => PS4Biomes.Contains(col.Name));
            BuildTreeViews(EditorValue);
        }

        private void addColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filterPrompt.SelectedTabIndex == 0)
            {
                using (TextPrompt prompt = new TextPrompt())
                {
                    prompt.OKButtonText = "Add";
                    prompt.contextLabel.Text = "Insert the name of the color you would like to add";
                    if (prompt.ShowDialog(this) == DialogResult.OK)
                    {
                        ColorContainer.Color entry = new ColorContainer.Color();
                        entry.Name = prompt.NewText;
                        entry.ColorPallette = Color.White;

                        if (EditorValue.Colors.Find(c => c.Name == entry.Name) != null)
                        {
                            MessageBox.Show(this, $"\"{entry.Name}\" already exists in this color table", "Color not added");
                        }

                        EditorValue.Colors.Add(entry);
                        BuildTreeViews(EditorValue);
                    }
                }
            }
        }

        private void removeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filterPrompt.SelectedItem is ColorContainer.Color color)
            {
                EditorValue.Colors.Remove(color);
            }
        }

        private void SetColor(Color color)
        {
            redUpDown.Value = color.R;
            greenUpDown.Value = color.G;
            blueUpDown.Value = color.B;

            pictureBox1.BackColor = Color.FromArgb(color.A == 0 ? 0xff : color.A, color);
            colorTextbox.Text = color.ToArgb().ToString("X06");
        }

        private void filterPrompt_OnSelectedItemChanged(object sender, EventArgs e)
        {
            if (filterPrompt.SelectedItem is ColorContainer.Color col)
            {
                SetColor(col?.ColorPallette ?? Color.AliceBlue);
                return;
            }
            if (filterPrompt.SelectedItem is ColorContainer.WaterColor waterColor)
            {
                SetColor(waterColor?.SurfaceColor ?? Color.AliceBlue);
                return;
            }
            SetColor(Color.Black);
        }

        private void compactViewToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            BuildTreeViews(EditorValue);
        }
    }
}
