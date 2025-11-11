namespace PckStudio.Interfaces
{
    public interface ISaveContext<T>
    {
        public bool AutoSave { get; }

        public void Save(T value);
    }
}