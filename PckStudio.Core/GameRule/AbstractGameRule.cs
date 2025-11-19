using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    public abstract class AbstractGameRule
    {
        private List<AbstractGameRule> _gameRules = new List<AbstractGameRule>();
        private IDictionary<string, string> _parameters = new Dictionary<string, string>();

        public abstract string Name { get; }

        public void AddRule(AbstractGameRule gameRule) => _gameRules.Add(gameRule);
        public void AddRules(IEnumerable<AbstractGameRule> gameRules) => _gameRules.AddRange(gameRules);

        public void AddParameter(string name, string value) => _parameters.Add(name, value);
        public void AddParameter<T>(string name, T value) => AddParameter(name, value.ToString());
        public void AddParameter(string name, bool value) => AddParameter(name, value.ToString().ToLower());
        public void AddParameters(KeyValuePair<string, string>[] values)
        {
            foreach (KeyValuePair<string, string> value in values)
            {
                AddParameter(value.Key, value.Value);
            }
        }
       
        private string GetString(char c, string prefix, string suffix) => $"{prefix}{(string.IsNullOrWhiteSpace(prefix) ? char.ToLower(c) : char.ToUpper(c))}{suffix}";
        
        public void AddParameter(Vector2 vec2, string prefix = "", string suffix = "")
        {
            AddParameter(GetString('x', prefix, suffix), vec2.X);
            AddParameter(GetString('y', prefix, suffix), vec2.Y);
        }


        public void AddParameter(Vector3 vec3, string prefix = "", string suffix = "")
        {
            AddParameter(new Vector2(vec3.X, vec3.Y), prefix, suffix);
            AddParameter(GetString('z', prefix, suffix), vec3.Z);
        }

        private GameRuleFile.GameRule InternalGetGameRule()
        {
            _ = Name ?? throw new ArgumentNullException(nameof(Name));
            IEnumerable<GameRuleFile.GameRule> a = _gameRules.Select(agr => agr.InternalGetGameRule());
            GameRuleFile.GameRule gameRule = new GameRuleFile.GameRule(Name);
            gameRule.AddParameters(_parameters.Select(kv => new GameRuleFile.GameRuleParameter(kv.Key, kv.Value)).ToArray());
            gameRule.AddRules(a);
            return gameRule;
        }

        public static implicit operator GameRuleFile.GameRule(AbstractGameRule abstractGameRule)
        {
            return abstractGameRule.InternalGetGameRule();
        }

        public static implicit operator GameRuleFile(AbstractGameRule abstractGameRule)
        {
            var grf = new GameRuleFile();
            grf.AddGameRules(abstractGameRule._gameRules.Select(agr => agr.InternalGetGameRule()));
            return grf;
        }
    }
}
