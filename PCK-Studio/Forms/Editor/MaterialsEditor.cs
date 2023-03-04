﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Materials;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PckStudio.Forms.Editor
{
	public partial class MaterialsEditor : MetroForm
	{
		// Behaviours File Format research by Miku and MattNL
		private readonly PCKFile.FileData _file;
		MaterialsFile materialFile;

		void SetUpTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (var entry in materialFile.entries)
			{
				TreeNode EntryNode = new TreeNode(entry.name);

				foreach (JObject content in Utilities.MaterialUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.name);
					if (prop is JProperty)
					{
						EntryNode.Text = (string)prop.Value;
						EntryNode.ImageIndex = Utilities.MaterialUtil.entityData["entities"].Children().ToList().IndexOf(content);
						EntryNode.SelectedImageIndex = EntryNode.ImageIndex;
						break;
					}
				}

				EntryNode.Tag = entry;

				treeView1.Nodes.Add(EntryNode);
			}
			treeView1.EndUpdate();
		}

		public MaterialsEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using (var stream = new MemoryStream(file.Data))
			{
				materialFile = MaterialsReader.Read(stream);
			}

			treeView1.ImageList = new ImageList();
			Utilities.MaterialUtil.entityImages.ToList().ForEach(img => treeView1.ImageList.Images.Add(img));
			treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			SetUpTree();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null) return;

			bool enable = e.Node.Tag is MaterialsFile.MaterialEntry && treeView1.SelectedNode != null;
			materialComboBox.Enabled = enable;

			if (e.Node.Tag is MaterialsFile.MaterialEntry entry)
			{
				materialComboBox.SelectedIndexChanged -= materialComboBox_SelectedIndexChanged;
				materialComboBox.SelectedIndex = materialComboBox.Items.IndexOf(entry.material_type);
				materialComboBox.SelectedIndexChanged += materialComboBox_SelectedIndexChanged;
			}
		}
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return;

			treeView1.SelectedNode.Remove();
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			materialComboBox.Enabled = false;
		}

		private void addNewPositionOverrideToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void addNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete) removeToolStripMenuItem_Click(sender, e);
		}

		private void treeView1_MouseHover(object sender, EventArgs e)
		{

		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (var stream = new MemoryStream())
			{
				materialFile = new MaterialsFile();

				foreach (TreeNode node in treeView1.Nodes)
				{
					if(node.Tag is MaterialsFile.MaterialEntry entry)
					{
						materialFile.entries.Add(entry);
					}
				}

				MaterialsWriter.Write(stream, materialFile);
				_file.SetData(stream.ToArray());
			}
			DialogResult = DialogResult.OK;
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var diag = new Additional_Popups.EntityForms.AddEntry(Utilities.MaterialUtil.entityData, Utilities.MaterialUtil.entityImages);

			if (diag.ShowDialog() == DialogResult.OK)
			{
				if (String.IsNullOrEmpty(diag.SelectedEntity)) return;
				if (materialFile.entries.FindAll(mat => mat.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity. Please use the \"Add New Position Override\" tool to add multiple overrides for entities", "Error", MessageBoxButtons.OK);
					return;
				}
				MaterialsFile.MaterialEntry NewEntry = new MaterialsFile.MaterialEntry();
				NewEntry.name = diag.SelectedEntity;
				NewEntry.material_type = "entity_alphatest";

				TreeNode NewEntryNode = new TreeNode(NewEntry.name);
				NewEntryNode.Tag = NewEntry;
				foreach (JObject content in Utilities.MaterialUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == NewEntry.name);
					if (prop is JProperty)
					{
						NewEntryNode.Text = (string)prop.Value;
						NewEntryNode.ImageIndex = Utilities.MaterialUtil.entityData["entities"].Children().ToList().IndexOf(content);
						NewEntryNode.SelectedImageIndex = NewEntryNode.ImageIndex;
						break;
					}
				}
				treeView1.Nodes.Add(NewEntryNode);

				addNewPositionOverrideToolStripMenuItem_Click(sender, e); // adds a Position Override to the new Override
			}
		}

		private void materialComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is MaterialsFile.MaterialEntry entry)
			{
				entry.material_type = materialComboBox.SelectedItem.ToString();
				treeView1.SelectedNode.Tag = entry;
			}
		}
	}
}
