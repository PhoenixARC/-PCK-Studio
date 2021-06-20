using System;
using System.IO;
using System.Drawing;
using MinecraftUSkinEditor.Models;
using PckStudio.Properties;

namespace MinecraftUSkinEditor.Models
{
	internal class CharacterModel : ModelBase
	{
		public CharacterModel()
		{
		}

		protected override Texture[] InitializeTextures()
		{
			return new Texture[]
			{
				new Texture(PckStudio.Properties.Resources.man)
			};
		}

		public override void AddToModelView(MinecraftModelView modelView)
		{
			System.Drawing.Image source = base.Textures[0].Source;
			Object3D object3D = new Box(source, new System.Drawing.Rectangle(8, 0, 0x10, 8), new System.Drawing.Rectangle(0, 8, 0x20, 8), new Point3D(0f, 0f, 0f), Effects.None);
			Object3D object3D2 = new Box(source, new System.Drawing.Rectangle(0x28, 0, 0x10, 8), new System.Drawing.Rectangle(0x20, 8, 0x20, 8), new Point3D(0f, 0f, 0f), Effects.None);
			Object3D object3D3 = new Box(source, new System.Drawing.Rectangle(0x2C, 0x10, 8, 4), new System.Drawing.Rectangle(0x28, 0x14, 0x20, 0xC), new Point3D(0f, 4f, 0f), Effects.FlipHorizontally);
			Object3D object3D4 = new Box(source, new System.Drawing.Rectangle(0x2C, 0x10, 8, 4), new System.Drawing.Rectangle(0x28, 0x14, 0x20, 0xC), new Point3D(0f, 4f, 0f), Effects.None);
			Object3D object3D5 = new Box(source, new System.Drawing.Rectangle(4, 0x10, 8, 4), new System.Drawing.Rectangle(0, 0x14, 0x10, 0xC), new Point3D(0f, 6f, 0f), Effects.FlipHorizontally);
			Object3D object3D6 = new Box(source, new System.Drawing.Rectangle(4, 0x10, 8, 4), new System.Drawing.Rectangle(0, 0x14, 0x10, 0xC), new Point3D(0f, 6f, 0f), Effects.None);
			Object3D object3D7 = new Box(source, new System.Drawing.Rectangle(0x14, 0x10, 0x10, 4), new System.Drawing.Rectangle(0x10, 0x14, 0x18, 0xC), new Point3D(0f, 0f, 0f), Effects.None);
			Object3DGroup object3DGroup = new Object3DGroup();
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
			modelView.AddDynamic(object3DGroup);
			modelView.AddStatic(object3D7);
			modelView.AddDynamic(object3D4);
			modelView.AddDynamic(object3D3);
			modelView.AddDynamic(object3D6);
			modelView.AddDynamic(object3D5);
		}

		public override string ToString()
		{
			return "Character";
		}

	}
}
