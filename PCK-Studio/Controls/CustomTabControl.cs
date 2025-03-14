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
        private Size CloseButtonSize = new Size(7, 7);
        private const int StartIndex = 1;

        [Browsable(true)]
        public event EventHandler<PageClosingEventArgs> PageClosing;

        public CustomTabControl()
            : base()
        {
        }

        private Rectangle GetCloseButtonArea(Rectangle tabArea)
        {
            Size closeBtnSz = CloseButtonSize;
            var closeBtnPt = new Point(
                tabArea.Right - closeBtnSz.Width,
                tabArea.Top + 2 + (tabArea.Height - closeBtnSz.Height) / 2);
            return new Rectangle(closeBtnPt, closeBtnSz);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (SelectedIndex < StartIndex)
                return;
            Rectangle tabArea = GetTabRect(SelectedIndex);
            Rectangle buttonArea = GetCloseButtonArea(tabArea);
            if (buttonArea.Contains(e.Location))
            {
                var eventArg = new PageClosingEventArgs(TabPages[SelectedIndex]);
                PageClosing?.Invoke(this, eventArg);
                if (!eventArg.Cancel)
                {
                    SelectedIndex -= 1;
                    TabPages.RemoveAt(SelectedIndex + 1);
                }
            }
        }

        protected override void OnCustomPaintForeground(MetroPaintEventArgs e)
        {
            base.OnCustomPaintForeground(e);
            if (SelectedIndex < StartIndex)
                return;
            // Draw Close button
            Rectangle tabArea = GetTabRect(SelectedIndex);

            Rectangle buttonArea = GetCloseButtonArea(tabArea);

            e.Graphics.FillRectangle(MetroPaint.GetStyleBrush(Style), buttonArea);
            e.Graphics.DrawString(
                CloseChar,
                Font,
                new SolidBrush(MetroPaint.ForeColor.Title(Theme)),
                buttonArea.Right - buttonArea.Width - 2, buttonArea.Top - 4);
        }

        //protected override void OnPaintForeground(PaintEventArgs e)
        //{
        //    base.OnPaintForeground(e);
        //    for (int i = StartIndex; i < TabPages.Count; i++)
        //    {
        //        // Draw Close button
        //        Rectangle tabArea = GetTabRect(i);

        //        Rectangle buttonArea = GetCloseButtonArea(tabArea);

        //        e.Graphics.FillRectangle(MetroPaint.GetStyleBrush(Style), buttonArea);
        //        e.Graphics.DrawString(
        //            CloseChar,
        //            Font,
        //            new SolidBrush(MetroPaint.ForeColor.Title(Theme)),
        //            buttonArea.Right - buttonArea.Width - 2, buttonArea.Top - 4);
        //    }
        //}
    }
}