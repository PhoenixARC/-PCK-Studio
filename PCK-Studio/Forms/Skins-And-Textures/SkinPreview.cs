using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
