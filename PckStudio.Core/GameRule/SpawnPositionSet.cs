using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    public sealed class SpawnPositionSet : AbstractGameRule
    {
        internal enum SpanPositionMethod
        {
            OnStart,
            OnRespawn
        }
        private SpanPositionMethod _method;

        internal SpawnPositionSet(SpanPositionMethod method) => _method = method;

        public void AddSpawnPosition(int x, int y, int z, int xRot, int yRot) => AddRule(new UpdatePlayer(new Vector3(x, y, z), new Vector2(xRot, yRot)));
        public void AddSpawnPosition(int x, int y, int z) => AddSpawnPosition(x, y, z, 0, 0);

        protected override GameRuleFile.GameRule GetGameRule()
        {
            var gameRule = new GameRuleFile.GameRule("SpawnPositionSet");
            gameRule.AddParameter(new GameRuleFile.IntParameter("method", (int)_method));
            return gameRule;
        }
    }
}
