using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace PckStudio.Core.GameRule
{
    public sealed class LevelRules : AbstractGameRule
    {
        public override string Name => "LevelRules";

        public MiniGameId MiniGame { get; }

        public static LevelRules GetDefault(Vector3 pos, Vector2 rot) => new LevelRules([new UpdatePlayer(pos, rot)]);
        public static LevelRules GetMiniGameLevelRules(MiniGameId miniGame) => new LevelRules(miniGame);

        public LevelRules(IEnumerable<AbstractGameRule> gameRules, MiniGameId miniGameId, params KeyValuePair<string, string>[] parameters)
        {
            AddRules(gameRules);
            MiniGame = miniGameId;
            if (miniGameId != MiniGameId.None)
                AddParameter("ruleType", (int)miniGameId);
            AddParameters(parameters);
        }

        public LevelRules(IEnumerable<AbstractGameRule> gameRules, params KeyValuePair<string, string>[] parameters) : this(gameRules, MiniGameId.None, parameters) { }

        public LevelRules(MiniGameId miniGameId) : this(Enumerable.Empty<AbstractGameRule>(), miniGameId) { }
    }
}
