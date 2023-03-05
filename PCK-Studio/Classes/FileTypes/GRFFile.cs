using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PckStudio.Classes.FileTypes
{
    public class GRFFile
    {   
        public static readonly string[] ValidGameRules = new string[]
            {
                "MapOptions",
                "ApplySchematic",
                "GenerateStructure",
                "GenerateBox",
                "PlaceBlock",
                "PlaceContainer",
                "PlaceSpawner",
                "BiomeOverride",
                "StartFeature",
                "AddItem",
                "AddEnchantment",
                "WeighedTreasureItem",
                "RandomItemSet",
                "DistributeItems",
                "WorldPosition",
                "LevelRules",
                "NamedArea",
                "ActiveChunkArea",
                "TargetArea",
                "ScoreRing",
                "ThermalArea",
                "PlayerBoundsVolume",
                "Killbox",
                "BlockLayer",
                "UseBlock",
                "CollectItem",
                "CompleteAll",
                "UpdatePlayer",
                "OnGameStartSpawnPositions",
                "OnInitialiseWorld",
                "SpawnPositionSet",
                "PopulateContainer",
                "DegradationSequence",
                "RandomDissolveDegrade",
                "DirectionalDegrade",
                "GrantPermissions",
                "AllowIn",
                "LayerGeneration",
                "LayerAsset",
                "AnyCombinationOf",
                "CombinationDefinition",
                "Variations",
                "BlockDef",
                "LayerSize",
                "UniformSize",
                "RandomizeSize",
                "LinearBlendSize",
                "LayerShape",
                "BasicShape",
                "StarShape",
                "PatchyShape",
                "RingShape",
                "SpiralShape",
                "LayerFill",
                "BasicLayerFill",
                "CurvedLayerFill",
                "WarpedLayerFill",
                "LayerTheme",
                "NullTheme",
                "FilterTheme",
                "ShaftsTheme",
                "BasicPatchesTheme",
                "BlockStackTheme",
                "RainbowTheme",
                "TerracottaTheme",
                "FunctionPatchesTheme",
                "SimplePatchesTheme",
                "CarpetTrapTheme",
                "MushroomBlockTheme",
                "TextureTheme",
                "SchematicTheme",
                "BlockCollisionException",
                "Powerup",
                "Checkpoint",
                "CustomBeacon",
                "ActiveViewArea",
            };

        public readonly GameRule Root = null;

        public int Crc => _crc;
        public bool IsWorld => _isWorld;
        public CompressionType CompressionLevel => _compressionLevel;

        private int _crc = 0;
        private bool _isWorld = false;
        private CompressionType _compressionLevel = CompressionType.None;

        public enum CompressionType : byte
        {
            None             = 0,
            Compressed       = 1,
            CompressedRle    = 2,
            CompressedRleCrc = 3,
        }

        /// <summary>
        /// Initializes a new GRFFile as a non-world grf file
        /// </summary>
        public GRFFile() : this(-1, false)
        {}

        public GRFFile(int crc, bool isWolrd) : this(crc, isWolrd, CompressionType.None)
        {}
        public GRFFile(int crc, bool isWolrd, CompressionType compressionLevel)
        {
            Root = new GameRule("__ROOT__", null);
            _compressionLevel = compressionLevel;
            _crc = crc;
            _isWorld = isWolrd;
        }

        public class GameRule
        {
            /// <summary> Contains all valid Parameter names </summary>
            public static readonly string[] ValidParameters = new string[]
            {
                "plus_x",
                "minus_x",
                "plus_z",
                "minus_z",
                "omni_plus_x",
                "omni_minus_x",
                "omni_plus_z",
                "omni_minus_z",
                "none",
                "plus_y",
                "minus_y",
                "plus_x",
                "minus_x",
                "plus_z",
                "minus_z",
                "descriptionName",
                "promptName",
                "dataTag",
                "enchantmentId",
                "enchantmentLevel",
                "itemId",
                "quantity",
                "auxValue",
                "slot",
                "name",
                "food",
                "health",
                "blockId",
                "useCoords",
                "seed",
                "flatworld",
                "filename",
                "rot",
                "data",
                "block",
                "entity",
                "facing",
                "edgeBlock",
                "fillBlock",
                "skipAir",
                "x",
                "x0",
                "x1",
                "y",
                "y0",
                "y1",
                "z",
                "z0",
                "z1",
                "chunkX",
                "chunkZ",
                "yRot",
                "xRot",
                "spawnX",
                "spawnY",
                "spawnZ",
                "orientation",
                "dimension",
                "topblockId",
                "biomeId",
                "feature",
                "minCount",
                "maxCount",
                "weight",
                "id",
                "probability",
                "method",
                "hasBeenInCreative",
                "cloudHeight",
                "fogDistance",
                "dayTime",
                "target",
                "speed",
                "dir",
                "type",
                "pass",
                "for",
                "random",
                "blockAux",
                "size",
                "scale",
                "freq",
                "func",
                "upper",
                "lower",
                "dY",
                "thickness",
                "points",
                "holeSize",
                "variant",
                "startHeight",
                "pattern",
                "colour",
                "primary",
                "laps",
                "liftForceModifier",
                "staticLift",
                "targetHeight",
                "speedBoost",
                "boostDirection",
                "condition_type",
                "condition_value_0",
                "condition_value_1",
                "beam_length",
            };

            public string Name { get; set; } = string.Empty;

            public GameRule Parent { get; } = null;
            public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
            public List<GameRule> ChildRules { get; } = new List<GameRule>();

            public GameRule(string name, GameRule parent)
            {
                Name = name;
                Parent = parent;
            }

            public GameRule AddRule(string gameRuleName) => AddRule(gameRuleName, false);

            /// <summary>Adds a new gamerule</summary>
            /// <param name="gameRuleName">Game rule to add</param>
            /// <param name="validate">Wether to check the given game rule</param>
            /// <returns>The Added GRFTag</returns>
            public GameRule AddRule(string gameRuleName, bool validate)
            {
                if (validate && !ValidGameRules.Contains(gameRuleName)) return null;
                var tag = new GameRule(gameRuleName, this);
                ChildRules.Add(tag);
                return tag;
            }

            public GameRule AddRule(string gameRuleName, params KeyValuePair<string,string>[] parameters)
            {
                var tag = AddRule(gameRuleName); // should never return null unless its called with the validate bool set to true
                foreach(var param in parameters)
                { 
                    tag.Parameters[param.Key] = param.Value;
                }
                return tag;
            }
        }

        public void AddGameRules(IEnumerable<GameRule> gameRules) => Root.ChildRules.AddRange(gameRules);
        
        public GameRule AddRule(string gameRuleName)
            => AddRule(gameRuleName, false);

        public GameRule AddRule(string gameRuleName, bool validate)
            => Root.AddRule(gameRuleName, validate);

        public GameRule AddRule(string gameRuleName, params KeyValuePair<string, string>[] parameters)
            => Root.AddRule(gameRuleName, parameters);
    }
}
