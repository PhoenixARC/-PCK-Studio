using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.ARC
{
    internal class ARCFileReader : StreamDataReader
    {


        private ARCFileReader() : base(true)
        { }

        public ConsoleArchive Parse(byte[] data, ConsoleArchive source)
        {
            return Parse(new MemoryStream(data), source);
        }
        public ConsoleArchive Parse(string Filepath, ConsoleArchive source)
        {
            return Parse(new MemoryStream(File.ReadAllBytes(Filepath)), source);
        }

        public ConsoleArchive Parse(MemoryStream s, ConsoleArchive archive)
        {
            List<ConsoleArchiveItem> items = new List<ConsoleArchiveItem>();
            Encoding encoding = Encoding.UTF8;
            int NumberOfFiles = ReadInt(s);


            for(int i = 0; i < NumberOfFiles; i++)
            {
                string name = ReadString(s);
                int pos = ReadInt(s);
                int size = ReadInt(s);

                ConsoleArchiveItem citem = new ConsoleArchiveItem(name, size, pos);
                items.Add(citem);
                

            }

            foreach(ConsoleArchiveItem citem in items)
            {
                if (!archive.Files.ContainsKey(citem.Name))
                    archive.Files.Add(citem.Name, ReadBytesFromPosition(s, citem.Size, citem.Position));
                else
                {
                    System.Diagnostics.Debug.WriteLine("Copy of: " + citem.Name + " | size:" + citem.Size + " | position:" + citem.Position);
                }
                s.Flush();
            }
            items.Clear();
            s.Close();
            s.Dispose();
            
            return archive;
        }
        private string ReadString(Stream stream)
        {
            int length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }

        private byte[] ReadBytesFromPosition(Stream s, int length, int position)
        {
            long originalPOS = s.Position;
            s.Position = position;
            byte[] ByteArray = ReadBytes(s, length);
            s.Position = originalPOS;
            return ByteArray;
        }

    }
}
