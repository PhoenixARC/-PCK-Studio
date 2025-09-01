using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OMI.Formats.Material;
using PckStudio.Internal;
using PckStudio.Core.Extensions;
using PckStudio.Core.Json;
using PckStudio.Internal.App;
using PckStudio.Interfaces;
using PckStudio.Json;

namespace PckStudio.Forms.Editor
{
	// Materials File Format research by PhoenixARC
	public partial class MaterialsEditor : EditorForm<MaterialContainer>
	{
		private readonly List<EntityInfo> MaterialData = Entities.BehaviourInfos;

		private bool ShowInvalidEntries;

		public MaterialsEditor(MaterialContainer materials, ISaveContext<MaterialContainer> saveContext)
			: base(materials, saveContext)
		{
			InitializeComponent();
            if (EditorValue.HasInvalidEntries())
            {
                DialogResult dr = MessageBox.Show(this, "Unsupported entities were found in this file. Would you like to display them?", "Invalid data found", MessageBoxButtons.YesNo);

                ShowInvalidEntries = dr == DialogResult.Yes;
            }

            treeView1.ImageList = new ImageList();
            ApplicationScope.EntityImages.ToList().ForEach(treeView1.ImageList.Images.Add);
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            UpdateTreeview();
        }

		void UpdateTreeview()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (MaterialContainer.Material entry in EditorValue)
			{
				TreeNode entryNode = new TreeNode(entry.Name);
				// index for invalid entry
				entryNode.ImageIndex = 127;
				entryNode.SelectedImageIndex = 127;
				entryNode.Tag = entry;

                EntityInfo material = MaterialData.Find(m => m.InternalName == entry.Name);
				
				// check for invalid material entry
				if (material is null)
                {
					entryNode.Text += " (Invalid)";
					if (!ShowInvalidEntries)
						continue;
				}
				else
				{
					entryNode.Text = material.DisplayName;
                    entryNode.SelectedImageIndex = entryNode.ImageIndex = MaterialData.IndexOf(material);
				}
				treeView1.Nodes.Add(entryNode);
			}
			treeView1.EndUpdate();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
				return;

			bool enable = e.Node.Tag is MaterialContainer.Material && treeView1.SelectedNode != null;
			materialComboBox.Enabled = enable;

			if (e.Node.Tag is MaterialContainer.Material entry)
			{
				materialComboBox.SelectedIndexChanged -= materialComboBox_SelectedIndexChanged;
				materialComboBox.SelectedIndex = materialComboBox.Items.IndexOf(entry.Type);
				materialComboBox.SelectedIndexChanged += materialComboBox_SelectedIndexChanged;
			}
		}
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1?.SelectedNode?.Tag is MaterialContainer.Material material)
			{
				EditorValue.Remove(material);
				UpdateTreeview();
			}
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			materialComboBox.Enabled = false;
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				removeToolStripMenuItem_Click(sender, e);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Save();
			DialogResult = DialogResult.OK;
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var diag = new Additional_Popups.EntityForms.AddEntry("materials", ApplicationScope.EntityImages);

			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(diag.SelectedEntity))
					return;
				if (EditorValue.FindAll(mat => mat.Name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity.", "Error", MessageBoxButtons.OK);
					return;
				}
				var newEntry = new MaterialContainer.Material(diag.SelectedEntity, "entity_alphatest");

				EditorValue.Add(newEntry);
				UpdateTreeview();
			}
		}

		private void materialComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is MaterialContainer.Material entry)
			{
				entry.Type = materialComboBox.SelectedItem.ToString();
				treeView1.SelectedNode.Tag = entry;
			}
		}
	}
}
