using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class OnGameStartSpawnPositions : AbstractGameRule
    {
        public SpawnPositionSet OnStart { get; } = new SpawnPositionSet(SpawnPositionSet.SpanPositionMethod.OnStart);
        public SpawnPositionSet OnRespawn { get; } = new SpawnPositionSet(SpawnPositionSet.SpanPositionMethod.OnRespawn);

        protected override GameRuleFile.GameRule GetGameRule()
        {
            AddRule(OnStart);
            AddRule(OnRespawn);
            return new GameRuleFile.GameRule("OnGameStartSpawnPositions");
        }
    }
}
