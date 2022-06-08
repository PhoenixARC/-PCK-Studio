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
        // loc key => language, name
        public Dictionary<string, Dictionary<string, string>> keys { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public List<string> _languages = new List<string>(languages.Length);

        public void InitializeDefault(string PackName)
        {
            AddLanguage("en-EN");
            AddLocKey("IDS_DISPLAY_NAME", PackName);
        }

        public static readonly string[] languages = new string[]
        {
            "cs-CS",
            "cs-CZ",

            "da-CH",
            "da-DA",
            "da-DK",

            "de-AT",
            "de-DE",

            "el-EL",
            "el-GR",

            "en-AU",
            "en-CA",
            "en-EN",
            "en-GB",
            "en-GR",
            "en-IE",
            "en-NZ",
            "en-US",

            "es-ES",
            "es-MX",

            "fi-BE",
            "fi-CH",
            "fi-FI",

            "fr-FR",
            "fr-CA",

            "it-IT",

            "ja-JP",

            "ko-KR",

            "no-NO",

            "nb-NO",

            "nl-NL",
            "nl-BE",

            "pl-PL",

            "pt-BR",
            "pt-PT",

            "ru-RU",

            "sk-SK",

            "sv-SE",

            "tr-TR",

            "la-LAS",

            "zh-CN",
            "zh-HK",
            "zh-SG",
            "zh-TW",
            "zh-CHT",
            "zh-HanS",
            "zh-HanT",
        };

        public void AddSingleLocKey(string locKey, string language, string value)
        {
            if (keys.ContainsKey(locKey)) throw new Exception("Loc key already exists");
            var dict = new Dictionary<string, string>();
            dict.Add(language, value);
            keys.Add(locKey, dict);
        }

        public void AddLocKey(string locKey, string value)
        {
            if (string.IsNullOrEmpty(locKey) || string.IsNullOrEmpty(value))
                throw new ArgumentNullException("string cant be null");
            if (keys.ContainsKey(locKey))
                throw new Exception("loc key already exists");
            foreach (var langauge in _languages)
            {
                AddSingleLocKey(locKey, langauge, value);
            }
        }
        public void ChangeSingleEntry(string locKey, string language, string newValue)
        {
            if (!keys.ContainsKey(locKey)) throw new KeyNotFoundException("Loc key not found");
            if (!keys[locKey].ContainsKey(language)) throw new KeyNotFoundException("Language Entry not found");
            keys[locKey][language] = newValue;
        }

        public void ChangeEntry(string locKey, string newValue)
        {
            if (string.IsNullOrEmpty(locKey) || string.IsNullOrEmpty(newValue))
                throw new ArgumentNullException("string cant be null");
            if (!keys.ContainsKey(locKey))
                throw new KeyNotFoundException("loc key not found");
            foreach (var langauge in _languages)
            {
                ChangeSingleEntry(locKey, langauge, newValue);
            }
        }


        public void RemoveEntry(string locKey)
        {
            if (!keys.ContainsKey(locKey)) throw new KeyNotFoundException("Loc key not found");
            keys.Remove(locKey);
        }

        private void AddLanguage(string language)
        {
            _languages.Add(language);
        }
    }
}