using System.IO;

namespace PckStudio.Core
{
    public readonly struct FileDialogFilter
    {
        public readonly string Description;
        public readonly string Pattern;

        public string Extension => Path.GetExtension(Pattern);

        public FileDialogFilter(string description, string pattern)
        {
            Description = description;
            Pattern = pattern;
        }

        public override string ToString()
        {
            return $"{Description}|{Pattern}";
        }
    }
}