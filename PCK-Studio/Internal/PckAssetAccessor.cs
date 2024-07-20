using OMI.Formats.Pck;

namespace PckStudio.Internal
{
    internal class PckAssetAccessor
    {
        internal readonly PckAssetAccessPermission Permission;

        private readonly PckAsset Asset;

        internal PckAssetAccessor(PckAssetAccessPermission permission, PckAsset asset)
        {
            Permission = permission;
            Asset = asset;
        }
    }
}
