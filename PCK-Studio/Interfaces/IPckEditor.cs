using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    internal interface IPckEditor
    {
        PckFile Pck { get; }

        bool Open(string filepath, OMI.Endianness endianness);

        bool Open(PckFile pck);

        void Save();

        void SaveAs();
        
        void SaveTo(string filepath);

        void Close();

        void UpdateView();
    }
}