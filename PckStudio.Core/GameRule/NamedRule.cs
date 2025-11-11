using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class NamedRule : AbstractGameRule
    {
        private readonly string _name;
        private readonly GameRuleFile.GameRuleParameter[] _parameters;

        public NamedRule(string name, params GameRuleFile.GameRuleParameter[] parameters)
        {
            _name = name;
            _parameters = parameters;
        }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            var rule = new GameRuleFile.GameRule(_name);
            rule.AddParameters(_parameters);
            return rule;
        }
    }
}
