using System;
using System.IO;
using PckStudio.Core;

namespace PckStudio.Interfaces
{
    public interface IModelImportProvider<T> where T : class
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
