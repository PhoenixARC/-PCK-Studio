using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing;

namespace PckStudio.Classes.Networking
{
    public class PCKCollections
    {
        WebClient client = new WebClient();
        public string CurrentPackDl = "";
        string cache = Program.Appdata + "cache/packs/";
        public string[] GetCategories()
        {
            string cat = "";
            try
            {
                cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/PCKCategories.txt");
                client.DownloadFile(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/PCKCategories.txt", cache + "PCKCategories.txt");
            }
            catch
            {
                cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/PCKCategories.txt");
                client.DownloadFile(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/PCKCategories.txt", cache + "PCKCategories.txt");
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetPackDescs(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                Console.WriteLine(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/Category/Category" + Category + ".txt");
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/Category/VitaCategory" + Category + ".txt");
                        break;
                    case (false):
                        cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/Category/Category" + Category + ".txt");
                        break;
                }
            }
            catch
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/Category/VitaCategory" + Category + ".txt");
                        break;
                    case (false):
                        cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/Category/Category" + Category + ".txt");
                        break;
                }
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string GetPackName(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                Console.WriteLine(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                        break;
                }
            }
            catch(Exception err)
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/pcks/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                        break;
                }
            }
            string[] data = cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return data[0];
        }

        public string[] GetPackData(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                        break;
                }
            }
            catch
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/pcks/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                        break;
                }
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Image GetPackImage(string Category, bool IsVita)
        {
            byte[] cat = new byte[] { };
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadData(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/Vita/image/" + Category + ".png");
                        break;
                    case (false):
                        cat = client.DownloadData(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/pcks/image/" + Category + ".png");
                        break;
                }
            }
            catch
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadData(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/pcks/Vita/image/" + Category + ".png");
                        break;
                    case (false):
                        cat = client.DownloadData(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/pcks/image/" + Category + ".png");
                        break;
                }
            }
            Stream fs = new MemoryStream(cat);
            Image image;
            image = Image.FromStream(fs);
            fs.Flush();
            fs.Dispose();
            return image;
        }

        public bool TryDownloadPack(string Category, bool IsVita, string PackCat)
        {
            try
            {
                string[] desc = GetPackData(Category, IsVita);
                Image image = GetPackImage(Category, IsVita);
                string DescPath = cache;
                Directory.CreateDirectory(cache + "descs/Vita/");
                Directory.CreateDirectory(cache + "images/Vita/");
                Directory.CreateDirectory(cache + "files/Vita/");
                Directory.CreateDirectory(cache + "Category/");

                switch (IsVita)
                {
                    case (true):
                        DescPath = cache + "descs/Vita/" + Category + ".desc";
                        image.Save(cache + "images/Vita/" + Category + ".png");
                        File.WriteAllText(DescPath, desc[0] + "\n" + desc[1] + "\n" + desc[2]);
                        File.WriteAllText(cache + "Category/VitaCategory" + PackCat + ".txt", "\n"+ Category);
                        byte[] bytes = client.DownloadData(desc[3]);
                        File.WriteAllBytes(cache + "files/Vita/" + Category + ".pck", bytes);
                        break;
                    case (false):
                        DescPath = cache + "descs/" + Category + ".desc";
                        image.Save(cache + "images/" + Category + ".png");
                        File.WriteAllText(DescPath, desc[0] + "\n" + desc[1] + "\n" + desc[2]);
                        File.WriteAllText(cache + "Category/Category" + PackCat + ".txt", "\n" + Category);
                        byte[] bytes2 = client.DownloadData(desc[3]);
                        File.WriteAllBytes(cache + "files/" + Category + ".pck", bytes2);
                        break;
                }
                image.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
