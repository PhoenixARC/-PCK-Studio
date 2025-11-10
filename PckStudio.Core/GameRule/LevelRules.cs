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

        public static LevelRules Default { get; } = new LevelRules([new UpdatePlayer(Vector3.Zero, Vector2.Zero)]);
        public static LevelRules GetLevelRules(MiniGameId miniGame) => new LevelRules(Enumerable.Empty<AbstractGameRule>(), new GameRuleFile.IntParameter("ruleType", (int)miniGame));

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
