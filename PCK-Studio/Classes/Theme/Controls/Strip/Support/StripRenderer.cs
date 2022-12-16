using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;

namespace CBH_Ultimate_Theme_Library.Theme.Controls.Strip.Support
{
    public class cStripRenderer : ToolStripRenderer
    {

        public event PaintMenuBackgroundEventHandler PaintMenuBackground;
        public delegate void PaintMenuBackgroundEventHandler(object sender, ToolStripRenderEventArgs e);
        public event PaintMenuBorderEventHandler PaintMenuBorder;
        public delegate void PaintMenuBorderEventHandler(object sender, ToolStripRenderEventArgs e);
        public event PaintMenuImageMarginEventHandler PaintMenuImageMargin;
        public delegate void PaintMenuImageMarginEventHandler(object sender, ToolStripRenderEventArgs e);
        public event PaintItemCheckEventHandler PaintItemCheck;
        public delegate void PaintItemCheckEventHandler(object sender, ToolStripItemImageRenderEventArgs e);
        public event PaintItemImageEventHandler PaintItemImage;
        public delegate void PaintItemImageEventHandler(object sender, ToolStripItemImageRenderEventArgs e);
        public event PaintItemTextEventHandler PaintItemText;
        public delegate void PaintItemTextEventHandler(object sender, ToolStripItemTextRenderEventArgs e);
        public event PaintItemBackgroundEventHandler PaintItemBackground;
        public delegate void PaintItemBackgroundEventHandler(object sender, ToolStripItemRenderEventArgs e);
        public event PaintItemArrowEventHandler PaintItemArrow;
        public delegate void PaintItemArrowEventHandler(object sender, ToolStripArrowRenderEventArgs e);
        public event PaintSeparatorEventHandler PaintSeparator;
        public delegate void PaintSeparatorEventHandler(object sender, ToolStripSeparatorRenderEventArgs e);

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (PaintMenuBackground != null)
            {
                PaintMenuBackground(this, e);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (PaintMenuImageMargin != null)
            {
                PaintMenuImageMargin(this, e);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (PaintMenuBorder != null)
            {
                PaintMenuBorder(this, e);
            }
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (PaintItemCheck != null)
            {
                PaintItemCheck(this, e);
            }
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (PaintItemImage != null)
            {
                PaintItemImage(this, e);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (PaintItemText != null)
            {
                PaintItemText(this, e);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (PaintItemBackground != null)
            {
                PaintItemBackground(this, e);
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (PaintItemArrow != null)
            {
                PaintItemArrow(this, e);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (PaintSeparator != null)
            {
                PaintSeparator(this, e);
            }
        }

    }
}
