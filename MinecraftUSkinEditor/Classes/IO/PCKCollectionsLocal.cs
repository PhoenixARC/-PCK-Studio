using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PckStudio.Classes.IO
{
    public  class PCKCollectionsLocal
    {
        string cache = Program.Appdata + "cache/packs/";
        public string[] GetLocalCategories()
        {
            string cat = "";
            if (File.Exists(cache + "PCKCategories.txt"))
                cat = File.ReadAllText(cache + "PCKCategories.txt");
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }


        public string[] GetLocalPackDescs(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = File.ReadAllText(cache + "Category/VitaCategory" + Category + ".txt");
                        break;
                    case (false):
                        cat = File.ReadAllText(cache + "Category/Category" + Category + ".txt");
                        break;
                }
            }
            catch
            {
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }


        public string GetLocalPackName(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = File.ReadAllText(cache + "descs/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = File.ReadAllText(cache + "descs/" + Category + ".desc");
                        break;
                }
            }
            catch (Exception err)
            {
            }
            string[] data = cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return data[0];
        }


        public string[] GetLocalPackData(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = File.ReadAllText(cache + "descs/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = File.ReadAllText(cache + "descs/" + Category + ".desc");
                        break;
                }
            }
            catch
            {
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }


        public Image GetLocalPackImage(string Category, bool IsVita)
        {
            Image image = null;
            try
            {
                switch (IsVita)
                {
                    case (true):
                        image = Image.FromFile(cache + "images/Vita/" + Category + ".png");
                        break;
                    case (false):
                        image = Image.FromFile(cache + "images/" + Category + ".png");
                        break;
                }
            }
            catch
            {
            }
            return image;
        }
    }
}
