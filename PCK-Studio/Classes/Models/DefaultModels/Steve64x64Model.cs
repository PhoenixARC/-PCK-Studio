using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Internal;
using PckStudio.Models;

namespace PckStudio.Classes.Models.DefaultModels
{
    internal class Steve64x64Model : ModelBase
    {
        SkinANIM _skinANIM;
        public Steve64x64Model(Texture texture, SkinANIM anim)
        {
            textures = new Texture[1] { texture };
            _skinANIM = anim;
            Initialize();
        }

        public override void AddToModelView(MinecraftModelView modelView)
        {
            _ = Textures[0] ?? throw new NullReferenceException(nameof(Textures));
            Image source = Textures[0].Source;

            (int top, int side) armWidth = _skinANIM.GetFlag(ANIM_EFFECTS.SLIM_MODEL) ? (6, 14) : (8, 16);

            Box head        = new Box(source, new Rectangle(8, 0, 16, 8),  new Rectangle(0, 8, 32, 8), new Point3D(0f, 0f, 0f));
            Box headOverlay = new Box(source, new Rectangle(40, 0, 16, 8), new Rectangle(32, 8, 32, 8), new Point3D(0f, 0f, 0f));
            headOverlay.Scale = OverlayScale;

            Box body        = new Box(source, new Rectangle(20, 16, 16, 4), new Rectangle(16, 20, 24, 12), new Point3D(0f, 0f, 0f));
            Box bodyOverlay = new Box(source, new Rectangle(20, 32, 16, 4), new Rectangle(16, 36, 24, 12), new Point3D(0f, 0f, 0f));
            bodyOverlay.Scale = OverlayScale;

            Box rightArm        = new Box(source, new Rectangle(44, 16, armWidth.top, 4), new Rectangle(40, 20, armWidth.side, 12), new Point3D(0f, 4f, 0f));
            Box rightArmOverlay = new Box(source, new Rectangle(44, 32, armWidth.top, 4), new Rectangle(40, 36, armWidth.side, 12), new Point3D(0f, 4f, 0f));
            rightArmOverlay.Scale = OverlayScale;

            Box leftArm        = new Box(source, new Rectangle(36, 48, armWidth.top, 4), new Rectangle(32, 52, armWidth.side, 12), new Point3D(0f, 4f, 0f));
            Box leftArmOverlay = new Box(source, new Rectangle(52, 48, armWidth.top, 4), new Rectangle(48, 52, armWidth.side, 12), new Point3D(0f, 4f, 0f));
            leftArmOverlay.Scale = OverlayScale;

            Box rightLeg        = new Box(source, new Rectangle(4, 16, 8, 4), new Rectangle(0, 20, 16, 12), new Point3D(0f, 6f, 0f));
            Box rightLegOverlay = new Box(source, new Rectangle(4, 32, 8, 4), new Rectangle(0, 52, 16, 12), new Point3D(0f, 6f, 0f));
            rightLegOverlay.Scale = OverlayScale;

            Box leftLeg        = new Box(source, new Rectangle(20, 48, 8, 4), new Rectangle(16, 52, 16, 12), new Point3D(0f, 6f, 0f));
            Box leftLegOverlay = new Box(source, new Rectangle( 4, 48, 8, 4), new Rectangle( 0, 52, 16, 12), new Point3D(0f, 6f, 0f));
            leftLegOverlay.Scale = OverlayScale;

            Object3DGroup headGroup = new Object3DGroup();
            Object3DGroup bodyGroup = new Object3DGroup();
            Object3DGroup leftArmGroup = new Object3DGroup();
            Object3DGroup rightArmGroup = new Object3DGroup();
            Object3DGroup leftLegGroup = new Object3DGroup();
            Object3DGroup rightLegGroup = new Object3DGroup();

            headGroup.RotationOrder = RotationOrders.XY;
            headGroup.MinDegrees1 = -80f;
            headGroup.MaxDegrees1 = 80f;

            headGroup.MinDegrees2 = -57f;
            headGroup.MaxDegrees2 = 57f;

            headGroup.Origin = new Point3D(0f, -4f, 0f);
            headGroup.RotationOrder = RotationOrders.XY;

            headGroup.Position = new Point3D(0f, 8f, 0f);
            headGroup.Add(head);
            headGroup.Add(headOverlay);

            bodyGroup.Position = new Point3D(0f, 2f, 0f);
            bodyGroup.Add(body);
            bodyGroup.Add(bodyOverlay);

            leftArmGroup.Position = new Point3D(6f, 6f, 0f);
            leftArmGroup.RotationOrder = RotationOrders.ZX;
            leftArmGroup.MinDegrees1 = 0f;
            leftArmGroup.MaxDegrees1 = 160f;
            leftArmGroup.MinDegrees2 = -170f;
            leftArmGroup.MaxDegrees2 = 60f;
            leftArmGroup.Add(leftArm);
            leftArmGroup.Add(leftArmOverlay);

            rightArmGroup.Position = new Point3D(-6f, 6f, 0f);
            rightArmGroup.RotationOrder = RotationOrders.ZX;
            rightArmGroup.MinDegrees1 = -160f;
            rightArmGroup.MaxDegrees1 = 0f;
            rightArmGroup.MinDegrees2 = -170f;
            rightArmGroup.MaxDegrees2 = 60f;
            rightArmGroup.Add(rightArm);
            rightArmGroup.Add(rightArmOverlay);

            leftLegGroup.Position = new Point3D(2f, -4f, 0f);
            leftLegGroup.RotationOrder = RotationOrders.ZX;
            leftLegGroup.MinDegrees1 = 0f;
            leftLegGroup.MaxDegrees1 = 70f;
            leftLegGroup.MinDegrees2 = -110f;
            leftLegGroup.MaxDegrees2 = 60f;
            leftLegGroup.Add(leftLeg);
            leftLegGroup.Add(leftLegOverlay);

            rightLegGroup.Position = new Point3D(-2f, -4f, 0f);
            rightLegGroup.RotationOrder = RotationOrders.ZX;
            rightLegGroup.MinDegrees1 = -70f;
            rightLegGroup.MaxDegrees1 = 0f;
            rightLegGroup.MinDegrees2 = -110f;
            rightLegGroup.MaxDegrees2 = 60f;
            rightLegGroup.Add(rightLeg);
            rightLegGroup.Add(rightLegOverlay);

            modelView.AddDynamic(headGroup);
            modelView.AddStatic(bodyGroup);
            modelView.AddDynamic(rightArmGroup);
            modelView.AddDynamic(leftArmGroup);
            modelView.AddDynamic(rightLegGroup);
            modelView.AddDynamic(leftLegGroup);
        }
    }
}
