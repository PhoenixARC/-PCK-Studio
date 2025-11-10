using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal abstract class AbstractGameRule
    {
        private List<AbstractGameRule> _gameRules = new List<AbstractGameRule>();
        protected void AddRule(AbstractGameRule gameRule) => _gameRules.Add(gameRule);
        protected void AddRules(IEnumerable<AbstractGameRule> gameRules) => _gameRules.AddRange(gameRules);

        protected abstract GameRuleFile.GameRule GetGameRule();
        public static implicit operator GameRuleFile.GameRule(AbstractGameRule abstractGameRule)
        {
            GameRuleFile.GameRule gameRule = abstractGameRule.GetGameRule();
            gameRule.AddRules(abstractGameRule._gameRules.Select(agr => agr.GetGameRule()));
            return gameRule;
        }
    }
}
