using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    // Dummy class
    internal sealed class InvalidDLCPackage : DLCPackage
    {
        internal static IDLCPackage Instance { get; } = new InvalidDLCPackage();

        private InvalidDLCPackage(string name, int identifier, IDLCPackage parentPackage)
            : base(name, identifier, parentPackage)
        {
        }

        private InvalidDLCPackage()
            : this(nameof(InvalidDLCPackage), -1, null)
        {

        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.Invalid;
    }
}
