#region Imports
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_ContextMenuStrip : System.Windows.Forms.ContextMenuStrip
    {
        private Rectangle Rect;

        private Graphics G;
        public CrEaTiiOn_ContextMenuStrip()
        {
            base.Renderer = Renderer;
        }

        public CrEaTiiOn_ContextMenuStrip(IContainer container) : base(container)
        {
        }

        private void Renderer_PaintMenuBackground(object sender, ToolStripRenderEventArgs e)
        {
            G = e.Graphics;

            G.Clear(Color.FromArgb(44, 44, 43));
        }


        private void Renderer_PaintMenuBorder(object sender, ToolStripRenderEventArgs e)
        {
            G = e.Graphics;

            using (Pen Border = new Pen(Color.FromArgb(42, 42, 41)))
            {
                G.DrawRectangle(Border, new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1));
            }
        }

        private void Renderer_PaintItemArrow(object sender, ToolStripArrowRenderEventArgs e)
        {
            G = e.Graphics;

            using (Font TextFont = new Font("Marlett", 12))
            {
                using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
                {
                    G.DrawString("8", TextFont, TextBrush, new Point(e.ArrowRectangle.X - 2, e.ArrowRectangle.Y + 1));
                }
            }
        }

        private void Renderer_PaintItemImage(object sender, ToolStripItemImageRenderEventArgs e)
        {
            G = e.Graphics;

            G.DrawImage(e.Image, new Rectangle(4, 1, 16, 16));
        }

        private void Renderer_PaintItemText(object sender, ToolStripItemTextRenderEventArgs e)
        {
            G = e.Graphics;

            using (Font ItemFont = new Font("Segoe UI", 9))
            {
                using (SolidBrush ItemBrush = new SolidBrush(Color.FromArgb(220, 220, 220)))
                {
                    G.DrawString(e.Text, ItemFont, ItemBrush, new Point(e.TextRectangle.X, e.TextRectangle.Y));
                }
            }
        }

        private void Renderer_PaintItemBackground(object sender, ToolStripItemRenderEventArgs e)
        {
            G = e.Graphics;

            Rect = e.Item.ContentRectangle;

            if (e.Item.Selected)
            {
                using (SolidBrush Fill = new SolidBrush(Color.FromArgb(54, 54, 53)))
                {
                    G.FillRectangle(Fill, new Rectangle(Rect.X - 1, Rect.Y - 1, Rect.Width + 4, Rect.Height - 1));
                }
            }
        }
    }
}
