using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.PCKCenter.model
{
    public class PCKCenterJSON
    {
        public Dictionary<string, EntryInfo> Data { get; set; }
    }

    public class EntryInfo
    {
        public string Name;
        public string Author;
        public string Description;
    }
}
