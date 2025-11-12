using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    public abstract class AbstractGameRule
    {
        private List<AbstractGameRule> _gameRules = new List<AbstractGameRule>();
        public void AddRule(AbstractGameRule gameRule) => _gameRules.Add(gameRule);
        public void AddRules(IEnumerable<AbstractGameRule> gameRules) => _gameRules.AddRange(gameRules);

        protected abstract GameRuleFile.GameRule GetGameRule();
        private GameRuleFile.GameRule InternalGetGameRule()
        {
            IEnumerable<GameRuleFile.GameRule> a = _gameRules.Select(agr => agr.InternalGetGameRule());
            GameRuleFile.GameRule gameRule = GetGameRule();
            gameRule.AddRules(a);
            return gameRule;
        }
        public static implicit operator GameRuleFile.GameRule(AbstractGameRule abstractGameRule)
        {
            return abstractGameRule.InternalGetGameRule();
        }
    }
}
