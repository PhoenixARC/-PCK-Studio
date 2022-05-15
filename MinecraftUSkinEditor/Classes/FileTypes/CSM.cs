using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Models;

namespace PckStudio.Classes
{
    class CSM
    {

        //Part Name
        //Part Parent(HEAD, BODY, LEG0, LEG1, ARM0, ARM1)
        //Part Name
        //Position-X
        //Position-Y
        //Position-Z
        //Size-X
        //Size-Y
        //Size-Z
        //UV-Y
        //UV-X

        public static List<string> CSMBlock = new List<string>();

        public static void TryParse(string CSM, MinecraftModelView modelView)
        {
            try
            {
                int i = 0;
                string[] CSMLines = CSM.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
                foreach (string line in CSMLines)
                {
                    if (i > 10)
                    {
                        GetModelPartFromCSM(CSMBlock, modelView);
                        CSMBlock.Clear();
                        i = 0;
                    }
                    CSMBlock.Add(line + "\n");
                    i++;
                }

                modelView.Invalidate();
            }
            catch { }
        }

        public static void GetModelPartFromCSM(List<string> CSM, MinecraftModelView modelView)
        {
            string PartName = CSM[0];
            string PartParent = CSM[1];
            string PartName2 = CSM[2];
            int PositionX = int.Parse(CSM[3]);
            int PositionY = int.Parse(CSM[4]);
            int PositionZ = int.Parse(CSM[5]);
            int SizeX = int.Parse(CSM[6]);
            int SizeY = int.Parse(CSM[7]);
            int SizeZ = int.Parse(CSM[8]);
            int UVY = int.Parse(CSM[9]);
            int UVX = int.Parse(CSM[10]);

            //RenderBox
            System.Drawing.Image source = Textures[0].Source;
            Object3D object3D = new Box(source, new System.Drawing.Rectangle(8, 0, 0x10, 8), new System.Drawing.Rectangle(0, 8, 0x20, 8), new Point3D(0f, 0f, 0f), Effects.None);
            Object3D object3D2 = new Box(source, new System.Drawing.Rectangle(0x28, 0, 0x10, 8), new System.Drawing.Rectangle(0x20, 8, 0x20, 8), new Point3D(0f, 0f, 0f), Effects.None);
            Object3D object3D3 = new Box(source, new System.Drawing.Rectangle(0x2C, 0x10, 8, 4), new System.Drawing.Rectangle(0x28, 0x14, 0x20, 0xC), new Point3D(0f, 4f, 0f), Effects.FlipHorizontally);


            //RenderGroup
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
            modelView.AddDynamic(object3DGroup);
        }

        public static Texture[] Textures =  new Texture[] { new Texture(Bitmap.FromFile(Environment.CurrentDirectory + "\\default.png")) };
    }
}
