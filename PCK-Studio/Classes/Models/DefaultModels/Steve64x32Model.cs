using System;
using System.IO;
using System.Drawing;
using PckStudio.Models;
using PckStudio.Properties;

namespace PckStudio.Models
{
	internal class Steve64x32Model : ModelBase
	{
        public Steve64x32Model(Image texture)
		{
			textures = new Image[1] { texture };
		}

		public override void AddToModelView(MinecraftModelView modelView)
		{
			_ = Textures[0] ?? throw new NullReferenceException(nameof(Textures));
            Image source = Textures[0];
			Box head        = new Box(source, new Rectangle( 8, 0, 16, 8), new Rectangle( 0, 8, 32, 8), new Point3D(0f, 0f, 0f));
			Box headOverlay = new Box(source, new Rectangle(40, 0, 16, 8), new Rectangle(32, 8, 32, 8), new Point3D(0f, 0f, 0f));
			headOverlay.Scale = OverlayScale;

			Box body = new Box(source, new Rectangle(20, 16, 16, 4), new Rectangle(16, 20, 24, 12), new Point3D(0f, 0f, 0f));

			Box leftArm  = new Box(source, new Rectangle(44, 16, 8,  4), new Rectangle(40, 20, 32, 12), new Point3D(0f, 4f, 0f));
			Box rightArm = new Box(source, new Rectangle(44, 16, 8,  4), new Rectangle(40, 20, 32, 12), new Point3D(0f, 4f, 0f));

			Box leftLeg  = new Box(source, new Rectangle(4, 16, 8, 4), new Rectangle(0, 20, 16, 12), new Point3D(0f, 6f, 0f));
			Box rightLeg = new Box(source, new Rectangle(4, 16, 8, 4), new Rectangle(0, 20, 16, 12), new Point3D(0f, 6f, 0f));

			Object3DGroup headGroup = new Object3DGroup();
			
			headGroup.RotationOrder = RotationOrders.XY;
			headGroup.MinDegrees1 = -80f;
			headGroup.MaxDegrees1 = 80f;

			headGroup.MinDegrees2 = -57f;
			headGroup.MaxDegrees2 = 57f;
			
			headGroup.Add(head);
			headGroup.Add(headOverlay);
			
			headGroup.Position = new Point3D(0f, 8f, 0f);
			headGroup.Origin = new Point3D(0f, -4f, 0f);
			headGroup.RotationOrder = RotationOrders.XY;

			body.Position = new Point3D(0f, 2f, 0f);

			leftArm.Position = new Point3D(6f, 6f, 0f);
			leftArm.RotationOrder = RotationOrders.ZX;
			leftArm.MinDegrees1 = 0f;
			leftArm.MaxDegrees1 = 160f;
			leftArm.MinDegrees2 = -170f;
			leftArm.MaxDegrees2 = 60f;

			rightArm.Position = new Point3D(-6f, 6f, 0f);
			rightArm.RotationOrder = RotationOrders.ZX;
			rightArm.MinDegrees1 = -160f;
			rightArm.MaxDegrees1 = 0f;
			rightArm.MinDegrees2 = -170f;
			rightArm.MaxDegrees2 = 60f;

			leftLeg.Position = new Point3D(2f, -4f, 0f);
			leftLeg.RotationOrder = RotationOrders.ZX;
			leftLeg.MinDegrees1 = 0f;
			leftLeg.MaxDegrees1 = 70f;
			leftLeg.MinDegrees2 = -110f;
			leftLeg.MaxDegrees2 = 60f;
			
			rightLeg.Position = new Point3D(-2f, -4f, 0f);
			rightLeg.RotationOrder = RotationOrders.ZX;
			rightLeg.MinDegrees1 = -70f;
			rightLeg.MaxDegrees1 = 0f;
			rightLeg.MinDegrees2 = -110f;
			rightLeg.MaxDegrees2 = 60f;

			modelView.AddDynamic(headGroup);
			modelView.AddStatic(body);
			modelView.AddDynamic(rightArm);
			modelView.AddDynamic(leftArm);
			modelView.AddDynamic(rightLeg);
			modelView.AddDynamic(leftLeg);
		}

		public override string ToString() => nameof(Steve64x32Model);
	}
}
