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

namespace PckStudio
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
			foreach(string locKey in currentLoc.keys.Keys)
				treeViewLocEntries.Nodes.Add(locKey);
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var node = e.Node;
			if (node == null ||
				!currentLoc.keys.ContainsKey(node.Text))
			{
				MessageBox.Show("Selected Node does not seem to be in the loc file");
				return;
			}
			tbl.Rows.Clear();
			buttonReplaceAll.Enabled = true;
			foreach (var l in currentLoc.keys[node.Text])
			{
				tbl.Rows.Add(l.Key, l.Value);
			}
		}

		private void renameDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode node = treeViewLocEntries.SelectedNode;
			RenamePrompt diag = new RenamePrompt(node.Text);
			diag.ShowDialog(this);
			if (diag.DialogResult == DialogResult.OK)
			{
				currentLoc.ChangeEntry(node.Text, diag.NewText);
				wasModified = true;
			}
			diag.Dispose();
		}

		private void addDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
            //foreach (LOCFile.Language l in currentLoc.langs)
            //    l.names.Insert(index, "NewString");

            //treeViewLocEntries.Nodes.Insert(index, "NewItem");
        }

        private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(treeViewLocEntries.SelectedNode != null && currentLoc.keys.ContainsKey(treeViewLocEntries.SelectedNode.Text))
			{
				if (currentLoc.keys.Remove(treeViewLocEntries.SelectedNode.Text))
				{
					treeViewLocEntries.SelectedNode.Remove();
					wasModified = true;
				}
            }
		}

		private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 1)
            {
				MessageBox.Show("something went wrong");
				return;
            }
			currentLoc.ChangeSingleEntry(treeViewLocEntries.SelectedNode.Text, tbl.Rows[e.RowIndex][0].ToString(), tbl.Rows[e.RowIndex][1].ToString());
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
			currentLoc.ChangeEntry(treeViewLocEntries.SelectedNode.Text, textBoxReplaceAll.Text);
			wasModified = true;
		}

        private void LOCEditor_Resize(object sender, EventArgs e)
        {
			DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
			column.Width = dataGridViewLocEntryData.Width - dataGridViewLocEntryData.Columns[0].Width;
		}
    }
}
