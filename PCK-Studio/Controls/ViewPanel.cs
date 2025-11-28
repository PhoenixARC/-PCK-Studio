using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Pck;

namespace PckStudio.Controls
{
    public class ViewPanel : UserControl
    {

        public virtual void LoadAsset(PckAsset asset, Action onChange) => throw new NotImplementedException("Derived class must implement this function.");
        public virtual void Reset() => throw new NotImplementedException("Derived class must implement this function.");
    }
}
