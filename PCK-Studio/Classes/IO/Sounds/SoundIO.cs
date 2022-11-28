using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Classes.IO.Sounds
{
    public class SoundIO
    {
        public Dictionary<string, SoundInfo> Read(string Filepath)
        {
            var jObj = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(File.ReadAllText(Filepath));
            var dict = JsonConvert.DeserializeObject<Dictionary<string, SoundInfo>>(jObj.ToString());

            return dict;
        }

        public string Serialize(Dictionary<string, SoundInfo> input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }
    }
}
