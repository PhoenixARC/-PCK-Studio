using System;

namespace PckStudio.Core.GameRule
{
    public sealed class OnGameStartSpawnPositions : AbstractGameRule
    {
        public override string Name => "OnGameStartSpawnPositions";
        public SpawnPositionSet OnStart { get; } = new SpawnPositionSet(SpawnPositionSet.SpanPositionMethod.OnStart);
        public SpawnPositionSet OnRespawn { get; } = new SpawnPositionSet(SpawnPositionSet.SpanPositionMethod.OnRespawn);

        public OnGameStartSpawnPositions()
        {
            AddRule(OnStart);
            AddRule(OnRespawn);
        }
    }
}
