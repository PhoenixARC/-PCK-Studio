using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PckStudio.Classes.FileTypes
{
    public class GRFFile
    {   
        private GRFTag _root = null;
        public GRFTag RootTag => _root;
        public int Crc => _crc;
        public bool IsWorld => _isWorld;

        private int _crc = 0;
        private bool _isWorld = false;

        public enum eCompressionType : byte
        {
            None       = 0,
            Zlib       = 1,
            ZlibRle    = 2,
            ZlibRleCrc = 3,
        }

        public class GRFTag
        {
            private GRFTag _parent = null;
            private Dictionary<string, string> _parameters = new Dictionary<string, string>();

            /// <summary>
            /// Contains all valid Parameter names
            /// </summary>
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

            public string Name { get; set; } = string.Empty;
            public GRFTag Parent => _parent;
            public Dictionary<string, string> Parameters => _parameters;
            public List<GRFTag> Tags { get; set; } = new List<GRFTag>();

            public GRFTag(string name, GRFTag parent)
            {
                Name = name;
                _parent = parent;
            }

            public GRFTag AddTag(string gameRuleName) => AddTag(gameRuleName, false);

            /// <summary>
            /// Adds a new tag to its child tags
            /// </summary>
            /// <param name="gameRuleName">Game rule to add</param>
            /// <param name="validate">Wether to check the given game rule</param>
            /// <returns>The Added GRFTag</returns>
            public GRFTag AddTag(string gameRuleName, bool validate)
            {
                if (validate && !ValidGameRules.Contains(gameRuleName)) return null;
                var tag = new GRFTag(gameRuleName, this);
                Tags.Add(tag);
                return tag;
            }

            public GRFTag AddTag(string gameRuleName, params KeyValuePair<string,string>[] parameters)
            {
                var tag = AddTag(gameRuleName); // should never return null unless its called with the validate bool set to true
                foreach(var param in parameters)
                { 
                    tag.Parameters[param.Key] = param.Value;
                }
                return tag;
            }
        }

        public GRFTag AddTag(string gameRuleName)
            => AddTag(gameRuleName, false);

        public GRFTag AddTag(string gameRuleName, bool validate)
            => _root.AddTag(gameRuleName, validate);

        public GRFTag AddTag(string gameRuleName, params KeyValuePair<string, string>[] parameters)
            => _root.AddTag(gameRuleName, parameters);

        public GRFFile(int crc, bool isWolrd)
        {
            _root = new GRFTag("__ROOT__", null);
            _crc = crc;
            _isWorld = isWolrd;
        }

        /// <summary>
        /// Initializes a new GRFFile as a non-world grf file
        /// </summary>
        public GRFFile() : this(-1, false)
        {
        }


    }
}
