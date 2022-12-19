using System.Collections.Generic;

namespace PckStudio.Classes.FileTypes
{
    // filepath to file data
    public class ConsoleArchive : Dictionary<string, byte[]>
    {
        public int SizeOfFile(string filepath) => this[filepath].Length;
    }
}