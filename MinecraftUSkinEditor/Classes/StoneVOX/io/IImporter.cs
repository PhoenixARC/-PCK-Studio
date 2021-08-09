namespace stonevox
{
    public interface IImporter
    {
        string extension { get; }

        QbModel read(string path);
    }
}