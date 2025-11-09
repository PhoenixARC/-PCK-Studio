using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class NamedArea : AbstractGameRule
    {
        public string Name { get; }

        public Vector3 Start { get; }
        public Vector3 End { get; }

        public NamedArea(string name, Vector3 start, Vector3 end)
        {
            Name = name;
            Start = start;
            End = end;
        }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            GameRuleFile.GameRule gameRule = new GameRuleFile.GameRule("NamedArea", null);
            gameRule.AddParameters(
                new GameRuleFile.GameRuleParameter("name", Name),
                new GameRuleFile.IntParameter("x0", (int)Start.X),
                new GameRuleFile.IntParameter("y0", (int)Start.Y),
                new GameRuleFile.IntParameter("z0", (int)Start.Z),
                new GameRuleFile.IntParameter("x1", (int)End.X),
                new GameRuleFile.IntParameter("y1", (int)End.Y),
                new GameRuleFile.IntParameter("z1", (int)End.Z)
                );
            return gameRule;
        }
    }
}
