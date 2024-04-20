using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    internal interface IPckSerializer<T>
    {
        public void Serialize(T obj, ref PckFileData file);
    }
}
