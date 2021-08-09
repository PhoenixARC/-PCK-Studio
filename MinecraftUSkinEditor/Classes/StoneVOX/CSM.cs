using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonevox
{
    public class CSM
    {
        public List<Box> boxes = new List<Box>();
        public CSM(string path)
        {
            Open(path);
        }
        void Open(string path)
        {
            string RawCSM = path;
            string[] lines = RawCSM.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            while (i < lines.Length / 11)
            {
                string PartName = lines[(i*11)+0];
                string ParentName = lines[(i*11)+1];
                string PartName2 = lines[(i*11)+2];
                int x = (int)Math.Round(float.Parse(lines[(i*11)+3]), 1, MidpointRounding.ToEven);
                int y = (int)Math.Round(float.Parse(lines[(i*11)+4]), 1, MidpointRounding.ToEven);
                int z = (int)Math.Round(float.Parse(lines[(i*11)+5]), 1, MidpointRounding.ToEven);
                int Sizex = (int)Math.Round(float.Parse(lines[(i*11)+6]), 1, MidpointRounding.ToEven);
                int Sizey = (int)Math.Round(float.Parse(lines[(i*11)+7]), 1, MidpointRounding.ToEven);
                int Sizez = (int)Math.Round(float.Parse(lines[(i*11)+8]), 1, MidpointRounding.ToEven);
                int uvx = (int)Math.Round(float.Parse(lines[(i*11)+9]), 1, MidpointRounding.ToEven);
                int uvy = (int)Math.Round(float.Parse(lines[(i*11)+10]), 1, MidpointRounding.ToEven);

                Box box = new Box();
                box.PartName = PartName;
                box.ParentName = ParentName;
                box.x = x;
                box.y = y;
                box.z = z;
                box.SizeX = Sizex;
                box.SizeY = Sizey;
                box.SizeZ = Sizez;
                box.UvX = uvx;
                box.UvY = uvy;
                boxes.Add(box);

                i++;
            }
        }
        
    }
    public class Box
    {
        public string PartName;
        public string ParentName;
        public int x;
        public int y;
        public int z;
        public int SizeX;
        public int SizeY;
        public int SizeZ;
        public int UvX;
        public int UvY;
    }
}
