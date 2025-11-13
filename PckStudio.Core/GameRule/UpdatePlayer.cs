using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class UpdatePlayer : AbstractGameRule
    {
        private Vector3 _spawn;
        private Vector2 _rot;

        public UpdatePlayer(Vector3 spawn, Vector2 rot)
        {
            _spawn = spawn;
            _rot = rot;
        }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            var gameRule = new GameRuleFile.GameRule("UpdatePlayer");
            gameRule.AddParameters(
                new GameRuleFile.FloatParameter("x", _spawn.X),
                new GameRuleFile.FloatParameter("y", _spawn.Y),
                new GameRuleFile.FloatParameter("z", _spawn.Z),
                new GameRuleFile.FloatParameter("xRot", _rot.X),
                new GameRuleFile.FloatParameter("yRot", _rot.Y)
                );
            return gameRule;
        }
    }
}
