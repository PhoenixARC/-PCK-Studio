using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OMI.Formats.Color;
using OMI.Formats.Pck;
using OMI.Workers.Color;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Editor
{
	public partial class COLEditor : ThemeForm
	{
		ColorContainer default_colourfile;
        ColorContainer colourfile;
        ColorContainer.Color clipboard_color;

		private readonly PckFile.FileData _file;

		List<TreeNode> colorCache = new List<TreeNode>();
		List<TreeNode> waterCache = new List<TreeNode>();
		List<TreeNode> underwaterCache = new List<TreeNode>();
		List<TreeNode> fogCache = new List<TreeNode>();

        private static ICollection<string> PS4Biomes = new List<string>
        {
            "bamboo_jungle",
            "bamboo_jungle_hills",
            "mesa_mutated",
            "mega_spruce_taiga_mutated",
            "mega_taiga_mutated"
        };

		private enum TitleVersion
		{
            TU12,
            TU13,
            TU14,
            TU19,
            TU31,
            TU32,
            TU43,
            TU46,
            TU51,
            TU53,
            TU54,
            TU69,
            _1_91_,
        }

        public COLEditor(PckFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using(var stream = new MemoryStream(file.Data))
			{
				var reader = new COLFileReader();
                colourfile = reader.FromStream(stream);
			}

			TU12ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU12);
			TU13ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU13);
			TU14ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU14);
			TU19ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU19);
			TU31ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU31);
			TU32ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU32);
			TU43ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU43);
			TU46ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU46);
			TU51ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU51);
			TU53ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU53);
			TU54ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU54);
			TU69ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion.TU69);
			_1_9_1ToolStripMenuItem.Click += (sender, e) => SetUpDefaultColourFile(sender, e, TitleVersion._1_91_);

			SetUpDefaultColourFile(null, EventArgs.Empty, TitleVersion.TU69, false);
		}

		private void SetUpDefaultColourFile(object sender, EventArgs e, TitleVersion id, bool useTargetVersion = true)
		{
			if (useTargetVersion)
			{
				var result = MessageBox.Show(this,
					"This function will set up your colour table to match that of the chosen version." +
					"You may lose some entries in the table. Are you sure you would like to continue?",
					"Target update version?", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
					return;
			}
			SetDefaultColourFile(id);
			SetUpTable(useTargetVersion ? default_colourfile : colourfile);
		}

		private void SetDefaultColourFile(TitleVersion titleVer)
		{
            var reader = new COLFileReader();
            var data = titleVer switch
            {
                TitleVersion.TU12 => Properties.Resources.tu12colours,
                TitleVersion.TU13 => Properties.Resources.tu13colours,
                TitleVersion.TU14 => Properties.Resources.tu14colours,
                TitleVersion.TU19 => Properties.Resources.tu19colours,
                TitleVersion.TU31 => Properties.Resources.tu31colours,
                TitleVersion.TU32 => Properties.Resources.tu32colours,
                TitleVersion.TU43 => Properties.Resources.tu43colours,
                TitleVersion.TU46 => Properties.Resources.tu46colours,
                TitleVersion.TU51 => Properties.Resources.tu51colours,
                TitleVersion.TU53 => Properties.Resources.tu53colours,
                TitleVersion.TU54 => Properties.Resources.tu54colours,
                TitleVersion.TU69 => Properties.Resources.tu69colours,
                TitleVersion._1_91_ => Properties.Resources._1_91_colours,
                _ => throw new InvalidDataException(titleVer.ToString()),
            };

            using (var stream = new MemoryStream(data))
                default_colourfile = reader.FromStream(stream);
        }


        void SetUpTable(ColorContainer colorFile)
		{
			colorTreeView.Nodes.Clear();
			waterTreeView.Nodes.Clear();
			underwaterTreeView.Nodes.Clear();
			fogTreeView.Nodes.Clear();

			List<string> currentEntries = new List<string>();

			foreach (var color in colorFile.Colors)
			{
				var entry = colourfile.Colors.Find(c => c.Name == color.Name);
				if (currentEntries.Contains(color.Name))
					continue;

				currentEntries.Add(color.Name);

				TreeNode colorNode = new TreeNode(color.Name);
				colorNode.Tag = entry ?? color;
				colorTreeView.Nodes.Add(colorNode);
				colorCache.Add(colorNode);
			}
			currentEntries.Clear();
			foreach (var waterColor in colorFile.WaterColors)
			{
				var entry = colourfile.WaterColors.Find(c => c.Name == waterColor.Name);
				if (currentEntries.Contains(waterColor.Name))
					continue;
				currentEntries.Add(waterColor.Name);

				TreeNode waterNode = new TreeNode(waterColor.Name);
				waterNode.Tag = entry ?? waterColor;
				waterTreeView.Nodes.Add(waterNode);
				waterCache.Add(waterNode);

				TreeNode underWaterNode = new TreeNode(waterColor.Name);
				underWaterNode.Tag = entry ?? waterColor;
				underwaterTreeView.Nodes.Add(underWaterNode);
				underwaterCache.Add(underWaterNode);
				
				TreeNode fogNode = new TreeNode(waterColor.Name);
				fogNode.Tag = entry ?? waterColor;
				fogTreeView.Nodes.Add(fogNode);
				fogCache.Add(fogNode);
			}
		}

		void SetValueChangedEventActive(bool activate)
		{
			if (activate)
			{
				alphaUpDown.ValueChanged += color_ValueChanged;
				redUpDown.ValueChanged += color_ValueChanged;
				greenUpDown.ValueChanged += color_ValueChanged;
				blueUpDown.ValueChanged += color_ValueChanged;
			}
            else
			{
				alphaUpDown.ValueChanged -= color_ValueChanged;
				redUpDown.ValueChanged -= color_ValueChanged;
				greenUpDown.ValueChanged -= color_ValueChanged;
				blueUpDown.ValueChanged -= color_ValueChanged;
			}
        }

        private void color_ValueChanged(object sender, EventArgs e)
        {
            Color fixed_color = new Color();
            if (tabControl.SelectedTab == colorsTab)
            {
                var colorEntry = (ColorContainer.Color)colorTreeView.SelectedNode.Tag;
                colorEntry.ColorPallette = fixed_color = Color.FromArgb(255, (int)redUpDown.Value, (int)greenUpDown.Value, (int)blueUpDown.Value);
            }
            else if (tabControl.SelectedTab != null && waterTreeView.SelectedNode != null) // just in case
            {
                var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
                fixed_color = Color.FromArgb((int)alphaUpDown.Value,
					(int)redUpDown.Value,
					(int)greenUpDown.Value,
					(int)blueUpDown.Value);
                if (tabControl.SelectedTab == waterTab)
					colorEntry.SurfaceColor = fixed_color;
                else if (tabControl.SelectedTab == underwaterTab)
					colorEntry.UnderwaterColor = fixed_color;
                else
					colorEntry.FogColor = fixed_color;
            }

            pictureBox1.BackColor = fixed_color;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (colorTreeView.SelectedNode?.Tag is null)
				return;
			var colorEntry = (ColorContainer.Color)colorTreeView.SelectedNode.Tag;
			var color = colorEntry.ColorPallette;
			SetValueChangedEventActive(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			UpdateDisplayColor(color);
            SetValueChangedEventActive(true);
		}

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if (waterTreeView.SelectedNode?.Tag is null)
				return;
			var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
			var color = colorEntry.SurfaceColor;
			SetValueChangedEventActive(false);
			alphaUpDown.Enabled = true;
			alphaUpDown.Visible = true;
			alphaLabel.Visible = true;
            alphaUpDown.Value = color.A;
			UpdateDisplayColor(color);
            SetValueChangedEventActive(true);
		}

		private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (underwaterTreeView.SelectedNode?.Tag is null)
				return;
			var colorEntry = (ColorContainer.WaterColor)underwaterTreeView.SelectedNode.Tag;
			var color = colorEntry.UnderwaterColor;
			SetValueChangedEventActive(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			UpdateDisplayColor(color);
            SetValueChangedEventActive(true);
		}

		private void treeView4_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (fogTreeView.SelectedNode?.Tag is null)
				return;
			var colorEntry = (ColorContainer.WaterColor)fogTreeView.SelectedNode.Tag;
			var color = colorEntry.FogColor;
			SetValueChangedEventActive(false);
            alphaLabel.Visible = false;
            alphaUpDown.Visible = false;
			UpdateDisplayColor(color);
            SetValueChangedEventActive(true);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
            if (colourfile.WaterColors.Find(e => PS4Biomes.Contains(e.Name)) != null)
			{
				var result = MessageBox.Show(this,
					"Biomes exclusive to PS4 Edition v1.91 were found in the water section of this colour table. " +
					"This will crash all other editions of the game and PS4 Edition v1.90 and below. " +
					"Would you like to remove them?",
					"Potentially unsupported biomes found", MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Yes)
				{
					foreach (var col in colourfile.WaterColors)
					{
						if (PS4Biomes.Contains(col.Name))
							colourfile.WaterColors.Remove(col);
					}
				}
			}

			using (var stream = new MemoryStream())
			{
				var writer = new COLFileWriter(colourfile);
                writer.WriteToStream(stream);
				_file.SetData(stream.ToArray());
			}

            DialogResult = DialogResult.OK;
        }

		public void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			var node = colorTreeView.SelectedNode;

			if (node?.Tag is not ColorContainer.Color)
				return;

			const Keys CTRL_C = Keys.Control | Keys.C;
			const Keys CTRL_V = Keys.Control | Keys.V;

			switch (e.KeyCode)
			{
				case Keys.Delete:
					restoreOriginalColorToolStripMenuItem_Click(sender, e);
					break;
				case CTRL_C:
					copyColorToolStripMenuItem_Click(sender, e);
					break;
				case CTRL_V:
					pasteColorToolStripMenuItem_Click(sender, e);
					break;
            };
		}

        private void treeView2_KeyDown(object sender, KeyEventArgs e)
        {
			var node = waterTreeView.SelectedNode;

			if (node?.Tag is not ColorContainer.WaterColor)
				return;

			if (e.KeyCode == Keys.Delete)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView3_KeyDown(object sender, KeyEventArgs e)
		{
			var node = underwaterTreeView.SelectedNode;

			if (node?.Tag is not ColorContainer.WaterColor)
				return;

			if (e.KeyCode == Keys.Delete)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView4_KeyDown(object sender, KeyEventArgs e)
		{
			var node = fogTreeView.SelectedNode;

			if (node?.Tag is not ColorContainer.WaterColor)
				return;

			if (e.KeyCode == Keys.Delete)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void colorBox_TextChanged(object sender, EventArgs e)
		{
			//TreeView tv = (TreeView)tabControl.SelectedTab.Controls[0];
			//if (tv.SelectedNode == null || tv.SelectedNode.Tag == null)
			//	return;
			//bool hasAlpha = tabControl.SelectedTab == waterTab;
			//alphaUpDown.Enabled = hasAlpha;
			//redUpDown.Value = StringToByteArrayFastest(colorTextbox.Text)[!hasAlpha ? 0 : 1];
			//greenUpDown.Value = StringToByteArrayFastest(colorTextbox.Text)[!hasAlpha ? 1 : 2];
			//blueUpDown.Value = StringToByteArrayFastest(colorTextbox.Text)[!hasAlpha ? 2 : 3];
			//int color = 0; /*colorEntry.color*/;
	        //int argb = (int)((0xff000000u) | (color >> 24));
			//colorTextbox.MaxLength = hasAlpha ? 8 : 6;
			//alphaLabel.Visible = false;
			//alphaUpDown.Visible = false;
			//if (hasAlpha)
			//{
			//	alphaLabel.Visible = true;
			//	alphaUpDown.Visible = true;
			//	alphaUpDown.Value = StringToByteArrayFastest(colorTextbox.Text)[0];
			//	argb = color >> 24 | color << 8;
			//}
			//pictureBox1.BackColor = Color.FromArgb(argb);
		}

		private void alpha_ValueChanged(object sender, EventArgs e)
		{
			if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
				waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
			{
				var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
				pictureBox1.BackColor = colorEntry.SurfaceColor = Color.FromArgb((int)alphaUpDown.Value, colorEntry.SurfaceColor);
			}
		}

		private void restoreOriginalColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetValueChangedEventActive(false);
			if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is ColorContainer.Color colorInfoD)
			{
				var entry = default_colourfile.Colors.Find(color => color.Name == colorTreeView.SelectedNode.Text);
				colorInfoD.ColorPallette = entry.ColorPallette;
				UpdateDisplayColor(entry.ColorPallette);
			}
			else if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
			waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is ColorContainer.WaterColor colorInfo)
			{
				var entry = default_colourfile.WaterColors.Find(color => color.Name == waterTreeView.SelectedNode.Text);
				colorInfo.SurfaceColor = entry.SurfaceColor;
				UpdateDisplayColor(entry.SurfaceColor);
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
				underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is ColorContainer.WaterColor colorInfoB)
			{
				var entry = default_colourfile.WaterColors.Find(color => color.Name == underwaterTreeView.SelectedNode.Text);
				colorInfoB.UnderwaterColor = entry.UnderwaterColor;
				UpdateDisplayColor(entry.UnderwaterColor);
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
				fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is ColorContainer.WaterColor colorInfoC)
			{
				var entry = default_colourfile.WaterColors.Find(color => color.Name == fogTreeView.SelectedNode.Text);
				colorInfoC.FogColor = entry.FogColor;
				UpdateDisplayColor(entry.FogColor);
            }
			SetValueChangedEventActive(true);
		}

		private void UpdateDisplayColor(Color color)
		{
			alphaUpDown.Value = color.A;
			redUpDown.Value = color.R;
			greenUpDown.Value = color.G;
			blueUpDown.Value = color.B;
			pictureBox1.BackColor = Color.FromArgb(0xff, color);
        }

		private void metroTextBox1_TextChanged(object sender, EventArgs e)
		{
			// Some code in this function is modified code from this StackOverflow answer - MattNL
			//https://stackoverflow.com/questions/8260322/filter-a-treeview-with-a-textbox-in-a-c-sharp-winforms-app

			//blocks repainting tree until all objects loaded
			colorTreeView.BeginUpdate();
			colorTreeView.Nodes.Clear();
			waterTreeView.BeginUpdate();
			waterTreeView.Nodes.Clear();
			underwaterTreeView.BeginUpdate();
			underwaterTreeView.Nodes.Clear();
			fogTreeView.BeginUpdate();
			fogTreeView.Nodes.Clear();
			if (!string.IsNullOrEmpty(filterTextBox.Text))
			{
				foreach (TreeNode cachedNode in colorCache)
				{
					if (cachedNode.Text.ToLower().Contains(filterTextBox.Text.ToLower()))
					{
						colorTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
					}
				}
				foreach (TreeNode cachedNode in waterCache)
				{
					if (cachedNode.Text.ToLower().Contains(filterTextBox.Text.ToLower()))
					{
						waterTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
					}
				}
				foreach (TreeNode cachedNode in underwaterCache)
				{
					if (cachedNode.Text.ToLower().Contains(filterTextBox.Text.ToLower()))
					{
						underwaterTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
					}
				}
				foreach (TreeNode cachedNode in fogCache)
				{
					if (cachedNode.Text.ToLower().Contains(filterTextBox.Text.ToLower()))
					{
						fogTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
					}
				}
			}
			else
			{
				foreach (TreeNode cachedNode in colorCache)
				{
					colorTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
				}
				foreach (TreeNode cachedNode in waterCache)
				{
					waterTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
				}
				foreach (TreeNode cachedNode in underwaterCache)
				{
					underwaterTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
				}
				foreach (TreeNode cachedNode in fogCache)
				{
					fogTreeView.Nodes.Add((TreeNode)cachedNode.Clone());
				}
			}
			//enables redrawing tree after all objects have been added
			colorTreeView.EndUpdate();
			waterTreeView.EndUpdate();
			underwaterTreeView.EndUpdate();
			fogTreeView.EndUpdate();
		}

		private void copyColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var colorToCopy = new ColorContainer.Color()
			{
				Name = string.Empty,
				ColorPallette = new Color()
			};
			if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode.Tag is ColorContainer.Color colorInfoD)
			{
				colorToCopy = colorInfoD;
			}
			else if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode.Tag is ColorContainer.WaterColor colorInfo)
			{
				colorToCopy.ColorPallette = colorInfo.SurfaceColor;
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode.Tag is ColorContainer.WaterColor colorInfoB)
			{
				colorToCopy.ColorPallette = colorInfoB.UnderwaterColor;
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode.Tag is ColorContainer.WaterColor colorInfoC)
			{
				colorToCopy.ColorPallette = colorInfoC.FogColor;
			}
			clipboard_color = colorToCopy;
		}

		private void pasteColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (clipboard_color == null) return;
			SetValueChangedEventActive(false);
			Color fixed_color = Color.FromArgb(255, Color.FromArgb(0xff, clipboard_color.ColorPallette));

			if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
				waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
			{
				var colorEntry = ((ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag);
				colorEntry.SurfaceColor = fixed_color;
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
				underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
			{
				var colorEntry = ((ColorContainer.WaterColor)underwaterTreeView.SelectedNode.Tag);
				colorEntry.UnderwaterColor = fixed_color;
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
				fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
			{
				var colorEntry = ((ColorContainer.WaterColor)fogTreeView.SelectedNode.Tag);
				colorEntry.FogColor = fixed_color;
			}
			else if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is ColorContainer.Color)
			{
				var colorEntry = ((ColorContainer.Color)colorTreeView.SelectedNode.Tag);
				colorEntry.ColorPallette = fixed_color;
			}

			redUpDown.Value = clipboard_color.ColorPallette.R;
			greenUpDown.Value = clipboard_color.ColorPallette.G;
			blueUpDown.Value = clipboard_color.ColorPallette.B;
			pictureBox1.BackColor = fixed_color;
            SetValueChangedEventActive(true);
		}

        private void SetColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorPick = new ColorDialog();
            colorPick.AllowFullOpen = true;
            colorPick.AnyColor = true;
            colorPick.SolidColorOnly = tabControl.SelectedTab == colorsTab;
            if (colorPick.ShowDialog() != DialogResult.OK) return;
            pictureBox1.BackColor = colorPick.Color;
            if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
                waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
            {
                var colorEntry = ((ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag);
                // preserves the alpha so the user can handle it since the color picker doesn't support alpha
                Color fixed_color = Color.FromArgb(colorEntry.SurfaceColor.A, colorPick.Color);
                colorEntry.SurfaceColor = fixed_color;
                pictureBox1.BackColor = fixed_color;
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
            else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
                underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
            {
                var colorEntry = ((ColorContainer.WaterColor)underwaterTreeView.SelectedNode.Tag);
                // the game doesn't care about the alpha value for underwater colors
                colorEntry.UnderwaterColor = Color.FromArgb(0, colorPick.Color);
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
                fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
            {
                var colorEntry = ((ColorContainer.WaterColor)fogTreeView.SelectedNode.Tag);
                // the game doesn't care about the alpha value for fog colors
                colorEntry.FogColor = Color.FromArgb(0, colorPick.Color);
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
            else if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
                colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is ColorContainer.Color)
            {
                var colorEntry = ((ColorContainer.Color)colorTreeView.SelectedNode.Tag);
                colorEntry.ColorPallette = colorPick.Color;
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
            colorPick.Dispose();
        }
    }
}
