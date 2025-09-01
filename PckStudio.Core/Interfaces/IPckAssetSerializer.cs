using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    public interface IPckAssetSerializer<T>
    {
        public void Serialize(T obj, ref PckAsset asset);
    }
}
