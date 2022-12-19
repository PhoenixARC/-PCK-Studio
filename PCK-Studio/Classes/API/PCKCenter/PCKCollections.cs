using System;
using System.IO;
using System.Net;
using System.Drawing;
using Newtonsoft.Json;
using PckStudio.API.PCKCenter.model;

namespace PckStudio.API.PCKCenter
{
    public class PCKCollections
    {
        WebClient client = new WebClient();
        public string CurrentPackDl = "";
        string cache = Program.AppDataCache + "/packs";
        public PCKCenterJSON CenterPacks;
        public LocalActions LocalAction = new LocalActions();

        public string[] GetCategories() => GetCategories(false);

        public string[] GetCategories(bool isVita)
        {
            try
            {
                return DownloadAndDeserializeJson<string[]>
                    ($"{Program.BaseAPIUrl}/center/packs/{(isVita ? "VitaCategiories.json" : "Categiories.json")}", client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Array.Empty<string>();
        }

        public PCKCenterJSON GetPackDescs(string category, bool isVita)
        {
            string cat;
            try
            {
                cat = client.DownloadString($"{Program.BaseAPIUrl}/center/packs/{(isVita ? "vita" : "normal")}/{category}.json");
            }
            catch
            {
                cat = client.DownloadString($"{Program.BackUpAPIUrl}/center/packs/{(isVita ? "vita" : "normal")}/{category}.json");
            }
            PCKCenterJSON Data = JsonConvert.DeserializeObject<PCKCenterJSON>(cat);
            return Data;
        }

        [Obsolete]
        public string[] GetPackDescription(string category, bool isVita)
        {
            return Array.Empty<string>();
            string cat;
            try
            {
                cat = client.DownloadString($"{Program.BaseAPIUrl}/studio/PCK/api/pcks{(isVita ? "/Vita" : "")}/{category}.desc");
            }
            catch
            {
                cat = client.DownloadString($"{Program.BackUpAPIUrl}/studio/PCK/api/pcks{(isVita ? "/Vita" : "")}/{category}.desc");
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Image GetPackImage(int packId, bool isVita)
        {
            byte[] data;
            try
            {
                data = client.DownloadData($"{Program.BaseAPIUrl}/center/packs/{(isVita ? "vita" : "normal")}/images/{packId}.png");
            }
            catch
            {
                data = client.DownloadData($"{Program.BackUpAPIUrl}/center/packs/{(isVita ? "vita" : "normal")}/images/{packId}.png");
            }
            Image image;
            using (MemoryStream fs = new MemoryStream(data))
            {
                image = Image.FromStream(fs);
            }
            return image;
        }

        public bool TryDownloadPack(string category, int packId, bool isVita)
        {
            try
            {
                Image packImage = GetPackImage(packId, isVita);
                packImage.Save($"{cache}/{(isVita ? "vita" : "normal")}/images/" + packId + ".png");
                packImage.Dispose();

                Directory.CreateDirectory(cache + "/normal/");
                Directory.CreateDirectory(cache + "/normal/images");
                Directory.CreateDirectory(cache + "/normal/pcks");
                Directory.CreateDirectory(cache + "/vita/");
                Directory.CreateDirectory(cache + "/vita/images");
                Directory.CreateDirectory(cache + "/vita/pcks");
                PCKCenterJSON Local = LocalAction.GetLocalJSON(category, isVita);
                client.DownloadFile($"{Program.BaseAPIUrl}/center/packs/{(isVita ? "vita" : "normal")}/pcks/{packId}.pck", $"{cache}/vita/pcks/{packId}.pck");
                
                Local = LocalAction.AddPack(Local, CenterPacks.Data[packId.ToString()], packId);
                LocalAction.SaveLocalJSON(Local, category, isVita);
                LocalAction.SaveLocalCategories(isVita);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        T DownloadAndDeserializeJson<T>(string address, WebClient client = null)
        {
            WebClient wc = client ?? new WebClient();
            string jsonData = wc.DownloadString(address);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        PCKCenterJSON DownloadCategoryJson(string categorie, bool isVita, WebClient client = null)
        {
            return DownloadAndDeserializeJson<PCKCenterJSON>
                ($"{Program.BaseAPIUrl}/center/packs/{(isVita ? "vita" :"normal")}/{categorie}.json", client);
        }
        
        public void TryTestPackInfo(bool isVita)
        {
            try
            {
                WebClient client = new WebClient();
                string categoryJSON = client.DownloadString(Program.BaseAPIUrl + "/center/packs/Categiories.json");
                string[] categories = JsonConvert.DeserializeObject<string[]>(categoryJSON);
                PCKCenterJSON result = DownloadCategoryJson(categories[2], false, client);
            }
            catch
            {

            }
        }
    }
}
