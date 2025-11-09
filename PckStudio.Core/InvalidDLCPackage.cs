using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core
{
    // Dummy class
    internal sealed class InvalidDLCPackage : DLCPackage
    {
        internal static IDLCPackage Instance { get; } = new InvalidDLCPackage();

        private InvalidDLCPackage(string name, int identifier, IDLCPackageLocationInfo packageInfo, IDLCPackage parentPackage)
            : base(name, identifier, packageInfo, parentPackage)
        {
        }

        private InvalidDLCPackage()
            : this(nameof(InvalidDLCPackage), -1, null, null)
        {

        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.Invalid;
    }
}
