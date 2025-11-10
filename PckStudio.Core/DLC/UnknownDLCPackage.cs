using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    public sealed class UnknownDLCPackage : DLCPackage
    {
        public PckFile PckFile { get; }

        public UnknownDLCPackage(string name, PckFile pckFile)
            : base(name ?? nameof(UnknownDLCPackage), -1, default, default)
        {
            PckFile = pckFile;
        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.Unknown;
    }
}
