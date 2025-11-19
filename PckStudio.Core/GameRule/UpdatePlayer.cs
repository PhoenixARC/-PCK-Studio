using System;
using System.Numerics;

namespace PckStudio.Core.GameRule
{
    public sealed class UpdatePlayer : AbstractGameRule
    {
        public override string Name => "UpdatePlayer";

        public UpdatePlayer(Vector3 spawn, Vector2 rot)
        {
            AddParameter(spawn);
            AddParameter(rot, suffix: "Rot");
        }
    }
}
