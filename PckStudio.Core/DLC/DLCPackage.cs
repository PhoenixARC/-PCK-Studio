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
        public static IDLCPackage Invalid = new InvalidDLCPackage();
        protected DLCPackage(string name, int identifier, IDLCPackage parentPackage)
        {
            Name = name;
            Identifier = identifier;
            ParentPackage = parentPackage;
        }

        private sealed class InvalidDLCPackage : DLCPackage
        {
            internal InvalidDLCPackage() : base(nameof(InvalidDLCPackage), -1, null) { }

            public override DLCPackageType GetDLCPackageType() => DLCPackageType.Invalid;
        }

        public int Identifier { get; }

        public string Name { get; } = string.Empty;

        public virtual string Description { get; } = string.Empty;

        public abstract DLCPackageType GetDLCPackageType();

        public IDLCPackage ParentPackage { get; }

        public bool IsRootPackage => ParentPackage is null;
    }
}
