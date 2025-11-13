using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;
using PckStudio.Core.Extensions;

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

        public NamedArea(string name, BoundingBox boundingBox) : this(name, boundingBox.Start.ToNumericsVector(), boundingBox.End.ToNumericsVector()) { }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            GameRuleFile.GameRule gameRule = new GameRuleFile.GameRule("NamedArea");
            gameRule.AddParameters(
                new GameRuleFile.GameRuleParameter("name", Name),
                new GameRuleFile.FloatParameter("x0", Start.X),
                new GameRuleFile.FloatParameter("y0", Start.Y),
                new GameRuleFile.FloatParameter("z0", Start.Z),
                new GameRuleFile.FloatParameter("x1", End.X),
                new GameRuleFile.FloatParameter("y1", End.Y),
                new GameRuleFile.FloatParameter("z1", End.Z)
                );
            return gameRule;
        }
    }
}
