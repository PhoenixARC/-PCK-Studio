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
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_ModernTabControl : TabControl
    {

        private Graphics G;

        private Rectangle Rect;
        public CrEaTiiOn_ModernTabControl()
        {
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            Alignment = TabAlignment.Left;
            ItemSize = new Size(40, 170);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            foreach (CrEaTiiOn_ModernTabPage T in TabPages)
            {
                T.Font = new Font("Segoe UI", 9);
                T.ForeColor = Color.FromArgb(220, 220, 220);
                T.BackColor = Color.FromArgb(20, 20, 20);
            }

            base.OnHandleCreated(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;

            G.Clear(Color.FromArgb(20, 20, 20));

            using (Pen Right = new Pen(Color.FromArgb(250, 36, 38)))
            {
                G.DrawLine(Right, ItemSize.Height + 3, 4, ItemSize.Height + 3, Height - 5);
            }

            for (int T = 0; T <= TabPages.Count - 1; T++)
            {
                Rect = GetTabRect(T);

                if (SelectedIndex == T)
                {
                    using (SolidBrush Selection = new SolidBrush(Color.FromArgb(250, 36, 38)))
                    {
                        G.FillRectangle(Selection, new Rectangle(Rect.X - 4, Rect.Y + 2, Rect.Width + 6, Rect.Height - 2));
                    }

                    using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(220, 220, 219)))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(TabPages[T].Text, TextFont, TextBrush, new Point(Rect.X + 45, Rect.Y + 11));
                        }
                    }

                    using (Pen Line = new Pen(Color.FromArgb(250, 36, 38)))
                    {
                        G.DrawLine(Line, Rect.X - 2, Rect.Y + 1, Rect.Width + 4, Rect.Y + 1);
                        G.DrawLine(Line, Rect.X - 2, Rect.Y + 39, Rect.Width + 4, Rect.Y + 39);
                    }

                }
                else
                {
                    using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(180, 180, 179)))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(TabPages[T].Text, TextFont, TextBrush, new Point(Rect.X + 45, Rect.Y + 11));
                        }
                    }
                }

                if ((ImageList != null))
                {
                    if (!(TabPages[T].ImageIndex < 0))
                    {
                        G.DrawImage(ImageList.Images[TabPages[T].ImageIndex], new Rectangle(Rect.X + 18, Rect.Y + 12, 16, 16));
                    }
                }
            }

            base.OnPaint(e);
        }
    }
}
