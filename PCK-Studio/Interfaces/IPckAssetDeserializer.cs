using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    internal interface IPckAssetDeserializer<T>
    {
        public T Deserialize(PckAsset asset);
    }
}