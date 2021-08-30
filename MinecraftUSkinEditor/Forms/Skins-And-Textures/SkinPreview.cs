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
        public SkinPreview(Image img)
        {
            InitializeComponent();
            Texture = img;
        }
        private void SkinPreview_Load(object sender, EventArgs e)
        {
            Texture[] txt = new Texture[] { new Texture(Texture) };
            RenderModl(txt);
        }


        public void RenderModl(Texture[] Textures)
        {

            //RenderBox
            System.Drawing.Image source = Textures[0].Source;
            Object3D object3D = new Box(source, new System.Drawing.Rectangle(8, 0, 0x10, 8), new System.Drawing.Rectangle(0, 8, 0x20, 8), new Point3D(0f, 0f, 0f), Effects.None);
            Object3D object3D2 = new Box(source, new System.Drawing.Rectangle(0x28, 0, 0x10, 8), new System.Drawing.Rectangle(0x20, 8, 0x20, 8), new Point3D(0f, 0f, 0f), Effects.None);
            Object3D object3D3 = new Box(source, new System.Drawing.Rectangle(0x2C, 0x10, 8, 4), new System.Drawing.Rectangle(0x28, 0x14, 0x20, 0xC), new Point3D(0f, 4f, 0f), Effects.FlipHorizontally);
            Object3D object3D4 = new Box(source, new System.Drawing.Rectangle(0x2C, 0x10, 8, 4), new System.Drawing.Rectangle(0x28, 0x14, 0x20, 0xC), new Point3D(0f, 4f, 0f), Effects.None);
            Object3D object3D5 = new Box(source, new System.Drawing.Rectangle(4, 0x10, 8, 4), new System.Drawing.Rectangle(0, 0x14, 0x10, 0xC), new Point3D(0f, 6f, 0f), Effects.FlipHorizontally);
            Object3D object3D6 = new Box(source, new System.Drawing.Rectangle(4, 0x10, 8, 4), new System.Drawing.Rectangle(0, 0x14, 0x10, 0xC), new Point3D(0f, 6f, 0f), Effects.None);
            Object3D object3D7 = new Box(source, new System.Drawing.Rectangle(0x14, 0x10, 0x10, 4), new System.Drawing.Rectangle(0x10, 0x14, 0x18, 0xC), new Point3D(0f, 0f, 0f), Effects.None);
            Object3DGroup object3DGroup = new Object3DGroup();


			//RenderGroup
			object3D2.Scale = 1.16f;
			object3DGroup.RotationOrder = RotationOrders.XY;
			object3DGroup.MinDegrees1 = -80f;
			object3DGroup.MaxDegrees1 = 80f;
			object3DGroup.MinDegrees2 = -57f;
			object3DGroup.MaxDegrees2 = 57f;
			object3DGroup.Add(object3D);
			object3DGroup.Add(object3D2);
			object3DGroup.Position = new Point3D(0f, 8f, 0f);
			object3DGroup.Origin = new Point3D(0f, -4f, 0f);
			object3DGroup.RotationOrder = RotationOrders.XY;
			object3D7.Position = new Point3D(0f, 2f, 0f);
			object3D3.Position = new Point3D(6f, 6f, 0f);
			object3D3.RotationOrder = RotationOrders.ZX;
			object3D3.MinDegrees1 = 0f;
			object3D3.MaxDegrees1 = 160f;
			object3D3.MinDegrees2 = -170f;
			object3D3.MaxDegrees2 = 60f;
			object3D4.Position = new Point3D(-6f, 6f, 0f);
			object3D4.RotationOrder = RotationOrders.ZX;
			object3D4.MinDegrees1 = -160f;
			object3D4.MaxDegrees1 = 0f;
			object3D4.MinDegrees2 = -170f;
			object3D4.MaxDegrees2 = 60f;
			object3D5.Position = new Point3D(2f, -4f, 0f);
			object3D5.RotationOrder = RotationOrders.ZX;
			object3D5.MinDegrees1 = 0f;
			object3D5.MaxDegrees1 = 70f;
			object3D5.MinDegrees2 = -110f;
			object3D5.MaxDegrees2 = 60f;
			object3D6.Position = new Point3D(-2f, -4f, 0f);
			object3D6.RotationOrder = RotationOrders.ZX;
			object3D6.MinDegrees1 = -70f;
			object3D6.MaxDegrees1 = 0f;
			object3D6.MinDegrees2 = -110f;
			object3D6.MaxDegrees2 = 60f;
			minecraftModelView1.AddDynamic(object3DGroup);
			minecraftModelView1.AddStatic(object3D7);
			minecraftModelView1.AddDynamic(object3D4);
			minecraftModelView1.AddDynamic(object3D3);
			minecraftModelView1.AddDynamic(object3D6);
			minecraftModelView1.AddDynamic(object3D5);
		}
    }
}
