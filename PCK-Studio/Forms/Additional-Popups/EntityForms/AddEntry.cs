using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PckStudio.Controls;
using PckStudio.Core.Json;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Json;


namespace PckStudio.Forms.Additional_Popups.EntityForms
{
	/// Wrapper class kept for simplicity
	public class AddEntry
	{
        string selectedEntity = "";
		public string SelectedEntity => selectedEntity;

		private FilterPrompt filterPrompt;

		public AddEntry(string dataType, System.Drawing.Image[] entityImages)
		{
			filterPrompt = new FilterPrompt();
            filterPrompt.OnSelectedItemChanged += FilterPrompt_OnSelectedItemChanged;
            TreeView treeViewEntity = filterPrompt.AddFilterPage("Entities", null, filterPredicate);
            ImageList entities = new ImageList();
			entities.ColorDepth = ColorDepth.Depth32Bit;
			entities.ImageSize = new System.Drawing.Size(32, 32);
			entities.Images.AddRange(entityImages);
			treeViewEntity.ImageList = entities;

            List<EntityInfo> entityInfos = dataType switch
			{
				"models" => Entities.ModelInfos,
				"materials" => Entities.MaterialInfos,
				"behaviours" => Entities.BehaviourInfos,
				_ => null,
			};

			int i = 0;

			foreach(EntityInfo entity in entityInfos)
            {
				TreeNode entityNode = new TreeNode(entity.DisplayName)
				{
					Tag = entity.InternalName,
					ImageIndex = i,
					SelectedImageIndex = i,
				};
				i++;
				if (!string.IsNullOrEmpty(entity.InternalName))
                {
					treeViewEntity.Nodes.Add(entityNode);
					(treeViewEntity.Tag as List<TreeNode>).Add(entityNode);
				}
			}

			treeViewEntity.Sort();
		}

        private void FilterPrompt_OnSelectedItemChanged(object sender, EventArgs e)
        {
			selectedEntity = filterPrompt.SelectedItem.ToString();
        }

        public DialogResult ShowDialog(IWin32Window owner)
		{
            var f = new ImmersiveForm();
            filterPrompt.Dock = DockStyle.Fill;
            filterPrompt.Update();
            f.Controls.Add(filterPrompt);
            return f.ShowDialog(owner);
        }

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is string entityData)
			{
				selectedEntity = entityData;
			}
		}

		private bool filterPredicate(string filterText, object nodeTag)
		{
			return nodeTag is string a && a.ToLower().Contains(filterText.ToLower());
        }
	}
}
