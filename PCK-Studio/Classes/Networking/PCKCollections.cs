using System;
using System.IO;
using System.Net;
using System.Drawing;
using Newtonsoft.Json;
using PckStudio.API.PCKCenter.model;
using PckStudio.API.PCKCenter;

namespace PckStudio.Classes.Networking
{
    public class PCKCollections
    {
        WebClient client = new WebClient();
        public string CurrentPackDl = "";
        string cache = Program.AppDataCache + "/packs/";
        public PCKCenterJSON CenterPacks;
        public LocalActions LocalAction = new LocalActions();
        public string[] GetCategories()
        {
            string cat = "";
            try
            {
                cat = client.DownloadString(Program.BaseAPIUrl + "/center/packs/Categiories.json");
            }
            catch
            {
                cat = client.DownloadString(Program.BaseAPIUrl + "/center/packs/VitaCategiories.json");
            }
            return JsonConvert.DeserializeObject<string[]>(cat);
        }

        public PCKCenterJSON GetPackDescs(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(Program.BaseAPIUrl + "/center/packs/vita/" + Category + ".json");
                        break;
                    case (false):
                        cat = client.DownloadString(Program.BaseAPIUrl + "/center/packs/normal/" + Category + ".json");
                        break;
                }
            }
            catch
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(Program.BackUpAPIUrl + "/center/packs/vita/" + Category + ".json");
                        break;
                    case (false):
                        cat = client.DownloadString(Program.BackUpAPIUrl + "/center/packs/normal/" + Category + ".json");
                        break;
                }
            }

            PCKCenterJSON Data = JsonConvert.DeserializeObject<PCKCenterJSON>(cat);
            return Data;
        }
        public string[] GetPackData(string Category, bool IsVita)
        {
            string cat = "";
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(Network.MainURL + "/studio/PCK/api/pcks/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = client.DownloadString(Network.MainURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                        break;
                }
            }
            catch
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadString(Network.BackUpURL + "/studio/PCK/api/pcks/Vita/" + Category + ".desc");
                        break;
                    case (false):
                        cat = client.DownloadString(Network.BackUpURL + "/studio/PCK/api/pcks/" + Category + ".desc");
                        break;
                }
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Image GetPackImage(int packID, bool IsVita)
        {
            byte[] cat = new byte[] { };
            try
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadData(Program.BaseAPIUrl + "/center/packs/vita/images/" + packID + ".png");
                        break;
                    case (false):
                        cat = client.DownloadData(Program.BaseAPIUrl + "/center/packs/normal/images/" + packID + ".png");
                        break;
                }
            }
            catch
            {
                switch (IsVita)
                {
                    case (true):
                        cat = client.DownloadData(Program.BackUpAPIUrl + "/center/packs/vita/images/" + packID + ".png");
                        break;
                    case (false):
                        cat = client.DownloadData(Program.BackUpAPIUrl + "/center/packs/normal/images/" + packID + ".png");
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

        public bool TryDownloadPack(int packID, bool IsVita, string Category)
        {
            try
            {

                Image image = GetPackImage(packID, IsVita);
                string DescPath = cache;
                Directory.CreateDirectory(cache + "normal/");
                Directory.CreateDirectory(cache + "normal/images");
                Directory.CreateDirectory(cache + "normal/pcks");
                Directory.CreateDirectory(cache + "vita/");
                Directory.CreateDirectory(cache + "vita/images");
                Directory.CreateDirectory(cache + "vita/pcks");
                PCKCenterJSON Local = LocalAction.GetLocalJSON(Category, IsVita);
                switch (IsVita)
                {
                    case (false):
                        image.Save(cache + "normal/images/" + packID + ".png"); 
                        client.DownloadFile(Program.BaseAPIUrl + "/center/packs/normal/pcks/" + packID + ".pck", cache + "normal/pcks/" + packID + ".pck");
                        break;
                    case (true):
                        image.Save(cache + "vita/images/" + packID + ".png");
                        client.DownloadFile(Program.BaseAPIUrl + "/center/packs/vita/pcks/" + packID + ".pck", cache + "vita/pcks/" + packID + ".pck");
                        break;
                }
                Local = LocalAction.AddPack(Local, CenterPacks.Data[packID.ToString()], packID);
                LocalAction.SaveLocalJSON(Local, Category, IsVita);
                LocalAction.SaveLocalCategories(IsVita);
                /**/
                image.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void TryTestPackInfo(bool IsVita)
        {
            try
            {
                WebClient wc = new WebClient();
                string CategoryJSON = wc.DownloadString(Program.BaseAPIUrl + "/center/packs/Categiories.json");
                string[] Categories = JsonConvert.DeserializeObject<string[]>(CategoryJSON);
                PCKCenterJSON Result = pk1(Categories[2]);
                Console.Write(""); // this is a breakpoint

            }
            catch
            {
                Console.Write(""); // this is a breakpoint
            }
        }
        PCKCenterJSON pk1(string categorie)
        {
            WebClient wc = new WebClient();
            string DataJSON = wc.DownloadString(Program.BaseAPIUrl + "/center/packs/normal/" + categorie + ".json");
            PCKCenterJSON Data = JsonConvert.DeserializeObject<PCKCenterJSON>(DataJSON);
            return Data;
        }
    }
}
