using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.Materials
{
    public class MaterialsReader : StreamDataReader<MaterialsFile>
    {
        public static MaterialsFile Read(Stream stream)
        {
            return new MaterialsReader().ReadFromStream(stream);
        }

        protected MaterialsReader() : base(false) // Doesn't seem that Behaviours uses little endian
        {
        }

        protected override MaterialsFile ReadFromStream(Stream stream)
        {
            MaterialsFile materialsFile = new MaterialsFile();
            _ = ReadInt(stream);
            int entryCount = ReadInt(stream);
            for (int i = 0; i < entryCount; i++)
            {
                string name = ReadString(stream);
                string material_type = ReadString(stream);
                materialsFile.entries.Add(new MaterialsFile.MaterialEntry(name, material_type));
            }
            return materialsFile;
        }

        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.ASCII);
        }
    }
}
