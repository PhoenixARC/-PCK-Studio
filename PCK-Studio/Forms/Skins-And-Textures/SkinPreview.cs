using System;
using System.Drawing;
using System.Windows.Forms;
using PckStudio.Internal;
using PckStudio.Rendering;

namespace PckStudio.Forms
{
    public partial class SkinPreview : Form
    {
        Image Texture;

        public SkinPreview(Image texture, SkinANIM anim)
        {
            InitializeComponent();
            Texture = texture;
            ModelView.Model = anim.GetFlag(SkinAnimFlag.SLIM_MODEL) ? Renderer3D.Models.Alex : Renderer3D.Models.Steve;

            ModelView.ShowHead = !anim.GetFlag(SkinAnimFlag.HEAD_DISABLED);
            ModelView.ShowHeadOverlay = !anim.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED);
            ModelView.ShowBody = !anim.GetFlag(SkinAnimFlag.BODY_DISABLED);
            ModelView.ShowBodyOverlay = !anim.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED);

            ModelView.ShowLeftArm = !anim.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED);
            ModelView.ShowLeftArmOverlay = !anim.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED);
            ModelView.ShowRightArm = !anim.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED);
            ModelView.ShowRightArmOverlay = !anim.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED);

            ModelView.ShowLeftLeg = !anim.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED);
            ModelView.ShowLeftLegOverlay = !anim.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED);
            ModelView.ShowRightLeg = !anim.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED);
            ModelView.ShowRightLegOverlay = !anim.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED);
        }

        private void SkinPreview_Load(object sender, EventArgs e) => RenderModel(Texture);
        
        public void RenderModel(Image source)
        {
            ModelView.Texture = source as Bitmap;
		}
    }
}
