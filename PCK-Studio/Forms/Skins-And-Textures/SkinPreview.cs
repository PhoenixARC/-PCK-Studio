using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PckStudio.Internal;

namespace PckStudio.Forms
{
    public partial class SkinPreview : Form
    {
        public SkinANIM ANIM { get; set; }

        private Image _texture;

        public SkinPreview()
        {
            InitializeComponent();
        }

        public SkinPreview(Image texture, IEnumerable<SkinBOX> modelData)
            : this()
        {
            _texture = texture;
            ModelView.ModelData.AddRange(modelData);
        }

        private void SkinPreview_Load(object sender, EventArgs e)
        {
            ModelView.ANIM = ANIM;
            ModelView.Texture = _texture as Bitmap;
		}
    }
}
