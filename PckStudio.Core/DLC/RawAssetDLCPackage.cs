using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI;
using OMI.Formats.Pck;

namespace PckStudio.Core.DLC
{
    public sealed class RawAssetDLCPackage : DLCPackage
    {
        public PckFile PckFile { get; }
        public ByteOrder ByteOrder { get; }

        public RawAssetDLCPackage(string name, PckFile pckFile, ByteOrder byteOrder)
            : this(name, -1, pckFile, byteOrder)
        { }
        public RawAssetDLCPackage(string name, int id, PckFile pckFile, ByteOrder byteOrder)
            : base(name ?? nameof(RawAssetDLCPackage), id, default)
        {
            PckFile = pckFile;
            ByteOrder = byteOrder;
        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.RawAssets;
    }
}
