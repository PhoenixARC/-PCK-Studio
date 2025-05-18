/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using PckStudio.Forms.Additional_Popups.Grf;
using PckStudio.Internal.Misc;
using OMI.Formats.GameRule;
using PckStudio.Properties;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Editor
{
    public partial class GameRuleFileEditor : MetroFramework.Forms.MetroForm
    {
        private GameRuleFile _file;

        public GameRuleFile Result => _file;

        private GameRuleFileEditor()
        {
            InitializeComponent();
            saveToolStripMenuItem.Visible = !Settings.Default.AutoSaveChanges;
        }

        public GameRuleFileEditor(GameRuleFile gameRuleFile) : this()
        {
            _file = gameRuleFile;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            RPC.SetPresence("GRF Editor", "Editing a GRF File");
            ReloadGameRuleTree();
        }

        private void LoadGameRuleTree(TreeNodeCollection root, GameRuleFile.GameRule parentRule)
        {
            foreach (GameRuleFile.GameRule rule in parentRule.ChildRules)
            {
                TreeNode node = new TreeNode(rule.Name);
                node.Tag = rule;
                root.Add(node);
                LoadGameRuleTree(node.Nodes, rule);
            }
        }

        private void ReloadGameRuleTree()
        {
            GrfTreeView.Nodes.Clear();
            if (_file is not null)
            {
                SetCompressionLevel();
                LoadGameRuleTree(GrfTreeView.Nodes, _file.Root);
            }
        }

        private void SetCompressionLevel()
        {
            switch (_file.Header.CompressionLevel)
            {
                case GameRuleFile.CompressionLevel.None:
                    noneToolStripMenuItem.Checked = true;
                    break;
                case GameRuleFile.CompressionLevel.Compressed:
                    compressedToolStripMenuItem.Checked = true;
                    break;
                case GameRuleFile.CompressionLevel.CompressedRle:
                    compressedRLEToolStripMenuItem.Checked = true;
                    break;
                case GameRuleFile.CompressionLevel.CompressedRleCrc:
                    compressedRLECRCToolStripMenuItem.Checked = true;
                    break;
            }
        }

        private void GrfTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TreeNode t && t.Tag is GameRuleFile.GameRule)
                ReloadParameterTreeView();
        }

        private void ReloadParameterTreeView()
        {
            GrfParametersTreeView.Nodes.Clear();
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule rule)
            foreach (KeyValuePair<string, string> param in rule.Parameters)
            {
                GrfParametersTreeView.Nodes.Add(new TreeNode($"{param.Key}: {param.Value}") { Tag = param});
            }
        }

        private void addDetailContextMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode == null || !(GrfTreeView.SelectedNode.Tag is GameRuleFile.GameRule))
                return;
            var grfTag = GrfTreeView.SelectedNode.Tag as GameRuleFile.GameRule;
            AddParameter prompt = new AddParameter();
            if (prompt.ShowDialog(this) == DialogResult.OK)
            {
                if (grfTag.Parameters.ContainsKey(prompt.ParameterName))
                {
                    MessageBox.Show(this, "Can't add detail that already exists.", "Error");
                    return;
                }
                grfTag.Parameters.Add(prompt.ParameterName, prompt.ParameterValue);
                ReloadParameterTreeView();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule rule &&
                GrfParametersTreeView.SelectedNode is TreeNode paramNode && paramNode.Tag is KeyValuePair<string, string> pair &&
                rule.Parameters.ContainsKey(pair.Key) && rule.Parameters.Remove(pair.Key))
            {
                ReloadParameterTreeView(); 
                return;
            }
            MessageBox.Show(this, "No Rule selected");
        }

        private void GrfDetailsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeToolStripMenuItem_Click(sender, e);
        }

        private void GrfDetailsTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule rule &&
                GrfParametersTreeView.SelectedNode is TreeNode paramNode && paramNode.Tag is KeyValuePair<string, string> param)
            {
                AddParameter prompt = new AddParameter(param.Key, param.Value, false);
                if (prompt.ShowDialog(this) == DialogResult.OK)
                {
                    rule.Parameters[prompt.ParameterName] = prompt.ParameterValue;
                    ReloadParameterTreeView();
                }
            }
        }

        private void addGameRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isValidNode = GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule;
            GameRuleFile.GameRule parentRule = isValidNode
               ? GrfTreeView.SelectedNode.Tag as GameRuleFile.GameRule
               : _file.Root;

            TreeNodeCollection root = isValidNode
                ? GrfTreeView.SelectedNode.Nodes
                : GrfTreeView.Nodes;

            using (TextPrompt prompt = new TextPrompt())
            {
                prompt.OKButtonText = "Add";
                if (MessageBox.Show(this, $"Add Game Rule to {parentRule.Name}", "Attention",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes &&
                    prompt.ShowDialog(this) == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(prompt.NewText))
                {
                    GameRuleFile.GameRule rule = parentRule.AddRule(prompt.NewText);
                    TreeNode node = new TreeNode(rule.Name);
                    node.Tag = rule;
                    root.Add(node);
                }
            }
        }

        private void removeGameRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule tag && RemoveTag(tag))
                t.Remove();
        }

        private bool RemoveTag(GameRuleFile.GameRule rule)
        {
            _ = rule.Parent ?? throw new ArgumentNullException(nameof(rule.Parent));
            foreach (GameRuleFile.GameRule subTag in rule.ChildRules.ToList())
                return RemoveTag(subTag);
            return rule.Parent.ChildRules.Remove(rule);
        }

        private void GrfTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeGameRuleToolStripMenuItem_Click(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_file.Header.unknownData[3] != 0)
            {
                MessageBox.Show(this, "World grf saving is currently unsupported");
                return;
            }
            DialogResult = DialogResult.OK;
            MessageBox.Show("Saved!");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void noneToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionLevel = GameRuleFile.CompressionLevel.None;
        }

        private void compressedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionLevel = GameRuleFile.CompressionLevel.Compressed;
        }

        private void compressedRLEToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionLevel = GameRuleFile.CompressionLevel.CompressedRle;
        }

        private void compressedRLECRCToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionLevel = GameRuleFile.CompressionLevel.CompressedRleCrc;
        }

        private void wiiUPSVitaToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionType = GameRuleFile.CompressionType.Zlib;
        }

        private void pS3ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionType = GameRuleFile.CompressionType.Deflate;
        }

        private void xbox360ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                _file.Header.CompressionType = GameRuleFile.CompressionType.XMem;
        }

        private void GameRuleFileEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.Default.AutoSaveChanges)
            {
                saveToolStripMenuItem_Click(sender, EventArgs.Empty);
            }
        }

        private void exportToJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "gameRules.json";
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialog1.FileName == "")
            {
                return;
            }

            TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, _file.Root.ChildRules);
            writer.Flush();
        }
    }
}
