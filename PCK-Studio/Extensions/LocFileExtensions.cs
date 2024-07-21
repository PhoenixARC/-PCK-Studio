using OMI.Formats.Languages;

namespace PckStudio.Extensions
{
    internal static class LocFileExtensions
    {

        public static void InitializeDefault(this LOCFile locFile, string packName) => locFile.Initialize("en-EN", ("IDS_DISPLAY_NAME", packName));
        
        public static void Initialize(this LOCFile locFile, string language, params (string, string)[] locKeyValuePairs)
        {
            locFile.AddLanguage(language);
            foreach ((string, string) locKeyValue in locKeyValuePairs)
                locFile.AddLocKey(locKeyValue.Item1, locKeyValue.Item2);
        }
    }
}
