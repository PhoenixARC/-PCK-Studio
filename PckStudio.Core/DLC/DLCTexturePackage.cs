using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        //! Data for x{16}Data.pck
        //! => colours.col
        private IDictionary<string, Color> _colors;
        private IDictionary<string, (Color surface, Color underwater, Color fog)> _waterColors;
        private AbstractModelContainer _customModels; //! can be null.. => models.bin
        private IDictionary<string, string> _materials; //! can be null.. 

        //! terrain mipmaps will be generated automatically. Add mipmap option to settings menu ? -null
        private Atlas _terrainAtlas;
        private Atlas _itemsAtlas;
        private Atlas _particlesAtlas;
        private Atlas _paintingAtlas;
        private Atlas _moonPhaseAtlas;
        private ArmorSet _leatherArmorSet;
        private ArmorSet _chainArmorSet;
        private ArmorSet _ironArmorSet;
        private ArmorSet _goldArmorSet;
        private ArmorSet _diamondArmorSet;
        private ArmorSet _turtleArmorSet;
        private Atlas _mapIconsAtlas;
        private Atlas _additionalMapIconsAtlas;
        private EnvironmentData _environmentData;

        private Animation _blockEntityBreakAnimation;
        private IDictionary<string, Animation> _itemAnimations;
        private IDictionary<string, Animation> _blockAnimations;
        private Image _sun;
        private Image _moon;

        //! TODO: add resources from "res/misc/"

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
            IDictionary<string, Color> colors,
            IDictionary<string, (Color surface, Color underwater, Color fog)> waterColors,
            AbstractModelContainer customModels,
            IDictionary<string, string> materials,
            Animation blockEntityBreakAnimation,
            IDictionary<string, Animation> itemAnimations,
            IDictionary<string, Animation> blockAnimations,
            Image sun,
            Image moon,
            IDLCPackage parentPackage
            )
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
            _colors = colors ?? new Dictionary<string, Color>();
            _waterColors = waterColors ?? new Dictionary<string, (Color, Color, Color)>();
            _customModels = customModels;
            _materials = materials;
            _blockEntityBreakAnimation = blockEntityBreakAnimation;
            _itemAnimations = itemAnimations ?? new Dictionary<string, Animation>();
            _blockAnimations = blockAnimations ?? new Dictionary<string, Animation>();
            _sun = sun;
            _moon = moon;
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

            Atlas terrain = Atlas.FromResourceLocation(Resources.terrain_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas)));
            Atlas items = Atlas.FromResourceLocation(Resources.items_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas)));
            Atlas particles = Atlas.FromResourceLocation(Resources.particles_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ParticleAtlas)));
            Atlas painting = Atlas.FromResourceLocation(Resources.paintings_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.PaintingAtlas)));
            Atlas moonPhases = Atlas.FromResourceLocation(Resources.moon_phases_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.MoonPhaseAtlas)));
            Atlas mapIconsAtlas = Atlas.FromResourceLocation(Resources.map_icons_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.MapIconAtlas)));
            Atlas additionalMapIconsAtlas = Atlas.FromResourceLocation(Resources.additional_map_icons_atlas, ResourceLocation.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.AdditionalMapIconsAtlas)));
            //ColorContainer colors = new COLFileReader().FromStream(new MemoryStream());
            IDictionary<string, Color> colors = null;
            IDictionary<string, (Color, Color, Color)> waterColors = null;

            Animation blockEntityBreakAnimation = new Animation(terrain.GetRange(0, 15, 10, ImageLayoutDirection.Horizontal).Select(t => t.Texture).ToArray(), true, 3);

            IDictionary<string, Animation> itemAnimations = GetItemAnimations();
            IDictionary<string, Animation> blockAnimations = GetBlockAnimations();

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
                colors, waterColors,
                new AbstractModelContainer(),
                new Dictionary<string, string>(),
                blockEntityBreakAnimation,
                itemAnimations,
                blockAnimations,
                sun: null,
                moon: null,
                parentPackage
                );
        }

        internal Atlas GetTerrainAtlas() => _terrainAtlas;

        internal Atlas GetItemsAtlas() => _itemsAtlas;

        internal Atlas GetParticleAtlas() => _particlesAtlas;

        internal Atlas GetPaintingAtlas() => _paintingAtlas;

        private static IDictionary<string, Animation> GetItemAnimations()
        {
            return new Dictionary<string, Animation>()
            {
                ["clock"] = new Animation(Resources.clock.Split(ImageLayoutDirection.Vertical), true),
                ["compass"] = new Animation(Resources.compass.Split(ImageLayoutDirection.Vertical), true),
            };
        }

        private static IDictionary<string, Animation> GetBlockAnimations()
        {
            return new Dictionary<string, Animation>()
            {
                ["fire_0"] = new Animation(Resources.fire_layer_0.Split(ImageLayoutDirection.Vertical), true),
                ["fire_1"] = new Animation(Resources.fire_layer_1.Split(ImageLayoutDirection.Vertical), true)
            };
        }
    }
}