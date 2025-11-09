using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class WorldPosition(int x, int y, int z) : AbstractGameRule
    {
        private Vector3 _pos = new Vector3(x, y, z);
        
        protected override GameRuleFile.GameRule GetGameRule()
        {
            var gameRule = new GameRuleFile.GameRule("WorldPosition");
            gameRule.AddParameters(
                new GameRuleFile.IntParameter("x", (int)_pos.X),
                new GameRuleFile.IntParameter("y", (int)_pos.Y),
                new GameRuleFile.IntParameter("z", (int)_pos.Z)
                );
            return gameRule;
        }
    }
}
