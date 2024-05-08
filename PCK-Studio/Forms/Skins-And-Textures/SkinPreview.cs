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

        private Image texture;
        private IEnumerable<SkinBOX> data;

        public SkinPreview(SkinModelInfo skin)
        {
            InitializeComponent();
            texture = skin.Texture;
            data = skin.AdditionalBoxes;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ModelView.Initialize(true);
            foreach (var item in data)
            {
                ModelView.ModelData.Add(item);
            }
            ModelView.Texture = texture;
        }
    }
}
