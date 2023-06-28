using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using MetroFramework.Controls;
using MetroFramework.Drawing;


namespace PckStudio.Controls
{
    internal class CustomTabControl : MetroTabControl
    {
        private const string CloseChar = "×";
        private Size CloseButtonSize = new Size(8, 8);
        private const int StartIndex = 1;

        [Browsable(true)]
        public event EventHandler<PageClosingEventArgs> PageClosing;

        public CustomTabControl()
            : base()
        {
        }

        private Rectangle GetCloseButtonArea(Rectangle tabArea)
        {
            var closeBtnSz = CloseButtonSize;
            var closeBtnPt = new Point(
                tabArea.Right - closeBtnSz.Width,
                tabArea.Top + (tabArea.Height - closeBtnSz.Height) / 2);
            return new Rectangle(closeBtnPt, closeBtnSz);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            for (var i = StartIndex; i < TabPages.Count; i++)
            {
                var tabArea = GetTabRect(i);
                var buttonArea = GetCloseButtonArea(tabArea);
                if (buttonArea.Contains(e.Location))
                {
                    var eventArg = new PageClosingEventArgs(TabPages[i]);
                    PageClosing?.Invoke(this, eventArg);
                    if (!eventArg.Cancel)
                    {
                        TabPages.RemoveAt(i);
                    }
                    return;
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnPaintForeground(PaintEventArgs e)
        {
            base.OnPaintForeground(e);
            for (int i = StartIndex; i < TabPages.Count; i++)
            {
                // Draw Close button
                var tabArea = GetTabRect(i);

                var buttonArea = GetCloseButtonArea(tabArea);

                e.Graphics.FillRectangle(MetroPaint.GetStyleBrush(Style), buttonArea);
                e.Graphics.DrawString(
                    CloseChar,
                    Font,
                    new SolidBrush(MetroPaint.ForeColor.Title(Theme)),
                    buttonArea.Right - buttonArea.Width - 1, buttonArea.Top - 3);
            }
        }
    }
}
