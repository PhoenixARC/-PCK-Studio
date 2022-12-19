using System;
using System.Drawing;
using PckStudio.Classes.Models.DefaultModels;
using PckStudio.ToolboxItems;
using PckStudio.Classes.Utils;
using PckStudio.Models;

namespace PckStudio.Forms
{
    public partial class SkinPreview : ThemeForm
    {
        Image Texture;
        ModelBase Model;

        public SkinPreview(Image img, SkinANIM anim, ModelBase model = null)
        {
            InitializeComponent();
            Texture = img;

            Model = model ?? new Steve64x32Model(Texture);
            if (img.Width == 64 && img.Height == 64)
            {
                Model = model ?? new Steve64x64Model(Texture, anim);
            }
        }

        private void SkinPreview_Load(object sender, EventArgs e) => RenderModel(Texture);
        
        public void RenderModel(Image source)
        {
			Model.AddToModelView(ModelView);
		}
    }
}
