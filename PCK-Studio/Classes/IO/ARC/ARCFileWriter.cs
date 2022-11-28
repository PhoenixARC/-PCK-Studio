using System.IO;
using System.Linq;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.ARC
{
    internal class ARCFileWriter : StreamDataWriter
    {
        private ConsoleArchive _archive;

        public static void Write(Stream stream, ConsoleArchive archive, bool useLittleEndian = false)
        {
            new ARCFileWriter(archive, useLittleEndian).WriteToStream(stream);
        }

        public ARCFileWriter(ConsoleArchive archive, bool useLittleEndian) : base(useLittleEndian)
        {
            _archive = archive;
        }

        protected override void WriteToStream(Stream stream)
        {
            var arc = _archive.ToArray();
            WriteInt(stream, arc.Length);
            int offset = 4 + arc.Sum(pair => 10 + pair.Key.Length);
            foreach (var pair in arc)
            {
                int size = pair.Value.Length;
                WriteString(stream, pair.Key);
                WriteInt(stream, offset);
                WriteInt(stream, size);
                offset += size;
            }
            foreach (var pair in arc)
            {
                WriteBytes(stream, pair.Value);
            }
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.ASCII);
        }
    }
}
