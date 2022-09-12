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
		COLFile colurfile;

		private readonly PCKFile.FileData _file;

		public COLEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using(var stream = new MemoryStream(file.data))
			{
                colurfile = COLFileReader.Read(stream);
			}

			foreach (var obj in colurfile.entries)
			{
				TreeNode tn = new TreeNode(obj.name);
				tn.Tag = obj;
				treeView1.Nodes.Add(tn);
			}
			foreach (var obj in colurfile.waterEntries)
			{
				TreeNode tn = new TreeNode(obj.name);
				tn.Tag = obj;
				treeView2.Nodes.Add(tn);
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView1.SelectedNode.Tag == null)
				return;

			var colorEntry = (COLFile.ColorEntry)treeView1.SelectedNode.Tag;
			alphaUpDown.Visible = false;
			alphaLabel.Visible = false;
			var color = colorEntry.color;
			redUpDown.Value = color >> 16 & 0xff;
			greenUpDown.Value = color >> 8 & 0xff;
			blueUpDown.Value = color & 0xff;
			pictureBox1.BackColor = Color.FromArgb(0xff << 24 | (int)color);
			
		}

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if (treeView2.SelectedNode.Tag == null)
				return;
			var colorEntry = (COLFile.ExtendedColorEntry)treeView2.SelectedNode.Tag;
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
		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (var stream = new MemoryStream())
			{
				COLFileWriter.Write(stream, colurfile);
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
			var node = treeView1.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ColorEntry colorInfo)
			{
                colurfile.entries.Remove(colorInfo);
                if (treeView1.Nodes.Count > 0) treeView1.Nodes.Remove(node);
            }
		}

        private void treeView2_KeyDown(object sender, KeyEventArgs e)
        {
			var node = treeView2.SelectedNode;
			if (e.KeyCode == Keys.Delete && node.Tag is COLFile.ExtendedColorEntry)
			{
                colurfile.waterEntries.Remove((COLFile.ExtendedColorEntry)node.Tag);
                if (treeView2.Nodes.Count > 0) treeView2.Nodes.Remove(node);
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
			if (tabControl.SelectedTab == waterTab && treeView2.SelectedNode != null &&
				treeView2.SelectedNode.Tag != null && treeView2.SelectedNode.Tag is COLFile.ExtendedColorEntry)
			{
				var colorEntry = ((COLFile.ExtendedColorEntry)treeView2.SelectedNode.Tag);
				colorEntry.color = (uint)colorPick.Color.ToArgb();
			}
			else if (tabControl.SelectedTab == colorsTab && treeView1.SelectedNode != null &&
				treeView1.SelectedNode.Tag != null && treeView1.SelectedNode.Tag is COLFile.ColorEntry)
			{
				var colorEntry = ((COLFile.ColorEntry)treeView1.SelectedNode.Tag);
				colorEntry.color = (uint)colorPick.Color.ToArgb() & 0xffffff;
			}
			colorPick.Dispose();
        }
    }
}
