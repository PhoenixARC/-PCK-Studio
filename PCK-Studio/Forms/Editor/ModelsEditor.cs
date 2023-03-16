using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Forms.Additional_Popups.EntityForms;
using Newtonsoft.Json.Linq;
using OMI.Formats.Model;
using OMI.Formats.Pck;
using OMI.Workers.Model;
using OMI.Formats.Behaviour;

namespace PckStudio.Forms.Editor
{
	public partial class ModelsEditor : MetroForm
	{
		// Behaviours File Format research by Miku and MattNL
		private readonly PckFile.FileData _file;
		ModelContainer modelFile;

		void SetUpTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (var entry in modelFile.Models.Values)
			{
				TreeNode EntryNode = new TreeNode(entry.Name);

				foreach (JObject content in Utilities.ModelsUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.Name);
					if (prop is JProperty)
					{
						EntryNode.Text = (string)prop.Value;
						EntryNode.ImageIndex = Utilities.ModelsUtil.entityData["entities"].Children().ToList().IndexOf(content);
						EntryNode.SelectedImageIndex = EntryNode.ImageIndex;
						break;
					}
				}

				EntryNode.Tag = entry;

				foreach (var bone in entry.Parts.Values)
				{
					TreeNode BoneNode = new TreeNode(bone.Name);
					BoneNode.Tag = bone;
					EntryNode.Nodes.Add(BoneNode);
					BoneNode.ImageIndex = 120;
					BoneNode.SelectedImageIndex = BoneNode.ImageIndex;

					foreach (var box in bone.Boxes)
					{
						TreeNode BoxNode = new TreeNode("Box");
						BoxNode.Tag = box;
						BoneNode.Nodes.Add(BoxNode);
						BoxNode.ImageIndex = 121;
						BoxNode.SelectedImageIndex = BoxNode.ImageIndex;
					}
				}

				treeView1.Nodes.Add(EntryNode);
			}
			treeView1.EndUpdate();
		}

		public ModelsEditor(PckFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using (var stream = new MemoryStream(file.Data))
			{
				var reader = new ModelFileReader();
				modelFile = reader.FromStream(stream);
			}

			treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.AddRange(Utilities.ModelsUtil.entityImages);
			treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			SetUpTree();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is null) return;
			treeView1.SelectedNode.Remove();
		}

		private void changeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is null || treeView1.SelectedNode.Tag is not Model entry)
				return;

			var diag = new AddEntry(Utilities.ModelsUtil.entityData, Utilities.ModelsUtil.entityImages);
			diag.acceptBtn.Text = "Save";

			if (diag.ShowDialog() == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(diag.SelectedEntity))
					return;


				if (modelFile.Models.ContainsKey(diag.SelectedEntity))
				{
					modelFile.Models.Remove(diag.SelectedEntity);
                }

				entry.Name = diag.SelectedEntity;
				treeView1.SelectedNode.Tag = entry;

				foreach (JObject content in Utilities.ModelsUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.Name);
					if (prop is JProperty)
					{
						treeView1.SelectedNode.Text = (string)prop.Value;
						treeView1.SelectedNode.ImageIndex = Utilities.ModelsUtil.entityData["entities"].Children().ToList().IndexOf(content);
						treeView1.SelectedNode.SelectedImageIndex = treeView1.SelectedNode.ImageIndex;
						break;
					}
				}
			}
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{

		}

		private void addNewPositionOverrideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// change into part
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride) treeView1.SelectedNode = treeView1.SelectedNode.Parent;

			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride)
			{
				TreeNode OverrideNode = new TreeNode("Position Override");
				OverrideNode.Tag = new BehaviourFile.RiderPositionOverride.PositionOverride();
				OverrideNode.ImageIndex = 103;
				OverrideNode.SelectedImageIndex = 103;
				treeView1.SelectedNode.Nodes.Add(OverrideNode);
			}
		}

		private void addNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var diag = new AddEntry(Utilities.ModelsUtil.entityData, Utilities.ModelsUtil.entityImages);

			if(diag.ShowDialog() == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(diag.SelectedEntity))
					return;
				if (modelFile.Models.ContainsKey(diag.SelectedEntity))
				{
					MessageBox.Show(this, "You cannot have two models for one entity.", "Error", MessageBoxButtons.OK);
					return;
				}

				Model newModel = new Model()
				{
					Name = diag.SelectedEntity,
					TextureSize = new Size(64, 64)
				};

				TreeNode modelNode = new TreeNode(newModel.Name);
				modelNode.Tag = newModel;
				foreach (JObject content in Utilities.ModelsUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == newModel.Name);
					if (prop is JProperty)
					{
						modelNode.Text = (string)prop.Value;
						modelNode.ImageIndex = Utilities.ModelsUtil.entityData["entities"].Children().ToList().IndexOf(content);
						modelNode.SelectedImageIndex = modelNode.ImageIndex;
						break;
					}
				}
				treeView1.Nodes.Add(modelNode);
				treeView1.SelectedNode = modelNode;

				//addNewPositionOverrideToolStripMenuItem_Click(sender, e); // adds a Position Override to the new Override
			}
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete) removeToolStripMenuItem_Click(sender, e);
		}

		private void treeView1_MouseHover(object sender, EventArgs e)
		{
			addNewPositionOverrideToolStripMenuItem.Visible = treeView1.SelectedNode != null;
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (var stream = new MemoryStream())
			{
				var writer = new ModelFileWriter(modelFile);
                writer.WriteToStream(stream);
				_file.SetData(stream.ToArray());
			}
			DialogResult = DialogResult.OK;
		}
	}
}
