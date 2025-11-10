using System.Drawing;
using OMI.Formats.GameRule;

namespace PckStudio.Core
{
    public enum MiniGameId : int
    {
        None,
        Battle,
        Tumble,
        Glide
    }

    public enum MapSize : int
    {
        Small,
        Large,
        Huge
    }

    public sealed class MapData
    {
        public string Name { get; }
        public Image Thumbnail { get; }
        public GameRuleFile Grf { get; }
        public GameRuleFile.GameRule LevelRules { get; }
        public NamedData<byte[]> World { get; }

        public MapData(string name, Image thumbnail, MiniGameId miniGame, MapSize mapSize, NamedData<byte[]> world)
        {
            Name = name;
            Thumbnail = thumbnail;
            Grf = new GameRuleFile();
            Grf.AddRule("MapOptions",
                new GameRuleFile.IntParameter("seed", 0),
                new GameRuleFile.IntParameter("spawnX", 0),
                new GameRuleFile.IntParameter("spawnY", 0),
                new GameRuleFile.IntParameter("spawnZ", 0),
                new GameRuleFile.BoolParameter("flatworld", false),
                new GameRuleFile.IntParameter("mapSize", (int)mapSize),
                new GameRuleFile.IntParameter("themeId", 0)
                );
            LevelRules = Grf.AddRule(GameRule.LevelRules.GetLevelRules(miniGame));

            World = world;
        }
    }
}
