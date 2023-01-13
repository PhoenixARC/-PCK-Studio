using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.COL;

namespace PckStudio.Forms.Editor
{
	public partial class COLEditor : MetroForm
	{
		COLFile default_colourfile = COLFileReader.Read(new MemoryStream(Properties.Resources.tu69colours));
		COLFile colourfile;
		COLFile.ColorEntry clipboard_color;

		private readonly PCKFile.FileData _file;

		List<TreeNode> colorCache = new List<TreeNode>();
		List<TreeNode> waterCache = new List<TreeNode>();
		List<TreeNode> underwaterCache = new List<TreeNode>();
		List<TreeNode> fogCache = new List<TreeNode>();

		public COLEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using(var stream = new MemoryStream(file.Data))
			{
                colourfile = COLFileReader.Read(stream);
			}

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

			SetUpTable(false);
		}

		private void SetUpDefaultFile(object sender, EventArgs e, int ID)
		{
			var result = MessageBox.Show(this, "This function will set up your colour table to match that of the chosen version. You may lose some entries in the table. Are you sure you would like to continue?", "Target update version?", MessageBoxButtons.YesNo);
			if (result == DialogResult.No) return;

			switch(ID)
			{
				case 0: using (var stream = new MemoryStream(Properties.Resources.tu12colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 1: using (var stream = new MemoryStream(Properties.Resources.tu13colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 2: using (var stream = new MemoryStream(Properties.Resources.tu14colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 3: using (var stream = new MemoryStream(Properties.Resources.tu19colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 4: using (var stream = new MemoryStream(Properties.Resources.tu31colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 5: using (var stream = new MemoryStream(Properties.Resources.tu32colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 6: using (var stream = new MemoryStream(Properties.Resources.tu43colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 7: using (var stream = new MemoryStream(Properties.Resources.tu46colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 8: using (var stream = new MemoryStream(Properties.Resources.tu51colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 9: using (var stream = new MemoryStream(Properties.Resources.tu53colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 10: using (var stream = new MemoryStream(Properties.Resources.tu54colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 11: using (var stream = new MemoryStream(Properties.Resources.tu69colours)) default_colourfile = COLFileReader.Read(stream); break;
				case 12: using (var stream = new MemoryStream(Properties.Resources._1_91_colours)) default_colourfile = COLFileReader.Read(stream); break;
				default: return;
			}
			SetUpTable(true);
		}

		void SetUpTable(bool targetVersion)
		{
			colorTreeView.Nodes.Clear();
			waterTreeView.Nodes.Clear();
			underwaterTreeView.Nodes.Clear();
			fogTreeView.Nodes.Clear();

			COLFile temp = targetVersion ? default_colourfile : colourfile;

			List<string> CurrentEntries = new List<string>();

			foreach (var obj in temp.entries)
			{
				COLFile.ColorEntry entry = colourfile.entries.Find(color => color.name == obj.name);
				TreeNode tn = new TreeNode(obj.name);
				tn.Tag = entry != null ? entry : obj;
				if (CurrentEntries.Contains(obj.name)) continue;
				CurrentEntries.Add(obj.name);
				colorTreeView.Nodes.Add(tn);
				colorCache.Add(tn);
			}
			CurrentEntries.Clear();
			foreach (var obj in temp.waterEntries)
			{
				COLFile.ExtendedColorEntry entry = colourfile.waterEntries.Find(color => color.name == obj.name);
				TreeNode tn = new TreeNode(obj.name);
				tn.Tag = entry != null ? entry : obj;
				if (CurrentEntries.Contains(obj.name)) continue;
				CurrentEntries.Add(obj.name);
				waterTreeView.Nodes.Add(tn);
				waterCache.Add(tn);
				TreeNode tnB = new TreeNode(obj.name);
				tnB.Tag = entry != null ? entry : obj;
				underwaterTreeView.Nodes.Add(tnB);
				underwaterCache.Add(tnB);
				TreeNode tnC = new TreeNode(obj.name);
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
			var colorEntry = (COLFile.ColorEntry)colorTreeView.SelectedNode.Tag;
			var color = colorEntry.color;
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)color);
			SetUpValueChanged(true);
		}

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if (waterTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ExtendedColorEntry)waterTreeView.SelectedNode.Tag;
			int color = (int)colorEntry.color;
			SetUpValueChanged(false);
			alphaUpDown.Enabled = true;
			alphaUpDown.Visible = true;
			alphaLabel.Visible = true;
			alphaUpDown.Value = color >> 24 & 0xff;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(color);
			SetUpValueChanged(true);
		}

		private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (underwaterTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ExtendedColorEntry)underwaterTreeView.SelectedNode.Tag;
			int color = (int)colorEntry.color_b;
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
			var colorEntry = (COLFile.ExtendedColorEntry)fogTreeView.SelectedNode.Tag;
			int color = (int)colorEntry.color_c;
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
			List<string> PS4Biomes = new List<string>();
			PS4Biomes.Add("bamboo_jungle");
			PS4Biomes.Add("bamboo_jungle_hills");
			PS4Biomes.Add("mesa_mutated");
			PS4Biomes.Add("mega_spruce_taiga_mutated");
			PS4Biomes.Add("mega_taiga_mutated");

			if (colourfile.waterEntries.Find(e => PS4Biomes.Contains(e.name)) != null)
			{
				var result = MessageBox.Show(this, "Biomes exclusive to PS4 Edition v1.91 were found in the water section of this colour table. This will crash all other editions of the game and PS4 Edition v1.90 and below. Would you like to remove them?", "Potentially unsupported biomes found", MessageBoxButtons.YesNoCancel);
				switch (result)
				{
					case DialogResult.Yes:
						foreach (var col in colourfile.waterEntries.ToList())
						{
							if(PS4Biomes.Contains(col.name)) colourfile.waterEntries.Remove(col);
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
				COLFileWriter.Write(stream, colourfile);
				_file.SetData(stream.ToArray());
			}
            DialogResult = DialogResult.OK;
        }

		static byte[] StringToByteArrayFastest(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The binary key cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		static int GetHexVal(char hex)
		{
			int val = hex;
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}

		public void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			var node = colorTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ColorEntry)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is COLFile.ColorEntry)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is COLFile.ColorEntry)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

        private void treeView2_KeyDown(object sender, KeyEventArgs e)
        {
			var node = waterTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is COLFile.ExtendedColorEntry)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is COLFile.ExtendedColorEntry)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView3_KeyDown(object sender, KeyEventArgs e)
		{
			var node = underwaterTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is COLFile.ExtendedColorEntry)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is COLFile.ExtendedColorEntry)
			{
				pasteColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView4_KeyDown(object sender, KeyEventArgs e)
		{
			var node = fogTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.C && node.Tag is COLFile.ExtendedColorEntry)
			{
				copyColorToolStripMenuItem_Click(sender, e);
			}
			else if (e.Control && e.KeyCode == Keys.V && node.Tag is COLFile.ExtendedColorEntry)
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
				var colorEntry = (COLFile.ColorEntry)colorTreeView.SelectedNode.Tag;
				fixed_color = Color.FromArgb(255, (int)redUpDown.Value, (int)greenUpDown.Value, (int)blueUpDown.Value);
				colorEntry.color = (uint)(((255 << 24) | (fixed_color.R << 16) | (fixed_color.G << 8) | fixed_color.B) & 0xffffffffL);
			}
			else if (tabControl.SelectedTab != null && waterTreeView.SelectedNode != null) // just in case
			{
				var colorEntry = (COLFile.ExtendedColorEntry)waterTreeView.SelectedNode.Tag;
				fixed_color = Color.FromArgb(tabControl.SelectedTab == waterTab ? (int)alphaUpDown.Value : 255, (int)redUpDown.Value, (int)greenUpDown.Value, (int)blueUpDown.Value);
				uint value = (uint)(((fixed_color.A << 24) | (fixed_color.R << 16) | (fixed_color.G << 8) | fixed_color.B) & 0xffffffffL);
				if (tabControl.SelectedTab == waterTab) colorEntry.color = value;
				else if (tabControl.SelectedTab == underwaterTab) colorEntry.color_b = value;
				else colorEntry.color_c = value;
				fixed_color = Color.FromArgb((int)value);
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
				waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)waterTreeView.SelectedNode.Tag);
				// preserves the alpha so the user can handle it since the color picker doesn't support alpha
				Color fixed_color = Color.FromArgb(Color.FromArgb((int)colorEntry.color).A, colorPick.Color);
				colorEntry.color = (uint)fixed_color.ToArgb();
				pictureBox1.BackColor = fixed_color;
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
				underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)underwaterTreeView.SelectedNode.Tag);
				// the game doesn't care about the alpha value for underwater colors
				colorEntry.color_b = (uint)Color.FromArgb(0, colorPick.Color).ToArgb();
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
				fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)fogTreeView.SelectedNode.Tag);
				// the game doesn't care about the alpha value for fog colors
				colorEntry.color_c = (uint)Color.FromArgb(0, colorPick.Color).ToArgb();
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			else if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is COLFile.ColorEntry)
			{
				var colorEntry = ((COLFile.ColorEntry)colorTreeView.SelectedNode.Tag);
				colorEntry.color = (uint)colorPick.Color.ToArgb() & 0xffffff;
				redUpDown.Value = colorPick.Color.R;
				greenUpDown.Value = colorPick.Color.G;
				blueUpDown.Value = colorPick.Color.B;
			}
			colorPick.Dispose();
        }

		private void alpha_ValueChanged(object sender, EventArgs e)
		{
			if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
				waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)waterTreeView.SelectedNode.Tag);
				Color fixed_color = Color.FromArgb((int)alphaUpDown.Value, Color.FromArgb((int)colorEntry.color));
				colorEntry.color = (uint)fixed_color.ToArgb();
				pictureBox1.BackColor = fixed_color;
			}
		}

		private void restoreOriginalColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetUpValueChanged(false);
			if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is COLFile.ColorEntry colorInfoD)
			{
				COLFile.ColorEntry entry = default_colourfile.entries.Find(color => color.name == colorTreeView.SelectedNode.Text);
				colorInfoD.color = entry.color;
				redUpDown.Value = colorInfoD.color >> 16 & 0xff;
				greenUpDown.Value = colorInfoD.color >> 8 & 0xff;
				blueUpDown.Value = colorInfoD.color & 0xff;
				pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)colorInfoD.color);
			}
			else if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
			waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry colorInfo)
			{
				COLFile.ExtendedColorEntry entry = default_colourfile.waterEntries.Find(color => color.name == waterTreeView.SelectedNode.Text);
				colorInfo.color = entry.color;
				alphaUpDown.Value = colorInfo.color >> 24 & 0xff;
				redUpDown.Value = colorInfo.color >> 16 & 0xff;
				greenUpDown.Value = colorInfo.color >> 8 & 0xff;
				blueUpDown.Value = colorInfo.color & 0xff;
				pictureBox1.BackColor = Color.FromArgb((int)colorInfo.color);
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
				underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry colorInfoB)
			{
				COLFile.ExtendedColorEntry entry = default_colourfile.waterEntries.Find(color => color.name == underwaterTreeView.SelectedNode.Text);
				colorInfoB.color_b = entry.color_b;
				alphaUpDown.Value = colorInfoB.color_b >> 24 & 0xff;
				redUpDown.Value = colorInfoB.color_b >> 16 & 0xff;
				greenUpDown.Value = colorInfoB.color_b >> 8 & 0xff;
				blueUpDown.Value = colorInfoB.color_b & 0xff;
				pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)colorInfoB.color_b);
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
				fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry colorInfoC)
			{
				COLFile.ExtendedColorEntry entry = default_colourfile.waterEntries.Find(color => color.name == fogTreeView.SelectedNode.Text);
				colorInfoC.color_c = entry.color_c;
				alphaUpDown.Value = colorInfoC.color_c >> 24 & 0xff;
				redUpDown.Value = colorInfoC.color_c >> 16 & 0xff;
				greenUpDown.Value = colorInfoC.color_c >> 8 & 0xff;
				blueUpDown.Value = colorInfoC.color_c & 0xff;
				pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)colorInfoC.color_c);
			}
			SetUpValueChanged(true);
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
			COLFile.ColorEntry colorToCopy = new COLFile.ColorEntry("", 0);
			if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode.Tag is COLFile.ColorEntry colorInfoD)
			{
				colorToCopy = colorInfoD;
			}
			else if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry colorInfo)
			{
				colorToCopy = colorInfo;
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry colorInfoB)
			{
				colorToCopy = colorInfoB;
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry colorInfoC)
			{
				colorToCopy = colorInfoC;
			}
			clipboard_color = colorToCopy;
		}

		private void pasteColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (clipboard_color == null) return;
			SetUpValueChanged(false);
			Color fixed_color = Color.FromArgb(255, Color.FromArgb(0xff << 24 | (int)clipboard_color.color));

			if (tabControl.SelectedTab == waterTab && waterTreeView.SelectedNode != null &&
				waterTreeView.SelectedNode.Tag != null && waterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)waterTreeView.SelectedNode.Tag);
				colorEntry.color = (uint)fixed_color.ToArgb();
			}
			else if (tabControl.SelectedTab == underwaterTab && underwaterTreeView.SelectedNode != null &&
				underwaterTreeView.SelectedNode.Tag != null && underwaterTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)underwaterTreeView.SelectedNode.Tag);
				colorEntry.color_b = (uint)fixed_color.ToArgb();
			}
			else if (tabControl.SelectedTab == fogTab && fogTreeView.SelectedNode != null &&
				fogTreeView.SelectedNode.Tag != null && fogTreeView.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)fogTreeView.SelectedNode.Tag);
				colorEntry.color_c = (uint)fixed_color.ToArgb();
			}
			else if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is COLFile.ColorEntry)
			{
				var colorEntry = ((COLFile.ColorEntry)colorTreeView.SelectedNode.Tag);
				colorEntry.color = (uint)fixed_color.ToArgb() & 0xffffff;
			}

			redUpDown.Value = clipboard_color.color >> 16 & 0xff;
			greenUpDown.Value = clipboard_color.color >> 8 & 0xff;
			blueUpDown.Value = clipboard_color.color & 0xff;
			pictureBox1.BackColor = fixed_color;
			SetUpValueChanged(true);
		}
	}
}
