using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OMI.Formats.Color;
using OMI.Formats.Material;
using OMI.Formats.Model;
using OMI.Formats.Pck;
using OMI.Workers.Color;
using PckStudio.Core.Interfaces;
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
        internal const string TexturePackDescriptionId = "IDS_TP_DESCRIPTION";

        private TextureResolution _resolution;

        //! Data for x{_resolution}Info.pck
        public sealed class MetaData
        {
            public Image ComparisonImg { get; }
            public Image IconImg { get; }

            public MetaData(Image comparisonImg, Image iconImg)
            {
                ComparisonImg = comparisonImg;
                IconImg = iconImg;
            }
        }

        public MetaData Info { get; }

        //! Data for x{16}Data.pck
        //! => colours.col
        private IDictionary<string, Color> _colors;
        private IDictionary<string, (Color surface, Color underwater, Color fog)> _waterColors;
        private ModelContainer _customModels; //! => models.bin
        private MaterialContainer _materials;

        //! terrain mipmaps will be generated automatically. Add mipmap option to settings menu ? -null
        private Atlas _terrainAtlas;
        private Atlas _itemsAtlas;
        private Atlas _particlesAtlas;
        private Atlas _paintingAtlas;

        private Image[] _blockEntityBreakImages; //! max = 10!
        private IDictionary<string, Animation> _itemAnimations;
        private IDictionary<string, Animation> _blockAnimations;

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
            IDictionary<string, Color> colors,
            IDictionary<string, (Color surface, Color underwater, Color fog)> waterColors,
            ModelContainer customModels,
            MaterialContainer materials,
            Image[] blockEntityBreakImages,
            IDictionary<string, Animation> itemAnimations,
            IDictionary<string, Animation> blockAnimations,
            IDLCPackageSerialization packageInfo,
            IDLCPackage parentPackage
            )
            : base(name, identifier, packageInfo, parentPackage)
        {
            Description = description;
            Info = metaData;
            _resolution = resolution;
            _terrainAtlas = terrainAtlas;
            _itemsAtlas = itemsAtlas;
            _particlesAtlas = particlesAtlas;
            _paintingAtlas = paintingAtlas;
            _colors = colors;
            _waterColors = waterColors;
            _customModels = customModels;
            _materials = materials;
            _blockEntityBreakImages = blockEntityBreakImages;
            _itemAnimations = itemAnimations;
            _blockAnimations = blockAnimations;
        }

        public TextureResolution GetResolution() => _resolution;

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.TexturePack;

        internal static IDLCPackage CreateDefaultPackage(IDLCPackage parentPackage)
            => CreateDefaultPackage(parentPackage.Name, parentPackage.Description, parentPackage.Identifier, parentPackage);

        internal static IDLCPackage CreateDefaultPackage(string name, string description, int identifier, IDLCPackage parentPackage = null)
        {
            TextureResolution resolution = TextureResolution.x16;
            MetaData metadata = new MetaData(Resources.Comparison, Resources.TexturePackIcon);

            Atlas terrain   = Atlas.FromResourceLocation(Resources.terrain_atlas  , ResourceLocation.GetFromCategory(ResourceCategory.BlockAtlas));
            Atlas items     = Atlas.FromResourceLocation(Resources.items_atlas    , ResourceLocation.GetFromCategory(ResourceCategory.ItemAtlas));
            Atlas particles = Atlas.FromResourceLocation(Resources.particles_atlas, ResourceLocation.GetFromCategory(ResourceCategory.ParticleAtlas));
            Atlas painting  = Atlas.FromResourceLocation(Resources.paintings_atlas, ResourceLocation.GetFromCategory(ResourceCategory.PaintingAtlas));
            //ColorContainer colors = new COLFileReader().FromStream(new MemoryStream());
            IDictionary<string, Color> colors = null;
            IDictionary<string, (Color, Color, Color)> waterColors = null;
            Image[] blockEntityBreakImages = terrain.GetRange(0, 15, 10, ImageLayoutDirection.Horizontal).Select(t => t.Texture).ToArray();

            return new DLCTexturePackage(
                name, description, identifier, metadata, resolution,
                terrain, items, particles, painting,
                colors, waterColors,
                new ModelContainer(),
                new MaterialContainer(),
                blockEntityBreakImages,
                null, null,
                null, parentPackage
                );
        }
    }
}