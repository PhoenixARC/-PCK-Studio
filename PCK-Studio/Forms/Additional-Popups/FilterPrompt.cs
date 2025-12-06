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
using System.Drawing;
using System.Windows.Forms;
using PckStudio.Core.Extensions;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	internal partial class FilterPrompt : UserControl
	{
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

        public new void Update()
        {
			base.Update();
            foreach (TabPage tabpage in tabController.TabPages)
            {
                if (tabpage.Controls[0] is not TreeView pageView || pageView.Tag is not TreeView backingView)
                    continue;

                pageView.BeginUpdate();
                pageView.Nodes.Clear();
				FilterPredicate filerPredicate = tabpage.Tag as FilterPredicate;
                foreach (TreeNode node in backingView.Nodes.GetLeafNodes())
                {
                    if (string.IsNullOrEmpty(filterTextBox.Text) ||
						node.FullPath.ToLower().Contains(filterTextBox.Text.ToLower()) ||
						(filerPredicate?.Invoke(filterTextBox.Text, node.Tag) ?? false))
                    {
                        pageView.Nodes.BuildNodeTreeBySeperator(node.FullPath, backingView.PathSeparator);
                    }
                }
                pageView.EndUpdate();
            }
        }

        public TreeView AddFilterPage(string categoryName, string key, FilterPredicate filterPredicate)
		{
			_ = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
			TabPage page = new TabPage(categoryName);
			page.BorderStyle = BorderStyle.None;
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
			var backingView = new TreeView()
            {
                Dock = DockStyle.Fill,
                BackColor = PageBackColor,
            };
            pageView.Tag = backingView;
            page.Controls.Add(pageView);
			tabController.TabPages.Add(page);
			return backingView;
        }

		public TreeView GetByKey(string key)
		{
			return tabController.TabPages[key].Controls[0] is TreeView view ? view : null;
		}

		private void filter_TextChanged(object sender, EventArgs e)
		{
			Update();
		}
	}
}
