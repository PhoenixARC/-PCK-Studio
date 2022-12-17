using System.ComponentModel;
using System.Windows.Forms;
using Dark.Net;

namespace PckStudio.Classes.ToolboxItems
{
    public partial class ThemeForm : Form
    {
        public ThemeForm()
            : base()
        {
            DarkNet.Instance.SetWindowThemeForms(this, Theme.Auto);
            Invalidate();
        }

        public ThemeForm(IContainer container)
            : this()
        {
            container.Add(this);
        }
    }
}
