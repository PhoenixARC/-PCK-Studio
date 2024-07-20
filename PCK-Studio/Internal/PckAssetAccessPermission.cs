using System;

namespace PckStudio.Internal
{
    [Flags]
    internal enum PckAssetAccessPermission
    {
        ReadAsset        = 0x01,
        WriteAsset       = 0x02,
        DeleteAsset      = 0x04,
        ModifyAsset      = 0x08,
        ReadProperties   = 0x10,
        WriteProperties  = 0x20,
        DeleteProperties = 0x40,
        ModifyProperties = 0x80,
    }
}
