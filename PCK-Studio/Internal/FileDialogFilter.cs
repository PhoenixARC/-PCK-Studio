namespace PckStudio.Internal
{
    internal struct FileDialogFilter
    {
        public readonly string Description;
        public readonly string Pattern;

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