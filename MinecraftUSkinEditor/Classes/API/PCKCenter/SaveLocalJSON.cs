using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.PCKCenter.model;

namespace API.PCKCenter
{
    public class LocalActions
    {
        string cache = PckStudio.Program.Appdata + "cache/packs/";
        public bool SaveLocalJSON(PCKCenterJSON JSONData, string category, bool isVita)
        {
            try
            {
                string outputString = JsonConvert.SerializeObject(JSONData, Formatting.Indented);

                switch (isVita)
                {
                    case false:
                        File.WriteAllText(cache + "normal/" + category + ".json", outputString);
                        break;
                    case true:
                        File.WriteAllText(cache + "vita/" + category + ".json", outputString);
                        break;
                }

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
                string JSONData = "";
                switch (isVita)
                {
                    case false:
                        JSONData = File.ReadAllText(cache + "normal/" + category + ".json");
                        break;
                    case true:
                        JSONData = File.ReadAllText(cache + "vita/" + category + ".json");
                        break;
                }
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
        public PCKCenterJSON AddPack(PCKCenterJSON JSONData,EntryInfo EInfo, int PackID)
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
                switch (isVita)
                {
                    case false:
                        foreach(string file in Directory.GetFiles(cache + "normal/"))
                        {
                            if (Path.GetExtension(file) == ".json")
                                Cats.Add(Path.GetFileNameWithoutExtension(file));
                        }
                        StringData = JsonConvert.SerializeObject(Cats.ToArray(), Formatting.Indented);
                        File.WriteAllText(cache + "Categiories.json", StringData);
                        break;
                    case true:
                        foreach (string file in Directory.GetFiles(cache + "vita/"))
                        {
                            if (Path.GetExtension(file) == ".json")
                                Cats.Add(Path.GetFileNameWithoutExtension(file));
                        }
                        StringData = JsonConvert.SerializeObject(Cats.ToArray(), Formatting.Indented);
                        File.WriteAllText(cache + "VitaCategiories.json", StringData);
                        break;
                }
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
