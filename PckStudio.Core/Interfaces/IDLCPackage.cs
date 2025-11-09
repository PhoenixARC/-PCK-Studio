namespace PckStudio.Core.Interfaces
{
    public interface IDLCPackage
    {
        string Name { get; }
        string Description { get; }
        int Identifier { get; }

        bool IsRootPackage { get; }

        IDLCPackageLocationInfo PackageInfo { get; }

        DLCPackageType GetDLCPackageType();

        IDLCPackage ParentPackage { get; }
    }
}
