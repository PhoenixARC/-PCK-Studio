using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class LOCFile
    {
        public Dictionary<string, Dictionary<string, string>> languages { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public void AddEntry(string key, string value)
        {
            foreach (var language in languages.Values)
            {
                language.Add(key, value);
            }
        }

        public void ChangeEntry(string keyId, string newValue)
        {
            foreach (var language in languages.Values)
            {
                if (!language.ContainsKey(keyId)) throw new Exception("Key not found");
                language[keyId] = newValue;
            }
        }

        public void ChangeSingleEntry(string language, string keyId, string newValue)
        {
            if (!languages.ContainsKey(language)) throw new Exception("Key not found");
            if (!languages[language].ContainsKey(keyId)) throw new Exception("Key Id not found");
            languages[language][keyId] = newValue;
        }

        public void RemoveEntry(string keyId)
        {
            foreach (var language in languages.Values)
            {
                if (language.ContainsKey(keyId))
                    language.Remove(keyId);
            }
        }
    }
}
