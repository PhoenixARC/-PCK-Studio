using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using PckStudio.Internal;
using PckStudio.Forms.Additional_Popups.Loc;
using OMI.Formats.Languages;
using PckStudio.Interfaces;

namespace PckStudio.Forms.Editor
{
	public partial class LOCEditor : EditorForm<LOCFile>
    {
        public LOCEditor(LOCFile locFile, ISaveContext<LOCFile> context)
            : base(locFile, context)
		{
			InitializeComponent();
			saveToolStripMenuItem.Visible = !context.AutoSave;
        }

		private void LOCEditor_Load(object sender, EventArgs e)
		{
			RPC.SetPresence("LOC Editor", "Editing localization File.");
			foreach(string locKey in EditorValue.LocKeys.Keys)
				treeViewLocKeys.Nodes.Add(locKey);
		}

		private void treeViewLocKeys_AfterSelect(object sender, TreeViewEventArgs e)
		{
            TreeNode node = e.Node;
			if (node == null ||
				!EditorValue.LocKeys.ContainsKey(node.Text))
			{
				MessageBox.Show(this, "Selected Node does not seem to be in the loc file");
				return;
			}
			ReloadTranslationTable();
		}

		private void addDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (TextPrompt prompt = new TextPrompt())
			{
				prompt.OKButtonText = "Add";
				if (prompt.ShowDialog(this) == DialogResult.OK && 
					EditorValue.AddLocKey(prompt.NewText, ""))
				{
					treeViewLocKeys.Nodes.Add(prompt.NewText);
				}
			}
        }

        private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewLocKeys.SelectedNode is TreeNode t && EditorValue.RemoveLocKey(t.Text))
			{
				treeViewLocKeys.SelectedNode.Remove();
				ReloadTranslationTable();
            }
		}

		private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
            if (e.ColumnIndex != 1 ||
				treeViewLocKeys.SelectedNode is null)
            {
				MessageBox.Show(this, "something went wrong");
				return;
            }
            DataGridViewRow row = dataGridViewLocEntryData.Rows[e.RowIndex];
			string locKey = treeViewLocKeys.SelectedNode.Text;
            string language = row.Cells[0].Value.ToString();
			string value    = row.Cells[1].Value.ToString();
            EditorValue.SetLocEntry(locKey, language, value);
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
				deleteDisplayIDToolStripMenuItem_Click(sender, e);
		}

		private void buttonReplaceAll_Click(object sender, EventArgs e)
		{
            for (int i = 0; i < dataGridViewLocEntryData.Rows.Count; i++)
            {
                dataGridViewLocEntryData.Rows[i].Cells[1].Value = textBoxReplaceAll.Text;
            }

			EditorValue.SetLocEntry(treeViewLocKeys.SelectedNode.Text, textBoxReplaceAll.Text);
		}

		private void ReloadTranslationTable()
        {
			dataGridViewLocEntryData.Rows.Clear();
			if (treeViewLocKeys.SelectedNode is null)
				return;
			foreach (KeyValuePair<string, string> locEntry in EditorValue.GetLocEntries(treeViewLocKeys.SelectedNode.Text))
                dataGridViewLocEntryData.Rows.Add(locEntry.Key, locEntry.Value);
		}

		private IEnumerable<string> GetAvailableLanguages()
        {
			foreach (var lang in LOCFile.ValidLanguages)
			{
				if (EditorValue.Languages.Contains(lang))
					continue;
				yield return lang;
			}
			yield break;
		}

		private void addLanguageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string[] avalibleLang = GetAvailableLanguages().ToArray();
			using (var dialog = new AddLanguage(avalibleLang))
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					EditorValue.AddLanguage(dialog.SelectedLanguage);
					ReloadTranslationTable();
				}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Save();
			DialogResult = DialogResult.OK;
        }
    }
}
