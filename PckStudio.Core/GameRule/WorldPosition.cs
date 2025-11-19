using System;
using System.Numerics;
namespace PckStudio.Core.GameRule
{
    internal sealed class WorldPosition : AbstractGameRule
    {
        public override string Name => "WorldPosition";

        public WorldPosition(int x, int y, int z)
        {
            AddParameter(new Vector3(x, y, z));
        }
    }
}
