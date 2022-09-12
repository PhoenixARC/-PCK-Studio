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
        static string cache = Program.AppDataCache + "/packs/";

        public PCKCenterJSON CenterPacks;
        public LocalActions LocalAction = new LocalActions();        
        
        public string[] GetLocalCategories(bool isVita)
        {
            try
            {
                List<string> Cats = new List<string>();
                foreach (string file in Directory.GetFiles(cache + (isVita ? "vita/" : "normal/")))
                {
                    if (Path.GetExtension(file) == ".json")
                        Cats.Add(Path.GetFileNameWithoutExtension(file));
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
            string filepath = cache + (IsVita ? "vita/" : "normal/") + Category + ".json";
            if (!File.Exists(filepath))
            {
                return new PCKCenterJSON();
            }
            string StringData = File.ReadAllText(filepath);
            return JsonConvert.DeserializeObject<PCKCenterJSON>(StringData);
        }


        public string GetLocalPackName(string Category, bool IsVita)
        {
            return GetLocalPackData(Category, IsVita)[0];
        }


        public string[] GetLocalPackData(string Category, bool IsVita)
        {
            string cat = "";
            string filepath = cache + (IsVita ? "descs/Vita/" : "descs/") + Category + ".desc";
            if (File.Exists(filepath))
            {
                cat = File.ReadAllText(filepath);
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }


        public Image GetLocalPackImage(int packID, bool IsVita)
        {
            string filepath = cache + (IsVita ? "vita/images/" : "normal/images/") + packID + ".png";
            if (File.Exists(filepath))
            {
                return Image.FromFile(filepath);
            }
            return null; // TODO: add default Pack Image ?
        }
    }
}
