using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.Sounds
{
    public class Type
    {
        public bool replace { get; set; }
        public List<Sound> sounds = new List<Sound>();
    }

    public class Sound
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool stream { get; set; }
    }
}
