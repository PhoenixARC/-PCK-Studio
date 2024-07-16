using System;
using System.Drawing;
using System.Windows.Forms;
using PckStudio.Classes.Models.DefaultModels;
using PckStudio.Internal;
using PckStudio.Models;

namespace PckStudio.Forms
{
    public partial class SkinPreview : Form
    {
        Image _texture;
        ModelBase _model;

        public SkinPreview(Image img, SkinANIM anim, ModelBase model = null)
        {
            InitializeComponent();
            _texture = img;

            _model = model ?? new Steve64x32Model(_texture);
            if (img.Width == 64 && img.Height == 64)
            {
                _model = model ?? new Steve64x64Model(_texture, anim);
            }
        }

        private void SkinPreview_Load(object sender, EventArgs e) => RenderModel(_texture);
        
        public void RenderModel(Image source)
        {
			_model.AddToModelView(ModelView);
		}
    }
}
