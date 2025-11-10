using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    public abstract class DLCPackage : IDLCPackage
    {
        protected DLCPackage(string name, int identifier, IDLCPackageSerialization packageInfo, IDLCPackage parentPackage)
        {
            Name = name;
            Identifier = identifier;
            PackageInfo = packageInfo;
            ParentPackage = parentPackage;
        }

        public int Identifier { get; }

        public IDLCPackageSerialization PackageInfo { get; }

        public string Name { get; } = string.Empty;

        public virtual string Description { get; } = string.Empty;

        public abstract DLCPackageType GetDLCPackageType();

        public IDLCPackage ParentPackage { get; }

        public bool IsRootPackage => ParentPackage is null;
    }
}
