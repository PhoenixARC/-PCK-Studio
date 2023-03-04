using PckStudio.Classes.FileTypes;
using System;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.Materials
{
    internal class MaterialsWriter : StreamDataWriter
    {
        private MaterialsFile materialsFile;

        public static void Write(Stream stream, MaterialsFile file)
        {
            new MaterialsWriter(file).WriteToStream(stream);
        }

        public MaterialsWriter(MaterialsFile file) : base(false)
        {
            materialsFile = file;
        }

        protected override void WriteToStream(Stream stream)
        {
            WriteInt(stream, 0);
            WriteInt(stream, materialsFile.entries.Count);
            foreach (var entry in materialsFile.entries)
            {
                WriteString(stream, entry.name);
                WriteString(stream, entry.material_type);
            }
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.ASCII);
        }
    }
}