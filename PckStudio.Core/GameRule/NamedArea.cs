using System;
using System.Numerics;
using PckStudio.Core.Extensions;

namespace PckStudio.Core.GameRule
{
    internal sealed class NamedArea : AbstractGameRule
    {
        public override string Name => "NamedArea";

        public Vector3 Start { get; }
        public Vector3 End { get; }

        public NamedArea(string name, Vector3 start, Vector3 end)
        {
            AddParameter("name", name);
            AddParameter(start, suffix: "0");
            AddParameter(end, suffix: "1");
            Start = start;
            End = end;
        }

        public NamedArea(string name, BoundingBox boundingBox) : this(name, boundingBox.Start.ToNumericsVector(), boundingBox.End.ToNumericsVector()) { }
    }
}
