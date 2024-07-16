/* Copyright (c) 2024-present miku-666
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
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	internal partial class FilterPrompt : MetroForm
	{
		public string AcceptButtonText { get => acceptButton.Text; set => acceptButton.Text = value; }
		public string CancelButtonText { get => cancelButton.Text; set => cancelButton.Text = value; }

		public Color PageBackColor { get; set; } = Color.FromArgb(64, 64, 64);

		private object _selectedItem;
		public object SelectedItem => _selectedItem;

		public int SelectedTabIndex => tabController.SelectedIndex;

		public delegate bool FilterPredicate(string filterText, object nodeTag);

        public event EventHandler OnSelectedItemChanged
		{
			add => Events.AddHandler(nameof(OnSelectedItemChanged), value);
			remove => Events.RemoveHandler(nameof(OnSelectedItemChanged), value);
		}

        public FilterPrompt()
		{
			InitializeComponent();
        }

		public TreeView AddFilterPage(string categoryName, string key, FilterPredicate filterPredicate)
		{
			_ = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
			TabPage page = new TabPage(categoryName);
			page.Name = key ?? categoryName;
			page.Tag = filterPredicate;
			var pageView = new TreeView()
			{
				Dock = DockStyle.Fill,
				BackColor = PageBackColor,
			};
			pageView.AfterSelect += (sender, e) =>
			{
				_selectedItem = e.Node.Tag;
				Events[nameof(OnSelectedItemChanged)]?.DynamicInvoke(this, EventArgs.Empty);
            };
            pageView.Tag = new List<TreeNode>(4);
            page.Controls.Add(pageView);
			tabController.TabPages.Add(page);
			return pageView;
        }

		public TreeView GetByKey(string key)
		{
			return tabController.TabPages[key].Controls[0] is TreeView view ? view : null;
		}

		private void filter_TextChanged(object sender, EventArgs e)
		{
			// Some code in this function is modified code from this StackOverflow answer - MattNL
			// https://stackoverflow.com/questions/8260322/filter-a-treeview-with-a-textbox-in-a-c-sharp-winforms-app

			// block re-painting control until all objects are loaded
			foreach (TabPage tabpage in tabController.TabPages)
			{
				if (tabpage.Tag is not FilterPredicate filerPredicate || tabpage.Controls[0] is not TreeView pageView || pageView.Tag is not List<TreeNode> pageCache)
					continue;

                if (string.IsNullOrEmpty(filterTextBox.Text))
				{
					pageView.Nodes.Clear();
					pageView.Nodes.AddRange(pageCache.ToArray());
					continue;
				}

                pageView.BeginUpdate();
                pageView.Nodes.Clear();
				foreach (TreeNode _node in pageCache)
				{
                    if (_node.Text.ToLower().Contains(filterTextBox.Text.ToLower()) || filerPredicate(filterTextBox.Text, _node.Tag))
					{
						pageView.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				pageView.EndUpdate();
			}
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void AcceptBtn_Click(object sender, EventArgs e)
		{
            DialogResult = _selectedItem is null ? DialogResult.Cancel : DialogResult.OK;
		}
	}
}
