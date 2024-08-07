using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Internal.Skin;
using PckStudio.Internal;

namespace PckStudio.Interfaces
{
    internal interface IModelImportProvider<T> where T : class
    {
        public string Name { get; }

        public FileDialogFilter DialogFilter { get; }

        public bool SupportImport { get; }
        public bool SupportExport { get; }

        public T Import(string filename);
        public T Import(Stream stream);

        public void Export(string filename, T model);
        public void Export(ref Stream stream, T model);
    }
}
