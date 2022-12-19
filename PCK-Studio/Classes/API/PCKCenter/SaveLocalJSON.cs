using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PckStudio.API.PCKCenter.model;

namespace PckStudio.API.PCKCenter
{
    public class LocalActions
    {
        string cache = Program.AppDataCache + "/packs/";
        public bool SaveLocalJSON(PCKCenterJSON JSONData, string category, bool isVita)
        {
            try
            {
                string outputString = JsonConvert.SerializeObject(JSONData, Formatting.Indented);
                File.WriteAllText(cache + (isVita ? "vita/" : "normal/") + category + ".json", outputString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public PCKCenterJSON GetLocalJSON(string category, bool isVita)
        {
            try
            {
                string JSONData = File.ReadAllText(cache + (isVita ? "vita/" : "normal/") + category + ".json");
                return JsonConvert.DeserializeObject<PCKCenterJSON>(JSONData);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                PCKCenterJSON JSONData = new PCKCenterJSON();
                JSONData.Data = new Dictionary<string, EntryInfo>();
                return JSONData;
            }
        }
        public PCKCenterJSON AddPack(PCKCenterJSON JSONData, EntryInfo EInfo, int PackID)
        {
            JSONData.Data.Add(PackID.ToString(), EInfo);
            return JSONData;
        }
        public PCKCenterJSON Removepack(PCKCenterJSON JSONData, int PackID)
        {
            JSONData.Data.Remove(PackID.ToString());
            return JSONData;
        }

        public bool SaveLocalCategories(bool isVita)
        {
            try
            {
                List<string> Cats = new List<string>();
                string StringData = "";
                foreach(string file in Directory.GetFiles(cache + (isVita ? "vita/" : "normal/")))
                {
                    if (Path.GetExtension(file) == ".json")
                        Cats.Add(Path.GetFileNameWithoutExtension(file));
                }
                StringData = JsonConvert.SerializeObject(Cats.ToArray(), Formatting.Indented);
                File.WriteAllText(cache + (isVita ? "VitaCategiories.json" : "Categiories.json"), StringData);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
