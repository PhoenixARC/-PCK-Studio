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
        public class InvalidLanguageException : Exception
        {
            private string _language;
            public string Language => _language;
            public InvalidLanguageException(string message, string language) : base(message)
            {
                _language = language;
            }
        }

        public static readonly string[] ValidLanguages = new string[]
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

            "la-LAS",

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

            "zh-CN",
            "zh-HK",
            "zh-SG",
            "zh-TW",
            "zh-CHT",
            "zh-HanS",
            "zh-HanT",
        };

        private Dictionary<string, Dictionary<string, string>> _lockeys = new Dictionary<string, Dictionary<string, string>>();
        private List<string> _languages = new List<string>(ValidLanguages.Length);
        public Dictionary<string, Dictionary<string, string>> LocKeys => _lockeys;
        public List<string> Languages => _languages;

        public void InitializeDefault(string packName)
            => Initialize("en-EN", ("IDS_DISPLAY_NAME", packName));
        public void Initialize(string language, params (string, string)[] locKeyValuePairs)
        {
            AddLanguage(language);
            foreach (var locKeyValue in locKeyValuePairs)
                AddLocKey(locKeyValue.Item1, locKeyValue.Item2);
        }

        public string GetLocEntry(string locKey, string language)
        {
            if (!LocKeys.ContainsKey(locKey))
                throw new KeyNotFoundException("Loc key not found");
            if (!Languages.Contains(language)) throw new KeyNotFoundException("Language Entry not found");
            return GetTranslation(locKey)[language]?? string.Empty;
        }

        private Dictionary<string, string> GetTranslation(string locKey)
        {
            if (!LocKeys.ContainsKey(locKey))
                LocKeys.Add(locKey, new Dictionary<string, string>());
            return LocKeys[locKey];
        }

        public void SetLocEntry(string locKey, string language, string value)
        {
            if (!LocKeys.ContainsKey(locKey))
                LocKeys.Add(locKey, new Dictionary<string, string>());
            if (!Languages.Contains(language))
                throw new KeyNotFoundException(nameof(language));
            GetTranslation(locKey)[language] = value;
        }

        public bool AddSingleLocEntry(string locKey, string language, string value)
        {
            if (string.IsNullOrEmpty(locKey) ||
                string.IsNullOrEmpty(language) ||
                string.IsNullOrEmpty(value) ||
                LocKeys.ContainsKey(locKey))
                return false;
            SetLocEntry(locKey, language, value);
            return true;
        }

        public bool AddLocKey(string locKey, string value)
        {
            if (LocKeys.ContainsKey(locKey))
                return false;
            Languages.ForEach( langauge => AddSingleLocEntry(locKey, langauge, value) );
            return true;
        }
        public void ChangeSingleEntry(string locKey, string language, string newValue) 
            => SetLocEntry(locKey, language, newValue);

        public void ChangeEntry(string locKey, string newValue)
            => Languages.ForEach(langauge => SetLocEntry(locKey, langauge, newValue));

        public void RemoveLocKey(string locKey)
        {
            if (!LocKeys.ContainsKey(locKey))
                throw new KeyNotFoundException(nameof(locKey));
            LocKeys.Remove(locKey);
        }

        public void AddLanguage(string language)
        {
            if (!ValidLanguages.Contains(language))
                throw new InvalidLanguageException("Invalid language", language);
            if (Languages.Contains(language))
                throw new InvalidLanguageException("Language already exists", language);
            Languages.Add(language);
            foreach(var key in LocKeys.Keys)
                SetLocEntry(key, language, "");
        }

        public void RemoveLanguage(string language)
        {
            if (!ValidLanguages.Contains(language))
                throw new InvalidLanguageException("Invalid language", language);
            if (!Languages.Contains(language))
                throw new InvalidLanguageException("Language doesn't exist", language);
            if (Languages.Remove(language))
                foreach (var translation in LocKeys.Values)
                    translation.Remove(language);
        }
    }
}