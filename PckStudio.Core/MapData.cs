using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using Cyotek.Data.Nbt;
using OMI.Formats.GameRule;
using PckStudio.Core.GameRule;

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
        public AbstractGameRule LevelRules { get; }
        internal AbstractGameRule Grf { get; }
        internal NamedData<byte[]> World { get; }

        public MapData(string name, Image thumbnail, MiniGameId miniGame, MapSize mapSize, NamedData<byte[]> world)
        {
            Name = name;
            Thumbnail = thumbnail;
            Grf = new RootGameRule();

            var levelData = MapReader.OpenSave(new MemoryStream(world.Value))["level.dat"];
            TagCompound levelDat = NbtDocument.LoadDocument(new MemoryStream(levelData)).DocumentRoot["Data"] as TagCompound;
            Vector3 spawn = Vector3.Zero;
            if (levelDat is not null)
                spawn = new Vector3((int)levelDat["SpawnX"].GetValue(), (int)levelDat["SpawnX"].GetValue(), (int)levelDat["SpawnY"].GetValue());
            
            Grf.AddRule(new NamedRule("MapOptions",
                new GameRuleFile.GameRuleParameter("seed", levelDat["RandomSeed"].GetValue().ToString()),
                new GameRuleFile.FloatParameter("spawnX", spawn.X),
                new GameRuleFile.FloatParameter("spawnY", spawn.Y),
                new GameRuleFile.FloatParameter("spawnZ", spawn.Z),
                new GameRuleFile.BoolParameter("flatworld", false),
                new GameRuleFile.GameRuleParameter("baseSaveName", world.Name),
                new GameRuleFile.IntParameter("mapSize", (int)mapSize),
                new GameRuleFile.IntParameter("themeId", 0))
                );

            LevelRules = GameRule.LevelRules.GetMiniGameLevelRules(miniGame);
            Grf.AddRule(LevelRules);

            World = world;
        }
    }
}
