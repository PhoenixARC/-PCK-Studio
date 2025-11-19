using System;
using System.Numerics;

namespace PckStudio.Core.GameRule
{
    public sealed class SpawnPositionSet : AbstractGameRule
    {
        public override string Name => "SpawnPositionSet";
        public void AddSpawnPosition(int x, int y, int z, int xRot, int yRot) => AddRule(new UpdatePlayer(new Vector3(x, y, z), new Vector2(xRot, yRot)));
        public void AddSpawnPosition(int x, int y, int z) => AddSpawnPosition(x, y, z, 0, 0);

        internal enum SpanPositionMethod
        {
            OnStart,
            OnRespawn
        }

        internal SpawnPositionSet(SpanPositionMethod method) => AddParameter("method", (int)method);
    }
}
