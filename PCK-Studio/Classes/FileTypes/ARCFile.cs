using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public struct ConsoleArchive
    {
        public ConsoleArchive()
        {
        }

        public Dictionary<string, byte[]> Files = new Dictionary<string, byte[]>();
    }
    public class ConsoleArchiveItem
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Position { get; set; }

        public ConsoleArchiveItem(string name, int size, int position)
        {
            Name = name;
            Size = size;
            Position = position;
        }
    }

    public class ConsoleArchiveActions
    {


        public ConsoleArchive AddItem(ConsoleArchive archive, string ItemName, byte[] data)
        {
            archive.Files.Add(ItemName, data);
            return archive;
        }
        public ConsoleArchive RemoveItem(ConsoleArchive archive, string ItemName)
        {
            if(archive.Files.ContainsKey(ItemName))
                archive.Files.Remove(ItemName);
            return archive;
        }
        public ConsoleArchive EditItem(ConsoleArchive archive, string ItemName, byte[] data)
        {
            archive.Files[ItemName] = data;
            return archive;
        }
        public ConsoleArchive Clear(ConsoleArchive archive)
        {
            archive.Files = null;
            archive = new ConsoleArchive();
            return archive;
        }


    }

}
