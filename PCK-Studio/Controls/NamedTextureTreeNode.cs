using System.Drawing;
using System.Windows.Forms;
using PckStudio.Core;

namespace PckStudio.Controls
{
    public class NamedTextureTreeNode : TreeNode
    {
        private readonly NamedData<Image> _namedTexture;

        public NamedTextureTreeNode(NamedData<Image> namedTexture)
            : base(namedTexture.Name)
        {
            Tag = namedTexture;
            _namedTexture = namedTexture;
        }

        public Image GetTexture() => _namedTexture.Value;
    }
}
