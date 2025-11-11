using System;
using PckStudio.Interfaces;

namespace PckStudio.Core
{
    public class DelegatedSaveContext<T> : ISaveContext<T>
    {
        private readonly Action<T> _saveAction;
        public bool AutoSave { get; }

        public void Save(T value) => _saveAction(value);

        public DelegatedSaveContext(bool autoSave, Action<T> saveAction)
        {
            AutoSave = autoSave;
            _saveAction = saveAction;
        }
    }
}