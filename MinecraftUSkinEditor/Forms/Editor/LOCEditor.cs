using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using RichPresenceClient;

namespace PckStudio.Forms.Editor
{
	public partial class LOCEditor : MetroForm
    {
		DataTable tbl;
		LOCFile currentLoc;
		bool wasModified = false;
		public bool WasModified => wasModified;

		public LOCEditor(LOCFile loc)
		{
			InitializeComponent();
			currentLoc = loc;
			tbl = new DataTable();
			tbl.Columns.Add(new DataColumn("Language") { ReadOnly = true });
			tbl.Columns.Add("Display Name");
			dataGridViewLocEntryData.DataSource = tbl;
            DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
            column.Width = dataGridViewLocEntryData.Width;
        }

		private void LOCEditor_Load(object sender, EventArgs e)
		{
			RPC.SetPresence("LOC Editor", "Editing loc File.");
			foreach(string locKey in currentLoc.LocKeys.Keys)
				treeViewLocKeys.Nodes.Add(locKey);
		}

        private void LOCEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
			RPC.SetPresence("Sitting alone", "Program by PhoenixARC");
        }

		private void treeViewLocKeys_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var node = e.Node;
			if (node == null ||
				!currentLoc.LocKeys.ContainsKey(node.Text))
			{
				MessageBox.Show("Selected Node does not seem to be in the loc file");
				return;
			}
			tbl.Rows.Clear();
			buttonReplaceAll.Enabled = true;
			foreach (var l in currentLoc.GetLocEntries(node.Text))
				tbl.Rows.Add(l.Key, l.Value);
		}

		private void renameDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// broken...
			TreeNode node = treeViewLocKeys.SelectedNode;
			using (RenamePrompt diag = new RenamePrompt(node.Text))
			{
				if (diag.ShowDialog() == DialogResult.OK)
				{
					currentLoc.SetLocEntry(node.Text, diag.NewText);
					wasModified = true;
				}
			}
		}

		private void addDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewLocKeys.SelectedNode != null &&
				!currentLoc.LocKeys.ContainsKey(treeViewLocKeys.SelectedNode.Text))
				using (RenamePrompt prompt = new RenamePrompt(""))
				{
					prompt.OKButton.Text = "Add";
					if (prompt.ShowDialog() == DialogResult.OK)
					{
						currentLoc.AddLocKey(prompt.NewText, "");
						wasModified = true;
					}
				}
        }

        private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(treeViewLocKeys.SelectedNode != null &&
				currentLoc.LocKeys.ContainsKey(treeViewLocKeys.SelectedNode.Text) &&
				currentLoc.LocKeys.Remove(treeViewLocKeys.SelectedNode.Text))
			{
				treeViewLocKeys.SelectedNode.Remove();
				wasModified = true;
            }
		}

		private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 1 ||
				treeViewLocKeys.SelectedNode == null)
            {
				MessageBox.Show("something went wrong");
				return;
            }
			currentLoc.SetLocEntry(treeViewLocKeys.SelectedNode.Text, tbl.Rows[e.RowIndex][0].ToString(), tbl.Rows[e.RowIndex][1].ToString());
			wasModified = true;
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
				deleteDisplayIDToolStripMenuItem_Click(sender, e);
		}

		private void buttonReplaceAll_Click(object sender, EventArgs e)
		{
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                tbl.Rows[i][1] = textBoxReplaceAll.Text;
            }

			currentLoc.SetLocEntry(treeViewLocKeys.SelectedNode.Text, textBoxReplaceAll.Text);
			wasModified = true;
		}

        private void LOCEditor_Resize(object sender, EventArgs e)
        {
			DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
			column.Width = dataGridViewLocEntryData.Width - dataGridViewLocEntryData.Columns[0].Width - 1;
		}

        private void addLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
		}
    }
}
