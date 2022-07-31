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
using System.Drawing.Drawing2D;
using Newtonsoft.Json.Linq;
using PckStudio.Properties;

namespace PckStudio.Forms.Utilities.AnimationEditor
{
	public partial class ChangeTile : MetroForm
	{
		string selectedTile = "";
		string oldTileName = "";
		bool isItem = false;
		public string SelectedTile => selectedTile;
		public bool IsItem => isItem;

		List<TreeNode> treeView1Cache = new List<TreeNode>();
		List<TreeNode> treeView2Cache = new List<TreeNode>();

		public ChangeTile(string oldName = "")
		{
			oldTileName = oldName;
			InitializeComponent();
			ImageList tiles = new ImageList();
			
			tiles.ColorDepth = ColorDepth.Depth32Bit;

			//for (int i = 0; i < 545; i++)
			//{
			//	int row = i / 16;
			//	int column = i % 16;

			//	Rectangle tileArea = new Rectangle(new Point(column * 16, row * 16), new Size(16, 16));

			//	Bitmap tileImage = new Bitmap(16, 16);
			//	using (Graphics gfx = Graphics.FromImage(tileImage))
			//	{
			//		gfx.SmoothingMode = SmoothingMode.None;
			//		gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			//		gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

			//		gfx.DrawImage(Resources.terrain_sheet, new Rectangle(0, 0, 16, 16), tileArea, GraphicsUnit.Pixel);
			//	}

			//	tiles.Images.Add(tileImage);
			//}
			//for (int i = 0; i < 273; i++)
			//{
			//	int row = i / 16;
			//	int column = i % 16;

			//	Rectangle tileArea = new Rectangle(new Point(column * 16, row * 16), new Size(16, 16));

			//	Bitmap tileImage = new Bitmap(16, 16);
			//	using (Graphics gfx = Graphics.FromImage(tileImage))
			//	{
			//		gfx.SmoothingMode = SmoothingMode.None;
			//		gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			//		gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

			//		gfx.DrawImage(Resources.items_sheet, new Rectangle(0, 0, 16, 16), tileArea, GraphicsUnit.Pixel);
			//	}

			//	tiles.Images.Add(tileImage);
			//}

			tiles.Images.AddRange(CreateImageList(Resources.terrain_sheet, 16, 16).ToArray());
			tiles.Images.AddRange(CreateImageList(Resources.items_sheet, 16, 16).ToArray());
			treeView1.ImageList = tiles;
			treeView2.ImageList = tiles;

			try
			{
                JObject tileData = JObject.Parse(Resources.tileData);
				int i = 0;

				if (tileData["Blocks"] != null)
				{
					foreach (JObject content in tileData["Blocks"].Children())
					{
						foreach (JProperty prop in content.Properties())
						{
							if (!string.IsNullOrEmpty((string)prop.Value))
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
					foreach (JObject content in tileData["Items"].Children())
					{
                        foreach (JProperty prop in content.Properties())
						{
							if (!string.IsNullOrEmpty((string)prop.Value))
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

				//Blocks.Controls.Add(treeView1);
				//Items.Controls.Add(treeView2);
			}
			catch (Newtonsoft.Json.JsonException j_ex)
			{
				MessageBox.Show(j_ex.Message, "Error");
				return;
			}
		}

		private IEnumerable<Image> CreateImageList(Image source, int width, int height)
        {
            int img_row_count = source.Width / width;
            int img_column_count = source.Height / height;
			for (int i = 0; i < img_column_count * img_row_count; i++)
			{
                int row = i / width;
                int column = i % height;
                Rectangle tileArea = new Rectangle(new Point(column * width, row * height), new Size(16, 16));
                Bitmap tileImage = new Bitmap(width, height);
				using (Graphics gfx = Graphics.FromImage(tileImage))
				{
					gfx.SmoothingMode = SmoothingMode.None;
					gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

					gfx.DrawImage(source, new Rectangle(0, 0, width, height), tileArea, GraphicsUnit.Pixel);
				}
				yield return tileImage;
			}
			yield break;
		}

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Tuple<string, int> tileData = e.Node.Tag as Tuple<string, int>;
			Console.WriteLine(tileData.Item1 + " - " + tileData.Item2);
			selectedTile = tileData.Item1;
			Console.WriteLine(selectedTile);
			isItem = e.Node.TreeView == treeView2;
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
			if (string.IsNullOrEmpty(selectedTile)) button2_Click(sender, e);
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
