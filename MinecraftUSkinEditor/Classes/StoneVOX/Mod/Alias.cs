using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonevox.Mod
{
    public class Alias
    {
        public string name;
        public FileReference file;
    }

    public class FunctionAlias
    {
        public enum endpoint
        {
            client,
            server
        }

        public string name;
        public FileReference controller;
        public endpoint enpoint;
    }
}
