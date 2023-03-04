using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class MaterialsFile
    {
        public List<MaterialEntry> entries { get; } = new List<MaterialEntry>();
        public struct MaterialEntry
        {
            public string name;
            public string material_type;
            public MaterialEntry(string name, string material_type) : this()
            {
                this.name = name;
                this.material_type = material_type;
            }
        }
    }
}
