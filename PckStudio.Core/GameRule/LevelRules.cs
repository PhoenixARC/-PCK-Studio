using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    public sealed class LevelRules : AbstractGameRule
    {
        private readonly GameRuleFile.GameRuleParameter[] _parameters;
        private readonly MiniGameId _miniGameId;

        public static LevelRules GetDefault(Vector3 pos, Vector2 rot) => new LevelRules([new UpdatePlayer(pos, rot)]);
        public static LevelRules GetMiniGameLevelRules(MiniGameId miniGame) => new LevelRules(miniGame);

        public LevelRules(IEnumerable<AbstractGameRule> gameRules, MiniGameId miniGameId, params GameRuleFile.GameRuleParameter[] parameters)
        {
            AddRules(gameRules);
            _parameters = parameters;
            _miniGameId = miniGameId;
        }

        public LevelRules(IEnumerable<AbstractGameRule> gameRules, params GameRuleFile.GameRuleParameter[] parameters) : this(gameRules, MiniGameId.None, parameters) { }

        public LevelRules(MiniGameId miniGameId) : this(Enumerable.Empty<AbstractGameRule>(), miniGameId) { }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            var gameRule = new GameRuleFile.GameRule("LevelRules");
            if (_miniGameId != MiniGameId.None)
                gameRule.AddParameter(new GameRuleFile.IntParameter("ruleType", (int)_miniGameId));
            gameRule.AddParameters(_parameters);
            return gameRule;
        }
    }
}
