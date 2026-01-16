using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OMI.Formats.Archive;
using OMI.Formats.Color;
using PckStudio.Core.Colors;
using PckStudio.Core.Extensions;
using PckStudio.Core.Interfaces;
using PckStudio.Core.Model;
using PckStudio.Core.Properties;

namespace PckStudio.Core.DLC
{
    public sealed class DLCTexturePackage : DLCPackage
    {
        public enum TextureResolution
        {
            x8,
            x16,
            x32,
            x48,
            x64,
            x80,
            x96,
            x112,
            x128
        }

        public override string Description { get; }
        internal const string TEXTUREPACK_DESCRIPTION_ID = "IDS_TP_DESCRIPTION";

        private TextureResolution _resolution;

        public sealed class MetaData(Image comparisonImg, Image iconImg)
        {
            public Image ComparisonImg { get; } = comparisonImg;
            public Image IconImg { get; } = iconImg;
        }

        public sealed class EnvironmentData(Image clouds, Image rain, Image snow)
        {
            public Image Clouds = clouds;
            public Image Rain = rain;
            public Image Snow = snow;
        }

        public MetaData Info { get; }

        private AbstractColorContainer _colorContainter;
        private AbstractModelContainer _customModels; //! can be null.. => models.bin
        private IDictionary<string, string> _materials; //! can be null.. 
        private IDictionary<string, Image> _itemModelTextures; //! can be null.. 
        private IDictionary<string, Image> _mobModelTextures; //! can be null.. 

        private Atlas _terrainAtlas;
        private Atlas _itemsAtlas;
        private Atlas _particlesAtlas;
        private Atlas _paintingAtlas;
        private Atlas _moonPhaseAtlas;
        private Atlas _mapIconsAtlas;
        private Atlas _additionalMapIconsAtlas;

        private ArmorSet _leatherArmorSet;
        private ArmorSet _chainArmorSet;
        private ArmorSet _ironArmorSet;
        private ArmorSet _goldArmorSet;
        private ArmorSet _diamondArmorSet;
        private ArmorSet _turtleArmorSet;
        
        private EnvironmentData _environmentData;

        private ConsoleArchive _mediaArc;

        private Animation _blockEntityBreakAnimation;
        private IDictionary<string, Animation> _itemAnimations;
        private IDictionary<string, Animation> _blockAnimations;
        private Image _sun;
        private Image _moon;

        //! TODO: add resources from "res/misc/"
        private readonly IDictionary<string, Image> _misc;

        internal DLCTexturePackage(
            string name,
            string description,
            int identifier,
            MetaData metaData,
            TextureResolution resolution,
            Atlas terrainAtlas,
            Atlas itemsAtlas,
            Atlas particlesAtlas,
            Atlas paintingAtlas,
            Atlas moonPhaseAtlas,
            Atlas mapIconsAtlas,
            Atlas additionalMapIconsAtlas,
            ArmorSet leatherArmorSet,
            ArmorSet chainArmorSet,
            ArmorSet ironArmorSet,
            ArmorSet goldArmorSet,
            ArmorSet diamondArmorSet,
            ArmorSet turtleArmorSet,
            EnvironmentData environmentData,
            AbstractColorContainer colorContainter,
            IDictionary<string, Image> itemModelTextures,
            IDictionary<string, Image> mobModelTextures,
            AbstractModelContainer customModels,
            IDictionary<string, string> materials,
            Animation blockEntityBreakAnimation,
            IDictionary<string, Animation> itemAnimations,
            IDictionary<string, Animation> blockAnimations,
            Image sun,
            Image moon,
            ConsoleArchive mediaArc,
            IDictionary<string, Image> misc,
            IDLCPackage parentPackage)
            : base(name, identifier, parentPackage)
        {
            Description = description;
            Info = metaData;
            _resolution = resolution;
            _terrainAtlas = terrainAtlas;
            _itemsAtlas = itemsAtlas;
            _particlesAtlas = particlesAtlas;
            _paintingAtlas = paintingAtlas;
            _moonPhaseAtlas = moonPhaseAtlas;
            _mapIconsAtlas = mapIconsAtlas;
            _additionalMapIconsAtlas = additionalMapIconsAtlas;
            _leatherArmorSet = leatherArmorSet;
            _chainArmorSet = chainArmorSet;
            _ironArmorSet = ironArmorSet;
            _goldArmorSet = goldArmorSet;
            _diamondArmorSet = diamondArmorSet;
            _turtleArmorSet = turtleArmorSet;
            _environmentData = environmentData;
            _colorContainter = colorContainter;
            _customModels = customModels;
            _materials = materials;
            _blockEntityBreakAnimation = blockEntityBreakAnimation ?? new Animation(terrainAtlas.GetRange(0, 15, 10, ImageLayoutDirection.Horizontal).Select(t => t.Texture).ToArray(), true);
            _itemAnimations = itemAnimations ?? new Dictionary<string, Animation>();
            _blockAnimations = blockAnimations ?? new Dictionary<string, Animation>();
            _sun = sun;
            _moon = moon ?? moonPhaseAtlas?[0]?.Texture ?? default;

            foreach (KeyValuePair<string, Animation> item in GetDefaultItemAnimations())
            {
                if (!_itemAnimations.ContainsKey(item.Key))
                    _itemAnimations.Add(item);
            }

            foreach (KeyValuePair<string, Animation> item in GetDefaultBlockAnimations())
            {
                if (!_blockAnimations.ContainsKey(item.Key))
                    _blockAnimations.Add(item);
            }

            _itemModelTextures = itemModelTextures;
            _mobModelTextures = mobModelTextures;
            _mediaArc = mediaArc;
            _misc = misc;
            SetTextureSizeForResolution();
        }

        private void SetTextureSizeForResolution()
        {
            Size size = GetTextureSize();

            Atlas[] atlases = [_terrainAtlas, _itemsAtlas, _particlesAtlas, _paintingAtlas];
            foreach (Atlas item in atlases)
            {
                item?.SetTileSize(size);
            }

            _blockEntityBreakAnimation.Resize(size);

            foreach (KeyValuePair<string, Animation> item in _itemAnimations)
            {
                item.Value.Resize(size);
            }

            foreach (KeyValuePair<string, Animation> item in _blockAnimations)
            {
                item.Value.Resize(size);
            }

        }

        public TextureResolution GetResolution() => _resolution;
        public Size GetTextureSize() => GetTextureSize(_resolution);
        public static Size GetTextureSize(TextureResolution resolution)
        {
            return resolution switch
            {
                TextureResolution.x8 => new Size(8, 8),
                TextureResolution.x16 => new Size(16, 16),
                TextureResolution.x32 => new Size(32, 32),
                TextureResolution.x48 => new Size(48, 48),
                TextureResolution.x64 => new Size(64, 64),
                TextureResolution.x80 => new Size(80, 80),
                TextureResolution.x96 => new Size(96, 96),
                TextureResolution.x112 => new Size(112, 112),
                TextureResolution.x128 => new Size(128, 128),
                _ => Size.Empty
            };
        }

        public static TextureResolution GetTextureResolution(Size size)
        {
            return size switch
            {
                { Width: 8, Height: 8 } => TextureResolution.x8,
                { Width: 16, Height: 16 } => TextureResolution.x16,
                { Width: 32, Height: 32 } => TextureResolution.x32,
                { Width: 48, Height: 48 } => TextureResolution.x48,
                { Width: 64, Height: 64 } => TextureResolution.x64,
                { Width: 80, Height: 80 } => TextureResolution.x80,
                { Width: 96, Height: 96 } => TextureResolution.x96,
                { Width: 112, Height: 112 } => TextureResolution.x112,
                { Width: 128, Height: 128 } => TextureResolution.x128,
                _ => TextureResolution.x16
            };
        }

        public static TextureResolution GetTextureResolutionFromString(string input)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));
            input = input.ToLower();
            var a = input.Split('/');
            if (a.Length == 2 && Enum.TryParse(a[0], true, out TextureResolution resolution))
                return resolution;

            if (input[0] == 'x')
            {
                int i = 1;
                char c;
                do
                {
                    c = input[i++];
                } while (char.IsDigit(c) && i < input.Length);
                if (Enum.TryParse(input.Substring(0, i), true, out resolution))
                    return resolution;
            }
            throw new ArgumentException("Invalid input string: " + input);
        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.TexturePack;

        internal static IDLCPackage CreateDefaultPackage(IDLCPackage parentPackage)
            => CreateDefaultPackage(parentPackage.Name, parentPackage.Description, parentPackage.Identifier, parentPackage);

        internal static IDLCPackage CreateDefaultPackage(string name, string description, int identifier, IDLCPackage parentPackage = null)
        {
            TextureResolution resolution = TextureResolution.x16;

            MetaData metadata = new MetaData(Resources.Comparison, Resources.TexturePackIcon);

            Atlas terrain = AtlasResource.Get(AtlasResource.AtlasType.BlockAtlas).GetDefaultAtlas();
            Atlas items = AtlasResource.Get(AtlasResource.AtlasType.ItemAtlas).GetDefaultAtlas();
            Atlas particles = AtlasResource.Get(AtlasResource.AtlasType.ParticleAtlas).GetDefaultAtlas();
            Atlas painting = AtlasResource.Get(AtlasResource.AtlasType.PaintingAtlas).GetDefaultAtlas();
            Atlas moonPhases = AtlasResource.Get(AtlasResource.AtlasType.MoonPhaseAtlas).GetDefaultAtlas();
            Atlas mapIconsAtlas = AtlasResource.Get(AtlasResource.AtlasType.MapIconAtlas).GetDefaultAtlas();
            Atlas additionalMapIconsAtlas = Atlas.FromResourceLocation(Resources.additional_map_icons_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.AdditionalMapIconsAtlas)));
            //ColorContainer colors = new COLFileReader().FromStream(new MemoryStream());
            IDictionary<string, Color> colors = null;
            IDictionary<string, (Color, Color, Color)> waterColors = null;

            Animation blockEntityBreakAnimation = new Animation(terrain.GetRange(0, 15, 10, ImageLayoutDirection.Horizontal).Select(t => t.Texture).ToArray(), true, 3);

            IDictionary<string, Animation> itemAnimations = GetDefaultItemAnimations();
            IDictionary<string, Animation> blockAnimations = GetDefaultBlockAnimations();

            return new DLCTexturePackage(
                name, description, identifier, metadata, resolution,
                terrain, items, particles, painting, moonPhases, mapIconsAtlas, additionalMapIconsAtlas,
                new ArmorSet(ArmorSetDescription.CLOTH, Resources.cloth, Resources.cloth_b),
                new ArmorSet(ArmorSetDescription.CHAIN, Resources.chain, default),
                new ArmorSet(ArmorSetDescription.IRON, Resources.iron, default),
                new ArmorSet(ArmorSetDescription.GOLD, Resources.gold, default),
                new ArmorSet(ArmorSetDescription.DIAMOND, Resources.diamond, default),
                new ArmorSet(ArmorSetDescription.TURTLE, Resources.turtle, default),
                new EnvironmentData(Resources.clouds, Resources.rain, Resources.snow),
                new AbstractColorContainer(colors, waterColors),
                itemModelTextures: null,
                mobModelTextures: null,
                customModels: new AbstractModelContainer(),
                materials: new Dictionary<string, string>(),
                blockEntityBreakAnimation: blockEntityBreakAnimation,
                itemAnimations: itemAnimations,
                blockAnimations: blockAnimations,
                sun: null,
                moon: null,
                mediaArc: null, 
                misc: null, 
                parentPackage: parentPackage
                );
        }

        internal Atlas GetTerrainAtlas() => _terrainAtlas ?? AtlasResource.Get(AtlasResource.AtlasType.BlockAtlas).GetDefaultAtlas();
        internal Atlas GetItemsAtlas() => _itemsAtlas ?? AtlasResource.Get(AtlasResource.AtlasType.ItemAtlas).GetDefaultAtlas();
        internal Atlas GetParticleAtlas() => _particlesAtlas ?? AtlasResource.Get(AtlasResource.AtlasType.ParticleAtlas).GetDefaultAtlas();
        internal Atlas GetPaintingAtlas() => _paintingAtlas ?? AtlasResource.Get(AtlasResource.AtlasType.PaintingAtlas).GetDefaultAtlas();
        internal Atlas GetMoonPhaseAtlas() => _moonPhaseAtlas ?? AtlasResource.Get(AtlasResource.AtlasType.MoonPhaseAtlas).GetDefaultAtlas();

        internal IDictionary<string, Animation> GetItemAnimations() => _itemAnimations;
        internal IDictionary<string, Animation> GetBlockAnimations() => _blockAnimations;
        internal IEnumerable<ArmorSet> GetArmorSets() => new ArmorSet[] { _leatherArmorSet, _chainArmorSet, _ironArmorSet, _goldArmorSet, _diamondArmorSet, _turtleArmorSet }.Where(armorSet => armorSet is not null);
        internal Animation GetBlockEntityBreakAnimation() => _blockEntityBreakAnimation;
        internal ConsoleArchive GetMediaArc() => _mediaArc;
        internal EnvironmentData GetEnvironmentData() => _environmentData;

        private static IDictionary<string, Animation> GetDefaultItemAnimations()
        {
            return new Dictionary<string, Animation>()
            {
                ["clock"] = new Animation(Resources.clock.Split(ImageLayoutDirection.Vertical), true),
                ["compass"] = new Animation(Resources.compass.Split(ImageLayoutDirection.Vertical), true),
            };
        }

        private static IDictionary<string, Animation> GetDefaultBlockAnimations()
        {
            return new Dictionary<string, Animation>()
            {
                ["fire_0"] = new Animation(Resources.fire_layer_0.Split(ImageLayoutDirection.Vertical), true),
                ["fire_1"] = new Animation(Resources.fire_layer_1.Split(ImageLayoutDirection.Vertical), true)
            };
        }

        internal Image GetSunTexture() => _sun;

        internal Image GetMoonTexture() => _moon;

        internal IEnumerable<KeyValuePair<string, Image>> GetItemModelTextures() => _itemModelTextures;

        internal IEnumerable<KeyValuePair<string, Image>> GetMobModelTextures() => _mobModelTextures;

        internal IDictionary<string, Image> GetMisc() => _misc;
    }
}