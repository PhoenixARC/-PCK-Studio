using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio.Forms.Utilities.AnimationEditor
{
	public partial class ChangeTile : MetroForm
	{
		string selectedTile = "";
		string oldTileName = "";
		public string SelectedTile
		{
			get { return selectedTile; }
			set { selectedTile = value; }
		}

		bool isItem = false;
		public bool IsItem
		{
			get { return isItem; }
		}

		List<TreeNode> treeView1Cache = new List<TreeNode>();
		List<TreeNode> treeView2Cache = new List<TreeNode>();

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Tuple<string, int> tileData = e.Node.Tag as Tuple<string, int>;
			Console.WriteLine(tileData.Item1 + " - " + tileData.Item2);
			selectedTile = tileData.Item1;
			Console.WriteLine(selectedTile);

			if (e.Node.TreeView == treeView1) isItem = false;
			if (e.Node.TreeView == treeView2) isItem = true;
		}

		public ChangeTile(string oldName = "")
		{
			oldTileName = oldName;
			InitializeComponent();

			ImageList tiles = new ImageList();
			tiles.ColorDepth = ColorDepth.Depth32Bit;

			for(int i = 1; i < 545; i++)
			{
				int row = (i - 1) / 16;
				int column = (i - 1) % 16;

				Rectangle tileArea = new Rectangle(new Point(column * 16, row * 16), new Size(16, 16));

				Bitmap tileImage = new Bitmap(16, 16);
				using (Graphics gfx = Graphics.FromImage(tileImage))
				{
					gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
					gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

					gfx.DrawImage(Properties.Resources.terrain_sheet, new Rectangle(0, 0, 16, 16), tileArea, GraphicsUnit.Pixel);
				}

				tiles.Images.Add(tileImage);
			}
			for (int i = 1; i < 273; i++)
			{
				int row = (i - 1) / 16;
				int column = (i - 1) % 16;

				Rectangle tileArea = new Rectangle(new Point(column * 16, row * 16), new Size(16, 16));

				Bitmap tileImage = new Bitmap(16, 16);
				using (Graphics gfx = Graphics.FromImage(tileImage))
				{
					gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
					gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

					gfx.DrawImage(Properties.Resources.items_sheet, new Rectangle(0, 0, 16, 16), tileArea, GraphicsUnit.Pixel);
				}

				tiles.Images.Add(tileImage);
			}

			try
			{
				Newtonsoft.Json.Linq.JObject tileData = Newtonsoft.Json.Linq.JObject.Parse(Encoding.Default.GetString(Properties.Resources.tileData));

				int i = 0;

				if (tileData["Blocks"] != null)
				{
					foreach (Newtonsoft.Json.Linq.JObject content in tileData["Blocks"].Children())
					{
						foreach (Newtonsoft.Json.Linq.JProperty prop in content.Properties())
						{
							if (!String.IsNullOrEmpty((string)prop.Value))
							{
								TreeNode tileNode = new TreeNode();
								tileNode.Text = (string)prop.Value;
								Tuple<string, int> finalTileData = new Tuple<string, int>(prop.Name, i);
								tileNode.Tag = finalTileData;
								tileNode.ImageIndex = i;
								tileNode.SelectedImageIndex = i;
								treeView1.Nodes.Add(tileNode);
								treeView1Cache.Add(tileNode);
							}
							i++;
						}
					}
				}
				if (tileData["Items"] != null)
				{
					foreach (Newtonsoft.Json.Linq.JObject content in tileData["Items"].Children())
					{
						foreach (Newtonsoft.Json.Linq.JProperty prop in content.Properties())
						{
							if (!String.IsNullOrEmpty((string)prop.Value))
							{
								TreeNode tileNode = new TreeNode();
								tileNode.Text = (string)prop.Value;
								Tuple<string, int> finalTileData = new Tuple<string, int>(prop.Name, i);
								tileNode.Tag = finalTileData;
								tileNode.ImageIndex = i;
								tileNode.SelectedImageIndex = i;
								treeView2.Nodes.Add(tileNode);
								treeView2Cache.Add(tileNode);
							}
							i++;
						}
					}
				}

				treeView1.ImageList = tiles;
				treeView2.ImageList = tiles;
				Blocks.Controls.Add(treeView1);
				Items.Controls.Add(treeView2);
			}
			catch (Newtonsoft.Json.JsonException j_ex)
			{
				MessageBox.Show(j_ex.Message, "Error");
				return;
			}
		}

		void filter_TextChanged(object sender, EventArgs e)
		{
			// Some code in this function is modified code from this StackOverflow answer - MattNL
			//https://stackoverflow.com/questions/8260322/filter-a-treeview-with-a-textbox-in-a-c-sharp-winforms-app

			//blocks repainting tree until all objects loaded
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			treeView2.BeginUpdate();
			treeView2.Nodes.Clear();
			if (metroTextBox1.Text != string.Empty)
			{
				foreach (TreeNode _node in treeView1Cache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						treeView1.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				foreach (TreeNode _node in treeView2Cache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						treeView2.Nodes.Add((TreeNode)_node.Clone());
					}
				}
			}
			else
			{
				foreach (TreeNode _node in treeView1Cache)
				{
					treeView1.Nodes.Add((TreeNode)_node.Clone());
				}
				foreach (TreeNode _node in treeView2Cache)
				{
					treeView2.Nodes.Add((TreeNode)_node.Clone());
				}
			}
			//enables redrawing tree after all objects have been added
			treeView1.EndUpdate();
			treeView2.EndUpdate();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			selectedTile = oldTileName;
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(selectedTile)) return;
			Close();
		}
	}
}
