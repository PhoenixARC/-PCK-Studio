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
        public SkinANIM ANIM
        {
            get => ModelView.ANIM;
            set => ModelView.ANIM = value;
        }

        public SkinPreview(Image texture, IEnumerable<SkinBOX> modelData)
        {
            InitializeComponent();
            foreach (var item in modelData)
            {
                ModelView.ModelData.Add(item);
            }
            ModelView.Texture = texture;
        }
    }
}
