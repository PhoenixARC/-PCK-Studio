using Dark.Net;
using System.ComponentModel;
using System.Windows.Forms;

namespace PckStudio.Classes.ToolboxItems
{
    public partial class ThemeForm : Form
    {
        public ThemeForm()
            : base()
        {
            DarkNet.Instance.SetWindowThemeForms(this, Theme.Auto);
        }

        public ThemeForm(IContainer container)
            : this()
        {
            container.Add(this);
        }
    }
}
