using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using PckStudio.Extensions;

namespace PckStudio.Internal
{
    internal sealed class PckAccessor
    {

        internal string[] AllowedDirectories { get; }
        internal PckAssetType[] AllowedAssetTypes { get; }
        internal PckAssetAccessPermission Permissions { get; }

        private readonly PckFile _pckFile;
        public PckAccessor(PckFile pckFile, string[] allowedDirectories, PckAssetType[] allowedAssetTypes, PckAssetAccessPermission permissions)
        {
            _pckFile = pckFile;
            AllowedDirectories = allowedDirectories;
            AllowedAssetTypes = allowedAssetTypes;
            Permissions = permissions;
        }

        private bool IsAllowed(string path, PckAssetType type) => IsPathAllowed(path) && IsTypeAllowed(type);

        internal bool TryGetAsset(string assetPath, PckAssetType assetType, out PckAssetAccessor asset)
        {
            if (!IsAllowed(assetPath, assetType))
            {
                throw new UnauthorizedAccessException($"Asset: {assetPath} is not inside the allowed accessor bounds");
            }
            bool success = _pckFile.TryGetAsset(assetPath, assetType, out PckAsset pckAsset);
            asset = new PckAssetAccessor(Permissions, pckAsset);
            return success;
        }

        internal PckAssetAccessor GetOrCreate(string assetPath, PckAssetType assetType)
        {
            if (!IsAllowed(assetPath, assetType))
            {
                throw new UnauthorizedAccessException($"Asset: {assetPath} is not inside the allowed accessor bounds");
            }
            return new PckAssetAccessor(Permissions, _pckFile.GetOrCreate(assetPath, assetType));
        }

        private bool IsPathAllowed(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            if (AllowedDirectories.ContainsAny(".", "/"))
                return true;
            foreach (string dir in AllowedDirectories)
            {
                if (path.StartsWith(dir))
                    return true;
            }
            return false;
        }

        private bool IsTypeAllowed(PckAssetType type)
        {
            if (!Enum.IsDefined(typeof(PckAssetType), type))
                return false;
            foreach (PckAssetType allowedType in AllowedAssetTypes)
            {
                if (type == allowedType)
                    return true;
            }
            return false;
        }
    }
}
