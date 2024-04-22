using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;
using Newtonsoft.Json.Linq;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Additional_Popups.EntityForms
{
	public partial class AddEntry : ThemeForm
	{
        string selectedEntity = "";

		private static JObject EntityJSONData = JObject.Parse(Properties.Resources.entityData);
		public string SelectedEntity => selectedEntity;

		List<TreeNode> treeViewEntityCache = new List<TreeNode>();

		public AddEntry(string dataType, System.Drawing.Image[] entityImages)
		{
			InitializeComponent();
			ImageList entities = new ImageList();
			entities.ColorDepth = ColorDepth.Depth32Bit;
			entities.ImageSize = new System.Drawing.Size(32, 32);
			entities.Images.AddRange(entityImages);
			treeViewEntity.ImageList = entities;

			try
			{
				int i = 0;

				if (EntityJSONData[dataType] != null)
				{
					foreach (JObject content in EntityJSONData[dataType].Children())
					{
						foreach (JProperty prop in content.Properties())
						{
							if (!string.IsNullOrEmpty((string)prop.Value))
							{
								TreeNode entityNode = new TreeNode((string)prop.Value)
								{
									Tag = prop.Name,
									ImageIndex = i,
									SelectedImageIndex = i,
								};
								treeViewEntity.Nodes.Add(entityNode);
								treeViewEntityCache.Add(entityNode);
							}
							i++;
						}
					}
				}
			}
			catch (Newtonsoft.Json.JsonException j_ex)
			{
				MessageBox.Show(j_ex.Message, "Error");
				return;
			}

			treeViewEntity.Sort();
		}

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is string entityData)
			{
				selectedEntity = entityData;
				Console.WriteLine(selectedEntity);
			}
		}

		void filter_TextChanged(object sender, EventArgs e)
		{
			// Some code in this function is modified code from this StackOverflow answer - MattNL
			//https://stackoverflow.com/questions/8260322/filter-a-treeview-with-a-textbox-in-a-c-sharp-winforms-app

			//blocks repainting tree until all objects loaded
			treeViewEntity.BeginUpdate();
			treeViewEntity.Nodes.Clear();
			if (!string.IsNullOrEmpty(metroTextBox1.Text))
			{
				foreach (TreeNode _node in treeViewEntityCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()) || 
						(_node.Tag as string).ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						treeViewEntity.Nodes.Add((TreeNode)_node.Clone());
					}
				}
			}
			else
			{
				foreach (TreeNode _node in treeViewEntityCache)
				{
					treeViewEntity.Nodes.Add((TreeNode)_node.Clone());
				}
			}
			//enables redrawing tree after all objects have been added
			treeViewEntity.EndUpdate();
		}

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddNtb_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedEntity)) CancelBtn_Click(sender, e);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
