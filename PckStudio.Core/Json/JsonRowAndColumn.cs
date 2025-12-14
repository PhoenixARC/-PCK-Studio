using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Core.Json
{
    internal class JsonRowAndColumn
    {
        [JsonProperty("row", Required = Required.Always)]
        public int Row { get; set; }
        [JsonProperty("column", Required = Required.Always)]
        public int Column { get; set; }
    }
}
