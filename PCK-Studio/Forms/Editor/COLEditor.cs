using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OMI.Formats.Color;

using OMI.Workers.Color;
using PckStudio.Core.Extensions;
using PckStudio.Core.Colors;
using PckStudio.Properties;
using PckStudio.Controls;
using PckStudio.Interfaces;

namespace PckStudio.Forms.Editor
{
    public partial class COLEditor : EditorForm<AbstractColorContainer>
    {
        AbstractColorContainer _defaultColourfile;
        string _clipboard_color = "#FFFFFF";

        TreeView colorTreeView;
        TreeView waterTreeView;
        TreeView underwaterTreeView;
        TreeView fogTreeView;

        private static readonly ICollection<string> PS4BiomeColors = new List<string>
        {
            "bamboo_jungle",
            "bamboo_jungle_hills",
            "mesa_mutated",
            "mega_spruce_taiga_mutated",
            "mega_taiga_mutated"
        };

        public COLEditor(AbstractColorContainer colorContainer, ISaveContext<AbstractColorContainer> saveContext)
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

            colorTreeView.ContextMenuStrip = ColorContextMenu;
            waterTreeView.ContextMenuStrip = ColorContextMenu;
            underwaterTreeView.ContextMenuStrip = ColorContextMenu;
            fogTreeView.ContextMenuStrip = ColorContextMenu;

            BuildTreeViews(EditorValue);
        }

        private void BuildTreeViews(AbstractColorContainer c)
        {
            colorTreeView.Nodes.Clear();
            waterTreeView.Nodes.Clear();
            underwaterTreeView.Nodes.Clear();
            fogTreeView.Nodes.Clear();
            int nodeDepth = Convert.ToInt32(compactViewToolStripMenuItem.Checked);

            foreach (KeyValuePair<string, Color> col in c.Colors)
            {
                TreeNode n = colorTreeView.Nodes.BuildNodeTreeBySeperator(col.Key, '_', nodeDepth);
                n.Tag = col;
            }

            foreach (KeyValuePair<string, (Color surface, Color underwater, Color fog)> col in c.WaterColors)
            {
                waterTreeView.Nodes.BuildNodeTreeBySeperator(col.Key, '_', 0).Tag = col;
                underwaterTreeView.Nodes.BuildNodeTreeBySeperator(col.Key, '_', 0).Tag = col;
                fogTreeView.Nodes.BuildNodeTreeBySeperator(col.Key, '_', 0).Tag = col;
            }

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
                _defaultColourfile = AbstractColorContainer.FromColorContainer(reader.FromStream(stream));
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

            if (filterPrompt.SelectedTabIndex == 0 && filterPrompt.SelectedItem is KeyValuePair<string, Color> colorEntry)
            {
                EditorValue.Colors[colorEntry.Key] = color;
            }
            else if (filterPrompt.SelectedItem is KeyValuePair<string, (Color Surface,Color Underwater, Color Fog)> waterColorEntry)
            {
                if (filterPrompt.SelectedTabIndex == 1)
                    EditorValue.WaterColors[waterColorEntry.Key] =  (color, waterColorEntry.Value.Underwater, waterColorEntry.Value.Fog);
                else if (filterPrompt.SelectedTabIndex == 2)
                    EditorValue.WaterColors[waterColorEntry.Key] = (waterColorEntry.Value.Surface, color, waterColorEntry.Value.Fog);
                else
                    EditorValue.WaterColors[waterColorEntry.Key] = (waterColorEntry.Value.Surface, waterColorEntry.Value.Underwater, color);
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
            if (filterPrompt.SelectedTabIndex == 0 && filterPrompt.SelectedItem is ColorContainer.Color color)
            {
                color.ColorPallette = colorPick.Color;
                SetColor(color.ColorPallette);
                return;
            }
            if (filterPrompt.SelectedItem is KeyValuePair<string, (Color Surface,Color Underwater, Color Fog)> waterColorEntry)
            {
                Color wColor =
                filterPrompt.SelectedTabIndex == 1 ? waterColorEntry.Value.Surface :
                filterPrompt.SelectedTabIndex == 2 ? waterColorEntry.Value.Underwater : waterColorEntry.Value.Fog;
                if (filterPrompt.SelectedTabIndex == 1)
                    EditorValue.WaterColors[waterColorEntry.Key] = (wColor, waterColorEntry.Value.Underwater, waterColorEntry.Value.Fog);
                else if (filterPrompt.SelectedTabIndex == 2)
                    EditorValue.WaterColors[waterColorEntry.Key] = (waterColorEntry.Value.Surface, wColor, waterColorEntry.Value.Fog);
                else
                    EditorValue.WaterColors[waterColorEntry.Key] = (waterColorEntry.Value.Surface, waterColorEntry.Value.Underwater, wColor);
            }
        }

        private void alpha_ValueChanged(object sender, EventArgs e)
        {
            var colorEntry = (KeyValuePair<string, (Color Surface,Color Underwater, Color Fog)>)waterTreeView.SelectedNode.Tag;
            EditorValue.WaterColors[colorEntry.Key] = colorEntry.Value;
            pictureBox1.BackColor = Color.FromArgb((int)alphaUpDown.Value, colorEntry.Value.Surface);
        }

        private void restoreOriginalColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filterPrompt.SelectedItem is KeyValuePair<string, Color> color)
            {
                if (!_defaultColourfile.Colors.TryGetValue(color.Key, out Color col_entry))
                {
                    HideColorEdit();
                    return;
                }
                EditorValue.Colors[color.Key] = col_entry;
                SetColor(col_entry);
            }
            if (filterPrompt.SelectedItem is KeyValuePair<string, (Color Surface, Color Underwater, Color Fog)> waterColor)
            {
                if (_defaultColourfile.WaterColors.TryGetValue(waterColor.Key, out (Color Surface, Color Underwater, Color Fog) waterColorEntry))
                {
                    HideColorEdit();
                    return;
                }

                Color wColor =
                    filterPrompt.SelectedTabIndex == 1 ? waterColorEntry.Surface :
                    filterPrompt.SelectedTabIndex == 2 ? waterColorEntry.Underwater: waterColorEntry.Fog;
                if (filterPrompt.SelectedTabIndex == 1)
                    EditorValue.WaterColors[waterColor.Key] = (wColor, waterColorEntry.Underwater, waterColorEntry.Fog);
                else if (filterPrompt.SelectedTabIndex == 2)
                    EditorValue.WaterColors[waterColor.Key] = (waterColorEntry.Surface, wColor, waterColorEntry.Fog);
                else
                    EditorValue.WaterColors[waterColor.Key] = (waterColorEntry.Surface, waterColorEntry.Underwater, wColor);
                SetColor(wColor);
            }
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
            foreach (string ps4BiomeColor in PS4BiomeColors)
            {
                if (EditorValue.WaterColors.ContainsKey(ps4BiomeColor))
                    EditorValue.WaterColors.Remove(ps4BiomeColor);
            }
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
                        if (EditorValue.Colors.ContainsKey(prompt.Name))
                        {
                            MessageBox.Show(this, $"\"{prompt.Name}\" already exists in this color table", "Color not added");
                        }

                        EditorValue.Colors.Add(prompt.Name, Color.White);
                        BuildTreeViews(EditorValue);
                    }
                }
            }
        }

        private void removeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filterPrompt.SelectedItem is ColorContainer.Color color)
            {
                EditorValue.Colors.Remove(color.Name);
            }
        }

        private void SetColor(Color color)
        {
            redUpDown.Value = color.R;
            greenUpDown.Value = color.G;
            blueUpDown.Value = color.B;

            pictureBox1.BackColor = Color.FromArgb(color.A == 0 ? 0xff : color.A, color);
            colorTextbox.Text = color.ToArgb().ToString("X06");
            ShowColorEdit();
            alphaUpDown.Visible = color.A != 0;
        }

        private void filterPrompt_OnSelectedItemChanged(object sender, EventArgs e)
        {
            if (filterPrompt.SelectedItem is KeyValuePair<string, Color> col)
            {
                SetColor(col.Value);
                return;
            }
            if (filterPrompt.SelectedItem is KeyValuePair<string, (Color Surface,Color Underwater, Color Fog)> waterColor)
            {
                SetColor(waterColor.Value.Surface);
                return;
            }
            HideColorEdit();
        }

        private void HideColorEdit()
        {
            redUpDown.Visible = false;
            greenUpDown.Visible = false;
            blueUpDown.Visible = false;
            alphaUpDown.Visible = false;

            pictureBox1.Visible = false;
            colorTextbox.Visible = false;
        }

        private void ShowColorEdit()
        {
            redUpDown.Visible = true;
            greenUpDown.Visible = true;
            blueUpDown.Visible = true;
            alphaUpDown.Visible = true;

            pictureBox1.Visible = true;
            colorTextbox.Visible = true;
        }

        private void compactViewToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            BuildTreeViews(EditorValue);
        }
    }
}
