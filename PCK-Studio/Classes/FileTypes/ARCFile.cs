using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    // filepath to file data
    public class ConsoleArchive : Dictionary<string, byte[]>
    {
        public int SizeOfFile(string filepath) => this[filepath].Length;
    }
}