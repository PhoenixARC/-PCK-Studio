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

namespace PckStudio
{
	public partial class LOCEditor : MetroFramework.Forms.MetroForm
	{
		DataTable tbl = new DataTable();
		LOCFile currentLoc;

		public LOCEditor(LOCFile loc)
		{
			InitializeComponent();
			currentLoc = loc;
			tbl.Columns.Add(new DataColumn("Language") { ReadOnly = true });
			tbl.Columns.Add("Display Name");
			dataGridViewLocEntryData.DataSource = tbl;
			DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
			column.Width = 600;
		}

		private void LOCEditor_Load(object sender, EventArgs e)
		{
			foreach(string id in currentLoc.languages.Keys)
				treeViewLocEntries.Nodes.Add(id);
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			tbl.Rows.Clear();
			if (treeViewLocEntries.SelectedNode == null ||
				!currentLoc.languages.ContainsKey(treeViewLocEntries.SelectedNode.Text))
			{
				MessageBox.Show("Selected Node does not seem to be in the loc file");
				return;
			}
			buttonReplaceAll.Enabled = true;
			foreach (var l in currentLoc.languages[treeViewLocEntries.SelectedNode.Text])
			{
				tbl.Rows.Add(l.Key, l.Value);
			}
		}

		private void renameDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode node = treeViewLocEntries.SelectedNode;
			renameLoc diag = new renameLoc(node.Text);
			diag.ShowDialog(this);
			if (diag.DialogResult == DialogResult.OK)
                currentLoc.ChangeEntry("TODO", diag.NewText);
			diag.Dispose(); //diposes generated metadata adding dialog data
		}

		private void addDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("TODO");
			//int index = treeViewLocEntries.SelectedNode.Index;

			//if(index == -1) index = currentLoc.ids.names.Count;

			//currentLoc.ids.names.Insert(index, "NewItem");

			//foreach (LOCFile.Language l in currentLoc.langs)
			//	l.names.Insert(index, "NewString");

			//treeViewLocEntries.Nodes.Insert(index, "NewItem");
		}

		private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(treeViewLocEntries.SelectedNode != null)
			{
				MessageBox.Show("TODO");
				//int index = treeViewLocEntries.SelectedNode.Index;

				//currentLoc.ids.names.RemoveAt(index);

				//foreach (LOCFile.Language l in currentLoc.langs)
				//	l.names.RemoveAt(index);

				//treeViewLocEntries.Nodes.RemoveAt(index);
			}
		}

		private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			MessageBox.Show("TODO");
			//for (int i = 0; i < tbl.Rows.Count; i++)
			//{
			//	byte[] data = Encoding.UTF8.GetBytes((string)tbl.Rows[i][1]);

			//	string final = string.Empty;

			//	foreach (byte b in data)
			//		final += (char)b;

			//	currentLoc.langs[i].names[treeViewLocEntries.SelectedNode.Index] = final;
			//}
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete && treeViewLocEntries.SelectedNode != null) //checks to make sure pressed key was del
			{
				int index = treeViewLocEntries.SelectedNode.Index;
				MessageBox.Show("TODO");
				//currentLoc.languages..RemoveAt(index);

				//foreach (var l in currentLoc.languages)
				//	l.names.RemoveAt(index);
				//treeViewLocEntries.Nodes.RemoveAt(index);
			}
		}

		private void buttonReplaceAll_Click(object sender, EventArgs e)
		{
			MessageBox.Show("TODO");
			//for (int i = 0; i < tbl.Rows.Count; i++)
			//{
			//	tbl.Rows[i][1] = ;
			//}
			//for (int i = 0; i < tbl.Rows.Count; i++)
			//{
			//	currentLoc.langs[i].names[treeViewLocEntries.SelectedNode.Index] = (string)tbl.Rows[i][1];
			//}
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{

		}
	}
}
