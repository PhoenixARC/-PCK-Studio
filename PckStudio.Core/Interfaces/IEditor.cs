using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    public interface IEditor<T> where T : class
    {
        T EditorValue { get; }

        ISaveContext<T> SaveContext { get; }

        void Save();

        void SaveAs();

        void Close();

        void UpdateView();
    }
}