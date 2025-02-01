using System;

namespace PckStudio.Interfaces
{
    public interface ISaveContext<T>
    {
        public bool AutoSave { get; }

        public void Save(T value);
    }

    public sealed class DelegatedSaveContext<T> : ISaveContext<T>
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