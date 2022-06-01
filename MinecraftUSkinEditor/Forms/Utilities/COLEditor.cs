using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Forms.Utilities
{
	public partial class COLEditor : MetroForm
	{
		Classes.COL.COLFile cf = new Classes.COL.COLFile();
		PCKFile.FileData mf;
		TreeView treeView1 = new TreeView(); // Normal Color Table
		TreeView treeView2 = new TreeView(); // Water Color Table
		public COLEditor(byte[] data, PCKFile.FileData MineFile)
		{
			InitializeComponent();
			metroLabel6.Visible = false;
			numericUpDown2.Visible = false;
			cf.Open(data);
			mf = MineFile;

			#region TreeView Controls Setup

			treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			treeView1.ForeColor = System.Drawing.Color.White;
			treeView1.LabelEdit = false;
			treeView1.LineColor = System.Drawing.Color.White;
			treeView1.Name = "treeView1";
			treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
			treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViews_KeyDown);
			treeView1.Dock = DockStyle.Fill;

			treeView2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			treeView2.ForeColor = System.Drawing.Color.White;
			treeView2.LabelEdit = false;
			treeView2.LineColor = System.Drawing.Color.White;
			treeView2.Name = "treeView2";
			treeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
			treeView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViews_KeyDown);
			treeView2.Dock = DockStyle.Fill;

			#endregion

			colorsTab.Controls.Add(treeView1);
			waterTab.Controls.Add(treeView2);
			tabControl.TabStop = true;

			foreach (object[] obj in cf.entries)
			{
				TreeNode tn = new TreeNode();
				tn.Text = obj[0].ToString();
				tn.Tag = obj[1].ToString();
				treeView1.Nodes.Add(tn);
			}
			foreach (object[] obj in cf.waterEntries)
			{
				TreeNode tn = new TreeNode();
				tn.Text = obj[0].ToString();
				tn.Tag = obj[1].ToString();
				treeView2.Nodes.Add(tn);
			}
		}

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			//metroTextBox2.Text = tv.SelectedNode.Text;
			TreeView tv = (TreeView)sender;
			if (tv.SelectedNode.Tag != null && metroTextBox1.Text.Length % 2 == 0)
			{
				bool hasAlpha = tabControl.SelectedTab == waterTab;
				metroTextBox1.Text = tv.SelectedNode.Tag.ToString();

				if (hasAlpha)
				{
					numericUpDown2.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[0];
					numericUpDown3.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[1];
					numericUpDown4.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[2];
					numericUpDown5.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[3];
					pictureBox1.BackColor = Color.FromArgb(StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[0], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[1], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[2], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[3]);
				}
				else
				{
					numericUpDown3.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[0];
					numericUpDown4.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[1];
					numericUpDown5.Value = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[2];
					pictureBox1.BackColor = Color.FromArgb(255, StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[0], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[1], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[2]);
				}
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			cf.entries.Clear();
			cf.waterEntries.Clear();
			foreach (TreeNode tn in treeView1.Nodes)
			{
				cf.entries.Add(new object[] {tn.Text, tn.Tag.ToString() });
			}
			foreach (TreeNode tn in treeView2.Nodes)
			{
				cf.waterEntries.Add(new object[] { tn.Text, tn.Tag.ToString() });
			}
			mf.data = cf.Save();
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
			int val = (int)hex;
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}

		public void treeViews_KeyDown(object sender, KeyEventArgs e)
		{
			TreeView tv = (TreeView)sender;
			if (e.KeyCode == Keys.Delete)
			{
				//removes minefile node
				//cf.entries.Remove((object[])treeView1.SelectedNode.Tag);
				//if(treeView1.Nodes.Count > 0) treeView1.Nodes.Remove(treeView1.SelectedNode);
			}
		}

		private void metroTextBox1_TextChanged(object sender, EventArgs e)
		{
			if (metroTextBox1.Text.Length % 2 != 0) return;

			TreeView tv = (TreeView)tabControl.SelectedTab.Controls[0];

			tv.SelectedNode.Tag = metroTextBox1.Text;

			bool hasAlpha = tabControl.SelectedTab == waterTab;

			if (tv.SelectedNode.Tag != null && (metroTextBox1.Text.Length == (hasAlpha ? 8 : 6)))
			{
				numericUpDown3.Value = StringToByteArrayFastest(metroTextBox1.Text)[!hasAlpha ? 0 : 1];
				numericUpDown4.Value = StringToByteArrayFastest(metroTextBox1.Text)[!hasAlpha ? 1 : 2];
				numericUpDown5.Value = StringToByteArrayFastest(metroTextBox1.Text)[!hasAlpha ? 2 : 3];
				metroTextBox1.Text = tv.SelectedNode.Tag.ToString();
				if (hasAlpha)
				{
					metroTextBox1.MaxLength = 8;
					metroLabel6.Visible = true;
					numericUpDown2.Visible = true;
					numericUpDown2.Value = StringToByteArrayFastest(metroTextBox1.Text)[0];
					pictureBox1.BackColor = Color.FromArgb(StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[0], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[1], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[2], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[3]);
				}
				else
				{
					metroTextBox1.MaxLength = 6;
					metroLabel6.Visible = false;
					numericUpDown2.Visible = false;
					pictureBox1.BackColor = Color.FromArgb(255, StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[0], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[1], StringToByteArrayFastest(tv.SelectedNode.Tag.ToString())[2]);
				}
			}
		}

		private void color_ValueChanged(object sender, EventArgs e)
		{
			TreeView tv = (TreeView)tabControl.SelectedTab.Controls[0];
			if (tv.SelectedNode == null) return;
			byte[] origHex = StringToByteArrayFastest(tv.SelectedNode.Tag.ToString());
			bool hasAlpha = tabControl.SelectedTab == waterTab;
			string hex = "";
			if (((NumericUpDown)sender).Name == "numericUpDown2")
			{
				hex += ((int)numericUpDown2.Value).ToString("X2");
				hex += origHex[1].ToString("X2");
				hex += origHex[2].ToString("X2");
				hex += origHex[3].ToString("X2");
			}
			else if (((NumericUpDown)sender).Name == "numericUpDown3")
			{
				if (hasAlpha) hex += origHex[0].ToString("X2");
				hex += ((int)numericUpDown3.Value).ToString("X2");
				hex += origHex[hasAlpha ? 2 : 1].ToString("X2");
				hex += origHex[hasAlpha ? 3 : 2].ToString("X2");
			}
			else if (((NumericUpDown)sender).Name == "numericUpDown4")
			{
				if (hasAlpha) hex += origHex[0].ToString("X2");
				hex += origHex[hasAlpha ? 1 : 0].ToString("X2");
				hex += ((int)numericUpDown4.Value).ToString("X2");
				hex += origHex[hasAlpha ? 3 : 2].ToString("X2");
			}
			else if (((NumericUpDown)sender).Name == "numericUpDown5")
			{
				if (hasAlpha) hex += origHex[0].ToString("X2");
				hex += origHex[hasAlpha ? 1 : 0].ToString("X2");
				hex += origHex[hasAlpha ? 2 : 1].ToString("X2");
				hex += ((int)numericUpDown5.Value).ToString("X2");
			}
			else // just in case some weird thing happens i dunno - matt
			{
				if (hasAlpha) hex += origHex[0].ToString("X2");
				hex += origHex[hasAlpha ? 1 : 0].ToString("X2");
				hex += origHex[hasAlpha ? 2 : 1].ToString("X2");
				hex += origHex[hasAlpha ? 3 : 2].ToString("X2");
			}

			Console.WriteLine(hex);
			metroTextBox1.Text = hex;
		}
	}
}
