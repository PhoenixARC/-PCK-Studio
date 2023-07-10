using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Extensions;
using OMI.Formats.Pck;
using OMI.Formats.Material;
using OMI.Workers.Material;
using System;

namespace PckStudio.Forms.Utilities
{
    public static class MaterialResources
    {
        public static byte[] MaterialsFileInitializer()
        {
            using var stream = new MemoryStream();
            var matFile = new MaterialContainer
            {
                new MaterialContainer.Material("bat", "entity_alphatest"),
                new MaterialContainer.Material("ender_dragon", "entity_emissive_alpha"),
                new MaterialContainer.Material("blaze_head", "entity_emissive_alpha"),
                new MaterialContainer.Material("drowned", "entity_emissive_alpha"),
                new MaterialContainer.Material("enderman", "entity_emissive_alpha"),
                new MaterialContainer.Material("enderman_invisible", "entity_emissive_alpha_only"),
                new MaterialContainer.Material("ghast", "entity_emissive_alpha"),
                new MaterialContainer.Material("guardian", "entity_alphatest"),
                new MaterialContainer.Material("magma_cube", "entity_emissive_alpha"),
                new MaterialContainer.Material("zombie_pigman", "entity"),
                new MaterialContainer.Material("phantom", "entity_emissive_alpha"),
                new MaterialContainer.Material("phantom_invisible", "entity_emissive_alpha_only"),
                new MaterialContainer.Material("sheep", "entity_change_color"),
                new MaterialContainer.Material("shulker", "entity_change_color"),
                new MaterialContainer.Material("skeleton", "entity_alphatest"),
                new MaterialContainer.Material("spider", "entity_emissive_alpha"),
                new MaterialContainer.Material("spider_invisible", "entity_emissive_alpha_only"),
                new MaterialContainer.Material("stray", "entity_alphatest"),
                new MaterialContainer.Material("iron_golem", "entity_alphatest"),
                new MaterialContainer.Material("wither_boss", "entity_alphatest"),
                new MaterialContainer.Material("wither_skeleton", "entity_alphatest"),
                new MaterialContainer.Material("wolf", "entity_alphatest_change_color")
            };
            var writer = new MaterialFileWriter(matFile);
            writer.WriteToStream(stream);
            return stream.ToArray();
        }
    }
}
