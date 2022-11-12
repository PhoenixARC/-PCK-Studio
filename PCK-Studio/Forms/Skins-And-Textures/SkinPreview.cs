using System;
using System.Drawing;
using System.Windows.Forms;
using PckStudio.Models;

namespace PckStudio.Forms
{
    public partial class SkinPreview : Form
    {
        Image Texture;
        ModelBase Model;

        public SkinPreview(Image img, ModelBase model = null)
        {
            InitializeComponent();
            Texture = img;
            Model = model ?? new Steve64x32Model(Texture);
        }

        private void SkinPreview_Load(object sender, EventArgs e) => RenderModel(Texture);
        
        public void RenderModel(Image source)
        {
			Model.AddToModelView(ModelView);
		}
    }
}
