using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Model;
using PckStudio.Forms.Additional_Popups.EntityForms;
using Newtonsoft.Json.Linq;

namespace PckStudio.Forms.Editor
{
	public partial class ModelsEditor : MetroForm
	{
		// Behaviours File Format research by Miku and MattNL
		private readonly PCKFile.FileData _file;
		ModelFile modelFile;

		void SetUpTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (var entry in modelFile.Models)
			{
				TreeNode EntryNode = new TreeNode(entry.name);

				foreach (JObject content in Utilities.ModelsUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.name);
					if (prop is JProperty)
					{
						EntryNode.Text = (string)prop.Value;
						EntryNode.ImageIndex = Utilities.ModelsUtil.entityData["entities"].Children().ToList().IndexOf(content);
						EntryNode.SelectedImageIndex = EntryNode.ImageIndex;
						break;
					}
				}

				EntryNode.Tag = entry;

				foreach (var bone in entry.parts)
				{
					TreeNode BoneNode = new TreeNode(bone.name);
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

		public ModelsEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using (var stream = new MemoryStream(file.Data))
			{
				modelFile = ModelFileReader.Read(stream);
			}

			treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.AddRange(Utilities.ModelsUtil.entityImages);
			treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			SetUpTree();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node is null) return;

			bool isValidOverride = e.Node.Tag is BehaviourFile.RiderPositionOverride.PositionOverride &&
				treeView1.SelectedNode != null;
			renameToolStripMenuItem.Visible = !isValidOverride;


			if (isValidOverride)
			{
                var posOverride = e.Node.Tag as BehaviourFile.RiderPositionOverride.PositionOverride;
			}
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is null) return;
			treeView1.SelectedNode.Remove();
		}

		private void changeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return;
			if (!(treeView1.SelectedNode.Tag is ModelFile.Model entry)) return;

			var diag = new AddEntry(Utilities.ModelsUtil.entityData, Utilities.ModelsUtil.entityImages);
			diag.acceptBtn.Text = "Save";

			if (diag.ShowDialog() == DialogResult.OK)
			{
				if (String.IsNullOrEmpty(diag.SelectedEntity)) return;
				if (modelFile.Models.FindAll(model => model.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two models for one entity", "Error", MessageBoxButtons.OK);
					return;
				}

				entry.name = diag.SelectedEntity;
				treeView1.SelectedNode.Tag = entry;

				foreach (JObject content in Utilities.ModelsUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.name);
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
				if (string.IsNullOrEmpty(diag.SelectedEntity)) return;
				if (modelFile.Models.FindAll(model => model.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two models for one entity.", "Error", MessageBoxButtons.OK);
					return;
				}
				ModelFile.Model NewModel = new ModelFile.Model(diag.SelectedEntity, 64, 64);

				TreeNode NewModelNode = new TreeNode(NewModel.name);
				NewModelNode.Tag = NewModel;
				foreach (JObject content in Utilities.ModelsUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == NewModel.name);
					if (prop is JProperty)
					{
						NewModelNode.Text = (string)prop.Value;
						NewModelNode.ImageIndex = Utilities.ModelsUtil.entityData["entities"].Children().ToList().IndexOf(content);
						NewModelNode.SelectedImageIndex = NewModelNode.ImageIndex;
						break;
					}
				}
				treeView1.Nodes.Add(NewModelNode);
				treeView1.SelectedNode = NewModelNode;

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
				modelFile = new ModelFile();

				foreach (TreeNode node in treeView1.Nodes)
				{
					if(node.Tag is ModelFile.Model entry)
					{
						entry.parts.Clear();
						foreach (TreeNode boneNode in node.Nodes)
						{
							if(boneNode.Tag is ModelFile.Model.Part boneEntry)
							{
								entry.parts.Add(boneEntry);
							}
						}

						modelFile.Models.Add(entry);
					}
				}

				ModelFileWriter.Write(stream, modelFile);
				_file.SetData(stream.ToArray());
			}
			DialogResult = DialogResult.OK;
		}
	}
}
