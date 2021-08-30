
namespace PckStudio.Models
{
    partial class MinecraftModelView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>



        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }


        private PckStudio.Models.BackgroundTypes backgroundType;

        private System.Drawing.Brush backgroundBrush = new System.Drawing.SolidBrush(System.Drawing.Color.SkyBlue);

        private System.EventHandler SkinDowloaded;

        private System.ComponentModel.BackgroundWorker downloader = new System.ComponentModel.BackgroundWorker();

        private System.Drawing.Image webSkin;

        private System.Drawing.Point mouseLastLocation;

        private PckStudio.Models.Object3D rotatingObject3D;

        private System.Drawing.Color backgroundColor = System.Drawing.Color.Transparent;

        private System.Drawing.Color backgroundGradientColor1 = System.Drawing.SystemColors.ControlDarkDark;

        private System.Drawing.Color backgroundGradientColor2 = System.Drawing.SystemColors.ControlLightLight;

        private static System.Drawing.Color textShadowColor = System.Drawing.Color.FromArgb(0x3F, 0x3F, 0x3F);

        private System.Drawing.Image backgroundTexture;

        private System.Drawing.Image usernameImage;

        private System.Drawing.Image versionImage;

        internal PckStudio.Models.Matrix3D GlobalTransformation = PckStudio.Models.Matrix3D.Identity;

        private System.Collections.Generic.List<PckStudio.Models.Texel> texelList = new System.Collections.Generic.List<PckStudio.Models.Texel>();

        private PckStudio.Models.TexelComparer texelComparer = new PckStudio.Models.TexelComparer();

        private System.Collections.Generic.List<PckStudio.Models.Object3D> object3DList = new System.Collections.Generic.List<PckStudio.Models.Object3D>();

        private System.Collections.Generic.List<PckStudio.Models.Object3D> dynamicObject3DtList = new System.Collections.Generic.List<PckStudio.Models.Object3D>();
        #endregion
    }
}
