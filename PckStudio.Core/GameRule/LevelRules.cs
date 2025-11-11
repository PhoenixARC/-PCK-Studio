using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class LevelRules : AbstractGameRule
    {
        private readonly GameRuleFile.GameRuleParameter[] _parameters;

        public static LevelRules GetDefault(Vector3 pos, Vector2 rot) => new LevelRules([new UpdatePlayer(pos, rot)]);
        public static LevelRules GetMiniGameLevelRules(MiniGameId miniGame) => new LevelRules(Enumerable.Empty<AbstractGameRule>(), new GameRuleFile.IntParameter("ruleType", (int)miniGame));

        public LevelRules(IEnumerable<AbstractGameRule> gameRules, params GameRuleFile.GameRuleParameter[] parameters)
        {
            AddRules(gameRules);
            _parameters = parameters;
        }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            var gameRule = new GameRuleFile.GameRule("LevelRules");
            gameRule.AddParameters(_parameters);
            return gameRule;
        }
    }
}
