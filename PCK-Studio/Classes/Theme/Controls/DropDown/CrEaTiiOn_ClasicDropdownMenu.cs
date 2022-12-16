#region Imports
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_ClasicDropdownMenu : CrEaTiiOn_ContextMenuStrip
    {
        private bool isMainMenu;
        private int menuItemHeight = 25;
        private Color menuItemTextColor = Color.Empty;
        private Color primaryColor = Color.FromArgb(20, 20, 20);
        private Bitmap menuItemHeaderSize;

        public CrEaTiiOn_ClasicDropdownMenu(IContainer container)
          : base(container)
        {
        }

        [Browsable(false)]
        public bool IsMainMenu
        {
            get => this.isMainMenu;
            set => this.isMainMenu = value;
        }

        [Browsable(false)]
        public int MenuItemHeight
        {
            get => this.menuItemHeight;
            set => this.menuItemHeight = value;
        }

        [Browsable(false)]
        public Color MenuItemTextColor
        {
            get => this.menuItemTextColor;
            set => this.menuItemTextColor = value;
        }

        [Browsable(false)]
        public Color PrimaryColor
        {
            get => this.primaryColor;
            set => this.primaryColor = value;
        }

        private void LoadMenuItemHeight()
        {
            this.menuItemHeaderSize = !this.isMainMenu ? new Bitmap(20, this.menuItemHeight) : new Bitmap(25, 45);
            foreach (ToolStripMenuItem toolStripMenuItem in (ArrangedElementCollection)this.Items)
            {
                toolStripMenuItem.ImageScaling = ToolStripItemImageScaling.None;
                if (toolStripMenuItem.Image == null)
                    toolStripMenuItem.Image = (Image)this.menuItemHeaderSize;
                foreach (ToolStripMenuItem dropDownItem1 in (ArrangedElementCollection)toolStripMenuItem.DropDownItems)
                {
                    dropDownItem1.ImageScaling = ToolStripItemImageScaling.None;
                    if (dropDownItem1.Image == null)
                        dropDownItem1.Image = (Image)this.menuItemHeaderSize;
                    foreach (ToolStripMenuItem dropDownItem2 in (ArrangedElementCollection)dropDownItem1.DropDownItems)
                    {
                        dropDownItem2.ImageScaling = ToolStripItemImageScaling.None;
                        if (dropDownItem2.Image == null)
                            dropDownItem2.Image = (Image)this.menuItemHeaderSize;
                        foreach (ToolStripMenuItem dropDownItem3 in (ArrangedElementCollection)dropDownItem2.DropDownItems)
                        {
                            dropDownItem3.ImageScaling = ToolStripItemImageScaling.None;
                            if (dropDownItem3.Image == null)
                                dropDownItem3.Image = (Image)this.menuItemHeaderSize;
                        }
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.DesignMode)
                return;
            this.Renderer = (ToolStripRenderer)new MenuRenderer(this.isMainMenu, this.primaryColor, this.menuItemTextColor);
            this.LoadMenuItemHeight();
        }
    }
}
