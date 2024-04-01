using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;
using Newtonsoft.Json.Linq;
using PckStudio.Internal.Json;

namespace PckStudio.Forms.Additional_Popups.EntityForms
{
	public partial class AddEntry : MetroForm
	{
        string selectedEntity = "";
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

			var entityInfos = dataType switch
			{
				"models" => Entities.ModelInfos,
				"materials" => Entities.MaterialInfos,
				"behaviours" => Entities.BehaviourInfos,
				_ => null,
			};

			int i = 0;

			foreach(var entity in entityInfos)
            {
				TreeNode entityNode = new TreeNode(entity.DisplayName)
				{
					Tag = entity.InternalName,
					ImageIndex = i,
					SelectedImageIndex = i,
				};
				i++;
				if (!String.IsNullOrEmpty(entity.InternalName))
                {
					treeViewEntity.Nodes.Add(entityNode);
					treeViewEntityCache.Add(entityNode);
				}
			}

			treeViewEntity.Sort();
		}

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is string entityData)
			{
				selectedEntity = entityData;
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

		private void AcceptBtn_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(selectedEntity)) CancelBtn_Click(sender, e);
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
