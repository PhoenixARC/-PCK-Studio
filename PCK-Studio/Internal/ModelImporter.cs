using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Interfaces;

namespace PckStudio.Internal
{
    internal abstract class ModelImporter<T> where T : class
    {
        private Dictionary<string, IModelImportProvider<T>> _importProviders = new Dictionary<string, IModelImportProvider<T>>();

        private sealed class InternalImportProvider : IModelImportProvider<T>
        {
            public string Name => nameof(InternalImportProvider);

            public FileDialogFilter DialogFilter => _dialogFilter;

            public bool SupportImport => _import != null;

            public bool SupportExport => _export != null;

            private FileDialogFilter _dialogFilter;
            private Func<string, T> _import;
            private Action<string, T> _export;

            public InternalImportProvider(FileDialogFilter dialogFilter, Func<string, T> import, Action<string, T> export)
            {
                _dialogFilter = dialogFilter;
                _import = import;
                _export = export;
            }

            public void Export(string filename, T model)
            {
                _ = _export ?? throw new NotImplementedException();
                _export(filename, model);
            }

            public T Import(string filename)
            {
                _ = _import ?? throw new NotImplementedException();
                return _import(filename);
            }

            public void Export(ref Stream stream, T model)
            {
                throw new NotImplementedException();
            }

            public T Import(Stream stream)
            {
                throw new NotImplementedException();
            }
        }

        public string SupportedModelFileFormatsFilter => string.Join("|", _importProviders.Values.Select(p => p.DialogFilter));

        public T Import(string filename)
        {
            if (!HasProvider(filename))
            {
                Trace.TraceWarning($"[{nameof(SkinModelImporter)}:Import] No provider found for '{Path.GetExtension(filename)}'.");
                return default;
            }

            IModelImportProvider<T> provider = GetProvider(filename);
            if (!provider.SupportImport)
            {
                throw new NotSupportedException($"Provider '{provider.Name}' does not support importing.");
            }

            return provider.Import(filename);
        }

        public void Export(string filename, T model)
        {
            if (model is null)
            {
                Trace.TraceError($"[{nameof(SkinModelImporter)}:Export] Model is null.");
                return;
            }

            if (!HasProvider(filename))
            {
                Trace.TraceWarning($"[{nameof(SkinModelImporter)}:Export] No provider found for '{Path.GetExtension(filename)}'.");
                return;
            }

            IModelImportProvider<T> provider = GetProvider(filename);
            if (!provider.SupportExport)
            {
                throw new NotSupportedException($"Provider '{provider.Name}' does not support exporting.");
            }
            provider.Export(filename, model);
        }

        internal bool AddProvider(IModelImportProvider<T> provider)
        {
            if (_importProviders.ContainsKey(provider.DialogFilter.Extension))
                return false;

            _importProviders.Add(provider.DialogFilter.Extension, provider);
            return true;
        }

        protected bool InternalAddProvider(FileDialogFilter dialogFilter, Func<string, T> import, Action<string, T> export)
        {
            return AddProvider(new InternalImportProvider(dialogFilter, import, export));
        }

        /// <summary>
        /// Translates coordinate unit system into our coordinate system
        /// </summary>
        /// <param name="origin">Position/Origin of the Object(Cube).</param>
        /// <param name="size">The Size of the Object(Cube).</param>
        /// <param name="translationUnit">Describes what axises need translation.</param>
        /// <returns>The translated position</returns>
        protected static Vector3 TransformSpace(Vector3 origin, Vector3 size, Vector3 translationUnit)
        {
            // The translation unit describes what axises need to be swapped
            // Example:
            //      translation unit = (1, 0, 0) => This translation unit will ONLY swap the X axis
            translationUnit = Vector3.Clamp(translationUnit, Vector3.Zero, Vector3.One);
            // To better understand see:
            // https://sharplab.io/#v2:C4LgTgrgdgNAJiA1AHwAICYCMBYAUKgBgAJVMA6AOQgFsBTMASwGMBnAbj1QGYT0iBhIgG88RMb3SjxI3OLlEAbgEMwRBlAAOEYEQC8RKLQDuRAGq0mwAPZguACkwwijogQCUHWfLHLVtAB4aFsC0cHoGxmbBNvYAtC7xTpgeUt6+RGC0LOEAKmBKUCwAYjbU/FY2cOpKISx26lrAKV7epACcdpkszd5i7Z1ZevoBQZahPeIAvqlEM9wkmABsUZYxRHkFxaXlldW1duartmqa2m4zMr2KKhmD+ofWtmT8ADZK1Br1p8BODzFkAC16FZftEngB5QwTbxdIgAKn06E8V1hsXuYK4ZEhtGRvVQAHYiLEurixNNcJMgA
            Vector3 transformUnit = -((translationUnit * 2) - Vector3.One);

            Vector3 pos = origin;
            // The next line essentialy does uses the fomular below just on all axis.
            // x = -(pos.x + size.x)
            pos *= transformUnit;
            pos -= size * translationUnit;
            return pos;
        }

        private bool HasProvider(string filename)
        {
            string fileExtension = Path.GetExtension(filename);
            return _importProviders.ContainsKey(fileExtension) && _importProviders[fileExtension] is not null;
        }

        private IModelImportProvider<T> GetProvider(string filename)
        {
            string fileExtension = Path.GetExtension(filename);
            return _importProviders.ContainsKey(fileExtension) ? _importProviders[fileExtension] : null;
        }
    }
}
