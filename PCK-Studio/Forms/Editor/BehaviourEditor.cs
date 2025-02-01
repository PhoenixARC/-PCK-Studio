using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Forms.Additional_Popups.EntityForms;
using Newtonsoft.Json.Linq;
using OMI.Formats.Behaviour;
using OMI.Workers.Behaviour;
using OMI.Formats.Pck;
using PckStudio.Properties;
using PckStudio.Internal;
using PckStudio.Extensions;
using PckStudio.Internal.Json;
using PckStudio.Internal.App;
using PckStudio.Interfaces;

namespace PckStudio.Forms.Editor
{
	// Behaviours File Format research by Miku and MattNL
	public partial class BehaviourEditor : Editor<BehaviourFile>
	{

		private readonly List<EntityInfo> BehaviourData = Entities.BehaviourInfos;

        public BehaviourEditor(BehaviourFile behaviourFile, ISaveContext<BehaviourFile> saveContext)
			: base(behaviourFile, saveContext)
        {
            InitializeComponent();

            saveToolStripMenuItem1.Visible = !saveContext.AutoSave;

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.AddRange(ApplicationScope.EntityImages);
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            SetUpTree();
        }

        void SetUpTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (BehaviourFile.RiderPositionOverride entry in EditorValue.entries)
			{
				TreeNode EntryNode = new TreeNode(entry.name);

                EntityInfo behaviour = BehaviourData.Find(b => b.InternalName == entry.name);
				EntryNode.Text = behaviour.DisplayName;
				EntryNode.ImageIndex = BehaviourData.IndexOf(behaviour);
				EntryNode.SelectedImageIndex = EntryNode.ImageIndex;
				EntryNode.Tag = entry;

				foreach (BehaviourFile.RiderPositionOverride.PositionOverride posOverride in entry.overrides)
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

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView1.SelectedNode is null)
				return;

			bool isValidOverride = treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride;
			MobIsTamedCheckbox.Enabled = isValidOverride;
			MobHasSaddleCheckbox.Enabled = isValidOverride;
			xUpDown.Enabled = isValidOverride;
			yUpDown.Enabled = isValidOverride;
			zUpDown.Enabled = isValidOverride;
			renameToolStripMenuItem.Visible = !isValidOverride;


			if (isValidOverride)
			{
                var posOverride = treeView1.SelectedNode.Tag as BehaviourFile.RiderPositionOverride.PositionOverride;
                MobIsTamedCheckbox.Checked = posOverride.EntityIsTamed;
				MobHasSaddleCheckbox.Checked = posOverride.EntityHasSaddle;
				xUpDown.Value = (decimal)posOverride.x;
				yUpDown.Value = (decimal)posOverride.y;
				zUpDown.Value = (decimal)posOverride.z;
			}
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is null)
				return;
			treeView1.SelectedNode.Remove();
		}

		private void MobIsTamedCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.EntityIsTamed = MobIsTamedCheckbox.Checked;
			}
		}

		private void MobHasSaddleCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.EntityHasSaddle = MobHasSaddleCheckbox.Checked;
			}
		}

		private void xUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.x = (float)xUpDown.Value;
			}
		}

		private void yUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.y = (float)yUpDown.Value;
			}
		}

		private void zUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride posOverride)
			{
				posOverride.z = (float)zUpDown.Value;
			}
		}

		private void changeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null)
				return;
			if (!(treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride entry))
				return;

			var diag = new AddEntry("behaviours", ApplicationScope.EntityImages);

			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(diag.SelectedEntity))
					return;
				if (EditorValue.entries.FindAll(behaviour => behaviour.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity. Please use the \"Add New Position Override\" tool to add multiple overrides for entities", "Error", MessageBoxButtons.OK);
					return;
				}

				entry.name = diag.SelectedEntity;
				treeView1.SelectedNode.Tag = entry;

                EntityInfo behaviour = BehaviourData.Find(b => b.InternalName == entry.name);

				treeView1.SelectedNode.Text = behaviour.DisplayName;
				treeView1.SelectedNode.ImageIndex = BehaviourData.IndexOf(behaviour);
				treeView1.SelectedNode.SelectedImageIndex = treeView1.SelectedNode.ImageIndex;
			}
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			MobIsTamedCheckbox.Enabled = false;
			MobHasSaddleCheckbox.Enabled = false;
			xUpDown.Enabled = false;
			yUpDown.Enabled = false;
			zUpDown.Enabled = false;
		}

		private void addNewPositionOverrideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is BehaviourFile.RiderPositionOverride.PositionOverride)
				treeView1.SelectedNode = treeView1.SelectedNode.Parent;

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
			var diag = new AddEntry("behaviours", ApplicationScope.EntityImages);

			if(diag.ShowDialog(this) == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(diag.SelectedEntity))
					return;
				if (EditorValue.entries.FindAll(behaviour => behaviour.name == diag.SelectedEntity).Count() > 0)
				{
					MessageBox.Show(this, "You cannot have two entries for one entity. Please use the \"Add New Position Override\" tool to add multiple overrides for entities", "Error", MessageBoxButtons.OK);
					return;
				}
				BehaviourFile.RiderPositionOverride NewOverride = new BehaviourFile.RiderPositionOverride(diag.SelectedEntity);

				TreeNode NewOverrideNode = new TreeNode(NewOverride.name);
				NewOverrideNode.Tag = NewOverride;

                EntityInfo behaviour = BehaviourData.Find(b => b.InternalName == NewOverride.name);
				NewOverrideNode.Text = behaviour.DisplayName;
				NewOverrideNode.ImageIndex = BehaviourData.IndexOf(behaviour);
				NewOverrideNode.SelectedImageIndex = NewOverrideNode.ImageIndex;

				treeView1.Nodes.Add(NewOverrideNode);
				treeView1.SelectedNode = NewOverrideNode;

				addNewPositionOverrideToolStripMenuItem_Click(sender, e); // adds a Position Override to the new Override
			}
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				removeToolStripMenuItem_Click(sender, e);
		}

		private void treeView1_MouseHover(object sender, EventArgs e)
		{
			addNewPositionOverrideToolStripMenuItem.Visible = treeView1.SelectedNode != null;
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
            EditorValue = new BehaviourFile();
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

                    EditorValue.entries.Add(entry);
				}
			}

			Save();
			DialogResult = DialogResult.OK;
		}

        private void BehaviourEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (Settings.Default.AutoSaveChanges)
			{
				saveToolStripMenuItem1_Click(sender, EventArgs.Empty);
			}
        }
    }
}
