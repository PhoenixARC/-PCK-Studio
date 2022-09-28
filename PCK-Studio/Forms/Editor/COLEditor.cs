using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.COL;

namespace PckStudio.Forms.Editor
{
	public partial class COLEditor : MetroForm
	{
		COLFile default_colourfile;
		COLFile colourfile;

		private readonly PCKFile.FileData _file;

		public COLEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using(var stream = new MemoryStream(file.data))
			{
                colourfile = COLFileReader.Read(stream);
			}

			using (var stream = new MemoryStream(Properties.Resources.colours))
			{
				default_colourfile = COLFileReader.Read(stream);
			}

			foreach (var obj in default_colourfile.entries)
			{
				COLFile.ColorEntry entry = colourfile.entries.Find(color => color.name == obj.name);
				TreeNode tn = new TreeNode(obj.name);
				tn.Tag = entry != null ? entry : obj;
				colorTreeView.Nodes.Add(tn);
			}
			foreach (var obj in colourfile.waterEntries)
			{
				COLFile.ExtendedColorEntry entry = colourfile.waterEntries.Find(color => color.name == obj.name);
				TreeNode tn = new TreeNode(obj.name);
				tn.Tag = entry != null ? entry : obj;
				waterTreeView.Nodes.Add(tn);
				TreeNode tnB = new TreeNode(obj.name);
				tnB.Tag = entry != null ? entry : obj;
				underwaterTreeView.Nodes.Add(tnB);
				TreeNode tnC = new TreeNode(obj.name);
				tnC.Tag = entry != null ? entry : obj;
				fogTreeView.Nodes.Add(tnC);
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (colorTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ColorEntry)colorTreeView.SelectedNode.Tag;
			var color = colorEntry.color;
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)color);
			
		}

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if (waterTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ExtendedColorEntry)waterTreeView.SelectedNode.Tag;
			int color = (int)colorEntry.color;
			alphaUpDown.Enabled = true;
			alphaUpDown.Visible = true;
			alphaLabel.Visible = true;
			alphaUpDown.Value = color >> 24 & 0xff;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(color);
		}

		private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (underwaterTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ExtendedColorEntry)underwaterTreeView.SelectedNode.Tag;
			int color = (int)colorEntry.color_b;
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(255, Color.FromArgb(0xff << 24 | color));
		}

		private void treeView4_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (fogTreeView.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ExtendedColorEntry)fogTreeView.SelectedNode.Tag;
			int color = (int)colorEntry.color_c;
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(255, Color.FromArgb(0xff << 24 | color));
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
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
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ColorEntry colorInfo)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
		}

        private void treeView2_KeyDown(object sender, KeyEventArgs e)
        {
			var node = waterTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry colorInfo)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
        }

		private void treeView3_KeyDown(object sender, KeyEventArgs e)
		{
			var node = underwaterTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry colorInfo)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
			}
		}

		private void treeView4_KeyDown(object sender, KeyEventArgs e)
		{
			var node = fogTreeView.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry colorInfo)
			{
				restoreOriginalColorToolStripMenuItem_Click(sender, e);
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
			//TreeView tv = (TreeView)tabControl.SelectedTab.Controls[0];
			//if (tv.SelectedNode == null) return;
			//byte[] origHex = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString());
			//bool hasAlpha = tabControl.SelectedTab == waterTab;
			//string hex = "";
			//if (((NumericUpDown)sender).Name == "numericUpDown2")
			//{
			//	hex += ((int)alphaUpDown.Value).ToString("X2");
			//	hex += origHex[1].ToString("X2");
			//	hex += origHex[2].ToString("X2");
			//	hex += origHex[3].ToString("X2");
			//}
			//else if (((NumericUpDown)sender).Name == "numericUpDown3")
			//{
			//	if (hasAlpha) hex += origHex[0].ToString("X2");
			//	hex += ((int)redUpDown.Value).ToString("X2");
			//	hex += origHex[hasAlpha ? 2 : 1].ToString("X2");
			//	hex += origHex[hasAlpha ? 3 : 2].ToString("X2");
			//}
			//else if (((NumericUpDown)sender).Name == "numericUpDown4")
			//{
			//	if (hasAlpha) hex += origHex[0].ToString("X2");
			//	hex += origHex[hasAlpha ? 1 : 0].ToString("X2");
			//	hex += ((int)greenUpDown.Value).ToString("X2");
			//	hex += origHex[hasAlpha ? 3 : 2].ToString("X2");
			//}
			//else if (((NumericUpDown)sender).Name == "numericUpDown5")
			//{
			//	if (hasAlpha) hex += origHex[0].ToString("X2");
			//	hex += origHex[hasAlpha ? 1 : 0].ToString("X2");
			//	hex += origHex[hasAlpha ? 2 : 1].ToString("X2");
			//	hex += ((int)blueUpDown.Value).ToString("X2");
			//}
			//else // just in case some weird thing happens i dunno - matt
			//{
			//	if (hasAlpha) hex += origHex[0].ToString("X2");
			//	hex += origHex[hasAlpha ? 1 : 0].ToString("X2");
			//	hex += origHex[hasAlpha ? 2 : 1].ToString("X2");
			//	hex += origHex[hasAlpha ? 3 : 2].ToString("X2");
			//}

			//Console.WriteLine(hex);
			//colorTextbox.Text = hex;
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
			if (tabControl.SelectedTab == colorsTab && colorTreeView.SelectedNode != null &&
				colorTreeView.SelectedNode.Tag != null && colorTreeView.SelectedNode.Tag is COLFile.ColorEntry colorInfoD)
			{
				MessageBox.Show("Color");
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
				MessageBox.Show("Water");
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
				MessageBox.Show("Underwater");
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
				MessageBox.Show("Fog");
				COLFile.ExtendedColorEntry entry = default_colourfile.waterEntries.Find(color => color.name == fogTreeView.SelectedNode.Text);
				colorInfoC.color_c = entry.color_c;
				alphaUpDown.Value = colorInfoC.color_c >> 24 & 0xff;
				redUpDown.Value = colorInfoC.color_c >> 16 & 0xff;
				greenUpDown.Value = colorInfoC.color_c >> 8 & 0xff;
				blueUpDown.Value = colorInfoC.color_c & 0xff;
				pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)colorInfoC.color_c);
			}
		}
	}
}
