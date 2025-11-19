using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using Cyotek.Data.Nbt;
using PckStudio.Core.Extensions;
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

#nullable enable
    public sealed class MapData
    {
        public string Name { get; }
        public Image? Thumbnail { get; }
        public AbstractGameRule LevelRules { get; }
        internal AbstractGameRule Grf { get; }
        internal NamedData<byte[]> World { get; }
        
        public MapData(string name, Image? thumbnail, NamedData<byte[]> world, MiniGameId miniGame = MiniGameId.None, MapSize mapSize = default)
        {
            Name = name;
            Thumbnail = thumbnail;
            World = world;

            var levelData = MapReader.OpenSave(new MemoryStream(world.Value))["level.dat"];
            TagCompound? levelDat = NbtDocument.LoadDocument(new MemoryStream(levelData))!.DocumentRoot?["Data"] as TagCompound;
            _ = levelDat ?? throw new NullReferenceException(nameof(levelDat));
            Vector3 spawn = levelDat.GetVector3("Spawn");

            var mapOptions = new NamedRule("MapOptions");
            mapOptions.AddParameter("seed", levelDat["RandomSeed"].GetValue());
            mapOptions.AddParameter(spawn, prefix: "spawn");
            mapOptions.AddParameter("flatworld", false);
            mapOptions.AddParameter("baseSaveName", world.Name);
            if (miniGame > MiniGameId.None)
            {
                mapOptions.AddParameter("mapSize", (int)mapSize);
                mapOptions.AddParameter("themeId", 0);
            }

            Grf = new RootGameRule();

            Grf.AddRule(mapOptions);

            LevelRules = miniGame > MiniGameId.None ? GameRule.LevelRules.GetMiniGameLevelRules(miniGame) : GameRule.LevelRules.GetDefault(spawn, Vector2.Zero);
            Grf.AddRule(LevelRules);

        }
#nullable disable
    }
}