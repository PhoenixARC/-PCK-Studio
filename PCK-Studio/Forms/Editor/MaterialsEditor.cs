using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using Newtonsoft.Json.Linq;
using OMI.Formats.Pck;
using OMI.Formats.Material;
using OMI.Workers.Material;
using PckStudio.Internal;

namespace PckStudio.Forms.Editor
{
	public partial class MaterialsEditor : MetroForm
	{
		// Materials File Format research by PhoenixARC
		private readonly PckFile.FileData _file;
		MaterialContainer materialFile;

		private readonly JObject EntityJSONData = JObject.Parse(Properties.Resources.entityData);

		void SetUpTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (var entry in materialFile)
			{
				TreeNode EntryNode = new TreeNode(entry.Name);

				foreach (JObject content in EntityJSONData["materials"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.Name);
					if (prop is JProperty)
					{
						EntryNode.Text = (string)prop.Value;
						EntryNode.ImageIndex = EntityJSONData["materials"].Children().ToList().IndexOf(content);
						EntryNode.SelectedImageIndex = EntryNode.ImageIndex;
						break;
					}
				}

				EntryNode.Tag = entry;

				treeView1.Nodes.Add(EntryNode);
			}
			treeView1.EndUpdate();
		}

		public MaterialsEditor(PckFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using (var stream = new MemoryStream(file.Data))
			{
				var reader = new MaterialFileReader();
                materialFile = reader.FromStream(stream);
			}

			treeView1.ImageList = new ImageList();
			ApplicationScope.EntityImages.ToList().ForEach(treeView1.ImageList.Images.Add);
			treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			SetUpTree();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null) return;

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
				materialFile = new MaterialContainer();

				foreach (TreeNode node in treeView1.Nodes)
				{
					if(node.Tag is MaterialContainer.Material entry)
					{
						materialFile.Add(entry);
					}
				}

				var writer = new MaterialFileWriter(materialFile);
				writer.WriteToStream(stream);
				_file.SetData(stream.ToArray());
			}
			DialogResult = DialogResult.OK;
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var diag = new Additional_Popups.EntityForms.AddEntry("materials", ApplicationScope.EntityImages);

			if (diag.ShowDialog() == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(diag.SelectedEntity)) return;
				if (materialFile.FindAll(mat => mat.Name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity. Please use the \"Add New Position Override\" tool to add multiple overrides for entities", "Error", MessageBoxButtons.OK);
					return;
				}
				var NewEntry = new MaterialContainer.Material(diag.SelectedEntity, "entity_alphatest");

				TreeNode NewEntryNode = new TreeNode(NewEntry.Name);
				NewEntryNode.Tag = NewEntry;
				foreach (JObject content in EntityJSONData["materials"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == NewEntry.Name);
					if (prop is JProperty)
					{
						NewEntryNode.Text = (string)prop.Value;
						NewEntryNode.ImageIndex = EntityJSONData["materials"].Children().ToList().IndexOf(content);
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
			if (treeView1.SelectedNode.Tag is MaterialContainer.Material entry)
			{
				entry.Type = materialComboBox.SelectedItem.ToString();
				treeView1.SelectedNode.Tag = entry;
			}
		}
	}
}
