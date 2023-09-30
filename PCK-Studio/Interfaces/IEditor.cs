using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    internal interface IEditor<T> where T : class
    {
        T Value { get; }

        bool Open(string filepath, OMI.Endianness endianness);

        bool Open(T value);

        void Save();

        void SaveAs();
        
        void SaveTo(string filepath);

        void Close();

        void UpdateView();
    }
}