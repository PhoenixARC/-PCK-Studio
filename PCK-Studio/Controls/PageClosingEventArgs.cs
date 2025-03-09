using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Controls
{
    internal class PageClosingEventArgs : CancelEventArgs
    {
        private readonly TabPage page;
        public TabPage Page => page;

        public PageClosingEventArgs(TabPage page)
            : base()
        {
            this.page = page;
        }
    }
}
