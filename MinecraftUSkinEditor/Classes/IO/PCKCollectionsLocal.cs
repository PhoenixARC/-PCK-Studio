using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;
using API.PCKCenter.model;
using API.PCKCenter;

namespace PckStudio.Classes.IO
{
    public  class PCKCollectionsLocal
    {
        string cache = Program.Appdata + "cache/packs/";

        public PCKCenterJSON CenterPacks;
        public LocalActions LocalAction = new LocalActions();        
        
        public string[] GetLocalCategories(bool isVita)
        {
            try
            {
                List<string> Cats = new List<string>();
                switch (isVita)
                {
                    case false:
                        foreach (string file in Directory.GetFiles(cache + "normal/"))
                        {
                            if (Path.GetExtension(file) == ".json")
                                Cats.Add(Path.GetFileNameWithoutExtension(file));
                        }
                        break;
                    case true:
                        foreach (string file in Directory.GetFiles(cache + "vita/"))
                        {
                            if (Path.GetExtension(file) == ".json")
                                Cats.Add(Path.GetFileNameWithoutExtension(file));
                        }
                        break;
                }
                return Cats.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new string[] { };
            }
        }


        public PCKCenterJSON GetLocalPackDescs(string Category, bool IsVita)
        {
            string StringData = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        StringData = File.ReadAllText(cache + "vita/ " + Category + ".json");
                        break;
                    case (false):
                        StringData = File.ReadAllText(cache + "normal/" + Category + ".json");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new PCKCenterJSON();
            }

            PCKCenterJSON Data = JsonConvert.DeserializeObject<PCKCenterJSON>(StringData);
            return Data;
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


        public Image GetLocalPackImage(int packID, bool IsVita)
        {
            Image image = null;
            try
            {
                switch (IsVita)
                {
                    case (true):
                        image = Image.FromFile(cache + "vita/images/" + packID + ".png");
                        break;
                    case (false):
                        image = Image.FromFile(cache + "normal/images/" + packID + ".png");
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
