using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using OMI.Formats.Color;
using OMI.Formats.Pck;
using OMI.Workers.Color;
using PckStudio.Properties;

namespace PckStudio.Forms.Editor
{
	public partial class COLEditor : MetroForm
	{
		ColorContainer default_colourfile;
        ColorContainer colourfile;
        ColorContainer.Color clipboard_color;

		private readonly PckFile.FileData _file;

		List<TreeNode> colorCache = new List<TreeNode>();
		List<TreeNode> waterCache = new List<TreeNode>();
		List<TreeNode> underwaterCache = new List<TreeNode>();
		List<TreeNode> fogCache = new List<TreeNode>();

		public COLEditor(PckFile.FileData file)
		{
			InitializeComponent();

			saveToolStripMenuItem1.Visible = !Settings.Default.AutoSaveChanges;

            _file = file;

			using(var stream = new MemoryStream(file.Data))
			{
				var reader = new COLFileReader();
                colourfile = reader.FromStream(stream);
			}

			SetUpMenu();

			SetUpDefaultFile(null, EventArgs.Empty, 11, false);
		}

		void SetUpMenu()
		{
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
		}

		private void SetUpDefaultFile(object sender, EventArgs e, int ID, bool targetVersion = true)
		{
			if(targetVersion)
			{
				var result = MessageBox.Show(this, "This function will set up your colour table to match that of the chosen version. You may lose some entries in the table. Are you sure you would like to continue?", "Target update version?", MessageBoxButtons.YesNo);
				if (result == DialogResult.No) return;
			}

			var reader = new COLFileReader();

			switch (ID)
			{
				case 0: using (var stream = new MemoryStream(Properties.Resources.tu12colours)) default_colourfile = reader.FromStream(stream); break;
				case 1: using (var stream = new MemoryStream(Properties.Resources.tu13colours)) default_colourfile = reader.FromStream(stream); break;
				case 2: using (var stream = new MemoryStream(Properties.Resources.tu14colours)) default_colourfile = reader.FromStream(stream); break;
				case 3: using (var stream = new MemoryStream(Properties.Resources.tu19colours)) default_colourfile = reader.FromStream(stream); break;
				case 4: using (var stream = new MemoryStream(Properties.Resources.tu31colours)) default_colourfile = reader.FromStream(stream); break;
				case 5: using (var stream = new MemoryStream(Properties.Resources.tu32colours)) default_colourfile = reader.FromStream(stream); break;
				case 6: using (var stream = new MemoryStream(Properties.Resources.tu43colours)) default_colourfile = reader.FromStream(stream); break;
				case 7: using (var stream = new MemoryStream(Properties.Resources.tu46colours)) default_colourfile = reader.FromStream(stream); break;
				case 8: using (var stream = new MemoryStream(Properties.Resources.tu51colours)) default_colourfile = reader.FromStream(stream); break;
				case 9: using (var stream = new MemoryStream(Properties.Resources.tu53colours)) default_colourfile = reader.FromStream(stream); break;
				case 10: using (var stream = new MemoryStream(Properties.Resources.tu54colours)) default_colourfile = reader.FromStream(stream); break;
				case 11: using (var stream = new MemoryStream(Properties.Resources.tu69colours)) default_colourfile = reader.FromStream(stream); break;
				case 12: using (var stream = new MemoryStream(Properties.Resources._1_91_colours)) default_colourfile = reader.FromStream(stream); break;
				default: return;
			}
			SetUpTable(targetVersion);
		}

		void SetUpTable(bool targetVersion)
		{
			colorTreeView.Nodes.Clear();
			waterTreeView.Nodes.Clear();
			underwaterTreeView.Nodes.Clear();
			fogTreeView.Nodes.Clear();

			ColorContainer temp = targetVersion ? default_colourfile : colourfile;

			List<string> CurrentEntries = new List<string>();

			foreach (var obj in temp.Colors)
			{
				var entry = colourfile.Colors.Find(color => color.Name == obj.Name);
				TreeNode tn = new TreeNode(obj.Name);
				tn.Tag = entry != null ? entry : obj;
				if (CurrentEntries.Contains(obj.Name)) continue;
				CurrentEntries.Add(obj.Name);
				colorTreeView.Nodes.Add(tn);
				colorCache.Add(tn);
			}
			CurrentEntries.Clear();
			foreach (var obj in temp.WaterColors)
			{
				var entry = colourfile.WaterColors.Find(color => color.Name == obj.Name);
				TreeNode tn = new TreeNode(obj.Name);
				tn.Tag = entry != null ? entry : obj;
				if (CurrentEntries.Contains(obj.Name)) continue;
				CurrentEntries.Add(obj.Name);
				waterTreeView.Nodes.Add(tn);
				waterCache.Add(tn);
				TreeNode tnB = new TreeNode(obj.Name);
				tnB.Tag = entry != null ? entry : obj;
				underwaterTreeView.Nodes.Add(tnB);
				underwaterCache.Add(tnB);
				TreeNode tnC = new TreeNode(obj.Name);
				tnC.Tag = entry != null ? entry : obj;
				fogTreeView.Nodes.Add(tnC);
				fogCache.Add(tnC);
			}
		}

		void SetUpValueChanged(bool add)
		{
			if(add)
			{
				//alphaUpDown.ValueChanged += color_ValueChanged;
				redUpDown.ValueChanged += color_ValueChanged;
				greenUpDown.ValueChanged += color_ValueChanged;
				blueUpDown.ValueChanged += color_ValueChanged;
			}
			else
			{
				//alphaUpDown.ValueChanged -= color_ValueChanged;
				redUpDown.ValueChanged -= color_ValueChanged;
				greenUpDown.ValueChanged -= color_ValueChanged;
				blueUpDown.ValueChanged -= color_ValueChanged;
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (colorTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (ColorContainer.Color)colorTreeView.SelectedNode.Tag;
			var color = colorEntry.ColorPallette.ToArgb();
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(0xff << 24 | color);
			SetUpValueChanged(true);
		}

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if (waterTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
			int color = colorEntry.SurfaceColor.ToArgb();
			SetUpValueChanged(false);
			alphaUpDown.Enabled = true;
			alphaUpDown.Visible = true;
			alphaLabel.Visible = true;
			alphaUpDown.Value = color >> 24 & 0xff;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = colorEntry.SurfaceColor;
			SetUpValueChanged(true);
		}

		private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (underwaterTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (ColorContainer.WaterColor)underwaterTreeView.SelectedNode.Tag;
			int color = colorEntry.UnderwaterColor.ToArgb();
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(255, Color.FromArgb(0xff << 24 | color));
			SetUpValueChanged(true);
		}

		private void treeView4_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (fogTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (ColorContainer.WaterColor)fogTreeView.SelectedNode.Tag;
			int color = colorEntry.FogColor.ToArgb();
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(255, Color.FromArgb(0xff << 24 | color));
			SetUpValueChanged(true);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
            List<string> PS4Biomes = new List<string>
            {
                "bamboo_jungle",
                "bamboo_jungle_hills",
                "mesa_mutated",
                "mega_spruce_taiga_mutated",
                "mega_taiga_mutated"
            };

            if (colourfile.WaterColors.Find(e => PS4Biomes.Contains(e.Name)) != null)
			{
				var result = MessageBox.Show(this, "Biomes exclusive to PS4 Edition v1.91 were found in the water section of this colour table. This will crash all other editions of the game and PS4 Edition v1.90 and below. Would you like to remove them?", "Potentially unsupported biomes found", MessageBoxButtons.YesNoCancel);
				switch (result)
				{
					case DialogResult.Yes:
						foreach (var col in colourfile.WaterColors.ToList())
						{
							if(PS4Biomes.Contains(col.Name)) colourfile.WaterColors.Remove(col);
						}
						break;
					case DialogResult.No:
						break;
					default:
						return;
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
			if (e.KeyCode == Keys.Delete && node.Tag is ColorContainer.Color)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is ColorContainer.Color)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is ColorContainer.Color)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

        private void treeView2_KeyDown(object sender, KeyEventArgs e)
        {
			var node = waterTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is ColorContainer.WaterColor)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is ColorContainer.WaterColor)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is ColorContainer.WaterColor)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView3_KeyDown(object sender, KeyEventArgs e)
		{
			var node = underwaterTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is ColorContainer.WaterColor)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is ColorContainer.WaterColor)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is ColorContainer.WaterColor)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView4_KeyDown(object sender, KeyEventArgs e)
		{
			var node = fogTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is ColorContainer.WaterColor)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is ColorContainer.WaterColor)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is ColorContainer.WaterColor)
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
   //         int argb = (int)((0xff000000u) | (color >> 24));
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
				fixed_color = Color.FromArgb(tabControl.SelectedTab == waterTab ? (int)alphaUpDown.Value : 255, (int)redUpDown.Value, (int)greenUpDown.Value, (int)blueUpDown.Value);
				if (tabControl.SelectedTab == waterTab) colorEntry.SurfaceColor = fixed_color;
				else if (tabControl.SelectedTab == underwaterTab) colorEntry.UnderwaterColor = fixed_color;
				else colorEntry.FogColor = fixed_color;
			}

			pictureBox1.BackColor = fixed_color;
		}

		private void setColorBtn_Click(object sender, EventArgs e)
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
				var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
				// preserves the alpha so the user can handle it since the color picker doesn't support alpha
				pictureBox1.BackColor = colorEntry.SurfaceColor = Color.FromArgb(colorEntry.SurfaceColor.A, colorPick.Color);
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
				underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
			{
				var colorEntry = (ColorContainer.WaterColor)underwaterTreeView.SelectedNode.Tag;
				// the game doesn't care about the alpha value for underwater colors
				colorEntry.UnderwaterColor = Color.FromArgb(0, colorPick.Color);
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
				fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is ColorContainer.WaterColor)
			{
				var colorEntry = (ColorContainer.WaterColor)fogTreeView.SelectedNode.Tag;
				// the game doesn't care about the alpha value for fog colors
				colorEntry.FogColor = Color.FromArgb(0, colorPick.Color);
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is ColorContainer.Color)
			{
				var colorEntry = (ColorContainer.Color)colorTreeView.SelectedNode.Tag;
				colorEntry.ColorPallette = colorPick.Color;
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			colorPick.Dispose();
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
			SetUpValueChanged(false);
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
			SetUpValueChanged(true);
		}

		private void UpdateDisplayColor(Color color)
		{
            alphaUpDown.Value = color.A;
            redUpDown.Value = color.R;
            greenUpDown.Value = color.G;
            blueUpDown.Value = color.B;
            pictureBox1.BackColor = Color.FromArgb(tabControl.SelectedTab == colorsTab ? 0xFF : color.A, color);
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
			if (!string.IsNullOrEmpty(metroTextBox1.Text))
			{
				foreach (TreeNode _node in colorCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						colorTreeView.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				foreach (TreeNode _node in waterCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						waterTreeView.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				foreach (TreeNode _node in underwaterCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						underwaterTreeView.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				foreach (TreeNode _node in fogCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						fogTreeView.Nodes.Add((TreeNode)_node.Clone());
					}
				}
			}
			else
			{
				foreach (TreeNode _node in colorCache)
				{
					colorTreeView.Nodes.Add((TreeNode)_node.Clone());
				}
				foreach (TreeNode _node in waterCache)
				{
					waterTreeView.Nodes.Add((TreeNode)_node.Clone());
				}
				foreach (TreeNode _node in underwaterCache)
				{
					underwaterTreeView.Nodes.Add((TreeNode)_node.Clone());
				}
				foreach (TreeNode _node in fogCache)
				{
					fogTreeView.Nodes.Add((TreeNode)_node.Clone());
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
				Name = "",
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
			SetUpValueChanged(false);
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
			SetUpValueChanged(true);
		}

        private void COLEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (Settings.Default.AutoSaveChanges)
			{
				saveToolStripMenuItem1_Click(sender, EventArgs.Empty);
			}
        }

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.CheckFileExists = true;
				ofd.Multiselect = false;
				ofd.Filter = "COL (Minecraft Color Table)|*.col";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					try
					{
						var reader = new COLFileReader();
						colourfile = reader.FromFile(ofd.FileName);
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, $"Failed to read the selected file\nError: {ex.Message}", "Failed to read .col file");
					}

					SetUpDefaultFile(null, EventArgs.Empty, 11, false);
				}
			}
		}
	}
}
