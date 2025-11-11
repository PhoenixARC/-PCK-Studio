using OMI.Formats.Pck;

namespace PckStudio.Interfaces
{
    public interface IPckAssetDeserializer<T>
    {
        public T Deserialize(PckAsset asset);
    }
}