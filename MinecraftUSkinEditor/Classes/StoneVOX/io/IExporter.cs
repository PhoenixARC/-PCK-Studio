namespace stonevox
{
    public interface IExporter
    {
        string extension { get; }

        void write(string path, string name, QbModel model);
    }
}