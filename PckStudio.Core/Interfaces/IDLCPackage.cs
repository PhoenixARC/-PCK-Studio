namespace PckStudio.Core.Interfaces
{
    public interface IDLCPackage
    {
        string Name { get; }
        string Description { get; }
        int Identifier { get; }

        bool IsRootPackage { get; }

        DLC.DLCPackageType GetDLCPackageType();

        IDLCPackage ParentPackage { get; }
    }
}
