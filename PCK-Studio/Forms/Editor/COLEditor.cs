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
using PckStudio.Core.Extensions;
using PckStudio.Properties;
using PckStudio.Internal;
using PckStudio.Interfaces;
using System.Collections.ObjectModel;

namespace PckStudio.Forms.Editor
{
	public partial class COLEditor : EditorForm<ColorContainer>
	{
		ColorContainer _defaultColourfile;
        string _clipboard_color = "#FFFFFF";

		List<TreeNode> colorCache = new List<TreeNode>();
		List<TreeNode> waterCache = new List<TreeNode>();
		List<TreeNode> underwaterCache = new List<TreeNode>();
		List<TreeNode> fogCache = new List<TreeNode>();

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

			SetUpDefaultFile(null, EventArgs.Empty, 11, false);

			colorTreeView.Select();
			colorTreeView.SelectedNode = colorTreeView.Nodes[0];
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

            SetUpTable(showMessageBox);
		}

		void AddEntry(TreeView treeView, List<TreeNode> cache, string name, object tag)
        {
			TreeNode tn = new TreeNode(name);
			tn.Tag = tag;
			treeView.Nodes.Add(tn);
			cache.Add(tn);
		}

        void RemoveEntry(TreeNode entry, List<TreeNode> cache)
        {
            cache.Remove(entry);
            entry.Remove();
        }

        void SetUpTable(bool targetVersion)
		{
			colorTreeView.Nodes.Clear();
			waterTreeView.Nodes.Clear();
			underwaterTreeView.Nodes.Clear();
			fogTreeView.Nodes.Clear();

			ColorContainer temp = targetVersion ? _defaultColourfile : EditorValue;

            List<string> currentEntries = new List<string>();

			colorCache.Clear();
			fogCache.Clear();
			underwaterCache.Clear();
			waterCache.Clear();

			// fixes the duplicate entry bug
			if (targetVersion)
            {
				foreach(ColorContainer.Color col in EditorValue.Colors)
                {
					if (_defaultColourfile.Colors.Find(c => c.Name == col.Name) == null)
						continue;
					currentEntries.Add(col.Name);
					AddEntry(colorTreeView, colorCache, col.Name, col);
				}
            }

			foreach (ColorContainer.Color col in temp.Colors)
			{
                ColorContainer.Color entry = EditorValue.Colors.Find(color => color.Name == col.Name);
				if (currentEntries.Contains(col.Name))
					continue;
                ColorContainer.Color color = entry ?? col;
				AddEntry(colorTreeView, colorCache, color.Name, color);
			}
			currentEntries.Clear();

			// fixes the duplicate entry bug
			if (targetVersion)
			{
				foreach (ColorContainer.WaterColor col in EditorValue.WaterColors)
				{
					if (_defaultColourfile.WaterColors.Find(c => c.Name == col.Name) == null)
						continue;
                    ColorContainer.WaterColor entry = EditorValue.WaterColors.Find(color => color.Name == col.Name);
                    ColorContainer.WaterColor color = entry ?? col;
					AddEntry(waterTreeView, waterCache, color.Name, color);
					AddEntry(underwaterTreeView, underwaterCache, color.Name, color);
					AddEntry(fogTreeView, fogCache, color.Name, color);
				}
			}

			foreach (ColorContainer.WaterColor col in temp.WaterColors)
			{
                ColorContainer.WaterColor entry = EditorValue.WaterColors.Find(color => color.Name == col.Name);
				if (currentEntries.Contains(col.Name))
					continue;
                ColorContainer.WaterColor color = entry ?? col;
				AddEntry(waterTreeView, waterCache, color.Name, color);
				AddEntry(underwaterTreeView, underwaterCache, color.Name, color);
				AddEntry(fogTreeView, fogCache, color.Name, color);
			}

			// force the filter function to run to carry filter over and fix treeview size
			metroTextBox1_TextChanged(null, null);
		}

		void SetUpValueChanged(bool add)
		{
			if(add)
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

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (colorTreeView.SelectedNode.Tag == null)
				return;

            addColorToolStripMenuItem.Visible = true;
            removeColorToolStripMenuItem.Visible = true;

            waterTreeView.SelectedNode = null;
			underwaterTreeView.SelectedNode = null;
			fogTreeView.SelectedNode = null;

			var colorEntry = (ColorContainer.Color)colorTreeView.SelectedNode.Tag;
            Color color = colorEntry.ColorPallette;
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color.R;
			greenUpDown.Value = color.G;
			blueUpDown.Value = color.B;
			pictureBox1.BackColor = Color.FromArgb(0xff, color);
			colorTextbox.Text = ColorTranslator.ToHtml(colorEntry.ColorPallette).TrimStart('#');
			SetUpValueChanged(true);
		}

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if (waterTreeView.SelectedNode.Tag == null)
				return;

			addColorToolStripMenuItem.Visible = false;
			removeColorToolStripMenuItem.Visible = false;

			colorTreeView.SelectedNode = null;
			underwaterTreeView.SelectedNode = null;
			fogTreeView.SelectedNode = null;

			var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
            Color color = colorEntry.SurfaceColor;
			SetUpValueChanged(false);
			
			alphaUpDown.Value = color.A;
			redUpDown.Value = color.R;
			greenUpDown.Value = color.G;
			blueUpDown.Value = color.B;

			alphaUpDown.Enabled = true;
			alphaUpDown.Visible = true;
			alphaLabel.Visible = true;
			pictureBox1.BackColor = colorEntry.SurfaceColor;
			colorTextbox.Text = ColorTranslator.ToHtml(colorEntry.SurfaceColor).TrimStart('#');
			SetUpValueChanged(true);
		}

		private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (underwaterTreeView.SelectedNode.Tag == null)
				return;

            addColorToolStripMenuItem.Visible = false;
            removeColorToolStripMenuItem.Visible = false;

            colorTreeView.SelectedNode = null;
			waterTreeView.SelectedNode = null;
			fogTreeView.SelectedNode = null;

			var colorEntry = (ColorContainer.WaterColor)underwaterTreeView.SelectedNode.Tag;
            Color color = colorEntry.UnderwaterColor;
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color.R;
			greenUpDown.Value = color.G;
			blueUpDown.Value = color.B;
			pictureBox1.BackColor = Color.FromArgb(0xff, color);
			colorTextbox.Text = ColorTranslator.ToHtml(colorEntry.UnderwaterColor).TrimStart('#');
			SetUpValueChanged(true);
		}

		private void treeView4_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (fogTreeView.SelectedNode.Tag == null)
				return;

            addColorToolStripMenuItem.Visible = false;
            removeColorToolStripMenuItem.Visible = false;

            colorTreeView.SelectedNode = null;
			waterTreeView.SelectedNode = null;
			underwaterTreeView.SelectedNode = null;

			var colorEntry = (ColorContainer.WaterColor)fogTreeView.SelectedNode.Tag;
            Color color = colorEntry.FogColor;
			SetUpValueChanged(false);
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color.R;
			greenUpDown.Value = color.G;
			blueUpDown.Value = color.B;
			pictureBox1.BackColor = Color.FromArgb(0xff, color);
			colorTextbox.Text = ColorTranslator.ToHtml(colorEntry.FogColor).TrimStart('#');
			SetUpValueChanged(true);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Save();
            DialogResult = DialogResult.OK;
        }

		public void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
            TreeNode node = colorTreeView.SelectedNode;
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
			TreeNode node = waterTreeView.SelectedNode;
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
            TreeNode node = underwaterTreeView.SelectedNode;
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
            TreeNode node = fogTreeView.SelectedNode;
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

		public bool IsValidHexString(string value)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(value, @"\A\b[0-9a-fA-F]+\b\Z") && value.Length == 6;
		}

		private void colorBox_TextChanged(object sender, EventArgs e)
		{
			if(IsValidHexString(colorTextbox.Text))
            {
				Color color = ColorTranslator.FromHtml("#" + colorTextbox.Text);

				redUpDown.Value = color.R;
				greenUpDown.Value = color.G;
				blueUpDown.Value = color.B;
            }
		}

		private void color_ValueChanged(object sender, EventArgs e)
		{
			Color color = Color.FromArgb(tabControl.SelectedTab == waterTab ? (int)alphaUpDown.Value : 255, (int)redUpDown.Value, (int)greenUpDown.Value, (int)blueUpDown.Value);

			if (tabControl.SelectedTab == colorsTab)
            {
				var colorEntry = (ColorContainer.Color)colorTreeView.SelectedNode.Tag;
				colorEntry.ColorPallette = color;
			}
			else
            {
				var waterColorEntry = (tabControl.SelectedTab.Controls[0] as TreeView).SelectedNode.Tag as ColorContainer.WaterColor;

				if (tabControl.SelectedTab == waterTab)
					waterColorEntry.SurfaceColor = color;
				else if (tabControl.SelectedTab == underwaterTab)
					waterColorEntry.UnderwaterColor = color;
				else
					waterColorEntry.FogColor = color;
			}

			pictureBox1.BackColor = color;
			colorTextbox.Text = ColorTranslator.ToHtml(color).TrimStart('#');
		}

		private void setColorBtn_Click(object sender, EventArgs e)
        {
			ColorDialog colorPick = new ColorDialog();
			colorPick.AllowFullOpen = true;
			colorPick.AnyColor = true;
			colorPick.SolidColorOnly = tabControl.SelectedTab == colorsTab;
			if (colorPick.ShowDialog(this) != DialogResult.OK)
				return;
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
			var colorEntry = (ColorContainer.WaterColor)waterTreeView.SelectedNode.Tag;
			pictureBox1.BackColor = colorEntry.SurfaceColor = Color.FromArgb((int)alphaUpDown.Value, colorEntry.SurfaceColor);
		}

		private void restoreOriginalColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(tabControl.SelectedTab is var tab && tab != null)
            {
				TreeNode node = (tabControl.SelectedTab.Controls[0] as TreeView).SelectedNode;

				Color color = Color.Empty;

				if (tab == colorsTab)
                {
					ColorContainer.Color col_entry = _defaultColourfile.Colors.Find(color => color.Name == node.Text);

                    if (col_entry == null)
						return;

                    color = col_entry.ColorPallette;

					colorTextbox.Text = ColorTranslator.ToHtml(color).TrimStart('#');
				}
				else
                {
                    ColorContainer.WaterColor waterEntry = _defaultColourfile.WaterColors.Find(color => color.Name == node.Text);

					if (waterEntry == null)
						return;

					color = 
						tab == waterTab ? waterEntry.SurfaceColor : 
						tab == underwaterTab ? waterEntry.UnderwaterColor : waterEntry.FogColor;

					if (tab == waterTab)
                    {
						alphaUpDown.Value = color.A;
					}

					redUpDown.Value = color.R;
					greenUpDown.Value = color.G;
					blueUpDown.Value = color.B;
				}
				pictureBox1.BackColor = Color.FromArgb(tab == colorsTab ? 0xFF : color.A, color);
			}
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
                foreach (TreeNode node in colorCache)
				{
					if (node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						colorTreeView.Nodes.Add((TreeNode)node.Clone());
					}
				}
                foreach (TreeNode node in waterCache)
				{
					if (node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						waterTreeView.Nodes.Add((TreeNode)node.Clone());
					}
				}
                foreach (TreeNode node in underwaterCache)
				{
					if (node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						underwaterTreeView.Nodes.Add((TreeNode)node.Clone());
					}
				}
                foreach (TreeNode node in fogCache)
				{
					if (node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						fogTreeView.Nodes.Add((TreeNode)node.Clone());
					}
				}
			}
			else
			{
				foreach (TreeNode node in colorCache)
				{
					colorTreeView.Nodes.Add((TreeNode)node.Clone());
				}
				foreach (TreeNode node in waterCache)
				{
					waterTreeView.Nodes.Add((TreeNode)node.Clone());
				}
				foreach (TreeNode node in underwaterCache)
				{
					underwaterTreeView.Nodes.Add((TreeNode)node.Clone());
				}
				foreach (TreeNode node in fogCache)
				{
					fogTreeView.Nodes.Add((TreeNode)node.Clone());
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
			_clipboard_color = colorTextbox.Text;
		}

		private void pasteColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			colorTextbox.Text = _clipboard_color;
		}

        private void colorTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
			string hexCheck = "0123456789abcdefABCDEF\b";

			e.Handled = !hexCheck.Contains(e.KeyChar);
		}

        private void stripPS4BiomesToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if(EditorValue.WaterColors.Count > 0)
            {
				foreach (ColorContainer.WaterColor col in EditorValue.WaterColors.ToList())
				{
					if (PS4Biomes.Contains(col.Name))
						EditorValue.WaterColors.Remove(col);
				}

				SetUpTable(false);
			}
		}

        private void addColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if(tabControl.SelectedTab == colorsTab)
			{
				using (TextPrompt prompt = new TextPrompt())
				{
					prompt.OKButtonText = "Add";
					prompt.contextLabel.Text = "Insert the name of the color you would like to add";
					if(prompt.ShowDialog(this) == DialogResult.OK)
					{
						ColorContainer.Color entry = new ColorContainer.Color();
						entry.Name = prompt.NewText;
						entry.ColorPallette = Color.FromArgb(0xFFFFFF);

						if(EditorValue.Colors.Find(c => c.Name == entry.Name) != null)
						{
							MessageBox.Show(this, $"\"{entry.Name}\" already exists in this color table", "Color not added");
						}

						EditorValue.Colors.Add(entry);
						AddEntry(colorTreeView, colorCache, entry.Name, entry);
                    }
				}
			}
        }

        private void removeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (tabControl.SelectedTab == colorsTab 
				&& colorTreeView.SelectedNode is TreeNode entry 
				&& entry != null 
				&& entry.Tag is ColorContainer.Color color)
            {
                EditorValue.Colors.Remove(color);
                RemoveEntry(entry, colorCache);
            }
        }
    }
}
