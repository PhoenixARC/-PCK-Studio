using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    internal interface IPckAssetSerializer<T>
    {
        public void Serialize(T obj, ref PckAsset asset);
    }
}
