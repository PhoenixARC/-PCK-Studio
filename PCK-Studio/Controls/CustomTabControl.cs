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
        private const string CloseText = "×";

        private const int StartIndex = 1;

        [Browsable(true)]
        public event EventHandler<PageClosingEventArgs> PageClosing;

        public CustomTabControl()
            : base()
        {
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var closeBtnSize = TextRenderer.MeasureText(CloseText, Font);
            for (var i = StartIndex; i < TabPages.Count; i++)
            {
                var tabRect = GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var closeRect = new Rectangle(
                    (tabRect.Right - closeBtnSize.Width),
                    tabRect.Top + (tabRect.Height - closeBtnSize.Height) / 2,
                    10,
                    10);
                if (closeRect.Contains(e.Location))
                {
                    var eventArg = new PageClosingEventArgs(TabPages[i]);
                    PageClosing?.Invoke(this, eventArg);
                    if (!eventArg.Cancel)
                    {
                        TabPages.RemoveAt(i);
                    }
                    break;
                }
            }
        }

        protected override void OnPaintForeground(PaintEventArgs e)
        {
            base.OnPaintForeground(e);
            for (int i = StartIndex; i < TabPages.Count; i++)
            {
                // Draw Close button
                var tabRect = GetTabRect(i);
                Point closePt = new Point(15, 5);
                e.Graphics.DrawRectangle(MetroPaint.GetStylePen(Style), tabRect.Right - closePt.X, tabRect.Top + closePt.Y, 10, 10);
                e.Graphics.FillRectangle(MetroPaint.GetStyleBrush(Style), tabRect.Right - closePt.X, tabRect.Top + closePt.Y, 10, 10);
                e.Graphics.DrawString(CloseText, Font, new SolidBrush(MetroPaint.ForeColor.Title(Theme)), tabRect.Right - (closePt.X), tabRect.Top + closePt.Y - 2);
            }
        }
    }
}
