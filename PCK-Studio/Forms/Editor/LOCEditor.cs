﻿using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Language;
using PckStudio.Extensions;
using PckStudio.Forms.Additional_Popups.Loc;
using PckStudio.Internal.Misc;
using PckStudio.Properties;
using PckStudio.ToolboxItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio.Forms.Editor
{
    public partial class LOCEditor : ThemeForm
    {
        LOCFile _currentLoc;
        PckAsset _asset;
        private bool _isModified = false; // Track changes to the LOC strings

        public LOCEditor(PckAsset asset)
        {
            InitializeComponent();
            _asset = asset;
            _currentLoc = asset.GetData(new LOCFileReader());
            saveToolStripMenuItem.Visible = !Settings.Default.AutoSaveChanges;
        }

        private void LOCEditor_Load(object sender, EventArgs e)
        {
            RPC.SetPresence("LOC Editor", "Editing localization File.");
            foreach (string locKey in _currentLoc.LocKeys.Keys)
                treeViewLocKeys.Nodes.Add(locKey);
        }

        private void treeViewLocKeys_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node == null || !_currentLoc.LocKeys.ContainsKey(node.Text))
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
                    _currentLoc.AddLocKey(prompt.NewText, ""))
                {
                    treeViewLocKeys.Nodes.Add(prompt.NewText);
                    _isModified = true;
                }
            }
        }

        private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewLocKeys.SelectedNode is TreeNode t && _currentLoc.RemoveLocKey(t.Text))
            {
                treeViewLocKeys.SelectedNode.Remove();
                ReloadTranslationTable();
                _isModified = true;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1 || treeViewLocKeys.SelectedNode is null)
            {
                MessageBox.Show(this, "something went wrong");
                return;
            }

            DataGridViewRow row = dataGridViewLocEntryData.Rows[e.RowIndex];
            string locKey = treeViewLocKeys.SelectedNode.Text;
            string language = row.Cells[0].Value.ToString();
            string value = row.Cells[1].Value.ToString();
            _currentLoc.SetLocEntry(locKey, language, value);
            _isModified = true;
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
            _currentLoc.SetLocEntry(treeViewLocKeys.SelectedNode.Text, textBoxReplaceAll.Text);
            _isModified = true;
        }

        private void ReloadTranslationTable()
        {
            dataGridViewLocEntryData.Rows.Clear();
            if (treeViewLocKeys.SelectedNode is null)
                return;
            foreach (KeyValuePair<string, string> locEntry in _currentLoc.GetLocEntries(treeViewLocKeys.SelectedNode.Text))
                dataGridViewLocEntryData.Rows.Add(locEntry.Key, locEntry.Value);
        }

        private IEnumerable<string> GetAvailableLanguages()
        {
            foreach (var lang in LOCFile.ValidLanguages)
            {
                if (_currentLoc.Languages.Contains(lang))
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
                    _currentLoc.AddLanguage(dialog.SelectedLanguage);
                    ReloadTranslationTable();
                    _isModified = true;
                }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _asset.SetData(new LOCFileWriter(_currentLoc, 2));
            _isModified = false; // Reset modified flag after saving
            DialogResult = DialogResult.OK;
        }

        private void LOCEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.Default.AutoSaveChanges)
            {
                saveToolStripMenuItem_Click(sender, EventArgs.Empty);
            }
            else if (_isModified) // Use local modified flag
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Close without saving?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}