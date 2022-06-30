using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class GRFFile
    {

        public GRFTag RootTag = null;
        public int Crc => _crc;
        public eCompressionType CompressionType => _compressionType;

        private int _crc = 0;
        private eCompressionType _compressionType = eCompressionType.None;

        [Flags]
        public enum eCompressionType : byte
        {
            None       = 0,
            Zlib       = 1,
            ZlibRle    = 2,
            /// <summary>
            /// custom enum value
            /// </summary>
            IsWolrdGrf = 0x80,
        }

        public class GRFTag
        {
            private GRFTag _parent = null;
            private SortedDictionary<string, string> _parameters = new SortedDictionary<string, string>();

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
            public SortedDictionary<string, string> Parameters => _parameters;
            public List<GRFTag> Tags { get; set; } = new List<GRFTag>();

            public GRFTag(string name, GRFTag parent)
            {
                Name = name;
                _parent = parent;
            }
        }

        public GRFFile(eCompressionType compressionType, int crc)
        {
            _compressionType = compressionType;
            _crc = crc;
        }

    }
}
