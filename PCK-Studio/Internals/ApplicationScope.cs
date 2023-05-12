using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Classes.Misc;

namespace PckStudio
{
    internal static class ApplicationScope
    {
        public static FileCacher AppDataCacher = new FileCacher(Program.AppDataCache);

    }
}
