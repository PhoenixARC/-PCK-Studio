using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Behaviour;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PckStudio.Forms.Editor
{
	public partial class BehaviourEditor : MetroForm
	{
		// Behaviours File Format research by Miku and MattNL
		private readonly PCKFile.FileData _file;
		BehaviourFile behaviourFile;

		void SetUpTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (var entry in behaviourFile.entries)
			{
				TreeNode EntryNode = new TreeNode(entry.name);

				foreach (JObject content in Utilities.BehaviourUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.name);
					if (prop is JProperty)
					{
						EntryNode.Text = (string)prop.Value;
						EntryNode.ImageIndex = Utilities.BehaviourUtil.entityData["entities"].Children().ToList().IndexOf(content);
						EntryNode.SelectedImageIndex = EntryNode.ImageIndex;
						break;
					}
				}

				EntryNode.Tag = entry;

				foreach (var posOverride in entry.overrides)
				{
					TreeNode OverrideNode = new TreeNode("Position Override");
					OverrideNode.Tag = posOverride;
					EntryNode.Nodes.Add(OverrideNode);
					OverrideNode.ImageIndex = 103;
					OverrideNode.SelectedImageIndex = OverrideNode.ImageIndex;
				}

				treeView1.Nodes.Add(EntryNode);
			}
			treeView1.EndUpdate();
		}

		public BehaviourEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			_file = file;

			using (var stream = new MemoryStream(file.Data))
			{
				behaviourFile = BehavioursReader.Read(stream);
			}

			treeView1.ImageList = new ImageList();
			Utilities.BehaviourUtil.entityImages.ToList().ForEach(img => treeView1.ImageList.Images.Add(img));
			treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			SetUpTree();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null) return;

			bool enable = e.Node.Tag is BehaviourFile.RiderPositionOverride.PositionOverride && treeView1.SelectedNode != null;
			flag1Checkbox.Enabled = enable;
			flag2Checkbox.Enabled = enable;
			xUpDown.Enabled = enable;
			yUpDown.Enabled = enable;
			zUpDown.Enabled = enable;

			if (e.Node.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				flag1Checkbox.Checked = posOverride._1;
				flag2Checkbox.Checked = posOverride._2;
				xUpDown.Value = (decimal)posOverride.x;
				yUpDown.Value = (decimal)posOverride.y;
				zUpDown.Value = (decimal)posOverride.z;

				renameToolStripMenuItem.Visible = false;
			}
			else renameToolStripMenuItem.Visible = true;
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return;

			treeView1.SelectedNode.Remove();
		}

		private void flag1Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride._1 = flag1Checkbox.Checked;
				treeView1.SelectedNode.Tag = posOverride;
			}
		}

		private void flag2Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride._2 = flag2Checkbox.Checked;
				treeView1.SelectedNode.Tag = posOverride;
			}
		}

		private void xUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.x = (float)xUpDown.Value;
				treeView1.SelectedNode.Tag = posOverride;
			}
		}

		private void yUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.y = (float)yUpDown.Value;
				treeView1.SelectedNode.Tag = posOverride;
			}
		}

		private void zUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.z = (float)zUpDown.Value;
				treeView1.SelectedNode.Tag = posOverride;
			}
		}

		private void changeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return;
			if (!(treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride entry)) return;

			var diag = new Additional_Popups.Behaviours.AddBehaviour();
			diag.acceptBtn.Text = "Save";

			if (diag.ShowDialog() == DialogResult.OK)
			{
				if (String.IsNullOrEmpty(diag.SelectedEntity)) return;
				if (behaviourFile.entries.FindAll(behaviour => behaviour.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity. Please use the \"Add New Position Override\" tool to add multiple overrides for entities", "Error", MessageBoxButtons.OK);
					return;
				}

				entry.name = diag.SelectedEntity;
				treeView1.SelectedNode.Tag = entry;

				foreach (JObject content in Utilities.BehaviourUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == entry.name);
					if (prop is JProperty)
					{
						treeView1.SelectedNode.Text = (string)prop.Value;
						treeView1.SelectedNode.ImageIndex = Utilities.BehaviourUtil.entityData["entities"].Children().ToList().IndexOf(content);
						treeView1.SelectedNode.SelectedImageIndex = treeView1.SelectedNode.ImageIndex;
						break;
					}
				}
			}
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			flag1Checkbox.Enabled = false;
			flag2Checkbox.Enabled = false;
			xUpDown.Enabled = false;
			yUpDown.Enabled = false;
			zUpDown.Enabled = false;
		}

		private void addNewPositionOverrideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride) treeView1.SelectedNode = treeView1.SelectedNode.Parent;

			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride entry)
			{
				TreeNode OverrideNode = new TreeNode("Position Override");
				OverrideNode.Tag = new BehaviourFile.RiderPositionOverride.PositionOverride();
				treeView1.SelectedNode.Nodes.Add(OverrideNode);
			}
		}

		private void addNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var diag = new Additional_Popups.Behaviours.AddBehaviour();

			if(diag.ShowDialog() == DialogResult.OK)
			{
				if (String.IsNullOrEmpty(diag.SelectedEntity)) return;
				if (behaviourFile.entries.FindAll(behaviour => behaviour.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity. Please use the \"Add New Position Override\" tool to add multiple overrides for entities", "Error", MessageBoxButtons.OK);
					return;
				}
				BehaviourFile.RiderPositionOverride NewOverride = new BehaviourFile.RiderPositionOverride(diag.SelectedEntity);

				TreeNode NewOverrideNode = new TreeNode(NewOverride.name);

				foreach (JObject content in Utilities.BehaviourUtil.entityData["entities"].Children())
				{
					var prop = content.Properties().FirstOrDefault(prop => prop.Name == NewOverride.name);
					if (prop is JProperty)
					{
						NewOverrideNode.Text = (string)prop.Value;
						NewOverrideNode.ImageIndex = Utilities.BehaviourUtil.entityData["entities"].Children().ToList().IndexOf(content);
						NewOverrideNode.SelectedImageIndex = NewOverrideNode.ImageIndex;
						break;
					}
				}
				treeView1.Nodes.Add(NewOverrideNode);
				treeView1.SelectedNode = NewOverrideNode;

				addNewPositionOverrideToolStripMenuItem_Click(sender, e); // adds a Position Override to the new Override
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
				behaviourFile = new BehaviourFile();

				foreach (TreeNode node in treeView1.Nodes)
				{
					if(node.Tag is BehaviourFile.RiderPositionOverride entry)
					{
						entry.overrides.Clear();
						Console.WriteLine();
						foreach (TreeNode overrideNode in node.Nodes)
						{
							if(overrideNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride overrideEntry)
							{
								entry.overrides.Add(overrideEntry);
							}
						}

						behaviourFile.entries.Add(entry);
					}
				}

				BehavioursWriter.Write(stream, behaviourFile);
				_file.SetData(stream.ToArray());
			}
			DialogResult = DialogResult.OK;
		}
	}
}
